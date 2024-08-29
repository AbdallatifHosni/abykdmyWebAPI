using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AbyKhedma.Services;
using Core.Dtos;
using Core.Models;
using AbyKhedma.Pagination;
using AbyKhedma.Persistance;
using CloudinaryDotNet.Actions;
using AbyKhedma.Core.Common;
using System.Linq;
using System.Security.Principal;

namespace AbyKhedma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CategoryController> _logger;
        private readonly CategoryService _categoryService;
        private readonly AppDbContext _appDbContext;
        private readonly IUriService _uriService;
        public CategoryController(CategoryService categoryService, AppDbContext appDbContext, IMapper mapper, IConfiguration configuration, ILogger<CategoryController> logger, IUriService uriService)
        {
            _categoryService = categoryService;
            this._appDbContext = appDbContext;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _uriService = uriService;
        }

        [HttpGet("getAll")]
        public ActionResult<CategoryModel> GetCategoryList([FromQuery] FilterDto filterDto)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var categories = _categoryService.GetCategoryList();
            if (categories != null)
            {
                var statements = _appDbContext.Statements.ToList();
                foreach (var c in categories)
                {
                    c.OpeningStatement = _mapper.Map<StatementModel>(statements.FirstOrDefault(s => c.OpeningStatementId != null && c.OpeningStatementId > 0 && s.Id == c.OpeningStatementId));
                    c.ClosingStatement = _mapper.Map<StatementModel>(statements.FirstOrDefault(s => c.ClosingStatementId != null && c.ClosingStatementId > 0 && s.Id == c.ClosingStatementId));
                }
            }
            var filteredList = categories
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = categories.Count();
            return Ok(PaginationHelper.CreatePagedReponse<CategoryModel>(filteredList, validFilter, totalRecords, _uriService, route));

        }

        [HttpGet("getAllForRequester")]
        public ActionResult<CategoryModel> GetCategoryListForRequester([FromQuery] FilterDto filterDto)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var identityId = User.Claims.FirstOrDefault(c => c.Type == "identityId");
            if (identityId == null)
            {
                return Unauthorized(new { Succeeded = false, Data = new { }, Message = "You are not authorized to do that", Errors = new string[] { } });
            }
            var categories = _categoryService.GetCategoryListForRequester(Int32.Parse(identityId.Value));
            //categories= categories.OrderDescending(c=>c.)
            if (categories != null)
            {
                var statements = _appDbContext.Statements.ToList();
                foreach (var c in categories)
                {
                    c.OpeningStatement = _mapper.Map<StatementModel>(statements.FirstOrDefault(s => c.OpeningStatementId != null && c.OpeningStatementId > 0 && s.Id == c.OpeningStatementId));
                    c.ClosingStatement = _mapper.Map<StatementModel>(statements.FirstOrDefault(s => c.ClosingStatementId != null && c.ClosingStatementId > 0 && s.Id == c.ClosingStatementId));
                }
            }
          //  var comparer = new NullsLastComparer<DateTime?>();

            var filteredList = categories
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = categories.Count();
            return Ok(PaginationHelper.CreatePagedReponse<CategoryModel>(filteredList, validFilter, totalRecords, _uriService, route));

        }
        [HttpPost("add")]
        public ActionResult<Task> AddCategory(CategoryToCreateDto categoryDto)
        {
            var roleClaim = User.Claims.FirstOrDefault(el => el.Type == "_role");//.FindFirst(ClaimTypes.Role).Value;
            var identity = User.Claims.FirstOrDefault(c => c.Type == "identityId");
            var userFullName = User.Claims.FirstOrDefault(c => c.Type == "fullName");
            if (roleClaim == null || !Constants.GetSystemRoles().Contains(roleClaim.Value.ToString()))
            {
                return Unauthorized(new { Succeeded = false, Data = new { }, Message = "You are not authorized to do that", Errors = new string[] { } });
            }
            var categoryId = _categoryService.AddCategory(categoryDto);
            if (categoryId == 0)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Failed to save", Errors = new string[] { } });
            }
            _appDbContext.AuditLogs.Add(new Entities.AuditLog()
            {
                UserId = Int32.Parse(identity.Value),
                ActivityTime = DateTime.Now,
                ActivityDescription = userFullName?.Value + "اضافة قسم  " + categoryDto.CategoryName
            }
            );
            _appDbContext.SaveChanges();

            return Ok(new { Succeeded = true, Data = new { id = categoryId }, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpPost("addSubCategory")]
        public ActionResult<Task> AddSubCategory(SubCategoryToCreateDto categoryDto)
        {
            var roleClaim = User.Claims.FirstOrDefault(el => el.Type == "_role");//.FindFirst(ClaimTypes.Role).Value;
            var identity = User.Claims.FirstOrDefault(c => c.Type == "identityId");
            var userFullName = User.Claims.FirstOrDefault(c => c.Type == "fullName");
            if (roleClaim == null || !Constants.GetSystemRoles().Contains(roleClaim.Value.ToString()))
            {
                return Unauthorized(new { Succeeded = false, Data = new { }, Message = "You are not authorized to do that", Errors = new string[] { } });
            }
            if (categoryDto.ParentCategoryId == null || categoryDto.ParentCategoryId <= 0)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Invalid Parent Category Id", Errors = new string[] { } });
            }
            var category = _categoryService.AddSubCategory(categoryDto);
            if (category == null)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Failed to save", Errors = new string[] { } });
            }
            if (category.ParentCategoryId != null)
            {

               var parentCategory= _appDbContext.Categories.FirstOrDefault(c => c.Id == category.ParentCategoryId.Value);
                if(parentCategory != null)
                {
                    parentCategory.HasChilds = true;
                    _appDbContext.Categories.Update(parentCategory);
                    _appDbContext.SaveChanges();
                }
            }


            _appDbContext.AuditLogs.Add(new Entities.AuditLog()
            {
                UserId = Int32.Parse(identity.Value),
                ActivityTime = DateTime.Now,
                ActivityDescription = userFullName?.Value + "اضافة قسم  " + categoryDto.CategoryName
            }
             );
            _appDbContext.SaveChanges();

            return Ok(new { Succeeded = true, Data = new { id = category.Id }, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpDelete("deleteCategory/{id}")]
        public ActionResult<Task> DeleteCategory(int id)
        {
            var roleClaim = User.Claims.FirstOrDefault(el => el.Type == "_role");//.FindFirst(ClaimTypes.Role).Value;
            var identity = User.Claims.FirstOrDefault(c => c.Type == "identityId");
            var userFullName= User.Claims.FirstOrDefault(c => c.Type == "fullName"); 
            if (roleClaim == null || !Constants.GetSystemRoles().Contains(roleClaim.Value.ToString()))
            {
                return Unauthorized(new { Succeeded = false, Data = new { }, Message = "You are not authorized to do that", Errors = new string[] { } });
            }
             var categoryList = _appDbContext.Categories.Where(c=>c.Id==id || c.ParentCategoryId==id).ToList();
            //var services=  _appDbContext.Services.Where(x => categoryIdList.Contains( x.CategoryID ) ).ToList();
            //  if (services!=null && services.Count>0)
            //  {
            //      return BadRequest(new { Succeeded = false, Data = new { }, Message = "There are tickets created on this category which must be deleted first", Errors = new string[] { } });
            //  }
            string DeletedCatsNameList = "";
            foreach (var cat in categoryList)
            {
                cat.Deleted = true;
                DeletedCatsNameList += "," + cat.CategoryName;
            }
             _appDbContext.Categories.UpdateRange(categoryList);

            _appDbContext.AuditLogs.Add(new Entities.AuditLog()
            {
                UserId = Int32.Parse(identity.Value),
                ActivityTime = DateTime.Now,
                ActivityDescription = userFullName?.Value+ "حذف الاقسام " + DeletedCatsNameList
            }
             );


            _appDbContext.SaveChanges(true);
            //if (res == -1)
            //{
            //    return BadRequest(new { Succeeded = false, Data = new { }, Message = "Can't delete system category", Errors = new string[] { } });
            //}
            return Ok(new { Succeeded = true, Data = new { }, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpPost("editCategory")]
        public ActionResult<Task> EditCategory( CategoryModel category)
        {
            var roleClaim = User.Claims.FirstOrDefault(el => el.Type == "_role");//.FindFirst(ClaimTypes.Role).Value;
            if (roleClaim == null || !Constants.GetSystemRoles().Contains(roleClaim.Value.ToString()))
            {
                return Unauthorized(new { Succeeded = false, Data = new { }, Message = "You are not authorized to do that", Errors = new string[] { } });
            }
            int res=_categoryService.UpdateCategory(category);
            if(res == 0)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "No such category exist", Errors = new string[] { } });
            }
            return Ok(new { Succeeded = true, Data = new { }, Message = string.Empty, Errors = new string[] { } });
        }
    }
}
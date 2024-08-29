using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using AutoMapper;
using Core.Dtos;
using Core.Models;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AbyKhedma.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _dbContext;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository dbContext, AppDbContext appDbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public List<CategoryModel> GetCategoryList()
        {
            var allCategories = _dbContext.GetAll().Where(el=>el.IsSystem!=true &&  el.Deleted != true).OrderBy(c => c.ParentCategoryId).ThenBy(c => c.Id).ToList();
            var categoriesHierachy = allCategories.Where(c => c.ParentCategoryId == 0 ).OrderBy(c => c.ParentCategoryId).ThenBy(c => c.Id).ToList();
            List<CategoryModel> categoryModelList = _mapper.Map<List<CategoryModel>>(categoriesHierachy);
            List<CategoryModel> categoryModelListRes = new List<CategoryModel>();
            foreach (var category in categoryModelList)
            {

                CategoryModel tmpCategoryModel = new CategoryModel();
                tmpCategoryModel = category;
                if (category.HasChilds == true)
                {
                    tmpCategoryModel = GetSubCategories(category, allCategories);
                }
                categoryModelListRes.Add(tmpCategoryModel);
            }
            return categoryModelListRes;
        }
        public List<CategoryModel> GetCategoryListForRequester(int userId)
        {
            var allCategories = _appDbContext.Categories.Where(el => el.IsSystem != true && el.Deleted != true).OrderBy(c => c.ParentCategoryId).ThenBy(c => c.Id).ToList();
            var requests = _appDbContext.Requests.Where(u => u.RequesterId == userId &&  u.IsArchived!=true).ToList();
            if (requests.Count > 0)
            {
                requests.ForEach(r =>
                {
                    if (r.CreatedDate > r.UpdatedDate)
                    {
                        r.UpdatedDate = r.CreatedDate;
                    }
                });
            }
            var lastestRequests = requests.Select(el => new { UpdatedDate = el.UpdatedDate, ServiceId = el.ServiceId })
                .GroupBy(el => el.ServiceId)
                .Select(e => new { UpdatedDate = e.Max(r => r.UpdatedDate), ServiceId = e.Key });
            var requestIds = lastestRequests.Select(s => s.ServiceId).ToList();
            var catsWithService = _appDbContext.Services.Where(s => requestIds.Contains(s.Id)).Select(el => new { CategoryId = el.CategoryID, ServiceId = el.Id }).ToList();
            var catWithUpdatedDate = catsWithService.Join(lastestRequests, cs => cs.ServiceId, r => r.ServiceId, (cs, r) => new CategoryRequestDate { CategoryId = cs.CategoryId, UpdatedDate = r.UpdatedDate }).ToList();
            catWithUpdatedDate = catWithUpdatedDate.GroupBy(el => el.CategoryId).Select(el => new CategoryRequestDate { CategoryId = el.Key, UpdatedDate = el.Max(r => r.UpdatedDate) }).ToList();
            var allCategoriesTmp = allCategories.OrderByDescending(c => c.ParentCategoryId).ThenByDescending(c => c.Id).ToList();

            List<Category> categories = new List<Category>();
            var catWithUpdatedDateListAll = new List<CategoryRequestDate>();
            catWithUpdatedDate.ForEach(c =>
            {
                var catItself = allCategoriesTmp.FirstOrDefault(cat => cat.Id == c.CategoryId);
                if (catItself!=null && catItself.ParentCategoryId != null && catItself.ParentCategoryId > 0)
                {
                    c.CategoryId = GetParentId(catItself.ParentCategoryId, allCategoriesTmp);
                }
            });


            var categoriesHierachy = allCategories.Where(c => c.ParentCategoryId == 0).OrderBy(c => c.ParentCategoryId).ThenBy(c => c.Id).ToList();
            List<CategoryModel> categoryModelList = _mapper.Map<List<CategoryModel>>(categoriesHierachy);
            List<CategoryModel> categoryModelListRes = new List<CategoryModel>();
            foreach (var category in categoryModelList)
            {
                CategoryModel tmpCategoryModel = new CategoryModel();
                tmpCategoryModel = category;
                if (category.HasChilds == true)
                {
                    tmpCategoryModel = GetSubCategories(category, allCategories);
                }
                tmpCategoryModel.UpdatedDate = catWithUpdatedDate.FirstOrDefault(it => it.CategoryId == category.Id)?.UpdatedDate;
                categoryModelListRes.Add(tmpCategoryModel);
            }
            return categoryModelListRes.OrderByDescending(ee=>ee.UpdatedDate).ToList();
        }

        private int GetParentId(int? parentCategoryId, List<Category> allCategoriesTmp)
        {
            var catItself = allCategoriesTmp.FirstOrDefault(cat => cat.Id == parentCategoryId);
            if (catItself !=null && catItself.ParentCategoryId != null && catItself.ParentCategoryId > 0)
            {
                return GetParentId(catItself.ParentCategoryId, allCategoriesTmp);
            }
            else
            {
                return parentCategoryId.Value;
            }
        }

        private CategoryModel GetSubCategories(CategoryModel categoryModel, List<Category> allCategories)
        {
            List<CategoryModel> categoryModelList = new List<CategoryModel>();
            categoryModel.SubCategories = _mapper.Map<List<CategoryModel>>(allCategories.Where(c => c.ParentCategoryId == categoryModel.Id).ToList());
            List<CategoryModel> categoryModelListRes = new List<CategoryModel>();
            foreach (var category in categoryModel.SubCategories)
            {
                CategoryModel tmpCategoryModel = new CategoryModel();
                tmpCategoryModel = category;
                if (category.HasChilds == true)
                {
                    tmpCategoryModel = GetSubCategories(category, allCategories);

                }
                DateTime? updateDate = allCategories.Where(p => p.ParentCategoryId == categoryModel.Id || p.Id == category.Id).Max(p => p.UpdatedDate);
                tmpCategoryModel.UpdatedDate = updateDate;
                categoryModelListRes.Add(tmpCategoryModel);
            }


            return categoryModel;
        }

        public int AddCategory(CategoryToCreateDto categoryToCreateDto)
        {
            int catId = 0;
            var dbCategory = _dbContext.GetAll()
                            .Where(x => x.CategoryName == categoryToCreateDto.CategoryName && x.Deleted!=true).FirstOrDefault();
            if (dbCategory != null)
            {
                return dbCategory.Id;
            }
            if (categoryToCreateDto.SubCategories != null && categoryToCreateDto.SubCategories.Count > 0)
            {
                Category category = new Category();
                category.CategoryName = categoryToCreateDto.CategoryName;
                category.Description = categoryToCreateDto.Description;
                category.ClosingStatementId = categoryToCreateDto.ClosingStatementId;
                category.OpeningStatementId = categoryToCreateDto.OpeningStatementId;
                category.Url = categoryToCreateDto.Url;
                category.UrlPublicId = categoryToCreateDto.UrlPublicId;
                category.HasChilds = true;
                _appDbContext.Categories.Add(category);
                _appDbContext.SaveChanges();
                catId = category.Id;
                categoryToCreateDto.SubCategories.ForEach(subCateory =>
                {
                    SaveSubCatgory(subCateory, category.Id);
                });
            }
            else
            {
                var category = _mapper.Map<Category>(categoryToCreateDto);
                _appDbContext.Categories.Add(category);
                _appDbContext.SaveChanges();
                catId = category.Id;
            }
            return catId;
        }
        public Category SetCategoryHasChilds(int id)
        {
            var category = _dbContext.GetById(id);
            category.HasChilds = true;
            _dbContext.Update(category);
            return category;
        }
        public int DeleteCategory(int id)
        {
            var category = _dbContext.GetById(id);
            if(category != null&& category.IsSystem ==true) { return -1; }
            category.Deleted = true;
            _dbContext.Update(category);
            return 1;
        }
        public int UpdateCategory(CategoryModel categoryModel)
        {
            var category = _dbContext.GetById(categoryModel.Id);
            if (category == null)
            {
                return 0;
            }
            category.OpeningStatementId = categoryModel.OpeningStatementId;
            category.ClosingStatementId = categoryModel.ClosingStatementId;
            category.CategoryName = categoryModel.CategoryName;
            category.Description = categoryModel.Description;
            category.Url = categoryModel.Url;
            category.UrlPublicId = categoryModel.UrlPublicId;

            _dbContext.Update(category);
            return 1;
        }
        public Category AddSubCategory(SubCategoryToCreateDto subcategoryToCreateDto)
        {

            var dbCategory = new Category();
            //_dbContext.GetAll()
            //                .Where(x => x.CategoryName == subcategoryToCreateDto.CategoryName).FirstOrDefault();
            //if (dbCategory != null)
            //{
            //    return dbCategory;
            //}

            if (subcategoryToCreateDto.SubCategories != null && subcategoryToCreateDto.SubCategories.Count > 0)
            {
                Category category = new Category();
                category.CategoryName = subcategoryToCreateDto.CategoryName;
                category.Description = subcategoryToCreateDto.Description;
                category.ClosingStatementId = subcategoryToCreateDto.ClosingStatementId;
                category.OpeningStatementId = subcategoryToCreateDto.OpeningStatementId;
                category.Url = subcategoryToCreateDto.Url;
                category.UrlPublicId = subcategoryToCreateDto.UrlPublicId;
                category.HasChilds = true;
                category.ParentCategoryId = subcategoryToCreateDto.ParentCategoryId;
                _appDbContext.Categories.Add(category);
                _appDbContext.SaveChanges();

                subcategoryToCreateDto.SubCategories.ForEach(subCateory =>
                {
                    SaveSubCatgory(subCateory, category.Id);
                });
            }
            else
            {
                var category = _mapper.Map<Category>(subcategoryToCreateDto);
                _appDbContext.Categories.Add(category);
                _appDbContext.SaveChanges();
                dbCategory = category;
            }
            return dbCategory;
        }
        private void SaveSubCatgory(SubCategoryToCreateDto categoryToCreateDto, int id)
        {
            if (categoryToCreateDto.SubCategories != null && categoryToCreateDto.SubCategories.Count > 0)
            {
                Category category = new Category();
                category.CategoryName = categoryToCreateDto.CategoryName;
                category.Description = categoryToCreateDto.Description;
                category.ClosingStatementId = categoryToCreateDto.ClosingStatementId;
                category.OpeningStatementId = categoryToCreateDto.OpeningStatementId;
                category.Url = categoryToCreateDto.Url;
                category.UrlPublicId = categoryToCreateDto.UrlPublicId;
                category.HasChilds = true;
                category.ParentCategoryId = id;
                _appDbContext.Categories.Add(category);
                _appDbContext.SaveChanges();
                categoryToCreateDto.SubCategories.ForEach(subCateory =>
                {
                    SaveSubCatgory(subCateory, category.Id);
                });
            }
            else
            {
                Category category = new Category();
                category.CategoryName = categoryToCreateDto.CategoryName;
                category.Description = categoryToCreateDto.Description;
                category.ClosingStatementId = categoryToCreateDto.ClosingStatementId;
                category.OpeningStatementId = categoryToCreateDto.OpeningStatementId;
                category.Url = categoryToCreateDto.Url;
                category.UrlPublicId = categoryToCreateDto.UrlPublicId;
                category.ParentCategoryId = id;
                _appDbContext.Categories.Add(category);
                _appDbContext.SaveChanges();
            }
        }


    }
}

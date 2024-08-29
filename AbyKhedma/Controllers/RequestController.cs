using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AbyKhedma.Services;
using Core.Dtos;
using Core.Models;
using AbyKhedma.Core.Models;
using AbyKhedma.Dtos;
using AbyKhedma.Core.Common;
using AbyKhedma.Pagination;
using System;
using AbyKhedma.Interfaces;
using AbyKhedma.Persistance;
using AbyKhedma.Entities;
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;

namespace AbyKhedma.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RequestController> _logger;
        private readonly RequestService _requestService;
        private readonly RequestFlowService _requestFlowService;
        private readonly ChatMessageService _chatMessageService;
        private readonly RequirementService _requirementService;
        private readonly AppDbContext _appDbContext;
        private readonly IUriService _uriService;
        public RequestController(RequestService requestService, RequestFlowService requestFlowService, ChatMessageService chatMessageService,
            RequirementService requirementService, AppDbContext appDbContext,
            IMapper mapper, IConfiguration configuration, ILogger<RequestController> logger, IUriService uriService)
        {
            _requestService = requestService;
            _requestFlowService = requestFlowService;
            _chatMessageService = chatMessageService;
            _requirementService = requirementService;
            _appDbContext = appDbContext;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _uriService = uriService;
        }

        [HttpGet("getAll/{requesterId}")]
        public ActionResult<List<RequestModel>> GetRequests([FromQuery] FilterDto filterDto, int requesterId)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var requests = _requestService.GetRequestListByRequesterId(requesterId);


            var filteredList = requests.OrderByDescending(r=>r.UpdatedDate)
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize).ToList();
            var totalRecords = requests.Count();
            return Ok(PaginationHelper.CreatePagedReponse<RequestModel>(_mapper.Map<List<RequestModel>>(filteredList), validFilter, totalRecords, _uriService, route));
        }
        [HttpGet("requestDetails/{id}")]
        public ActionResult<RequestModel> GetRequestDetails(int id)
        {
            var request = _requestService.GetRequestById(id);
            if (request == null)
            {
                return NotFound(new { Succeeded = false, Data = new { }, Message = "Not Found", Errors = new string[] { } });
            }
            var requestModel = _mapper.Map<RequestModel>(request);
            return Ok(new { Succeeded = true, Data = requestModel, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("requestHistory/{id}")]
        public ActionResult<RequestModel> GetRequestHistory(int id)
        {
            var request = _requestService.GetRequestById(id);
            if (request == null)
            {
                return NotFound(new { Succeeded = false, Data = new { }, Message = "Not Found", Errors = new string[] { } });
            }
            var requestHistoryDto = _mapper.Map<RequestHistoryDto>(request);
            requestHistoryDto.RequestStatusHistoryList = _appDbContext.RequestAudits.Include(a => a.Status).Where(a => a.RequestId == requestHistoryDto.Id).ToList();
            return Ok(new { Succeeded = true, Data = requestHistoryDto, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("getCompleted/{requesterId}")]
        public ActionResult<List<RequestModel>> GetCompletedRequestsByRequesterId(int requesterId)
        {
            var requestModel = _requestService.GetCompletedRequestListByRequesterId(requesterId);
            return Ok(new { Succeeded = true, Data = requestModel, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("getNotCompleted/{requesterId}")]
        public ActionResult<List<RequestModel>> GetNotCompletedRequestsByRequesterId(int requesterId)
        {
            var requestModel = _requestService.GetNotCompletedRequestListByRequesterId(requesterId);
            return Ok(new { Succeeded = true, Data = requestModel, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("getRequestList")]
        public ActionResult<List<RequestModel>> GetRequestList([FromQuery] FilterDto filterDto)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var requestModelList = _requestService.GetRequestList();

            var filteredList = requestModelList
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = requestModelList.Count();
            return Ok(PaginationHelper.CreatePagedReponse<RequestModel>(filteredList, validFilter, totalRecords, _uriService, route));
        }

        [HttpGet("getRequestListByEmployeeId/{employeeId}")]
        public ActionResult<RequestModel> GetRequestListByEmployeeId([FromQuery] FilterDto filterDto, int employeeId)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var requestModelList = _requestService.GetRequestListByEmployeeId(employeeId);

            var filteredList = requestModelList
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = requestModelList.Count();
            return Ok(PaginationHelper.CreatePagedReponse<RequestModel>(filteredList, validFilter, totalRecords, _uriService, route));
        }
        [HttpPost("getRequestListByCriteria")]
        public ActionResult<RequestModel> GetRequestListByCriteria([FromQuery] FilterDto filterDto, [FromBody] RequestSearchDto searchDto)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var requestModelList = _requestService.GetRequestListByCriteria(searchDto);

            var filteredList = requestModelList
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = requestModelList.Count();
            return Ok(PaginationHelper.CreatePagedReponse<RequestModel>(filteredList, validFilter, totalRecords, _uriService, route));
        }
        [HttpPost("create")]
        public async Task<ActionResult<Task>> CreateRequestAsync(RequestForEditDto requestForEditDto)
        {
            var createdRequest = _requestService.AddRequest(requestForEditDto);
            if (createdRequest != null)
            {
                await _appDbContext.RequestAudits.AddAsync(new RequestAudit { RequestId = createdRequest.Id, StatusId = createdRequest.StatusId });
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Bad Request", Errors = new string[] { } });

            }
            var requirements = _requirementService.GetRequirementsByServiceId(requestForEditDto.ServiceId).OrderBy(r => r.StepOrder).ThenBy(r => r.Id);
            _logger.LogInformation("got requirements");
            var requestFlowListToSave = new List<RequestFlowToCreateDto>();
            foreach (var requirement in requirements)
            {
                requestFlowListToSave.Add(new RequestFlowToCreateDto()
                {
                    EmployeeId = null,
                    RequestId = createdRequest.Id,
                    Status = (int)RequestFlowStatus.NotCompleted,
                    RequirementId = requirement.Id
                });
            }

            await _requestFlowService.AddRequestFlowListAsync(requestFlowListToSave);
            _logger.LogInformation("created request flow");
            var requestFlowModel = _requestFlowService.GetRequestNextUnFinishedStep(createdRequest.Id);
            if (requestFlowModel == null)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "لايوجد متطلبات اخرى للطلب", Errors = new string[] { } });
            }
            _logger.LogInformation("got  Request Next UnFinished Step");
            _chatMessageService.AddChatMessage(new ChatMessageToCreateDto { RequestFlowId = requestFlowModel.Id, ToUserId = requestForEditDto.RequesterId });
            _logger.LogInformation("first step has been added to the chat");
            if (requestForEditDto.AssignedEmployeeId > 0)
            {
                var requester = _appDbContext.Users.FirstOrDefault(u => u.Id == requestForEditDto.RequesterId);
                var service = _appDbContext.Services.FirstOrDefault(u => u.Id == requestForEditDto.ServiceId);
                _appDbContext.AuditLogs.Add(new Entities.AuditLog()
                {
                    UserId = requestForEditDto.AssignedEmployeeId.Value,
                    ActivityTime = DateTime.Now,
                    ActivityDescription = " قام بأنشاء تذكرة جديدة " + service.ServiceName + " للعميل " + " " + requester.FullName
                }
                );
                await _appDbContext.SaveChangesAsync();
            }
            return Ok(new { Succeeded = true, Data = new { RequestId = createdRequest.Id }, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpPost("update")]
        public  ActionResult<Task> UpdateRequest(RequestModel requestModel)
        {
            var identityId = User.Claims.FirstOrDefault(c => c.Type == "identityId");

            var role = User.Claims.FirstOrDefault(c => c.Type == "_role");
            if (identityId == null || role == null)
            {
                return Unauthorized(new { Succeeded = false, Data = new { }, Message = "", Errors = new string[] { } });
            }

            var activityDescription = "";
            var request = _appDbContext.Requests.FirstOrDefault(u => u.Id == requestModel.Id);
            if (request == null)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Bad Request", Errors = new string[] { } });
            }
            var requester = _appDbContext.Users.FirstOrDefault(u => u.Id == request.RequesterId);
            var service = _appDbContext.Services.FirstOrDefault(u => u.Id == request.ServiceId);
            if (request.AssignedEmployeeId != requestModel.AssignedEmployeeId && requestModel.AssignedEmployeeId != null)
            {
                var employee = _appDbContext.Users.FirstOrDefault(u => u.Id == requestModel.AssignedEmployeeId);
                activityDescription = " تم بتعيين الموظف " + employee.FullName + " لخدمة " + service.ServiceName + " للعميل " + " " + requester.FullName;
                _appDbContext.AuditLogs.Add(new Entities.AuditLog()
                {
                    UserId = requestModel.AssignedEmployeeId.Value,
                    ActivityTime = DateTime.Now,
                    ActivityDescription = activityDescription
                }
               );
                  _appDbContext.SaveChanges();
            }
            if (request.StatusId != requestModel.StatusId && requestModel.StatusId > 0)
            {

                if(requestModel.StatusId == (int)RequestStatus.FeEntizarEldaf3)
                {
                    var closeRequestFlow = new RequestFlow { EmployeeId = requestModel.AssignedEmployeeId, RequestId = requestModel.Id, RequirementId = 365, Status = (int)RequestFlowStatus.NotCompleted };
                    _appDbContext.RequestFlows.Add(closeRequestFlow);
                    _appDbContext.SaveChanges();
                    _chatMessageService.AddChatMessage(new ChatMessageToCreateDto { RequestFlowId = closeRequestFlow.Id, ToUserId = request.RequesterId });
                }

                int? empId = null;
                if (Constants.GetSystemRoles().Contains(role.Value))
                {
                    empId = Int32.Parse(identityId.Value);
                }
                  _appDbContext.RequestAudits.Add(new RequestAudit { RequestId = requestModel.Id, StatusId = requestModel.StatusId.Value, EmployeeId = empId });
                  _appDbContext.SaveChanges();
                var status = _appDbContext.StatusLookup.FirstOrDefault(u => u.Id == requestModel.StatusId);
                activityDescription = " تم تغيير الطلب رقم  " + requestModel.Id.ToString() + " عن خدمة" + service.ServiceName + " الى " + status.Title + " للعميل " + " " + requester.FullName;
                _appDbContext.AuditLogs.Add(new Entities.AuditLog()
                {
                    UserId = Int32.Parse(identityId.Value),
                    ActivityTime = DateTime.Now,
                    ActivityDescription = activityDescription
                }
               );

                if (requestModel.StatusId == (int)RequestStatus.TamEltanfeez || requestModel.StatusId == (int)RequestStatus.TamElelghaa)
                {
                    var closeRequestFlow = new RequestFlow { EmployeeId = requestModel.AssignedEmployeeId, RequestId = requestModel.Id, RequirementId = 78, Status = (int)RequestFlowStatus.Completed };
                    _appDbContext.RequestFlows.Add(closeRequestFlow);
                    _appDbContext.SaveChanges();
                    _chatMessageService.AddChatMessage(new ChatMessageToCreateDto { RequestFlowId = closeRequestFlow.Id, ToUserId = request.RequesterId });
                }
                 _appDbContext.SaveChanges();
            }
            _requestService.UpdateRequest(requestModel);
            return Ok(new { Succeeded = true, Data = new { }, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("getDashboardStats")]
        public ActionResult<List<RequestStatusStats>> GetDashboardStats()
        {
            var mySPResult = _appDbContext.RequestStatusStats
                                 .FromSqlInterpolated($"EXEC dbo.sp_DashboardStats")
                                 .ToList();
            return Ok(new { Succeeded = true, Data = mySPResult, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("getEmployeeStats")]
        public ActionResult<List<RequestStatusStats>> GetEmployeeStats([FromQuery] int empId, int nrOfDays)
        {
            var mySPResult = _appDbContext.RequestStatusStats
                                 .FromSqlInterpolated($"EXEC dbo.sp_EmployeeStats {empId} , {nrOfDays}")
                                 .ToList();
            return Ok(new { Succeeded = true, Data = mySPResult, Message = string.Empty, Errors = new string[] { } });
        }
    }
}
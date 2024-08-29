using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AbyKhedma.Services;
using Core.Dtos;
using Core.Models;
using AbyKhedma.Pagination;
using AbyKhedma.Persistance;
using AbyKhedma.Dtos;
using System.Security.Claims;
using AbyKhedma.Core.Common;
using Microsoft.AspNetCore.SignalR;
using AbyKhedma.SignalRHubs;
using AbyKhedma.Entities;
using Microsoft.EntityFrameworkCore;

namespace AbyKhedma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatMessageController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChatMessageController> _logger;
        private readonly IUriService _uriService;
        private readonly IHubContext<NotificationUserHub> _hub;
        private readonly ChatMessageService _chatMessageService;
        private readonly RequestFlowService _requestFlowService;
        private readonly RequirementService _requirementService;
        private readonly RequestService _requestService;
        private readonly AppDbContext _appDbContext;

        public ChatMessageController(ChatMessageService chatMessageService, RequestFlowService requestFlowService, RequirementService requirementService,
            RequestService requestService, AppDbContext appDbContext, IMapper mapper, IConfiguration configuration, ILogger<ChatMessageController> logger, IUriService uriService, IHubContext<NotificationUserHub> hub)
        {
            _chatMessageService = chatMessageService;
            _requestFlowService = requestFlowService;
            _requirementService = requirementService;
            _requestService = requestService;
            this._appDbContext = appDbContext;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _uriService = uriService;
            _hub = hub;
        }
        // need to secure this endpoint only these two peers only can retreive using this requestId
        [HttpGet("getMessages/{requestId}")]
        public ActionResult<List<ChatMessageModel>> GetChatMessageList([FromQuery] FilterDto filterDto, int requestId)
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == "_role");
            if (role == null)
            {
                return Unauthorized(new { Succeeded = false, Data = new { }, Message = "You have to be logged in first", Errors = new string[] { } });
            }

            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var request = _appDbContext.Requests.FirstOrDefault(r => r.Id == requestId);
            if (request == null)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Invalid parameter", Errors = new string[] { } });
            }

            if (role != null && role.Value == Constants.EmployeeRole)
            {
                request.IsShowedByTheEmployee = true;
                _appDbContext.Requests.Update(request);
                _appDbContext.SaveChanges();
            }
            if (role != null && role.Value == Constants.RequesterRole)
            {
                request.IsShowedByTheRequester = true;
                _appDbContext.Requests.Update(request);
                _appDbContext.SaveChanges();
            }
            var requestFlowList = _requestFlowService.GetRequestFlowListByRequestId(requestId).ToList();
            if (requestFlowList == null || requestFlowList.Count == 0)
            {
                return Ok(PaginationHelper.CreatePagedReponse<ChatMessageModel>(new List<ChatMessageModel> { }, validFilter, 0, _uriService, route));
            }
            var chatMessages = _chatMessageService.GetChatMessageList(requestFlowList.Select(rf => rf.Id).ToList()).OrderBy(c => c.Id);

            var requirementModelList = _requirementService.GetSystemRequirementsWithServiceRequirementsByServiceId(request.ServiceId);//xxxxxxxxxxxxxxxxxxx

            var filteredList = chatMessages
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = chatMessages.Count();
            foreach (var chatMessage in chatMessages)
            {
                var tmpRequestFlow = requestFlowList.Where(rf => rf.Id == chatMessage.RequestFlowId).FirstOrDefault();
                chatMessage.Requirement = requirementModelList.Where(r => r.Id == tmpRequestFlow.RequirementId).FirstOrDefault();
            }


            return Ok(PaginationHelper.CreatePagedReponse<ChatMessageModel>(filteredList, validFilter, totalRecords, _uriService, route));
        }

        private async Task<ChatMessageModel> GetChatMessageByIdAsync(ChatMessage chatMessage, int requestId)
        {
            var chatMessageModel = _mapper.Map<ChatMessageModel>(chatMessage);
            var requestFlowList = _requestFlowService.GetRequestFlowListByRequestId(requestId).ToList();
            var request = _requestService.GetRequestById(requestId);
            var requirementModelList = _requirementService.GetRequirementsByServiceId(request.ServiceId);
            var tmpRequestFlow = requestFlowList.Where(rf => rf.Id == chatMessage.RequestFlowId).FirstOrDefault();
            var requirement = requirementModelList.Where(r => r.Id == tmpRequestFlow.RequirementId).FirstOrDefault();

            chatMessageModel.Requirement = requirement;
            if (chatMessage.ToUserId > 0)
            {
                var userChatMessages = _chatMessageService.GetChatMessageList(new List<int> { tmpRequestFlow.Id });
                userChatMessages.ForEach(m =>
                {
                    var tmpRequestFlow = requestFlowList.Where(rf => rf.Id == m.RequestFlowId).FirstOrDefault();
                    var requirement = requirementModelList.Where(r => r.Id == tmpRequestFlow.RequirementId).FirstOrDefault();
                    m.Requirement = requirement;
                });
                await _hub.Clients.User(chatMessage.ToUserId.ToString()).SendAsync("ChatUpdate", userChatMessages);
            }
            return chatMessageModel;
        }

        [HttpPost("add")]
        public async Task<ActionResult<Task>> AddChatMessage(ChatMessageToCreateDto chatMessageToCreateDto)
        {
            var identityClaim = User.Claims.FirstOrDefault(el => el.Type == "identityId");//.FindFirst(ClaimTypes.Role).Value;
            if (identityClaim == null)
            {
                return Unauthorized(new { Succeeded = false, Data = new { }, Message = "You have to be logged in first", Errors = new string[] { } });
            }
            var role = User.Claims.FirstOrDefault(c => c.Type == "_role");
            if (role == null)
            {
                return Unauthorized(new { Succeeded = false, Data = new { }, Message = "You have to be logged in first", Errors = new string[] { } });
            }
            var requestFlow = _appDbContext.RequestFlows.FirstOrDefault(c => c.Id == chatMessageToCreateDto.RequestFlowId);
            if (requestFlow == null)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Invalid request flow Id", Errors = new string[] { } });
            }
            var dbRequirement = _appDbContext.Requirements.FirstOrDefault(r => r.Id == requestFlow.RequirementId);
            if (dbRequirement == null)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Invalid requirement id", Errors = new string[] { } });
            }
            if (dbRequirement.RequirementType == 2 && (chatMessageToCreateDto.AnswerId == null || chatMessageToCreateDto.AnswerId == 0))
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Error: Answer Id is missing", Errors = new string[] { } });
            }
            var ChatMessage = new ChatMessage();
            ChatMessage = _chatMessageService.UpdateChatMessage(chatMessageToCreateDto);
            if (ChatMessage == null || ChatMessage.Id == 0)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Failed to save", Errors = new string[] { } });
            }

            var request = _appDbContext.Requests.FirstOrDefault(r=>r.Id==requestFlow.RequestId);
            if (request != null)
            {
                if (role != null && role.Value == Constants.RequesterRole)
                {
                    request.IsShowedByTheEmployee = false;
                }
                if (role != null && role.Value == Constants.EmployeeRole)
                {
                    request.IsShowedByTheEmployee = false;
                    request.IsLockedToDateTime = DateTime.UtcNow.AddMinutes(Constants.LockedRequestTimeInMin);
                    request.AssignedEmployeeId =Int32.Parse(identityClaim.Value);
                }
                request.UpdatedDate = DateTime.Now;
                if (requestFlow.RequirementId == 78)//archive the request
                {
                    request.IsArchived = true; ;
                }
                _appDbContext.Requests.Update(request);
                _appDbContext.SaveChanges();
            }

            if (chatMessageToCreateDto.AnswerId > 0)
            {
                _requestFlowService.UpdateRequestFlow(new RequestFlowModel() { Id = chatMessageToCreateDto.RequestFlowId, Status = (int)RequestFlowStatus.Completed });
                var requestFlowModel = _requestFlowService.GetRequestNextUnFinishedStep(requestFlow.RequestId.Value);
                if (requestFlowModel != null)
                {
                    _logger.LogInformation("got  Request Next UnFinished Step");
                    var ChatMessage2 = _chatMessageService.AddChatMessage(new ChatMessageToCreateDto { RequestFlowId = requestFlowModel.Id, ToUserId = chatMessageToCreateDto.FromUserId });
                    if (ChatMessage2 == null)
                    {
                        return Ok(new { Succeeded = true, Data = new { }, Message = "Pending dashboard approval", Errors = new string[] { } });
                    }
                    await GetChatMessageByIdAsync(ChatMessage2, requestFlow.RequestId.Value);
                    _logger.LogInformation("first step has been added to the chat");
                }
            }
            return Ok(new { Succeeded = true, Data = new { Id = ChatMessage.Id }, Message = string.Empty, Errors = new string[] { } });
        }
    }
}
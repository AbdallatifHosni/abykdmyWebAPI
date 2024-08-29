using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AbyKhedma.Services;
using Core.Models;
using AbyKhedma.Pagination;
using AbyKhedma.Core.Common;
using AbyKhedma.Persistance;
using Microsoft.EntityFrameworkCore;
using AbyKhedma.SignalRHubs;
using Microsoft.AspNetCore.SignalR;
using AbyKhedma.Entities;
using FirebaseAdmin.Messaging;
using Message = FirebaseAdmin.Messaging.Message;
using Core.Dtos;

namespace AbyKhedma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestFlowController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RequestFlowController> _logger;
        private readonly IUriService _uriService;
        private readonly RequestFlowService _requestFlowService;
        private readonly AppDbContext _appDbContext;
        private readonly IHubContext<NotificationUserHub> _notificationUserHubContext;
        private readonly ChatMessageService _chatMessageService;
        private readonly IHubContext<NotificationUserHub> _hub;

        public RequestFlowController(RequestFlowService requestFlowService, AppDbContext appDbContext, IMapper mapper, IConfiguration configuration, ILogger<RequestFlowController> logger, IUriService uriService,
            IHubContext<NotificationUserHub> notificationUserHubContext, ChatMessageService chatMessageService, IHubContext<NotificationUserHub> hub)
        {
            _requestFlowService = requestFlowService;
            _appDbContext = appDbContext;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _uriService = uriService;
            _notificationUserHubContext = notificationUserHubContext;
            this._chatMessageService = chatMessageService;
            this._hub = hub;
        }

        [HttpPost("getByCriteria")]
        public ActionResult<List<RequestFlowModel>> GetRequestFlowByCriteria([FromBody] RequestFlowModel requestFlowModel)
        {
            var requestFlowModelList = _requestFlowService.GetRequestFlowByCriteria(requestFlowModel);
            return Ok(new { Succeeded = true, Data = requestFlowModelList, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("GetRequestFinishedLastStep/{requestId}")]
        public ActionResult<RequestFlowModel> GetRequestFlowLastFinishedStepByCriteria(int requestId)
        {
            var requestFlowModelList = _requestFlowService.GetRequestLastFinishedStep(requestId);
            return Ok(new { Succeeded = true, Data = requestFlowModelList, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("GetRequestNextUnFinishedStep/{requestId}")]
        public ActionResult<RequestFlowModel> GetRequestNextUnFinishedStep(int requestId)
        {
            var requestFlowModelList = _requestFlowService.GetRequestNextUnFinishedStep(requestId);
            return Ok(new { Succeeded = true, Data = requestFlowModelList, Message = string.Empty, Errors = new string[] { } });
        }



        [HttpPost("update")]
        public ActionResult<Task> UpdateRequestFlow(RequestFlowModel requestFlowModel)
        {
            var identityId = User.Claims.FirstOrDefault(c => c.Type == "identityId");
            var role = User.Claims.FirstOrDefault(c => c.Type == "_role");
            if (role is null || role.Value != Constants.EmployeeRole)
            {
                return Unauthorized(new { Succeeded = false, Data = new { }, Message = "You are not authorized to access this resource", Errors = new string[] { } });
            }
            var dbRequestFlow = _requestFlowService.UpdateRequestFlow(requestFlowModel);


            if (dbRequestFlow == null)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = string.Empty, Errors = new string[] { } });
            }
            var request = new Request();
            var nextRequestFlowModel = _requestFlowService.GetRequestNextUnFinishedStep(dbRequestFlow.RequestId.Value);
            if (nextRequestFlowModel != null)
            {
                 request = _appDbContext.Requests.FirstOrDefault(r => r.Id == dbRequestFlow.RequestId);
                _logger.LogInformation("got  Request Next UnFinished Step");
                var ChatMessage2 = _chatMessageService.AddChatMessage(new ChatMessageToCreateDto { RequestFlowId = nextRequestFlowModel.Id, ToUserId = request.RequesterId });
             //  var newMessage=    GetChatMessageByIdAsync(ChatMessage2, nextRequestFlowModel.RequestId);
                _logger.LogInformation("first step has been added to the chat");
            }
            else
            {
                return Ok(new { Succeeded = true, Data = new { }, Message = string.Empty, Errors = new string[] { } });
            }

            
            if (request != null&&request.Id>0)
            {
                request.UpdatedDate = DateTime.Now;
                _appDbContext.Requests.Update(request);
                _appDbContext.SaveChanges();
            }

            if (role.Value == Constants.EmployeeRole)
            {
                var requestFlow = _appDbContext.RequestFlows.FirstOrDefault(r => r.Id == requestFlowModel.Id);
                var requirement = _appDbContext.Requirements.FirstOrDefault(r => r.Id == requestFlow.RequirementId);
                var requestFlowStatus = "";
                switch (requestFlowModel.Status)
                {
                    case 1:
                        requestFlowStatus = "قيد المراجعة";
                        break;
                    case 2:
                    default:
                        requestFlowStatus = "تمت الموافقة";
                        break;
                }
                _appDbContext.AuditLogs.Add(new Entities.AuditLog()
                {
                    UserId = Int32.Parse(identityId.Value),
                    ActivityTime = DateTime.Now,
                    ActivityDescription = " قام بعمل " + requestFlowStatus + "  عن رد العميل ل " + requirement.Description
                }
                );

                _appDbContext.SaveChangesAsync();
                try
                {
                    var message = new Message()
                    {
                        Notification = new Notification
                        {
                            Title = "ابى خدمة",
                            Body = "جرى تحديث على الطلب الخاص بكم - " + request.Description,
                        },
                        Data = new Dictionary<string, string>()
                        {
                            ["CustomData"] = "جرى تحديث على الطلب الخاص بكم - " + request.Description
                        },
                        Token = request.Requester.DeviceToken
                    };

                    var messaging = FirebaseMessaging.DefaultInstance;
                    messaging.SendAsync(message);
                }
                catch
                {
                }


                //_notificationUserHubContext.Clients.Client(requestFlow.User.Id.ToString()).SendAsync("UserNotifications", message);

            }

            return Ok(new { Succeeded = true, Data = new { }, Message = string.Empty, Errors = new string[] { } });
        }

        private async Task<ChatMessageModel> GetChatMessageByIdAsync(ChatMessage chatMessage, int requestId)
        {
            var chatMessageModel = _mapper.Map<ChatMessageModel>(chatMessage);
            var requestFlowList = _requestFlowService.GetRequestFlowListByRequestId(requestId).ToList();
            var request = _appDbContext.Requests.FirstOrDefault(r=>r.Id==requestId);
            var requirementModelList =_mapper.Map<List<RequirementModel>>( _appDbContext.Requirements.Where(r=>r.ServiceID== request.ServiceId));
            var tmpRequestFlow = requestFlowList.Where(rf => rf.Id == chatMessage.RequestFlowId).FirstOrDefault();
            var newRequirement = requirementModelList.Where(r => r.Id == tmpRequestFlow.RequirementId).FirstOrDefault();

            chatMessageModel.Requirement = newRequirement;
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



        [HttpPost("notifications")]
        public ActionResult<Task> GetNotifications()
        {
            var identityId = User.Claims.FirstOrDefault(c => c.Type == "identityId");
            //var role = User.Claims.FirstOrDefault(c => c.Type == "_role");
            if (identityId == null)
            {
                return Unauthorized();
            }
            _notificationUserHubContext.Clients.Client(identityId.ToString()).SendAsync("UserNotifications", "تحديث على الطلب الخاص بكم - " + identityId.ToString());
            return Ok();
        }
    }
}
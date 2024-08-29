using AbyKhedma.Core.Common;
using AbyKhedma.Core.Models;
using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using AutoMapper;
using Core.Dtos;
using Core.Models;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AbyKhedma.Services
{
    public class RequestFlowService
    {
        private readonly IRequestFlowRepository _dbContext;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public RequestFlowService(IRequestFlowRepository dbContext, AppDbContext appDbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            this._appDbContext = appDbContext;
            _mapper = mapper;
        }
        public List<RequestFlowModel> GetRequestFlowByCriteria(RequestFlowModel requestFlowModel)
        {
            var requestFlows = _dbContext.GetAll().Where(r =>
            (r.Id == requestFlowModel.Id || requestFlowModel.Id == 0) &&
            (r.RequestId == requestFlowModel.RequestId || requestFlowModel.RequestId == 0) &&
            (r.RequirementId == requestFlowModel.Requirement.Id || requestFlowModel.Requirement.Id == 0)
            ).ToList();
            return _mapper.Map<List<RequestFlowModel>>(requestFlows);
        }
        public RequestFlowModel GetRequestLastFinishedStep(int requestId)
        {
            var requestFlows = _appDbContext.RequestFlows.Where(r => r.RequestId == requestId && r.Status == (int)RequestFlowStatus.Completed).ToList();
            if (requestFlows.Count == 0)
            {
                return null;
            }
            int lastRequestFlowId = requestFlows.Max(r => r.Id);
            var requestFlow = requestFlows.FirstOrDefault(r => r.Id == lastRequestFlowId);
            var requestModel = _mapper.Map<RequestFlowModel>(requestFlow);
            requestModel.Requirement = _mapper.Map<RequirementModel>(_appDbContext.Requirements.Include(r => r.Answers).FirstOrDefault(r => requestFlow.RequirementId != null && r.Id == requestFlow.RequirementId));
            return requestModel;
        }

        public List<RequestFlowModel> GetRequestFlowListByRequestId(int requestId)
        {
            var requestFlows = _appDbContext.RequestFlows.Where(r => r.RequestId == requestId).ToList();
            var requestModel = _mapper.Map<List<RequestFlowModel>>(requestFlows);
            return requestModel;
        }
        public    RequestFlowModel  GetRequestFlowById(int id)
        {
            var requestFlow =  _appDbContext.RequestFlows.FirstOrDefault(r => r.Id == id) ;
            var requestModel = _mapper.Map<RequestFlowModel>(requestFlow);
            return requestModel;
        }
        public RequestFlowModel GetRequestNextUnFinishedStep(int requestId)
        {
            var requestFlows = _appDbContext.RequestFlows.Where(r => r.RequestId == requestId && r.Status == (int)RequestFlowStatus.NotCompleted).ToList();
            if (requestFlows.Count == 0)
            {
                return null;
            }
            int nextRequestFlowId = requestFlows.Min(r => r.Id);
            var requestFlow = requestFlows.FirstOrDefault(r => r.Id == nextRequestFlowId);
            var requestModel = _mapper.Map<RequestFlowModel>(requestFlow);
            requestModel.Requirement = _mapper.Map<RequirementModel>(_appDbContext.Requirements.Include(r=>r.Answers).FirstOrDefault(r => requestFlow.RequirementId != null && r.Id == requestFlow.RequirementId));
            return requestModel;
        }

        public int AddRequestFlow(RequestFlowToCreateDto requestFlow)
        {
            var requestFlowToCreate = _mapper.Map<RequestFlow>(requestFlow);
            var createdRequestFlow = _dbContext.Create(requestFlowToCreate);
            return createdRequestFlow.Id;
        }
        public async Task<int> AddRequestFlowListAsync(List<RequestFlowToCreateDto> requestFlowList)
        {
            var requestFlowEntityList = _mapper.Map<List<RequestFlow>>(requestFlowList);
            await _appDbContext.RequestFlows.AddRangeAsync(requestFlowEntityList);
            await _appDbContext.SaveChangesAsync();
            return 1;
        }
        public RequestFlow UpdateRequestFlow(RequestFlowModel requestFlow)
        {
            var dbRequestFlows = _dbContext.GetAll().FirstOrDefault(r => r.Id == requestFlow.Id);
            if (dbRequestFlows == null)
            {
                return null;
            }
            dbRequestFlows.Status = requestFlow.Status;
            dbRequestFlows.UpdatedDate = DateTime.UtcNow;
            _dbContext.Update(dbRequestFlows);
            return dbRequestFlows;
        }
    }
}

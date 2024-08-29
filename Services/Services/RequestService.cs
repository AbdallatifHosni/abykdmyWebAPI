using AbyKhedma.Core.Common;
using AbyKhedma.Core.Models;
using AbyKhedma.Dtos;
using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using AutoMapper;
using Core.Dtos;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace AbyKhedma.Services
{
    public class RequestService
    {
        private readonly IRequestRepository _dbContext;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public RequestService(IRequestRepository dbContext, AppDbContext appDbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public List<RequestModel> GetCompletedRequestListByRequesterId(int requesterId)
        {
            var requests = _appDbContext.Requests.Include(r => r.Service)
                .Include(r => r.Requester).Include(r => r.Employee).Include(r => r.StatusLookup).Where(r => r.RequesterId == requesterId && r.StatusId == (int)RequestStatus.TamEltanfeez).ToList();
            return _mapper.Map<List<RequestModel>>(requests);
        }
        public List<RequestModel> GetNotCompletedRequestListByRequesterId(int requesterId)
        {
            var requests = _appDbContext.Requests.Include(r => r.Service).Include(r => r.Requester).Include(r => r.Employee).Include(r => r.StatusLookup).Where(r => r.RequesterId == requesterId && Constants.GetInCompletedRequestStatusList().Contains(r.StatusId)).ToList();
            return _mapper.Map<List<RequestModel>>(requests);
        }
        public List<RequestModel> GetRequestListByEmployeeId(int employeeId)
        {
            var requests = _appDbContext.Requests.Include(r => r.Service).ThenInclude(s => s.Category).Include(r => r.Requester).Include(r => r.Employee).Include(r => r.StatusLookup).Where(r => r.AssignedEmployeeId != null && r.AssignedEmployeeId == employeeId).ToList();

            return _mapper.Map<List<RequestModel>>(requests);
        }
        public List<RequestModel> GetRequestListByCriteria(RequestSearchDto searchDto)
        {
            var requests = _appDbContext.Requests.Include(r => r.Service).Include(r => r.Requester).Include(r => r.Employee).Include(r => r.StatusLookup)
                .Where(r => ((r.AssignedEmployeeId != null && r.AssignedEmployeeId == searchDto.AssignedEmployeeId) || searchDto.AssignedEmployeeId == null)
                && (searchDto.StatusId == null || r.StatusId == searchDto.StatusId)
                && (searchDto.RequestId == null || r.Id == searchDto.RequestId)
                && (searchDto.RequesterPhoneNumber == null || (r.Requester.PhoneNumber != null && r.Requester.PhoneNumber == searchDto.RequesterPhoneNumber))
                && (searchDto.RequesterName == null || (r.Requester.FullName != null && r.Requester.FullName.Trim().Replace(" ","").Contains(searchDto.RequesterName.Trim().Replace(" ", ""))))
                ).ToList();
            return _mapper.Map<List<RequestModel>>(requests);
        }
        public List<RequestModel> GetRequestList()
        {
            var requests = _appDbContext.Requests.Include(r => r.Service).Include(r => r.Requester).Include(r => r.Employee).Include(r => r.StatusLookup).ToList();
            var dbRequests = new List<Request>();
            requests.ForEach(r =>
            {
                if (r.IsLockedToDateTime < DateTime.Now)
                {
                    r.AssignedEmployeeId = null;
                    r.IsLockedToDateTime = null;
                    dbRequests.Add(r);
                }
            }
            );
            _appDbContext.Requests.UpdateRange(dbRequests);
            _appDbContext.SaveChanges();
            return _mapper.Map<List<RequestModel>>(requests);
        }
        public IQueryable<Request> GetRequestListByRequesterId(int requesterId)
        {
            var requests = _appDbContext.Requests.Where (r=>r.IsArchived!=true).Include(r => r.Service).ThenInclude(s => s.Category).Include(r => r.Requester).Include(r => r.Employee).Include(r => r.StatusLookup).Where(r => r.RequesterId == requesterId);
            return requests;
        }
        public Request GetRequestById(int id)
        {
            var request = _appDbContext.Requests.Include(r => r.Service).Include(r => r.Requester).Include(r => r.Employee).Include(r => r.StatusLookup).FirstOrDefault(r => r.Id == id);
            return request;
        }
        public Request AddRequest(RequestForEditDto request)
        {
            var requestToCreate = _mapper.Map<Request>(request);
            var service = _appDbContext.Services.FirstOrDefault(s => s.Id == request.ServiceId);
            requestToCreate.Description = service.ServiceName;
            requestToCreate.UpdatedDate = DateTime.Now;
            _appDbContext.Requests.Add(requestToCreate);
            _appDbContext.SaveChanges();
            return requestToCreate;
        }
        public int UpdateRequest(RequestModel requestModel)
        {
            var dbRequest = _dbContext.GetAll().FirstOrDefault(r => r.Id == requestModel.Id);
            if (dbRequest == null)
            {
                return 0;
            }
            if (requestModel?.StatusId > 0)
            {
                dbRequest.StatusId = requestModel.StatusId.Value;
            }


            if ((dbRequest.IsLockedToDateTime == null || dbRequest.IsLockedToDateTime < DateTime.UtcNow) && requestModel?.AssignedEmployeeId != null)
            {
                dbRequest.AssignedEmployeeId = requestModel.AssignedEmployeeId;
                dbRequest.IsLockedToDateTime = DateTime.UtcNow.AddMinutes(Constants.LockedRequestTimeInMin);
            }

            _dbContext.Update(dbRequest);
            return 1;
        }

    }
}

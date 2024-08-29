using AbyKhedma.Core.Common;
using AbyKhedma.Entities;
using AutoMapper;
using Core.Dtos;
using Core.Models;
using Infrastructure.Interfaces;
using System.Data;

namespace AbyKhedma.Services
{
    public class StatementService
    {
        private readonly IStatementRepository _dbContext;
        private readonly IMapper _mapper;

        public StatementService(IStatementRepository dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public List<StatementModel> GetStatementList()
        {
            var  statements = _dbContext.GetAll().ToList();
            return _mapper.Map<List<StatementModel>>(statements);
        }
        public StatementModel GetStatementById(int id)
        {
            var statement = _dbContext.GetAll().FirstOrDefault(s => s.Id == id);
            return _mapper.Map<StatementModel>(statement);
        }
        public List<StatementModel> GetOpeningStatements()
        {
            var services = _dbContext.GetAll().Where(s => s.StatemenType ==(int)StatementTypes.OpeningStatement).ToList();
            return _mapper.Map<List<StatementModel>>(services);
        }
        public List<StatementModel> GetClosingStatements()
        {
            var services = _dbContext.GetAll().Where(s => s.StatemenType == (int)StatementTypes.ClosingingStatement).ToList();
            return _mapper.Map<List<StatementModel>>(services);
        }
        public int AddStatement(StatementToCreateDto  statementToCreateDto)
        {
            var statement = _dbContext.GetAll()
                            .Where(x => x.StatemenType == statementToCreateDto.StatemenType && x.StatementText == statementToCreateDto.StatementText).FirstOrDefault();
            if (statement != null)
            {
                return statement.Id;
            }
            var statementToCreate = _mapper.Map<Statement>(statementToCreateDto);
            var createdStatement = _dbContext.Create(statementToCreate);
            return createdStatement.Id;
        }
    }
}

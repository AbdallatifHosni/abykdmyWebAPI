using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using AutoMapper;
using Core.Dtos;
using Core.Models;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace AbyKhedma.Services
{
    public class SubAnswerRequirementService
    {

        private readonly AppDbContext _appDbContext;


        public SubAnswerRequirementService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void AddSubAnswerRequirement(SubAnswerRequirement subAnswerRequirement)
        {
            _appDbContext.SubAnswerRequirements.Add(subAnswerRequirement);
            _appDbContext.SaveChanges();
        }
    }
}

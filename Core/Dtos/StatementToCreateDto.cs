using AbyKhedma.Entities;
using Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Core.Dtos
{
    public class StatementToCreateDto
    {
        public string StatementText { get; set; }
        public int StatemenType { get; set; }
    }
}

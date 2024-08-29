using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using AutoMapper;
using Core.Dtos;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AbyKhedma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttachmentController : ControllerBase
    {

        private readonly IAttachmentService _attachmentService;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public AttachmentController(IAttachmentService attachmentService, AppDbContext dbContext, IMapper mapper)
        {
            _attachmentService = attachmentService;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpPost("add-file")]
        public async Task<ActionResult<AttachmentDto>> AddFile(IFormFile file)
        {
            var result = await _attachmentService.AddFileAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var fileDto = new AttachmentDto
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                FileType=1
                
            };
            var fileToSave = _mapper.Map<Attachment>(fileDto);
            _dbContext.Files.Add(fileToSave);
            await _dbContext.SaveChangesAsync();
            return Ok(new { Succeeded = true, Data = fileToSave, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpPost("add-record")]
        public async Task<ActionResult<AttachmentDto>> AddRecord(IFormFile file)
        {
            var result = await _attachmentService.AddAudioVideoFileAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var fileDto = new AttachmentDto
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                FileType = 2
            };
            var fileToSave = _mapper.Map<Attachment>(fileDto);
            _dbContext.Files.Add(fileToSave);
            await _dbContext.SaveChangesAsync();
            return Ok(new { Succeeded = true, Data = fileToSave, Message = string.Empty, Errors = new string[] { } });
        }


        [HttpPost("add-raw-file")]
        public async Task<ActionResult<AttachmentDto>> AddRawFile(IFormFile file)
        {
            var result = await _attachmentService.UploadRawFileAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var fileDto = new AttachmentDto
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                FileType = 3
            };
            var fileToSave = _mapper.Map<Attachment>(fileDto);
            _dbContext.Files.Add(fileToSave);
            await _dbContext.SaveChangesAsync();
            return Ok(new { Succeeded = true, Data = fileToSave, Message = string.Empty, Errors = new string[] { } });
        }

        [HttpDelete("delete-file/{fileId}")]
        public async Task<ActionResult> DeleteFile(string fileId)
        {
            if (!string.IsNullOrEmpty(fileId) && !string.IsNullOrWhiteSpace(fileId))
            {
                var result = await _attachmentService.DeleteFileAsync(fileId);
                if (result.Error != null)
                {
                    return BadRequest(result.Error.Message);
                }
                else
                {
                    var fileToRemove = await _dbContext.Files.FirstOrDefaultAsync(f => f.PublicId == fileId);
                    if (fileToRemove != null)
                    {
                        _dbContext.Files.Remove(fileToRemove);
                        await _dbContext.SaveChangesAsync();
                    }
                    return Ok(new { Succeeded = true, Data = new { },  Errors = new string[] { } });
                }
            }
            return BadRequest(new { Succeeded = false, Data = new { }, Message = "Failed to delete the file", Errors = new string[] { } });
        }

    }
}
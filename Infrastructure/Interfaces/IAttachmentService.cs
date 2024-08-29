using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Interfaces
{
    public interface IAttachmentService
    {
        Task<ImageUploadResult> AddFileAsync(IFormFile file);
        Task<VideoUploadResult> AddAudioVideoFileAsync(IFormFile file);
        Task<DeletionResult> DeleteFileAsync(string publicId);
        Task<RawUploadResult> UploadRawFileAsync(IFormFile file);
    }
}

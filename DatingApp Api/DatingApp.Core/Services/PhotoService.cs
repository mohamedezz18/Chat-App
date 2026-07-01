using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.Core.ServiceContracts;
using DatingApp.Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatingApp.Core.Services
{
    public class PhotoService : IPhotoService
    {
        public readonly Cloudinary _Cloudinary; 

        public PhotoService(IOptions<CloudinarySettings> cloudinarySettings)
        {
            var account = new Account(cloudinarySettings.Value.CloudName,
                cloudinarySettings.Value.ApiKey, cloudinarySettings.Value.ApiSecret);

            _Cloudinary = new Cloudinary(account);

        }
        
        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            return await _Cloudinary.DestroyAsync(deleteParams);
        }

        public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file)
        {
            var UploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var UpdloadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "da-ang200"
                };

                UploadResult = await _Cloudinary.UploadAsync(UpdloadParams);

            }
            return UploadResult;
        }
    }
}

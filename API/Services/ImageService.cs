using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace API.Services
{
    public class ImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly bool _isConfigured;

        public ImageService(IConfiguration config)
        {
            var cloudName = config["Cloudinary:CloudName"];
            var apiKey = config["Cloudinary:ApiKey"];
            var apiSecret = config["Cloudinary:ApiSecret"];
            _isConfigured = !string.IsNullOrWhiteSpace(cloudName)
                && !string.IsNullOrWhiteSpace(apiKey)
                && !string.IsNullOrWhiteSpace(apiSecret);

            if (_isConfigured)
            {
                _cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
            }
        }

        public async Task<ImageUploadResult> AddImageAsync(IFormFile file)
        {
            if (!_isConfigured)
            {
                throw new InvalidOperationException("Cloudinary credentials are not configured.");
            }

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream)
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            if (!_isConfigured)
            {
                throw new InvalidOperationException("Cloudinary credentials are not configured.");
            }

            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}
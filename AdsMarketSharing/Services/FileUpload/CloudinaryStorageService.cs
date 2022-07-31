﻿using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Models;
using AdsMarketSharing.DTOs.File;
using AdsMarketSharing.Enum;

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace AdsMarketSharing.Services.FileUpload
{
    public class CloudinaryStorageService : IFileStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly Account _cloudinaryAccount;

        public CloudinaryStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            string cloud = _configuration.GetSection("AppSettings:CloudinaryURL:CloudName").Value;
            string apiKey = _configuration.GetSection("AppSettings:CloudinaryURL:ApiKey").Value;
            string apiSecret = _configuration.GetSection("AppSettings:CloudinaryURL:ApiSecret").Value;
            _cloudinaryAccount = new Account(cloud,apiKey,apiSecret);
        }
        public async Task<ServiceResponse<AttachmentResponseDTO>> SaveImage(string newFileName, Stream fileStream, string newFolder)
        {
            var cloudinary = new Cloudinary(_cloudinaryAccount);
            var response = new ServiceResponse<AttachmentResponseDTO>();
            try
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(newFileName, fileStream),
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = true,
                    Folder = newFolder,

                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new ServiceResponseException<ResponseStatus>(400, ResponseStatus.Failed, uploadResult.Error.Message);
                }

                response.Data = new AttachmentResponseDTO
                {
                    FilePath = uploadResult.Url.AbsoluteUri,
                    FileSize= fileStream.Length,
                    FileTag= uploadResult.Format,
                    FileType= uploadResult.ResourceType,
                    Name= uploadResult.PublicId,
                    PublicPath=uploadResult.Url.AbsoluteUri,
                };
                response.Message = "Save image successfully";
                response.ServerMessage = "Completed service";
                response.Status = ResponseStatus.Successed;
                response.StatusCode = 200;
            }
            catch (ServiceResponseException<ResponseStatus> exception)
            {
                response.Data = null;
                response.Message = "Save image failed";
                response.ServerMessage = exception.Message;
                response.Status = exception.Value;
                response.StatusCode = exception.StatusCode;
            }
            return response;
        }
        public async Task<ServiceResponse<string>> CreateFolder(string folderName)
        {
            var cloudinary = new Cloudinary(_cloudinaryAccount);
            var response = new ServiceResponse<string>();
            try
            {
                var createFolderResult = await cloudinary.CreateFolderAsync($"AdsMarketSharing/{folderName}");

                if (createFolderResult.Error != null)
                {
                    throw new ServiceResponseException<ResponseStatus>(400, ResponseStatus.Failed, createFolderResult.Error.Message);
                }
                response.Data = createFolderResult.Path;
                response.Message = "Create folder success";
                response.ServerMessage = "Completed service";
                response.Status = ResponseStatus.Successed;
                response.StatusCode = 201;
            }
            catch (ServiceResponseException<ResponseStatus> exception)
            {
                response.Data = "";
                response.Message = "Create folder failed";
                response.ServerMessage = exception.Message;
                response.Status = exception.Value;
                response.StatusCode = exception.StatusCode;
            }
            return response;
        }





        public async Task<ServiceResponse<bool>> AddNewFile(IFormFile formFile)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var cloudinary = new Cloudinary(_cloudinaryAccount);
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(formFile.FileName,formFile.ContentDisposition),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true
                };
                var uploadResult = cloudinary.Upload(uploadParams);

                Console.WriteLine(uploadResult.JsonObj);

                response.Data = true;
                response.ServerMessage = "";
                response.Message = "";
                response.Status = ResponseStatus.Successed;
                response.StatusCode = 200;
            }
            catch (ServiceResponseException<ResponseStatus> exception)
            {
                response.Data = true;
                response.ServerMessage = exception.Message;
                response.Message = "";
                response.Status = exception.Value;
                response.StatusCode = exception.StatusCode;
            }
            return response;
        }
        public Task<ServiceResponse<bool>> DeleteFile(IFormFile formFile)
        {
            throw new System.NotImplementedException();
        }
        public Task<ServiceResponse<bool>> EditFile(IFormFile formFile)
        {
            throw new System.NotImplementedException();
        }
        public Task<ServiceResponse<bool>> ReadFile(IFormFile formFile)
        {
            throw new System.NotImplementedException();
        }
        public Task<ServiceResponse<bool>> SaveFile(IFormFile formFile)
        {
            throw new NotImplementedException();
        }


    }
}
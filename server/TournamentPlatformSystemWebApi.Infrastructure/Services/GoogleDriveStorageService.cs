using System;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using TournamentPlatformSystemWebApi.Application.Interfaces;

namespace TournamentPlatformSystemWebApi.Infrastructure.Services;

// Note: requires NuGet package: Google.Cloud.Storage.V1 and Google.Apis.Auth
public class GoogleDriveStorageService : IStorageService
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;

    public GoogleDriveStorageService(IConfiguration configuration)
    {
        var gd = configuration.GetSection("GoogleCloud");
        var serviceAccountJson = gd["ServiceAccountJson"] ?? string.Empty;
        _bucketName = gd["BucketName"] ?? string.Empty;

        if (string.IsNullOrWhiteSpace(serviceAccountJson))
            throw new ArgumentException("Google Cloud service account JSON must be configured (GoogleCloud:ServiceAccountJson)");

        if (string.IsNullOrWhiteSpace(_bucketName))
            throw new ArgumentException("Google Cloud bucket name must be configured (GoogleCloud:BucketName)");

        var credential = GoogleCredential.FromJson(serviceAccountJson);
        _storageClient = StorageClient.Create(credential);
    }

    public async Task<StorageUploadResult> UploadAsync(Stream stream, string fileName, string contentType)
    {
        try
        {
            if (stream.CanSeek)
                stream.Position = 0;

            var uploadOptions = new UploadObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.PublicRead
            };

            var obj = await _storageClient.UploadObjectAsync(_bucketName, fileName, contentType, stream, uploadOptions);

            var url = $"https://storage.googleapis.com/{_bucketName}/{Uri.EscapeDataString(obj.Name)}";

            return new StorageUploadResult(obj.Name, url);
        }
        catch (Exception ex)
        {
            throw new Exception("Failedo upload file to Google Cloud Storage: " + ex.Message, ex);
        }
    }
}

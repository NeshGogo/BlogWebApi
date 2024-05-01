using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Contracts;
using Domain.Storages;

namespace Persistence.Repositories
{
    internal class FileStorageAzureRepository : IFileStorage
    {
        private readonly ILoggerManager _loggerManager;
        private readonly BlobServiceClient _blobServiceClient;

        public FileStorageAzureRepository(BlobServiceClient blobServiceClient, ILoggerManager loggerManager)
        {
            _loggerManager = loggerManager;
            _blobServiceClient = blobServiceClient;
        }

        public Task<string> EditFileAsync(byte[] content, string extension, string container, string route, string contentType, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFileAsync(string container, string route, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SaveFileAsync(byte[] content, string extension, string container, string contentType, CancellationToken cancellation = default)
        {
            _loggerManager.LogInfo("--> saving file to azure storage....");
           
            var containerClient =  _blobServiceClient.GetBlobContainerClient(container.ToLower());
            await containerClient.CreateIfNotExistsAsync();

            var fileName = $"{Guid.NewGuid()}{extension}";
            var blobClient = containerClient.GetBlobClient(fileName);

            var stream = new MemoryStream(content);
            await blobClient.UploadAsync(stream, cancellation);

            _loggerManager.LogInfo($"--> File {fileName} uploaded to azure storage successfully!");
            
            return blobClient.Uri.ToString();             
        }
    }
}

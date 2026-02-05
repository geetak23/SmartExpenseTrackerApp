using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace SmartExpenseTracker.Infrastructure.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(IConfiguration config)
        {
            var client = new BlobServiceClient(
                config["AzureBlob:ConnectionString"]);

            _container = client.GetBlobContainerClient("receipts");
            _container.CreateIfNotExists();
        }

        public async Task<string> UploadAsync(Stream stream, string fileName)
        {
            var blob = _container.GetBlobClient(fileName);
            await blob.UploadAsync(stream, overwrite: true);
            return blob.Uri.ToString();
        }
    }
}

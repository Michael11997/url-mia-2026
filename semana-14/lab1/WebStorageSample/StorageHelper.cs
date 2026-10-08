using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Blobs;

namespace WebStorageSample
{
    public class StorageHelper
    {
        private static BlobContainerClient GetContainerClient(
            string containerEndpoint,
            string containerName)
        {
            var blobContainerUri =
                new Uri(new Uri(containerEndpoint), containerName);

            return new BlobContainerClient(
                blobContainerUri,
                new DefaultAzureCredential());
        }

        public static async Task UploadBlob(
            string containerEndpoint,
            string containerName,
            string blobName,
            string blobContents)
        {
            BlobContainerClient containerClient =
                GetContainerClient(containerEndpoint, containerName);

            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient =
                containerClient.GetBlobClient(blobName);

            byte[] byteArray =
                Encoding.UTF8.GetBytes(blobContents);

            using MemoryStream stream =
                new MemoryStream(byteArray);

            await blobClient.UploadAsync(
                stream,
                overwrite: true);
        }

        public static async Task UploadFile(
            string containerEndpoint,
            string containerName,
            string blobName,
            Stream stream)
        {
            BlobContainerClient containerClient =
                GetContainerClient(containerEndpoint, containerName);

            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient =
                containerClient.GetBlobClient(blobName);

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            await blobClient.UploadAsync(
                stream,
                overwrite: true);
        }

        public static async Task<List<string>> ListBlobs(
            string containerEndpoint,
            string containerName)
        {
            BlobContainerClient containerClient =
                GetContainerClient(containerEndpoint, containerName);

            await containerClient.CreateIfNotExistsAsync();

            List<string> blobs = new List<string>();

            await foreach (var blob in containerClient.GetBlobsAsync())
            {
                blobs.Add(blob.Name);
            }

            return blobs;
        }

        public static async Task<string> GetBlob(
            string containerEndpoint,
            string containerName,
            string blobName)
        {
            BlobContainerClient containerClient =
                GetContainerClient(containerEndpoint, containerName);

            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient =
                containerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                var response =
                    await blobClient.DownloadAsync();

                using StreamReader reader =
                    new StreamReader(response.Value.Content);

                return await reader.ReadToEndAsync();
            }

            return "";
        }
    }
}
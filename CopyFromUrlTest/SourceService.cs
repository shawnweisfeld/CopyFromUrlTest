using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyFromUrlTest
{
    public class SourceService
    {
        private readonly ILogger<SourceService> _logger;
        private TelemetryClient _telemetryClient;
        private Config _config;
        private IRandomizerBytes _randomBytes;

        public SourceService(ILogger<SourceService> logger,
            TelemetryClient telemetryClient,
            Config config)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _config = config;

            _randomBytes = RandomizerFactory.GetRandomizer(new FieldOptionsBytes()
            {
                Min = (int)(Math.Pow(2, 20) * _config.FileSizeMB),
                Max = (int)(Math.Pow(2, 20) * _config.FileSizeMB)
            });
        }



        public async Task CreateBlobsAsync()
        {
            using (_telemetryClient.StartOperation<DependencyTelemetry>("CreateBlobsAsync"))
            {
                BlobContainerClient container = new BlobContainerClient(_config.Source, _config.ContainerName);

                //create the container if needed
                await container.CreateIfNotExistsAsync();
                await container.SetAccessPolicyAsync(PublicAccessType.Blob);

                for (int i = 0; i < _config.NumFiles; i++)
                {
                    string fileName = $"{Guid.NewGuid().ToString().Replace("-", "").ToLower()}.obj";
                    await container.UploadBlobAsync(fileName, new MemoryStream(_randomBytes.Generate()));
                }
            }
        }

        public async Task<List<string>> GetSourceBlobs()
        {
            var blobs = new List<string>();
            BlobContainerClient container = new BlobContainerClient(_config.Source, _config.ContainerName);

            //get all the blobs in the container
            await foreach (var item in container.GetBlobsAsync())
            {
                blobs.Add(item.Name);
            }

            //return the list of blobs
            return blobs;
        }


    }
}

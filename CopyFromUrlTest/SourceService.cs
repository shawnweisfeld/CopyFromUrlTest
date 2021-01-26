using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
                Min = (int)Math.Pow(2, 20) * _config.MinFileSizeMB,
                Max = (int)Math.Pow(2, 20) * _config.MaxFileSizeMB
            });
        }



        public List<SourceItem> GetSourceBlobs()
        {
            using (_telemetryClient.StartOperation<DependencyTelemetry>("Get Source Blobs"))
            {
                var sources = new List<Task<List<SourceItem>>>();

                for (int i = 0; i < _config.Source.Count; i++)
                {
                    sources.Add(GetSourceBlobs(i));
                }

                Task.WaitAll(sources.ToArray());

                return sources.Select(x => x.Result).SelectMany(x => x).ToList();
            }
        }

        private async Task<List<SourceItem>> GetSourceBlobs(int accountIndex)
        {
            var blobs = new List<SourceItem>();
            BlobContainerClient container = new BlobContainerClient(_config.Source[accountIndex], _config.ContainerName);

            //create the container if needed
            await container.CreateIfNotExistsAsync();
            await container.SetAccessPolicyAsync(PublicAccessType.Blob);

            //get all the blobs in the container
            await foreach (var item in container.GetBlobsAsync())
            {
                blobs.Add(new SourceItem() {Account = accountIndex, FileName = item.Name });
            }

            //if the container is empty fill it
            while (_config.NumFiles > blobs.Count())
            {
                string fileName = $"{Guid.NewGuid().ToString().Replace("-", "").ToLower()}.obj";
                await container.UploadBlobAsync(fileName, new MemoryStream(_randomBytes.Generate()));
                blobs.Add(new SourceItem() { Account = accountIndex, FileName = fileName });
            }

            //return the list of blobs
            return blobs;
        }

    }
}

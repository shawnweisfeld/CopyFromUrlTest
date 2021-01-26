using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CopyFromUrlTest
{
    public class CopyBlobService
    {
        private readonly ILogger<CopyBlobService> _logger;
        private TelemetryClient _telemetryClient;
        private Config _config;
        private BlobContainerClient[] _sourceClients;
        private BlobContainerClient _destCleint;
        private readonly SemaphoreSlim _slim;

        public CopyBlobService(ILogger<CopyBlobService> logger,
            TelemetryClient telemetryClient,
            Config config)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _config = config;

            _sourceClients = _config.Source.Select(x => new BlobContainerClient(x, _config.ContainerName)).ToArray();
            _destCleint = new BlobContainerClient(_config.Destination, _config.ContainerName);
            _slim = new SemaphoreSlim(Environment.ProcessorCount * _sourceClients.Length);

        }

        public async Task CopyFromUri(List<SourceItem> sourceItems)
        {
            //shuffle the source lists to level the load on the source accounts
            sourceItems.OrderBy(x => Guid.NewGuid());

            await _destCleint.CreateIfNotExistsAsync();

            using (var op = _telemetryClient.StartOperation<DependencyTelemetry>("CopyFromUri"))
            {
                op.Telemetry.Properties.Add("Run", _config.Run);
                op.Telemetry.Properties.Add("Number of Files", $"{sourceItems.Count}");
                op.Telemetry.Properties.Add("Number of Cores", $"{Environment.ProcessorCount}");
                op.Telemetry.Properties.Add("Number of Sources", $"{_sourceClients.Length}");

                foreach (var item in sourceItems)
                {
                    _slim.Wait();

                    var fileName = $"{Guid.NewGuid().ToString().Replace("-", "").ToLower()}.obj";
                    var source = _sourceClients[item.Account].GetBlockBlobClient(item.FileName);
                    var dest = _destCleint.GetBlockBlobClient(fileName);

                    _logger.LogInformation($"CopyFromUri {source.Uri.AbsoluteUri} -> {fileName}");
                    await dest.SyncCopyFromUriAsync(source.Uri);

                    _slim.Release();
                }
            }

        }

        public async Task CopyBytes(List<SourceItem> sourceItems)
        {
            //shuffle the source lists to level the load on the source accounts
            sourceItems.OrderBy(x => Guid.NewGuid());

            await _destCleint.CreateIfNotExistsAsync();

            using (var op = _telemetryClient.StartOperation<DependencyTelemetry>("CopyBytes"))
            {
                op.Telemetry.Properties.Add("Run", _config.Run);
                op.Telemetry.Properties.Add("Number of Files", $"{sourceItems.Count}");
                op.Telemetry.Properties.Add("Number of Cores", $"{Environment.ProcessorCount}");
                op.Telemetry.Properties.Add("Number of Sources", $"{_sourceClients.Length}");

                foreach (var item in sourceItems)
                {
                    _slim.Wait();

                    var fileName = $"{Guid.NewGuid().ToString().Replace("-", "").ToLower()}.obj";
                    var source = _sourceClients[item.Account].GetBlockBlobClient(item.FileName);
                    var dest = _destCleint.GetBlockBlobClient(fileName);

                    _logger.LogInformation($"CopyBytes {source.Uri.AbsoluteUri} -> {fileName}");
                    await dest.UploadAsync((await source.DownloadAsync()).Value.Content);

                    _slim.Release();
                }
            }

        }



    }
}

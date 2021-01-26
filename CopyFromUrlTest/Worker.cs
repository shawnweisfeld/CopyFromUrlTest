using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CopyFromUrlTest
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private TelemetryClient _telemetryClient;
        private Config _config;
        private SourceService _setupService;
        private CopyBlobService _copyBlobService;

        public Worker(ILogger<Worker> logger,
            IHostApplicationLifetime hostApplicationLifetime,
            TelemetryClient telemetryClient,
            Config config,
            SourceService setupService,
            CopyBlobService copyBlobService)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
            _telemetryClient = telemetryClient;
            _config = config;
            _setupService = setupService;
            _copyBlobService = copyBlobService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() =>
            {
                _logger.LogInformation("Worker Cancelling");
            });

            try
            {
                var todo = _setupService.GetSourceBlobs();

                await _copyBlobService.CopyFromUri(todo);
                await _copyBlobService.CopyBytes(todo);

            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Operation Canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled Exception");
            }
            finally
            {
                _logger.LogInformation("Flushing App Insights");
                _telemetryClient.Flush();
                Task.Delay(5000).Wait();

                _hostApplicationLifetime.StopApplication();
            }

        }
    }
}

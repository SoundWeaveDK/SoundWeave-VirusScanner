using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using nClam;

namespace SoundWeave_VirusScanner
{
    internal class FileScanner
    {
        public FileScanner(string serverName, int serverPort, ILogger log)
        {
            clam = new(serverName, serverPort);
            this.log = log;
        }

        readonly ClamClient clam;
        readonly ILogger log;

        public void ScanFile(Stream myBlob, string blobName, string containerName, string connectionString)
        {
            ClamScanResult scanResult = clam.SendAndScanFileAsync(myBlob).Result;

            BlobServiceClient blobServiceClient = new(Environment.GetEnvironmentVariable(connectionString));
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            switch (scanResult.Result)
            {
                case ClamScanResults.Clean:
                    log.LogInformation("The file is clean!");
                    break;
                case ClamScanResults.VirusDetected:
                    log.LogInformation("Virus Found!");
                    log.LogInformation("Virus name: {0}", scanResult.InfectedFiles.First().VirusName);

                    if (blobClient.Exists())
                    {
                        blobClient.Delete();
                        log.LogInformation("Infected file deleted.");
                    }
                    else
                    {
                        log.LogInformation("Infected file not found.");
                    }

                    break;
                case ClamScanResults.Error:
                    log.LogInformation("Error scanning file: {0}", scanResult.RawResult);
                    break;
            }
        }
    }
}

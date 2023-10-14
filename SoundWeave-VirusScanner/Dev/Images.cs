using System;
using System.IO;
using System.Linq;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using nClam;

namespace SoundWeave_VirusScanner.Dev
{
    public class Images
    {
        static readonly string serverName = "localhost";
        static readonly int serverPort = 3310;

        [FunctionName("ScanImagesDEVELOPMENT")]
        public void ScanImagesDEVELOPMENT([BlobTrigger("images/{name}")] Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            ClamClient clam = new(serverName, serverPort);

            ClamScanResult scanResult = clam.SendAndScanFileAsync(myBlob).Result;

            BlobServiceClient blobServiceClient = new(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("images");
            BlobClient blobClient = blobContainerClient.GetBlobClient(name);

            switch (scanResult.Result)
            {
                case ClamScanResults.Clean:
                    log.LogInformation("The file is clean!");

                    if (blobClient.Exists())
                    {
                        Console.WriteLine("joe biden");
                    }
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

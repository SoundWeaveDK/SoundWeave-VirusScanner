using System;
using System.IO;
using System.Linq;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using nClam;

namespace SoundWeave_VirusScanner.Dev
{
    public class Podcasts
    {
        static readonly string serverName = "localhost";
        static readonly int serverPort = 3310;
        
        [FunctionName("ScanPodcastsDEVELOPMENT")]
        public void ScanPodcastsDEVELOPMENT([BlobTrigger("podcasts/{name}")] Stream myBlob, string name, ILogger log)
        {
            try
            {
                log.LogInformation($"Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

                FileScanner fileScanner = new(serverName, serverPort, log);
                fileScanner.ScanFile(myBlob, name, "podcasts", "AzureWebJobsStorage");
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
        }
    }
}

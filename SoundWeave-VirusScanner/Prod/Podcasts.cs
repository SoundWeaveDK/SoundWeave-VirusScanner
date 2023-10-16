using System;
using System.IO;
using System.Linq;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using nClam;

namespace SoundWeave_VirusScanner.Prod
{
    public class Podcasts
    {
        static readonly string serverName = "4.207.105.166";
        static readonly int serverPort = 3310;

        [FunctionName("ScanPodcasts")]
        public void ScanPodcasts([BlobTrigger("podcasts/{name}", Connection = "StorageAccountConnection")] Stream myBlob, string name, ILogger log)
        {
            try
            {
                log.LogInformation($"Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

                FileScanner fileScanner = new(serverName, serverPort, log);
                fileScanner.ScanFile(myBlob, name, "podcasts", "StorageAccountConnection");
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
        }
    }
}

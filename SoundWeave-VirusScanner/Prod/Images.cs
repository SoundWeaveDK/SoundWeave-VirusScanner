using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using nClam;

namespace SoundWeave_VirusScanner.Prod
{
    public class Images
    {
        static readonly string serverName = "4.207.105.166";
        static readonly int serverPort = 3310;

        [FunctionName("ScanImages")]
        public void ScanImages([BlobTrigger("images/{name}", Connection = "StorageAccountConnection")] Stream myBlob, string name, ILogger log)
        {
            try
            {
                log.LogInformation($"Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

                FileScanner fileScanner = new(serverName, serverPort, log);
                fileScanner.ScanFile(myBlob, name, "images", "StorageAccountConnection");
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
        }
    }
}

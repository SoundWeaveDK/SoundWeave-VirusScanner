using System.IO;
using System.Linq;
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
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            ClamClient clam = new(serverName, serverPort);

            ClamScanResult scanResult = clam.SendAndScanFileAsync(myBlob).Result;

            switch (scanResult.Result)
            {
                case ClamScanResults.Clean:
                    log.LogInformation("The file is clean!");
                    break;
                case ClamScanResults.VirusDetected:
                    log.LogInformation("Virus Found!");
                    log.LogInformation("Virus name: {0}", scanResult.InfectedFiles.First().VirusName);
                    break;
                case ClamScanResults.Error:
                    log.LogInformation("Error scanning file: {0}", scanResult.RawResult);
                    break;
            }
        }
    }
}

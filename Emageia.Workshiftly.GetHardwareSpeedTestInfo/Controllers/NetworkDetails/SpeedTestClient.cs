using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.NetworkDetails.Abstract;
using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emageia.Workshiftly.GetHardwareSpeedTestInfo.Common;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Controllers.NetworkDetails
{
    public class SpeedTestClient : ISpeedTestClient
    {
        //private const string ConfigUrl = "https://www.speedtest.net/speedtest-config.php";

        //private static readonly string[] ServersUrls = {
        //    "https://www.speedtest.net/speedtest-servers-static.php",
        //    "https://c.speedtest.net/speedtest-servers-static.php",
        //    "https://www.speedtest.net/speedtest-servers.php",
        //    "https://c.speedtest.net/speedtest-servers.php"
        //};

        private static string conurl = "V+iSYIqWuh7Yx5Rc/dgPmSJQ+NiDmyls23P/jGpqxLbFpUj2DlSZEjK4eQouKrMrJGLm1lhC7/cMYhHGV+cNvWgc/rn/SvZTTSZWJUCi5PANr4/L8JW2XJIdH7r8tq+0XDPWAE+KYBuAlU91jnecSsBf4b97FkmomOUtDM5jRFU=";
        private static string curl1 = Config.StrDecrypt(conurl);
        private static string curl2 = "OOzFkvvmBnsDNTSEWrSGBMYnbqRZlz5yuVlfnIwtLn4eCtf3zJa5pMhyo/plyb13Zrn/vXN2dayZE0zMsghMg2If7xSyr9jUNodQ+GcH86DeuIqAfIXpGxKFM5rhb93QBt1rXRlYPlVyPdj2yz3RrGs8Fks3BvLjWClWW2mqD8A=";
        private static string curl3 = "fCmu1emO0S6W/riuV/F+Az0EYZcyGDNGyTFdSXiKcpmsRBlpTNa1FB0aW9eqQhfaoDS81Wb5QLVX+hEYTyCOultYtgBhpyjWFGVjG/x58dRVUiknANFLxYl5skYcuQU2WQWoy3BRGQmIeGOZ8ih02WbOruO+cqYtLMyTBCo3Ew0=";
        private static string curl4 = "3SCIayNkjZMI64vEgt6ijXXxtF5rJGKVtgTxKD0GyyCN10RbakyEePjDvMxC/rKihz3+j6BzPwWtzJsVVx3tzusoTSd5xhrtAiNP48ZydWhx6QsKWP6EX79+LxkZC8fxHhj7SZ84ZcEnNqEUgptvCNa+bOMMD7j209y3LXa8W0A=";
        private static string curl5 = "00G5k6AP5ySVMiXqIiNuN3d+Y70kynqXpT4p+3Jmct9B9u+keDyuHlpg9/mqw263YE7gEfrVUx8rNYLy2rKLq4vRn+maY+oUxZn1XcvRTGJvOojkD7B8YRRW+2QPyCjBRQ0LqbDIyU6gWGHEu4HtrGGCnYMzurqpmOqjo9/+mqo=";
        private readonly string ConfigUrl = string.Format(curl1);
        private static readonly string[] ServersUrls = {

            Config.StrDecrypt( curl2),
            Config.StrDecrypt( curl3),
            Config.StrDecrypt( curl4),
            Config.StrDecrypt( curl5)

        };

        private static readonly int[] DownloadSizes = { 350, 750, 1500, 3000 };
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const int MaxUploadSize = 4; // 400 KB

        #region ISpeedTestClient

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException"></exception>
        public Settings GetSettings()
        {
            try
            {
                using (var client = new SpeedTestHttpClient())
                {
                    var settings = client.GetConfig<Settings>(ConfigUrl).GetAwaiter().GetResult();

                    var serversConfig = new ServersList();
                    foreach (var serversUrl in ServersUrls)
                    {
                        try
                        {
                            serversConfig = client.GetConfig<ServersList>(serversUrl).GetAwaiter().GetResult();
                            if (serversConfig.Servers.Count > 0) break;
                        }
                        catch(Exception ex)
                        {
                            //
                        }
                    }

                    if (serversConfig.Servers.Count <= 0)
                    {
                       // throw new InvalidOperationException("SpeedTest does not return any server");
                    }

                    List<string> ignoredIds = new List<string>(settings.ServerConfig.IgnoreIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

                    //var ignoredIds = settings.ServerConfig.IgnoreIds.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    serversConfig.CalculateDistances(settings.Client.GeoCoordinate);
                    settings.Servers = serversConfig.Servers
                        .Where(s => !ignoredIds.Contains(s.Id.ToString()))
                        .OrderBy(s => s.Distance)
                        .ToList();

                    return settings;
                }
            }
            catch (Exception ex)
            {

                return null;
            }
            
            
        }

        /// <inheritdoc />
        public int TestServerLatency(Server server, int retryCount = 3)
        {
            var latencyUri = CreateTestUrl(server, "latency.txt");
            var timer = new Stopwatch();

            using (var client = new SpeedTestHttpClient())
            {
                for (var i = 0; i < retryCount; i++)
                {
                    string testString;
                    try
                    {
                        timer.Start();
                        testString = client.GetStringAsync(latencyUri).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                    catch (WebException)
                    {
                        continue;
                    }
                    finally
                    {
                        timer.Stop();
                    }

                    if (!testString.StartsWith("test=test"))
                    {
                      //  throw new InvalidOperationException("Server returned incorrect test string for latency.txt");
                    }
                }

                return (int)timer.ElapsedMilliseconds / retryCount;
            };

            
        }

        /// <inheritdoc />
        public double TestDownloadSpeed(Server server, int simultaneousDownloads = 2, int retryCount = 2)
        {
            var testData = GenerateDownloadUrls(server, retryCount);
            double testSpeed = 0;
            try
            {
                testSpeed = TestSpeed(testData, async (client, url) =>
                {
                    var data = await client.GetByteArrayAsync(url).ConfigureAwait(false);
                    return data.Length;
                }, simultaneousDownloads);
            }
            catch (Exception ex)
            {

             //   throw;
            }
            //return TestSpeed(testData, async (client, url) =>
            //{
            //    var data = await client.GetByteArrayAsync(url).ConfigureAwait(false);
            //    return data.Length;
            //}, simultaneousDownloads);

            return testSpeed;
        }

        /// <inheritdoc />
        public double TestUploadSpeed(Server server, int simultaneousUploads = 2, int retryCount = 2)
        {
            var testData = GenerateUploadData(retryCount);
            return TestSpeed(testData, async (client, uploadData) =>
            {
                await client.PostAsync(server.Url, new StringContent(uploadData));
                return uploadData.Length;
            }, simultaneousUploads);
        }

        #endregion

        #region Helpers

        private static double TestSpeed<T>(IEnumerable<T> testData, Func<HttpClient, T, Task<int>> doWork, int concurrencyCount = 2)
        {
            var timer = new Stopwatch();
            var throttler = new SemaphoreSlim(concurrencyCount);

            timer.Start();
            var downloadTasks = testData.Select(async data =>
            {
                await throttler.WaitAsync().ConfigureAwait(false);
                var client = new SpeedTestHttpClient();
                try
                {
                    var size = await doWork(client, data).ConfigureAwait(false);
                    return size;
                }
                finally
                {
                    client.Dispose();
                    throttler.Release();
                }
            }).ToArray();

            Task.WaitAll(downloadTasks);
            timer.Stop();

            double totalSize = downloadTasks.Sum(task => task.Result);
            return (totalSize * 8 / 1024) / ((double)timer.ElapsedMilliseconds / 1000);
        }

        private static IEnumerable<string> GenerateUploadData(int retryCount)
        {
            var random = new Random();
            var result = new List<string>();

            for (var sizeCounter = 1; sizeCounter < MaxUploadSize + 1; sizeCounter++)
            {
                var size = sizeCounter * 200 * 1024;
                var builder = new StringBuilder(size);

                builder.AppendFormat("content{0}=", sizeCounter);

                for (var i = 0; i < size; ++i)
                {
                    builder.Append(Chars[random.Next(Chars.Length)]);
                }

                for (var i = 0; i < retryCount; i++)
                {
                    result.Add(builder.ToString());
                }
            }

            return result;
        }

        private static string CreateTestUrl(Server server, string file)
        {
            return new Uri(new Uri(server.Url), ".").OriginalString + file;
        }

        private static IEnumerable<string> GenerateDownloadUrls(Server server, int retryCount)
        {
            var downloadUriBase = CreateTestUrl(server, "random{0}x{0}.jpg?r={1}");
            foreach (var downloadSize in DownloadSizes)
            {
                for (var i = 0; i < retryCount; i++)
                {
                    yield return string.Format(downloadUriBase, downloadSize, i);
                }
            }
        }

        #endregion
    }
}

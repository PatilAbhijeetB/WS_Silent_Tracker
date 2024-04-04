using Emageia.Workshiftly.CoreFunction.IoC.Service.Http.Adapter;
using Emageia.Workshiftly.CoreFunction.IoC.Utility;
using Emageia.Workshiftly.Domain.Concrete;
using Emageia.Workshiftly.Entity;
using Emageia.Workshiftly.Entity.HttpClientModel;
using Emageia.Workshiftly.Entity.HttpClientModel.activewindo;
using Emageia.Workshiftly.Entity.HttpClientModel.screens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Emageia.Workshiftly.CoreFunction.IoC.Service.Http
{
    internal static class HttpAdapterOld
    {
        #region properties
        private static double ACTIVE_WINDOW_SYNC_BATCH_SIZE = 20;
        private static double SCREENSHOT_SYNC_BATCH_SIZE = 10;
        private static double TASK_SYNC_BATCH_SIZE = 30;
        private static double URL_SYNC_BATCH_SIZE = 10;
        private static string Url = "";
        enum EnvironmentType { DEVELOPMENT, STAGING, PRODUCTION };
        #endregion

        //public HttpRequestCaller()
        //{
        //    EnvironmentType environmentType;
        //    string BUID_ENVIRONMENT = ConfigurationSettings.AppSettings["BUID_ENVIRONMENT"];
        //    //string BUID_ENVIRONMENT = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings["countoffiles"];

        //    bool sucess = Enum.TryParse<EnvironmentType>(BUID_ENVIRONMENT, out environmentType);

        //    if (!sucess)
        //    {

        //    }
        //    switch (environmentType)
        //    {
        //        case EnvironmentType.DEVELOPMENT:
        //            Url = ConfigurationSettings.AppSettings["DEVELOPMENT_API"];
        //            break;
        //        case EnvironmentType.STAGING:
        //            Url = ConfigurationSettings.AppSettings["STAGING_API"];
        //            break;
        //        case EnvironmentType.PRODUCTION:
        //            Url = ConfigurationSettings.AppSettings["PRODUCTION_API"];
        //            break;
        //    }


        //}



        #region Active window sync
        /// <summary>
        /// updata Active windo sync to the server
        /// </summary>
        public static async void syncActivityWindows()
        {
            try
            {

                CommonUtility.LogWriteLines("Success", "HttpRequestCaller.cs syncActivityWindows", "Starting the Sync Activity Windows!", "******" + CommonUtility.ServerPath + "/users/");
                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {


                    var syncActiveWindow = _clientDbContext.ActivietyWindows.Where(active => active.isSynced == false && active.userId == CommonUtility.UserSessions.id).ToList();


                    var para = JsonConvert.SerializeObject(syncActiveWindow);
                    StringContent data = new StringContent(para, Encoding.UTF8, "application/json");



                    using (HttpClient client = new HttpClient())
                    {
                       // client.BaseAddress = new Uri("https://qa.workshiftly.com/api/users/");
                       // client.BaseAddress = new Uri("https://staging-api.workshiftly.com/api/users/");
                          client.BaseAddress = new Uri("https://portal.workshiftly.com/api/users/");
                        //  client.BaseAddress = new Uri(CommonUtility.ServerPath+"/users/");

                        client.DefaultRequestHeaders.Accept.Add(
                           new MediaTypeWithQualityHeaderValue("application/json"));

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CommonUtility.UserSessions.authToken);

                        HttpResponseMessage response = client.PostAsync(CommonUtility.UserSessions.id + "/active-windows", data).Result;


                        if (response.IsSuccessStatusCode)
                        {
                            var returnUrls = await response.Content.ReadAsStringAsync();
                            var returnData = JsonConvert.DeserializeObject<ActiveWindowReturn>(returnUrls);


                            var upadata = syncActiveWindow.Select(c => { c.isSynced = true; return c; });

                            _clientDbContext.ActivietyWindows.UpdateRange(upadata);
                            await _clientDbContext.SaveChangesAsync();
                            CommonUtility.LogWriteLines("Success", "HttpRequestCaller.cs syncActivityWindows", "Successfully Active window sync and update sqlite Db!", "******");

                        }
                        else
                        {
                            CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs syncActivityWindows", "There was a problem Active window sync !", "****** response status " + response.IsSuccessStatusCode + "*****" + client.BaseAddress + CommonUtility.UserSessions.id + "/active-windows");
                        }
                    }

                    CommonUtility.LogWriteLines("Success", "HttpRequestCaller.cs syncActivityWindows", "Ending the Sync Activity Windows!", "******");
                }


            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs syncActivityWindows", "There was a problem Active window sync and update sqlite Db ===== ", ex.Message.ToString());

            }

        }

        #endregion



        #region Sync ScreenshotStorage

        public static async void syncScreenshots()
        {
            try
            {
                CommonUtility.LogWriteLines("Success", "HttpRequestCaller.cs syncScreenshots", "ScreenShot timeslotDefinition.IsCompleted", "******");
                var userId = CommonUtility.UserSessions.id;
                var companyId = CommonUtility.UserSessions.companyId;
                cleanCompletedJobs("putObject");
                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {

                    var screenshots = _clientDbContext.Screenshots.Where(screen =>
                                   screen.synced == false && screen.userId == userId && screen.companyId == companyId);

                    if (screenshots == null)
                    {
                        return;
                    }

                    var screeenshotsCount = screenshots.Count();
                    var totalbatches = Math.Ceiling((float)screeenshotsCount / SCREENSHOT_SYNC_BATCH_SIZE);
                    int limit = (int)SCREENSHOT_SYNC_BATCH_SIZE;

                    for (int batchId = 1; batchId <= totalbatches; batchId++)
                    {
                        int offset = (batchId == 1) ? 0 : (batchId * limit);

                        List<Screenshot> currentBatch = screenshots.Skip(offset).Take(limit).ToList();

                        long expiration = 60L * 60;
                        initScreenshotStorageURLMeta2(currentBatch, "putObject", "base64", expiration);

                    }

                    completePendingJobs("putObject");
                    //pushTheScreenShot3();
                }
                cleanCompletedJobs("putObject");
                CommonUtility.LogWriteLines("Success", "HttpRequestCaller.cs syncScreenshots", "ScreenShot timeslotDefinition.IsCompleted", "******");

            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs syncScreenshots", ex.InnerException.Message.ToString(), ex.Message.ToString());

            }
        }


        public static async void initScreenshotStorageURLMeta2(List<Screenshot> screenshots, String action, String contentEncoding, long expiration)
        {
            try
            {


                Dictionary<String, Screenshot> screenshotMap = new Dictionary<String, Screenshot>();
                List<ScreenshotMeta> headers = new List<ScreenshotMeta>();

                screenshots.ForEach(screenshot =>
                {


                    screenshotMap.Add(screenshot.fileName, screenshot);

                    headers.Add(new ScreenshotMeta
                    {
                        objectType = "screenshot",
                        action = "putObject",
                        expiration = (Int32)expiration,
                        key = screenshot.fileName,
                        completed = false

                    });

                });

                //  var para = JsonConvert.SerializeObject(metaObjs);


                /////////////////////////////////////////////////


                var heder = new ScreenMetaHeder
                {
                    companyId = CommonUtility.UserSessions.companyId,
                    metaObjects = headers

                };


                var paras = JsonConvert.SerializeObject(heder);
                StringContent data = new StringContent(paras, Encoding.UTF8, "application/json");


                /////////////////////////////////////////////////



                using (HttpClient client = new HttpClient())
                {
                    //client.BaseAddress = new Uri("https://qa.workshiftly.com/api/users/");
                   // client.BaseAddress = new Uri("https://staging-api.workshiftly.com/api/users/");
                      client.BaseAddress = new Uri("https://portal.workshiftly.com/api/users/");


                    //   client.BaseAddress = new Uri(CommonUtility.ServerPath + "/users/");

                    client.DefaultRequestHeaders.Accept.Add(
                       new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CommonUtility.UserSessions.authToken);
                    HttpResponseMessage response = client.PostAsync(CommonUtility.UserSessions.id + "/screenshots", data).Result;


                    if (response.IsSuccessStatusCode)
                    {
                        var returnUrls = await response.Content.ReadAsStringAsync();
                        var returnData = JsonConvert.DeserializeObject<ServerReturnObject>(returnUrls);



                        var upadata = returnData.data.Select(c =>
                        {

                            Screenshot value = screenshotMap.FirstOrDefault(x => x.Key == c.key).Value;

                            string data64 = value != null ? value.Data : null;
                            c.data = data64;
                            //c.completed = true;
                            return c;

                        }).ToList();

                        var secre = screenshots.Select(x => { x.synced = true; return x; }).ToList();

                        var nie = upadata;
                        using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                        {
                            _clientDbContext.Screenshots.UpdateRange(secre);
                            await _clientDbContext.ScreenshotMetas.AddRangeAsync(upadata);
                            await _clientDbContext.SaveChangesAsync();
                        }

                        // CommonUtility.LogWriteLine("There was a problem installing the application!");
                    }
                    else
                    {
                        CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs initScreenshotStorageURLMeta2", "There was a problem ScreenshotStorageURLMeta sync !", "****** response status " + response.IsSuccessStatusCode + "****" + CommonUtility.UserSessions.id + "/screenshots");

                        // CommonUtility.LogWriteLine("There was a problem installing the application!");
                        // MessageBox.Show("Error Code");
                        //return false;
                    }
                }


                //List<StorageSignedURLMeta> metaObjs = new List<StorageSignedURLMeta>();
            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs initScreenshotStorageURLMeta2", ex.InnerException.Message.ToString(), ex.Message.ToString());

            }

        }

        private static async void completePendingJobs(string action)
        {
            try
            {
                CommonUtility.LogWriteLines("Success", "HttpRequestCaller.cs completePendingJobs", "Start sync screenshot to AWS complete Pending Jobs!", "******");
                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {// && screen.userId == userId && screen.companyId == companyId
                    List<ScreenshotMeta> syncScreenShotData = _clientDbContext.ScreenshotMetas.Where(screen => screen.action == "putObject" && screen.completed == false).ToList();


                    if (syncScreenShotData.Any())
                    {
                        completeScreenshotPutObjectJobs(syncScreenShotData);
                    }

                }
                CommonUtility.LogWriteLines("Success", "HttpRequestCaller.cs completePendingJobs", "End sync screenshot to AWS complete Pending Jobs!", "******");
            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs completePendingJobs", ex.InnerException.Message.ToString(), ex.Message.ToString());

            }
            //throw new NotImplementedException();
        }

        private static async void completeScreenshotPutObjectJobs(List<ScreenshotMeta> syncScreenShotData)
        {
            try
            {
                Int32 currentTimestamp = Int32.Parse(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                //using (HttpClient client = new HttpClient())
                //{

                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {
                    foreach (ScreenshotMeta screenshot in syncScreenShotData)
                    {
                        Int32 expirationTimestamp = screenshot.expiration;
                        if (expirationTimestamp <= currentTimestamp)
                        {
                            _clientDbContext.ScreenshotMetas.Remove(screenshot);

                        }

                        string data64 = screenshot.data;

                        await pushTheScreenShot(data64, screenshot.url, screenshot);

                    }
                }

                // }
            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs completeScreenshotPutObjectJobs", ex.InnerException.Message.ToString(), ex.Message.ToString());

            }
        }

        /// <summary>
        /// Sync Screen Shot update status and Remove function call
        /// </summary>
        /// <param name="action"></param>
        private static async void cleanCompletedJobs(string action)
        {
            try
            {
                CommonUtility.LogWriteLines("Success", "HttpRequestCaller.cs syncScreenshots", "Start sync screenshot to HTTPPUT using update ETAG!", "******");
                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext()) //
                {
                    List<ScreenshotMeta> syncScreenShotData = _clientDbContext.ScreenshotMetas.Where(screen => screen.action == "putObject" && screen.completed == false).ToList();
                    List<Screenshot> syncScreenShot = _clientDbContext.Screenshots.Where(screen => screen.synced == true && screen.userId == CommonUtility.UserSessions.id && screen.companyId == CommonUtility.UserSessions.companyId).ToList();

                    List<Screenshot> matchScreenshot = CleanCompletedJobsList(syncScreenShotData, syncScreenShot);



                    if (matchScreenshot.Any())
                    {
                        var para = JsonConvert.SerializeObject(matchScreenshot);
                        using (HttpClient client = new HttpClient())
                        {
                            //client.BaseAddress = new Uri(CommonUtility.ServerPath + "/users/");
                           // client.BaseAddress = new Uri("https://qa.workshiftly.com/api/users/");
                           // client.BaseAddress = new Uri("https://staging-api.workshiftly.com/api/users/");
                             client.BaseAddress = new Uri("https://portal.workshiftly.com/api/users/");
                            client.DefaultRequestHeaders.Accept.Add(
                               new MediaTypeWithQualityHeaderValue("application/json"));

                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CommonUtility.UserSessions.authToken);
                            StringContent data = new StringContent(para, Encoding.UTF8, "application/json");
                            HttpResponseMessage response = client.PutAsync(CommonUtility.UserSessions.id + "/screenshots", data).Result;


                            if (response.IsSuccessStatusCode)
                            {
                                var returnUrls = await response.Content.ReadAsStringAsync();
                                //var returnData = JsonConvert.DeserializeObject<ServerReturnObject>(returnUrls);


                                try
                                {
                                    var uspdated = syncScreenShotData.Select(c =>
                                    {
                                        c.completed = true;
                                        return c;
                                    });

                                    using (ClientDataStoreDbContext _clientDbContexts = new ClientDataStoreDbContext())
                                    {
                                        // https://qa.workshiftly.com/api
                                        _clientDbContexts.ScreenshotMetas.UpdateRange(uspdated);
                                        await _clientDbContexts.SaveChangesAsync();
                                        CommonUtility.LogWriteLines("Success", "HttpRequestCaller.cs cleanCompletedJobs", "Successfully Updated complete Screenshot Put Object Jobs!", "******");

                                    }
                                }
                                catch (Exception ex)
                                {
                                    CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs cleanCompletedJobs", " Error  screenshot RemoveRange", ex.Message.ToString());
                                }




                                //  CleanPutObject(syncScreenShotData, matchScreenshot);
                            }
                            else
                            {
                                CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs cleanCompletedJobs", " Error  sync screenshot to AWS ", "Error");
                            }
                        }
                    }

                }
                CommonUtility.LogWriteLines("Success", "HttpRequestCaller.cs cleanCompletedJobs", "End sync screenshot to AWS complete Pending Jobs!", "******");

            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs cleanCompletedJobs", ex.InnerException.Message.ToString(), ex.Message.ToString());

            }
            //throw new NotImplementedException();
        }



        /// <summary>
        /// Clean Sync Image
        /// </summary>
        /// <param name="syncScreenShotData"></param>
        /// <param name="matchScreenshot"></param>
        private async static void CleanPutObject(List<ScreenshotMeta> syncScreenShotData, List<Screenshot> matchScreenshot)
        {
            try
            {
                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {

                    _clientDbContext.ScreenshotMetas.RemoveRange(syncScreenShotData);
                    _clientDbContext.Screenshots.RemoveRange(matchScreenshot);
                    await _clientDbContext.SaveChangesAsync();
                    CommonUtility.LogWriteLines("Success", "HttpRequestCaller.cs CleanPutObject", "success screenshot RemoveRange", "******");
                }
            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs CleanPutObject", "success screenshot RemoveRange", ex.Message.ToString());
            }
        }

        private static List<Screenshot> CleanCompletedJobsList(List<ScreenshotMeta> syncScreenShotData, List<Screenshot> syncScreenShot)
        {
            try
            {

                return syncScreenShot.Where(screenshot =>

                    syncScreenShotData.Any(screenMeta => screenMeta.key == screenshot.fileName))
                    .Select(screenshot => { screenshot.Data = null; return screenshot; })
                    .ToList();

            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs CleanCompletedJobsList", ex.InnerException.Message.ToString(), ex.Message.ToString());
                return syncScreenShot;

            }

        }


        /// <summary>
        /// Image Sync to aws
        /// </summary>
        /// <param name="data64"> Image </param>
        /// <param name="url">server url</param>
        /// <param name="screenshot">Screenshot Object</param>
        /// <returns></returns>
        private static async Task pushTheScreenShot(string data64, string url, ScreenshotMeta screenshot)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(data64);

                using (HttpClient client = new HttpClient())
                {

                    ByteArrayContent byteContent = new ByteArrayContent(imageBytes);
                    byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                    HttpResponseMessage response = await client.PutAsync(new Uri(url), byteContent);
                    //image/png            application/octet-stream

                    if (response.IsSuccessStatusCode)
                    {
                        CommonUtility.LogWriteLines("Success", "HttpRequestCaller.cs pushTheScreenShot", "*******Successfully sync to the aws !" + screenshot.key, "******");
                    }
                    else
                    {
                        CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs pushTheScreenShot", "*****response faild sync to the aws!" + screenshot.key, " * ** Error *** status code" + response.IsSuccessStatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs pushTheScreenShot", "***Exception sync to the aws!-- > " + screenshot.key, ex.Message.ToString());
            }
        }


        #endregion


        #region sync activity log
        public static async void CallPostSyncWorkStatusLogs()
        {
            try
            {
                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {


                    var workStatusLogs = _clientDbContext.WorkStatusLogs.Where(active =>
                    active.isSynced == false && active.userId == CommonUtility.UserSessions.id && active.companyId == CommonUtility.UserSessions.companyId).ToList();

                    var workStatusLogss = _clientDbContext.WorkStatusLogs.ToList();


                    var para = JsonConvert.SerializeObject(workStatusLogs);
                    StringContent data = new StringContent(para, Encoding.UTF8, "application/json");



                    using (HttpClient client = new HttpClient())
                    {
                       // client.BaseAddress = new Uri("https://qa.workshiftly.com/api");
                       // client.BaseAddress = new Uri("https://staging-api.workshiftly.com/api");
                          client.BaseAddress = new Uri("https://portal.workshiftly.com/api");
                        //client.BaseAddress = new Uri(CommonUtility.ServerPath + "/");
                        //  client.BaseAddress = new Uri("https://qa.workshiftly.com/api/users/63102b27-5ac7-4f93-8706-044ef00aee17/");

                        client.DefaultRequestHeaders.Accept.Add(
                           new MediaTypeWithQualityHeaderValue("application/json"));

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CommonUtility.UserSessions.authToken);

                        HttpResponseMessage response = client.PostAsync("users/" + CommonUtility.UserSessions.id + "/work-status-log", data).Result;
                        //HttpResponseMessage response = client.PostAsync("work-status-log", data).Result;


                        if (response.IsSuccessStatusCode)
                        {
                            var returnUrls = await response.Content.ReadAsStringAsync();
                            //var returnData = JsonConvert.DeserializeObject<ActiveWindowReturn>(returnUrls);


                            var upadata = workStatusLogs.Select(c => { c.isSynced = true; return c; });

                            _clientDbContext.WorkStatusLogs.UpdateRange(upadata);
                            await _clientDbContext.SaveChangesAsync();
                            CommonUtility.LogWriteLines("Success", "HttpRequestCaller.cs CallPostSyncWorkStatusLogs", "*******Successfully Sync Work Status Logs !", "******");

                        }
                        else
                        {
                            CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs CallPostSyncWorkStatusLogs", "*****response faild sync Work Status Logs!", " * ** Error ***" + response.IsSuccessStatusCode + "*****" + client.BaseAddress + "users/" + CommonUtility.UserSessions.id + "/work-status-log");

                            // MessageBox.Show("Error Code");
                            //return false;
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs CallPostSyncWorkStatusLogs", ex.InnerException.Message.ToString(), ex.Message.ToString());

                //return false;
            }

        }

        #endregion


        #region Testing methed  -> must Delete after 

        public static async void initScreenshotStorageURLMeta(List<Screenshot> screenshots, String action, String contentEncoding, long expiration)
        {
            List<StorageSignedURLMeta> metaObjs = new List<StorageSignedURLMeta>();
            Dictionary<String, Screenshot> screenshotMap = new Dictionary<String, Screenshot>();


            screenshots.ForEach(screenshot =>
            {

                StorageSignedURLMeta metaObj = new StorageSignedURLMeta();
                metaObj.Action = action;
                metaObj.ContentEncoding = contentEncoding;
                metaObj.Expiration = (Int32)expiration;
                metaObj.Key = screenshot.fileName;

                metaObjs.Add(metaObj);

                screenshotMap.Add(screenshot.fileName, screenshot);

            });

            var para = JsonConvert.SerializeObject(metaObjs);
            using (HttpClient client = new HttpClient())
            {
              //  client.BaseAddress = new Uri("https://qa.workshiftly.com/api/users/");
              //  client.BaseAddress = new Uri("https://staging-api.workshiftly.com/api/users/");
                  client.BaseAddress = new Uri("https://portal.workshiftly.com/api/users/");
                client.DefaultRequestHeaders.Accept.Add(
                   new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CommonUtility.UserSessions.authToken);
                StringContent data = new StringContent(para, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(CommonUtility.UserSessions.id + "/screenshots", data).Result;


                if (response.IsSuccessStatusCode)
                {
                    var returnUrls = await response.Content.ReadAsStringAsync();
                    var returnData = JsonConvert.DeserializeObject<ServerReturnObject>(returnUrls);



                    var upadata = returnData.data.Select(c =>
                    {

                        Screenshot value = screenshotMap.FirstOrDefault(x => x.Key == c.key).Value;

                        string data64 = value != null ? value.Data : null;
                        c.data = data64;
                        c.completed = true;
                        return c;

                    });


                    using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                    {

                        await _clientDbContext.ScreenshotMetas.AddRangeAsync(upadata);
                        await _clientDbContext.SaveChangesAsync();
                    }

                    // CommonUtility.LogWriteLine("There was a problem installing the application!");
                }
                else
                {
                    // CommonUtility.LogWriteLine("There was a problem installing the application!");
                    // MessageBox.Show("Error Code");
                    //return false;
                }
            }


            //List<StorageSignedURLMeta> metaObjs = new List<StorageSignedURLMeta>();

        }
        private static async Task UploadAsFormData()
        {
            string url = "http://localhost:9000/workshiftly-screenshots-production/c568e7e2-6357-44f2-b2eb-8cc13b1a63f8/63102b27-5ac7-4f93-8706-044ef00aee17/2022-05-24/1653387057?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAIOSFODNN7EXAMPLE%2F20220524%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20220524T101130Z&X-Amz-Expires=3600&X-Amz-Signature=7f799b787bf1a0f725b9543b3eac7592e671519ebba0851c202c30d8c38a0bd8&X-Amz-SignedHeaders=host";
            var datas = "";// Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(datas);
            string FileFormdataKeyName = "file";
            // var Stream = new StreamReader(filePath);
            using (var client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {

                    content.Add(new StreamContent(new MemoryStream(imageBytes)), FileFormdataKeyName);
                    //var FormDataMessage = await content.ReadAsStringAsync();                
                    using (var message = await client.PostAsync(new Uri(url), content))
                    {
                        var Response = await message.Content.ReadAsStringAsync();
                        Console.WriteLine(message.StatusCode);
                        Console.WriteLine(Response);
                    }
                }
            }
            // return true;
        }
        private static async Task pushTheScreenShot3()
        {
            string url = "";
            var datas = "";
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(datas);



            using (var client = new HttpClient())
            {
                using (var content =
                    new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(new MemoryStream(imageBytes)));
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

                    //HttpResponseMessage response = client.PutAsync(new Uri(url), content).Result;


                    //if (response.IsSuccessStatusCode)
                    //{
                    //    var returnUrls = await response.Content.ReadAsStringAsync();


                    //}
                    //else
                    //{

                    //}
                    //using (
                    //   var message =
                    //       await client.PostAsync("http://www.directupload.net/index.php?mode=upload", content))
                    //{
                    //    var input = await message.Content.ReadAsStringAsync();


                    //}
                }
            }
            using (HttpClient client = new HttpClient())
            {
                //var para = JsonConvert.SerializeObject(heder);
                StringContent data = new StringContent(JsonConvert.SerializeObject(datas), Encoding.UTF8, "image/png");
                StringContent datss = new StringContent(JsonConvert.SerializeObject(datas));
                datss.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
                //client.DefaultRequestHeaders.Accept.Add(
                //   new MediaTypeWithQualityHeaderValue("image/jpg"));
                ByteArrayContent byteContent = new ByteArrayContent(imageBytes);
                byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");


                HttpResponseMessage response = await client.PutAsync(new Uri(url), byteContent);
                var returnUrls = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var returnUrlss = await response.Content.ReadAsStringAsync();


                }
                else
                {

                }
            }
        }

        private static async Task pushTheScreenShot2()
        {
            string url = "";
            var datas = "";
            var heder = new ScreensPutObject
            {
                data = datas,


            };




            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(datas);

            //var nui =  new MemoryStream(Encoding.UTF8.GetBytes(datas));
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);

                // ms.ToArray();
                //var fileStreamResponse = await new HttpClient().PutAsync(url,
                //                                                         imageBytes.ToArray());
                //fileStreamResponse.EnsureSuccessStatusCode();


                HttpClient client = new HttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();
                //byteArray Image
                // ByteArrayContent baContent = new ByteArrayContent(nui);

                //  content.Add(baContent);

                // byte[] data = new byte[] { 1, 2, 3, 4, 5 };
                ByteArrayContent byteContent = new ByteArrayContent(imageBytes);
                var fileStreamResponse = await new HttpClient().PutAsync(
                                                                           new Uri(url),
                                                                           byteContent);
                fileStreamResponse.EnsureSuccessStatusCode();
                //upload MultipartFormDataContent content async and store response in response var
                //var response =
                //    await client.PostAsync(url, content);

                ////read response result as a string async into json var
                //var responsestr = response.Content.ReadAsStringAsync().Result;
            }


            var para = JsonConvert.SerializeObject(heder);
            StringContent data = new StringContent(JsonConvert.SerializeObject(datas), Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            {
                //  client.BaseAddress = new Uri(url);
                //    CommonUtility.LogWriteLine("There was a problem installing the application!");
                //client.DefaultRequestHeaders.Accept.Add(
                //   new MediaTypeWithQualityHeaderValue("application/json"));

                //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CommonUtility.UserSessions.authToken);
                //  StringContent datas = new StringContent(datas, Encoding.UTF8, "application/json");
                //HttpResponseMessage response = client.PutAsync(url, data).Result;
                //var returnUrlss = await response.Content.ReadAsStringAsync();





                //////////////////////////////////
                //var fileStreamResponse = await new HttpClient().PutAsync(
                //                                                           new Uri(url),
                //                                                           data);
                //fileStreamResponse.EnsureSuccessStatusCode();
                ////////////////////////////////////
                ///





                //if (response.IsSuccessStatusCode)
                //{
                //    var returnUrls = await response.Content.ReadAsStringAsync();
                //    var returnData = JsonConvert.DeserializeObject<string>(returnUrls);



                //}
                //else
                //{

                //}
            }
        }


        private static async Task pushTheScreenShot1(string data64, string url, ScreenshotMeta screenshot)
        {
            //var heder = new ScreensPutObject
            //{
            //    data = data64,


            //};
            //var para = JsonConvert.SerializeObject(heder);
            StringContent data = new StringContent(data64, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(
                   new MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage responses = client.PutAsync(url, data).Result;

                HttpResponseMessage response = client.PutAsync(url, data).Result;
                var returnUrlss = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var returnUrls = await response.Content.ReadAsStringAsync();
                    var returnData = JsonConvert.DeserializeObject<string>(returnUrls);

                    using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                    {
                        screenshot.completed = true;
                        _clientDbContext.ScreenshotMetas.Update(screenshot);
                        await _clientDbContext.SaveChangesAsync();

                    }

                    CommonUtility.LogWriteLine("**************************Successfully sync to the aws !" + screenshot.key);

                }
                else
                {
                    //     CommonUtility.LogWriteLine("There was a problem installing the application!");
                    // MessageBox.Show("Error Code");
                    //return false;
                }
            }
        }

        public static async void callPutScreenshotUploadPresignedURLs()
        {
            try
            {
                using (ClientDataStoreDbContext _clientDbContext = new ClientDataStoreDbContext())
                {


                    //var parameters = new Dictionary<string, string>();
                    //parameters["companyId"] = CommonUtility.UserSessions.companyId;

                    List<ScreenshotMeta> headers = new List<ScreenshotMeta>();
                    headers.Add(new ScreenshotMeta
                    {
                        action = "putObject",
                        key = "c568e7e2-6357-44f2-b2eb-8cc13b1a63f8/63102b27-5ac7-4f93-8706-044ef00aee17/2022-03-24/1648113189"

                    });

                    //  parameters["metaObjects"] = JsonConvert.SerializeObject(headers);



                    var heder = new ScreenMetaHeder
                    {
                        companyId = CommonUtility.UserSessions.companyId,
                        metaObjects = headers

                    };


                    var para = JsonConvert.SerializeObject(heder);
                    StringContent data = new StringContent(para, Encoding.UTF8, "application/json");



                    using (HttpClient client = new HttpClient())
                    {


                       // client.BaseAddress = new Uri("https://qa.workshiftly.com/api/users/");
                       // client.BaseAddress = new Uri("https://staging-api.workshiftly.com/api/users/");
                          client.BaseAddress = new Uri("https://portal.workshiftly.com/api/users/");

                        client.DefaultRequestHeaders.Accept.Add(
                           new MediaTypeWithQualityHeaderValue("application/json"));

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CommonUtility.UserSessions.authToken);

                        HttpResponseMessage response = client.PostAsync(CommonUtility.UserSessions.id + "/screenshots", data).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            var returnUrls = await response.Content.ReadAsStringAsync();
                            var returnData = JsonConvert.DeserializeObject<ServerReturnObject>(returnUrls);

                            // var screenShotsDb = _clientDbContext.screenshots.Where(actives=>actives.IsSynced == false && actives.UserId == CommonUtility.UserSessions.id).ToList(); 

                            var listScreenShotsUrl = new List<ScreenshotMeta>();
                            foreach (var item in returnData.data)
                            {
                                var objectMeta = new ScreenshotMeta();
                                objectMeta.key = item.key;
                                objectMeta.action = item.action;
                                objectMeta.url = item.url;
                                objectMeta.expiration = item.expiration;
                                listScreenShotsUrl.Add(objectMeta);
                            }

                            await _clientDbContext.AddAsync(listScreenShotsUrl);
                            await _clientDbContext.SaveChangesAsync();

                        }
                        else
                        {
                            // MessageBox.Show("Error Code");

                        }
                    }

                }


            }
            catch (Exception ex)
            {

            }

        }


        public static async void log()
        {
            HttpClient client = new HttpClient();
          //  client.BaseAddress = new Uri("https://qa.workshiftly.com/api/");
         //   client.BaseAddress = new Uri("https://staging-api.workshiftly.com/api/");
             client.BaseAddress = new Uri("https://portal.workshiftly.com/api/");

            var nn = new { email = "tenant_admin@app.com", password = "password" };

            var parameters = new Dictionary<string, string>();
            parameters["email"] = "sirisena@mailinator.com";
            parameters["password"] = "Password@123";

            HttpResponseMessage response = client.PostAsync("client-login", new FormUrlEncodedContent(parameters)).Result;
            // var resposnse = client.PostAsync("api/api-auth?email=" + nn.email + "& password=" + nn.password, null).Result;



            //var settings = new JsonSerializerSettings
            //{
            //    NullValueHandling = NullValueHandling.Ignore,
            //    MissingMemberHandling = MissingMemberHandling.Ignore
            //};

            if (response.IsSuccessStatusCode)
            {
                var resp = await response.Content.ReadAsStringAsync();

            }
            else
            {
                //*   MessageBox.Show("These credentials do not match our records.");

            }
        }

        #endregion
    }
}

using Emageia.Workshiftly.CoreFunction.IoC.Utility;
using Emageia.Workshiftly.Domain.Concrete;
using Emageia.Workshiftly.Entity.HttpClientModel.activewindo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Emageia.Workshiftly.Entity;
using Emageia.Workshiftly.CoreFunction.IoC.SocketIoClient;
using System.Threading;

namespace Emageia.Workshiftly.CoreFunction.IoC.Service.SocketServer
{
    public class HttpRequestSocketCaller
    {

        #region Active Window sync 
        public static void syncShocketActivityWindows(List<ActivietyWindows> activietyWindows)
        {
            try
            {
               // var oin = await Task.Run(() => CommonUtility.socketIOAdapter.ActiveWindowPost(activietyWindows)).Result;
                  CommonUtility.socketIOAdapter.ActiveWindowPost(activietyWindows);
                

            }
            catch (Exception)
            {
                  
                 // return false;
            }
        }

        public static async void shocketCallPostSyncWorkStatusLogs(List<WorkStatusLog> workStatusLogs)
        {
            try
            {
                // var oin  Task.Run(() => CommonUtility.socketIOAdapter.ActiveWindowPost(activietyWindows))= await.Result;
                if(workStatusLogs.Count > 0) {
                    //Thread InstallClient = new Thread(async () =>  CommonUtility.socketIOAdapter.WorkStatusLogDataPost(workStatusLogs));
                    //InstallClient.Start();
                  // CommonUtility.socketIOAdapter.WorkStatusLogDataPost(workStatusLogs); 
                }
               


            }
            catch (Exception)
            {


            }
        }

        //public static async Task<bool> syncShocketActivityWindows(List<ActivietyWindows> activietyWindows)
        //{
        //    try
        //    {
        //       // var oin = await Task.Run(() => CommonUtility.socketIOAdapter.ActiveWindowPost(activietyWindows)).Result;
        //         var nio = CommonUtility.socketIOAdapter.ActiveWindowPost(activietyWindows).Result;
        //        return nio;

        //    }
        //    catch (Exception)
        //    {

        //        return false;
        //    }
        //}
        //public static async Task<bool> syncShocketActivityWindows(List<ActivietyWindows> activietyWindows)
        //{
        //    //try
        //    //{

        //    //    CommonUtility.LogWriteLines("Success", "HttpRequestCaller.cs syncActivityWindows", "Starting the Sync Activity Windows!", "******" + CommonUtility.ServerPath + "/users/");

        //    //   var oin  =await Task.Run(() => SocketIOAdapter.ActiveWindowPost(activietyWindows)).Result;


        //    //    return true;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    //CommonUtility.LogWriteLines("Error", "HttpRequestCaller.cs syncActivityWindows", "There was a problem Active window sync and update sqlite Db ===== ", ex.Message.ToString());
        //    //    return false;
        //    //}

        //}
        #endregion
    }
}

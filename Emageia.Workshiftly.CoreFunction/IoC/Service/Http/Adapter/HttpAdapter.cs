using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Emageia.Workshiftly.CoreFunction.IoC.Service.Http.Adapter
{
    public static class HttpAdapter
    {
        public static HttpClient ApiClient { get; set; }

        public static void InitializeClient()
        {
            var jwt = "";
            ApiClient = new HttpClient();
           // ApiClient.BaseAddress = new Uri("https://qa.workshiftly.com/api");
          //  ApiClient.BaseAddress = new Uri("https://staging-api.workshiftly.com/api");
            ApiClient.BaseAddress = new Uri("https://portal.workshiftly.com/api");
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jon"));
            ApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        }


    }
}



















//var httpClient = new HttpClient();
//httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

//var json = JsonConvert.SerializeObject(command,
//    Formatting.None, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
//httpClient.DefaultRequestHeaders.Authorization =
//    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

//var content = new StringContent(json, Encoding.UTF8, "application/json");

//var response = httpClient.PostAsync("https://api.nexmo.com/v1/calls", content).Result;
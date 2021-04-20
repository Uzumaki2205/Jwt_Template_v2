using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Jwt_Template.Models
{
    public class RequestHelper
    {
        public static string baseURL = HttpContext.Current.Request.Url.Scheme 
            + "://"+ HttpContext.Current.Request.Url.Authority;

        // GET Request With default baseUrl and url you define
        public static async Task<HttpResponseMessage> GetRequest(string urlRequest)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseURL);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");

                HttpResponseMessage response = await httpClient.GetAsync(urlRequest);
                return response;
            }
        }

        // GET Request With default baseurl + url you define + token authorizations
        public static async Task<HttpResponseMessage> GetRequestWithToken(string urlRequest, string token)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseURL);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.GetAsync(urlRequest);
                return response;
            }
        }

        // GET Request With baseUrl and url you define
        public static async Task<HttpResponseMessage> GetRequest(string baseUrl, string urlRequest)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseUrl);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");

                HttpResponseMessage response = await httpClient.GetAsync(urlRequest);
                return response;
            }
        }

        // GET Request With baseUrl and url you define + token authorization
        public static async Task<HttpResponseMessage> GetRequest(string baseUrl, string urlRequest, string token)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseUrl);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.GetAsync(urlRequest);
                return response;
            }
        }

        // Post Request With baseUrl and url you define + content url encoded
        public static async Task<HttpResponseMessage> PostRequest(string baseUrl, string urlRequest, FormUrlEncodedContent parameters)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseUrl);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");

                HttpResponseMessage response = await httpClient.PostAsync(urlRequest, parameters);

                return response;
            }
        }

        //Post Request With baseUrl and url you define + content url encoded
        public static async Task<HttpResponseMessage> PostRequest(string urlRequest, FormUrlEncodedContent parameters)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseURL);
                //httpClient.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));//ACCEPT header

                HttpResponseMessage response = await httpClient.PostAsync(urlRequest, parameters);

                return response;
            }
        }

        public static async Task<HttpResponseMessage> PostRequestJson(string urlRequest, FormUrlEncodedContent parameters)
        {
            var json = JsonConvert.SerializeObject(parameters, Formatting.Indented);
            var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseURL);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                var response = await httpClient.PostAsync(urlRequest, stringContent);

                return response;
            }
        }
    }
}
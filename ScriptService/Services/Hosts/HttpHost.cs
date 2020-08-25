using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Errors.Exceptions;
using Utf8Json;
using Utf8Json.Resolvers;

namespace ScriptService.Services.Hosts {

    /// <summary>
    /// host providing access to http requests
    /// </summary>
    public class HttpHost  {
        readonly HttpClient httpclient = new HttpClient();

        void CheckHttpResponse(HttpResponseMessage response) {
            if(!response.IsSuccessStatusCode)
                throw new HttpServiceException(response);
        }

        /// <summary>
        /// creates a new request object which can get sent
        /// </summary>
        /// <param name="method">http verb to use</param>
        /// <param name="url">url to send request to</param>
        /// <returns>message to be sent</returns>
        public HttpRequestMessage Request(string method, string url) {
            return new HttpRequestMessage(new HttpMethod(method), url);
        }

        /// <summary>
        /// sends a http request
        /// </summary>
        /// <param name="request">request to send</param>
        /// <returns>response message</returns>
        public Task<HttpResponseMessage> Send(HttpRequestMessage request) {
            return httpclient.SendAsync(request);
        }

        HttpRequestMessage CreateRequest(string url, HttpMethod method, object body) {
            HttpRequestMessage request = new HttpRequestMessage(method, url);

            if (body != null) {
                if (body is HttpContent httpcontent)
                    request.Content = httpcontent;
                else
                    request.Content = new StringContent(JsonSerializer.ToJsonString(body, StandardResolver.ExcludeNullCamelCase), Encoding.UTF8, "application/json");
            }

            return request;
        }

        async Task<object> Send(HttpMethod method, string url, object content) {
            HttpResponseMessage response = await httpclient.SendAsync(CreateRequest(url, method, content));
            CheckHttpResponse(response);
            if(response.Content.Headers.ContentLength == 0)
                return Task.FromResult((object)null);
            return await JsonSerializer.DeserializeAsync<object>(await response.Content.ReadAsStreamAsync());
        }

        /// <summary>
        /// sends data to a server using POST
        /// </summary>
        /// <param name="url">url to send message to</param>
        /// <param name="content">content to send</param>
        /// <returns>response object</returns>
        public Task<object> Post(string url, object content) {
            return Send(HttpMethod.Post, url, content);
        }

        /// <summary>
        /// sends data to a server using PUT
        /// </summary>
        /// <param name="url">url to send message to</param>
        /// <param name="content">content to send</param>
        /// <returns>response object</returns>
        public Task<object> Put(string url, object content) {
            return Send(HttpMethod.Put, url, content);
        }

        /// <summary>
        /// sends data to a server using PATCH
        /// </summary>
        /// <param name="url">url to send message to</param>
        /// <param name="content">content to send</param>
        /// <returns>response object</returns>
        public Task<object> Patch(string url, object content) {
            return Send(HttpMethod.Patch, url, content);
        }

        /// <summary>
        /// receives data from a server using GET
        /// </summary>
        /// <param name="url">url to send message to</param>
        /// <returns>response object</returns>
        public Task<object> Get(string url) {
            return Send(HttpMethod.Get, url, null);
        }

        /// <summary>
        /// deletes data on a server using DELETE
        /// </summary>
        /// <param name="url">url identifying resource to delete</param>
        /// <returns>response object</returns>
        public Task Delete(string url) {
            return Send(HttpMethod.Delete, url, null);
        }
    }
}
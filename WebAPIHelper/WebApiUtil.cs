using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebAPIHelper.Models;

namespace WebAPIHelper
{
    /// <summary>
    /// Enum with all supported HttpMethods
    /// </summary>
    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    /// <summary>
    /// Class with main methods to consume an API
    /// </summary>
    public static class WebApiUtil
    {
        /// <summary>
        /// Converts any single level object do a http query string
        /// and then appends a '?' to the beginning
        /// </summary>
        /// <typeparam name="T">Type of object to convert to query string</typeparam>
        /// <param name="obj">Instance to convert to query string</param>
        /// <returns>A string in data string format</returns>
        public static string ToQueryString<T>(this T obj) where T : class
        {
            string ret = "";

            if (ret != null)
            {
                var props = obj.GetType().GetProperties();
                foreach (var prop in props)
                {
                    var v = prop.GetValue(obj, null);
                    if (v != null)
                    {
                        if (ret.Length > 0)
                        {
                            ret += "&";
                        }

                        ret += $"{prop.Name}={Uri.EscapeDataString(v.ToString())}";
                    }
                }

                if (ret.Length > 0)
                {
                    ret = "?" + ret;
                }
            }

            return ret;
        }

        /// <summary>
        /// Create a http request
        /// </summary>
        /// <typeparam name="T">Type of object to convert to query string</typeparam>
        /// <param name="method">Http method to be used in the request</param>
        /// <param name="url">API url</param>
        /// <param name="queryStringObject">Instance to convert to query string</param>
        /// <returns>A new HttpWebRequest</returns>
        public static HttpWebRequest CreateRequest<T>(HttpMethod method, string url, T queryStringObject) where T : class
        {
            if (queryStringObject == null)
            {
                throw new ArgumentNullException(nameof(queryStringObject));
            }
            url += queryStringObject.ToQueryString();
            return CreateRequest(method, url);
        }

        /// <summary>
        /// Create a http request
        /// </summary>
        /// <param name="method">Http method to be used in the request</param>
        /// <param name="url">API url</param>
        /// <returns>A new HttpWebRequest</returns>
        public static HttpWebRequest CreateRequest(HttpMethod method, string url)
        {
            var http = (HttpWebRequest)WebRequest.Create(url);
            http.ContentLength = 0;
            http.Method = method.ToString();
            return http;
        }

        /// <summary>
        /// Append headers to the request
        /// </summary>
        /// <param name="http">Any instance of HttpWebRequest</param>
        /// <param name="headers">string array of headers</param>
        /// <returns>The same instance of HttpWebRequest that was passed from http parameter</returns>
        public static HttpWebRequest AddHeaders(this HttpWebRequest http, params string[] headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (header.Length > 0)
                    {
                        http.Headers.Add(header);
                    }
                }
            }
            return http;
        }

        /// <summary>
        /// Add a json body to the request
        /// </summary>
        /// <typeparam name="T">Type of body</typeparam>
        /// <param name="http">Any instance of HttpWebRequest</param>
        /// <param name="body">Instance of anything you need to send as body</param>
        /// <returns>The same instance of HttpWebRequest that was passed from http parameter</returns>
        public static HttpWebRequest AddBody<T>(this HttpWebRequest http, T body)
        {
            if (body != null)
            {
                var json = JsonConvert.SerializeObject(body);
                http.ContentType = "application/json";
                byte[] postByte = Encoding.UTF8.GetBytes(json);
                http.ContentLength = postByte.Length;

                Stream req = http.GetRequestStream();
                req.Write(postByte, 0, postByte.Length);
                req.Flush();
            }
            return http;
        }

        /// <summary>
        /// Executes the HttpWebRequest and return if the request successded or failed.
        /// </summary>
        /// <param name="http">Any instance of HttpWebRequest</param>
        /// <returns>Instance of Custom Reponse</returns>
        public static CustomResponse GetResponseOrException(this HttpWebRequest http)
        {
            return http.GetResponseOrExceptionAsync().Result;
        }

        /// <summary>
        /// Executes the HttpWebRequest and return if the request successded or failed.
        /// Also returns the value from the request
        /// </summary>
        /// <typeparam name="T">Type of the return value</typeparam>
        /// <param name="http">Any instance of HttpWebRequest</param>
        /// <returns>Instance of Custom Reponse</returns>
        public static CustomResponse<T> GetResponseOrException<T>(this HttpWebRequest http)
        {
            return GetResponseOrExceptionAsync<T>(http).Result;
        }

        /// <summary>
        /// Executes the HttpWebRequest and return if the request successded or failed.
        /// </summary>
        /// <param name="http">Any instance of HttpWebRequest</param>
        /// <returns>Instance of Custom Reponse</returns>
        public static async Task<CustomResponse> GetResponseOrExceptionAsync(this HttpWebRequest http)
        {
            try
            {
                await http.GetResponseAsync();
                return CustomResponse.Success();
            }
            catch (WebException ex) when (ex.Response != null)
            {
                return CustomResponse.Error(ex.GetErrorMessage());
            }
            catch (Exception ex)
            {
                return CustomResponse.Error(ex.Message);
            }
        }

        /// <summary>
        /// Executes the HttpWebRequest and return if the request successded or failed.
        /// Also returns the value from the request
        /// </summary>
        /// <typeparam name="T">Type of the return value</typeparam>
        /// <param name="http">Any instance of HttpWebRequest</param>
        /// <returns>Instance of Custom Reponse</returns>
        public static async Task<CustomResponse<T>> GetResponseOrExceptionAsync<T>(this HttpWebRequest http)
        {
            try
            {
                using (var response = await http.GetResponseAsync())
                {
                    using (var data = response.GetResponseStream())
                    {
                        using (var stream = new StreamReader(data))
                        {
                            var json = stream.ReadToEnd();
                            var o = JsonConvert.DeserializeObject<T>(json);

                            return CustomResponse<T>.Success(o);
                        }
                    }
                }
            }
            catch (WebException ex) when (ex.Response != null)
            {
                return CustomResponse<T>.Error(ex.GetErrorMessage());
            }
            catch (Exception ex)
            {
                return CustomResponse<T>.Error(ex.Message);
            }
        }

        /// <summary>
        /// Extracts an error message from a WebException
        /// </summary>
        /// <param name="we">Web exception to be handled</param>
        /// <returns>String containing error message from the response</returns>
        private static string GetErrorMessage(this WebException we)
        {
            using (var stream = we.Response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
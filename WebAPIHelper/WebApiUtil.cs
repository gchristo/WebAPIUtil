using WebAPIHelper.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace WebAPIHelper
{
    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public static class WebApiUtil
    {
        static WebApiUtil()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

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

        public static HttpWebRequest CreateRequest<T>(HttpMethod method, string url, T queryStringObject) where T : class
        {
            url += queryStringObject.ToQueryString();
            return CreateRequest(method, url);
        }

        public static HttpWebRequest CreateRequest(HttpMethod method, string url)
        {
            var http = (HttpWebRequest)WebRequest.Create(url);
            http.Method = method.ToString();
            return http;
        }

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

        public static CustomResponse GetResponseOrException(this HttpWebRequest http)
        {
            var ret = new CustomResponse();
            try
            {
                http.GetResponse();
                ret.ResponseType = ResponseType.SUCCESS;
            }
            catch (WebException ex) when (ex.Response != null)
            {
                using (var stream = ex.Response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        ret.ResponseType = ResponseType.ERROR;
                        ret.Message = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ret;
        }

        public static CustomResponse<T> GetResponseOrException<T>(this HttpWebRequest http) where T : class, new()
        {
            var ret = new CustomResponse<T>();
            try
            {
                using (var response = (HttpWebResponse)http.GetResponse())
                {
                    using (var data = response.GetResponseStream())
                    {
                        using (var stream = new StreamReader(data))
                        {
                            var json = stream.ReadToEnd();

                            ret.ResponseType = ResponseType.SUCCESS;
                            ret.ReturnValue = JsonConvert.DeserializeObject<T>(json);
                        }
                    }
                }
            }
            catch (WebException ex) when (ex.Response != null)
            {
                using (var stream = ex.Response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        ret.ResponseType = ResponseType.ERROR;
                        ret.Message = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ret;
        }
    }
}
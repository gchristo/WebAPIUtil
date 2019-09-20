using Models;
using System;
using WebAPIHelper;

namespace TestNetFramework
{
    class Program
    {
        private static readonly string URL = "http://localhost:5000/api/MyAPI";

        static void Main(string[] args)
        {
            Console.WriteLine("PRESS ANY KEY TO START");
            Console.ReadKey();

            Console.WriteLine("TEST WITH ASYNC");
            AsyncTest();

            Console.WriteLine("TEST WITH BODY");
            TestWithBody();

            Console.WriteLine("--------------------");
            Console.WriteLine("TEST WITH QUERY STRING");
            TestWithQueryString();

            Console.WriteLine("PRESS ANY KEY TO CLOSE");
            Console.ReadKey();
        }

        private static void AsyncTest()
        {
            var t1 = WebApiUtil
                        .CreateRequest(HttpMethod.POST, URL + "/async")
                        .AddBody(10)
                        .GetResponseOrExceptionAsync<DateTime>();

            var startT1 = DateTime.Now;
            Console.WriteLine("Started t1: " + startT1);

            var t2 = WebApiUtil
                        .CreateRequest(HttpMethod.POST, URL + "/async")
                        .AddBody(5)
                        .GetResponseOrExceptionAsync<DateTime>();

            var startT2 = DateTime.Now;
            Console.WriteLine("Started t2: " + startT2);

            var t3 = WebApiUtil
                        .CreateRequest(HttpMethod.POST, URL + "/async")
                        .AddBody(2)
                        .GetResponseOrExceptionAsync<DateTime>();

            var startT3 = DateTime.Now;
            Console.WriteLine("Started t3: " + startT3);

            Console.WriteLine("TotalTime t1: " + t1.Result.ReturnValue.Subtract(startT1));
            Console.WriteLine("TotalTime t2: " + t2.Result.ReturnValue.Subtract(startT2));
            Console.WriteLine("TotalTime t3: " + t3.Result.ReturnValue.Subtract(startT3));
        }

        private static void TestWithBody()
        {
            var b = new TestRequestBody() { i = 10, s = "hello" };

            var resp = WebApiUtil.CreateRequest(HttpMethod.POST, URL)
                                .AddBody(b)
                                .GetResponseOrException<TestResponse>();

            if (resp.ResponseType == WebAPIHelper.Models.ResponseType.ERROR)
            {
                Console.WriteLine(resp.Message);
            }
            else
            {
                Console.WriteLine("Nice");
                Console.WriteLine(resp.ReturnValue.stringValue);
            }
        }

        private static void TestWithQueryString()
        {
            var b = new { number = 50, text = "this is my string" };

            var resp = WebApiUtil.CreateRequest(HttpMethod.GET, URL, b)
                                .GetResponseOrException<TestResponse>();

            if (resp.ResponseType == WebAPIHelper.Models.ResponseType.ERROR)
            {
                Console.WriteLine(resp.Message);
            }
            else
            {
                Console.WriteLine("Nice");
                Console.WriteLine(resp.ReturnValue.stringValue);
            }
        }
    }
}

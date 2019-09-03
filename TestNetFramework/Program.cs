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

            Console.WriteLine("TEST WITH BODY");
            TestWithBody();

            Console.WriteLine("--------------------");
            Console.WriteLine("TEST WITH QUERY STRING");
            TestWithQueryString();

            Console.WriteLine("PRESS ANY KEY TO CLOSE");
            Console.ReadKey();
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

using Internal.Runtime.CompilerServices;
using MOOS.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text;

namespace MOOS.Api
{
    internal static unsafe class APINET
    {
        public static unsafe void* HandleSystemCall(string name)
        {
            switch (name)
            {
                case "HttpClient_Create":
                    return (delegate*<string, int, IntPtr>)&API_HttpClient_Create;
                case "HttpClient_GetAsync":
                    return (delegate*<IntPtr, string, IntPtr>)&HttpClient_GetAsync;
            }

            return null;
        }

        static IntPtr HttpClient_GetAsync(IntPtr handler, string path)
        {
            HttpClient client = Unsafe.As<IntPtr, HttpClient>(ref handler);
            return client.GetAsync(path);
        }

        static IntPtr API_HttpClient_Create(string host, int port)
        {
            return new HttpClient(host, port);
        }
    }
}

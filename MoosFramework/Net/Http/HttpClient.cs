using Internal.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Net.Http
{

    public class HttpClient
    {
        [DllImport("HttpClient_Create")]
        static extern IntPtr HttpClient_Create(string host, int port);

        [DllImport("HttpClient_GetAsync")]
        static extern IntPtr HttpClient_GetAsync(IntPtr handler, string path);

        HttpContent http { set; get; } 
        IntPtr handler { set; get; }

        public HttpClient()
        {
            http =  new HttpContent();
        }

        public HttpContent GetAsync(string requestUri)
        {
            var (protocol, host, port, path) = ExtractUrlComponents(requestUri);

            if (handler == IntPtr.Zero)
            {
                handler = HttpClient_Create(host, port);
                Console.WriteLine($"Handler: {handler}");
            }

            IntPtr _handler = HttpClient_GetAsync(handler, path);
            Console.WriteLine($"_handler: {_handler}");

            http = Unsafe.As<IntPtr, HttpContent>(ref _handler);

            http.Protocol = protocol;
            http.Host = host;
            http.Port = port;
            http.Path = path;
            return http;
        }

        (string, string , int, string) ExtractUrlComponents(string url)
        {
            string protocol = string.Empty, host = string.Empty, path = string.Empty;
            int port = 80;

            if(string.IsNullOrEmpty(url))
            {
                return (protocol, host, port, path);
            }

            url = url.ToLower();

            // Buscar el índice de "://", indicando el final del protocolo
            int protocolIndex = url.IndexOf("://");
            if (protocolIndex != -1)
            {
                // Extraer el protocolo
                protocol = url.Substring(0, protocolIndex);

                // Eliminar el protocolo de la URL
                url = url.Substring(protocolIndex + 3);
            }

            // Buscar el índice de "/", indicando el final del host
            int pathIndex = url.IndexOf('/');
            if (pathIndex != -1)
            {
                // Extraer el host
                host = url.Substring(0, pathIndex);

                // Extraer el path
                path = url.Substring(pathIndex);
            }
 
            if(protocol == "https")
            {
                port = 443;
            }

            return (protocol, host, port, path);
        }
    }
}

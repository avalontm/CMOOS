using Internal.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Net.Http
{
    public class HttpClient
    {
        [DllImport("HttpClient_Create")]
        static extern IntPtr HttpClient_Create(string host, int port);

        [DllImport("HttpClient_GetAsync")]
        static extern IntPtr HttpClient_GetAsync(IntPtr handler, string path);

        IntPtr handler { set; get; }

        string protocol = "http", host = "localhost", path = "/";
        int port = 80;


        public HttpClient()
        {

        }

        public HttpContent GetAsync(string requestUri)
        {
            ExtractUrlComponents(requestUri);

            Console.WriteLine($"protocol: {protocol}");
            Console.WriteLine($"host: {host}");
            Console.WriteLine($"port: {port}");
            Console.WriteLine($"path: {path}");

            if (handler == IntPtr.Zero)
            {
                handler = HttpClient_Create(host, port);
            }

            IntPtr _handler = HttpClient_GetAsync(handler, path);

            HttpContent http = Unsafe.As<IntPtr, HttpContent>(ref _handler);

            http.Protocol = protocol;
            http.Host = host;
            http.Port = port;
            http.Path = path;

            return http;
        }

        void ExtractUrlComponents(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
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
            else
            {
                // Si no hay "/", la URL es solo el host
                host = url;
                path = "/";
            }

            if (protocol == "https")
            {
                port = 443;
            }
        }


        public override void Dispose()
        {
            protocol.Dispose();
            host.Dispose();
            path.Dispose();
            port.Dispose();
            handler.Dispose();
            base.Dispose();
        }
    }
}

using System;

namespace System.Net.Http
{
    public class HttpContent
    {
        public int Status { set; get; }
        public int Lenght { get; set; }
        public string Content { set; get; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
        public string Protocol { get; set; }

        public HttpContent()
        {
            Content = string.Empty;
        }

        public override string ToString()
        {
            return Content;
        }
    }
}

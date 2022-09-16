using System;

namespace System.Net.Http
{
    public class HttpContent
    {
        public int Status { set; get; }
        public int Lenght { get; set; }
        public string Content { set; get; }

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

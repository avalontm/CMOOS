using MOOS;
using MOOS.Driver;
using MOOS.NET.Config;
using MOOS.NET.IPv4;
using MOOS.NET.IPv4.TCP;
using MOOS.NET.IPv4.UDP.DNS;
using System;
using System.Helpers;
using System.Text;
using System.Windows;

namespace System.Net.Http
{
    internal class HttpClient
    {
        private TcpClient client {  get; set; }
        private int port { get; set; }
        private string protocol { get; set; }
        private string host { get; set; }
        private int timeout { get; set; }
        private Address address { get; set; }

        public HttpClient(string host, int port = 80)
        {
            if (port == 443)
            {
                this.protocol = "https";
            }
            else
            {
                this.protocol = "http";
            }
            this.host = host;
            this.port = port;
            this.timeout = 10;
            this.client = new TcpClient(this.port);
        }

        public HttpContent GetAsync(string path)
        {
            HttpContent http = new HttpContent();
            http.Status = 404;
 
            DnsClient dns = new DnsClient();

            dns.Connect(DNSConfig.DNSNameservers[0]); //DNS Server address
           
            dns.SendAsk(host);

            while (address == null)
            {
                address = dns.Receive();
            }

            if (!client.IsConnected)
            {
                if (!client.Connect(this.address, this.port, timeout * 1000))
                {
                    return http;
                }
            }

            string header = $"GET {path} HTTP/1.1\r\n";
            header += $"Host: {host}\r\n";
            header += "Connection: close\r\n";
            header += "Accept: text/plain; charset=utf-8\r\n";
            header += "User-Agent: Moos/0.0.1\r\n";
            header += "Accept-Encoding: gzip, deflate, sdch\r\n";
            header += "\r\n\r\n";

            byte[] data = Encoding.UTF8.GetBytes(header);
          
            client.Send(data);

            /** Receive data **/
            EndPoint endpoint = new EndPoint(Address.Zero, 0);
            byte[] receive = client.Receive(ref endpoint);  //set endpoint to remote machine IP:port

            if (receive == null || receive.Length == 0)
            {
                return http;
            }

            http.Status = 200;

            string response = Encoding.ASCII.GetString(receive);

            if (!string.IsNullOrEmpty(response))
            {
                var index = BinaryMatch(receive, Encoding.ASCII.GetBytes("\r\n\r\n")) + 4;

                string headers = Encoding.ASCII.GetString(receive, 0, index);

                if (headers.IndexOf("Content-Encoding: gzip") > 0)
                {
                    Console.WriteLine("Not implement.");
                }
                else
                {
                    //http.Lenght = Convert.HexToDec(lenght);
                    http.Content = Encoding.ASCII.GetString(receive, index, receive.Length - index);
                }
            }

            Close();
            return http;
        }

        public void Close()
        {
            if (client.IsConnected)
            {
                client.Close();
            }
        }

        int BinaryMatch(byte[] input, byte[] pattern)
        {
            int sLen = input.Length - pattern.Length + 1;
            for (int i = 0; i < sLen; ++i)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; ++j)
                {
                    if (input[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}

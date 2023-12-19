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
    public class HttpClient
    {
        TcpClient client;
        int port;
        string protocol;
        string host;
        int timeout = 10;

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
            this.client = new TcpClient(this.port);
        }

        public HttpContent GetAsync(string path)
        {
           
            HttpContent http = new HttpContent();
            http.Status = 404;
 
            DnsClient dns = new DnsClient();

            Timer.Sleep(100);
    
            dns.Connect(DNSConfig.DNSNameservers[0]); //DNS Server address
           
            dns.SendAsk(host);

            Address _address = null;

            while (_address == null)
            {
                _address = dns.Receive();
            }

            Timer.Sleep(100);

            if (!client.IsConnected())
            {
                if (!client.Connect(_address, this.port, timeout * 1000))
                {
                    return http;
                }
            }

            string header = $"GET /{path} HTTP/1.1\r\n";
            header += $"Host: {host}\r\n";
            header += "Connection: Keep-Alive\r\n";
            header += "Accept: */*\r\n";
            header += "User-Agent: Moos/0.0.1\r\n";
            header += "Accept-Encoding: gzip, deflate, br\r\n";
            header += "\r\n\r\n";

            byte[] data = Encoding.UTF8.GetBytes(header);
          
            client.Send(data);
            Console.WriteLine($"[Send] {data.Length}");

            /** Receive data **/
            EndPoint endpoint = new EndPoint(Address.Zero, 0);
            byte[] receive = client.Receive(ref endpoint);  //set endpoint to remote machine IP:port

            if (receive == null || receive.Length == 0)
            {
                return http;
            }

            http.Status = 200;
            Console.WriteLine($"[STATUS ] {http.Status}");
            string response = Encoding.UTF8.GetString(receive);
            Console.WriteLine($"[RESPONSE] {response}");
            if (!string.IsNullOrEmpty(response))
            {
                var index = BinaryMatch(receive, Encoding.UTF8.GetBytes("\r\n\r\n")) + 4;

                string result = response.Substring(index, response.Length - 1);

                index = BinaryMatch(Encoding.UTF8.GetBytes(result), Encoding.UTF8.GetBytes("\r\n"));
                string lenght = result.Substring(0, index).Trim();
                http.Lenght = Convert.HexToDec(lenght);
                http.Content = result.Substring((lenght.Length + 2), http.Lenght + (lenght.Length + 2));
            }

            Close();
            return http;
        }

        public void Close()
        {
            if (client.IsConnected())
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

using MOOS;
using MOOS.NET.IPv4;
using MOOS.NET.IPv4.TCP;
using System;
using System.Helpers;
using System.Text;

namespace System.Net.Http
{
    public class HttpClient
    {
        TcpClient client;
        Address address;
        int port;
        string host;
        int timeout = 10;
        public HttpClient(string host, int port = 80)
        {
            this.address = Address.Parse(host);
            this.port = port;
            this.host = host;
            client = new TcpClient(port);
        }

        public HttpContent GetAsync(string path)
        {
            HttpContent http = new HttpContent();
            http.Status = 404;

            if (!client.IsConnected())
            {
                client.Connect(this.address, this.port, timeout * 1000);
            }

            if (!client.IsConnected())
            {
                Console.WriteLine($"[HttpClient] Not Connected!");
                return http;
            }

            string header = $"GET http://{host}:{port}/{path} HTTP/1.1\r\n";
            header += $"Host: {host}:{port}\r\n";
            header += "Connection: keep-alive\r\n";
            header += "Accept: */*\r\n";
            header += "User-Agent: Moos/0.0.1\r\n";
            header += "Accept-Encoding: gzip, deflate, br\r\n";
            header += "\r\n\r\n";

            byte[] data = Encoding.ASCII.GetBytes(header);

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

            var index = BinaryMatch(receive, Encoding.ASCII.GetBytes("\r\n\r\n")) + 4;

            string result = response.Substring(index, response.Length - 1);

            index = BinaryMatch(Encoding.ASCII.GetBytes(result), Encoding.ASCII.GetBytes("\r\n"));
            string lenght = result.Substring(0, index).Trim();
            http.Lenght = Convert.HexToDec(lenght);
            http.Content = result.Substring((lenght.Length + 2), http.Lenght + (lenght.Length + 2));
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

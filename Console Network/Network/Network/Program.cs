using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Network
{
    class Program
    {
        static string host;
        static int readPort;
        static int sendPort;

        static int key;

        static void Main(string[] args)
        {
            host = "127.0.0.1";

            Console.Write("Порт для чтения: ");
            readPort = int.Parse(Console.ReadLine());

            Console.Write("Порт для отправки: ");
            sendPort = int.Parse(Console.ReadLine());

            Console.Write("Ключ шифрации: ");
            key = int.Parse(Console.ReadLine());

            Thread readThread = new Thread(ReadData) { IsBackground = true };
            readThread.Start();

            SendData();
        }

        static void SendData()
        {
            UdpClient client = new UdpClient();

            try
            {
                while(true)
                {
                    string message = Console.ReadLine();
                    string cryptoMessage = Crypto(message, key);

                    byte[] data = Encoding.UTF8.GetBytes(cryptoMessage);

                    client.Send(data, data.Length, host, sendPort);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void ReadData()
        {
            UdpClient client = new UdpClient(readPort);
            IPEndPoint ip = null;

            try
            {
                while (true)
                {
                    byte[] data = client.Receive(ref ip);
                    string message = Encoding.UTF8.GetString(data);
                    string cryptoMessage = Crypto(message, key);

                    Console.WriteLine($"{ip.Address.ToString()}:{ip.Port} - {cryptoMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static string Crypto(string text, int key)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);

            for (int i = 0; i < data.Length; i++)
                data[i] ^= (byte)key;

            return Encoding.UTF8.GetString(data);
        }
    }
}

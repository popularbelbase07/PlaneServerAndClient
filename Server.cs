using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ModelLib;
using Newtonsoft.Json;

namespace PlaneServer
{
   public class Client

    {
        static void Main(string[] args)
        {
            int port = 6789;
            int clientNr = 0;
            Console.WriteLine("Hello echo server");
            IPAddress ip = GetIp();
            TcpListener serverListener = StartServer(ip, port);
            do
            {
                TcpClient clientConnection = GetConnectionSocket(serverListener, ref clientNr);
                Task.Run(() => ReadWriteStream(clientConnection, ref clientNr));

            } while (clientNr != 0);

            StopServer(serverListener);
        }

        private static void StopServer(TcpListener serverListener)
        {
            serverListener.Stop();
            Console.WriteLine("Server Listener Stopped !!");
        }

        private static TcpClient GetConnectionSocket(TcpListener serverListener, ref int clientNr)
        {
            TcpClient connectionSocket = serverListener.AcceptTcpClient();
            clientNr++;
            Console.WriteLine("client " + clientNr + "Connected");
            return connectionSocket;
        }

        private static void ReadWriteStream(TcpClient connectionSocket, ref int clientNr)
        {
            Stream ns = connectionSocket.GetStream();
            StreamWriter sw = new StreamWriter(ns);
            StreamReader sr = new StreamReader(ns);
            sw.AutoFlush = true;
            //string message = sr.ReadLine();
            //Deserialized the object
            Car recievecar = JsonConvert.DeserializeObject<Car>(sr.ReadLine());
            string message = recievecar.RegistrationNumber;
           
            Thread.Sleep(1000);
            string Answer = " ";
            while (message != null && message != " ")
            {
                Console.WriteLine("Client " + clientNr + "Message");
                Answer = message.ToUpper();
                sw.WriteLine(Answer);
                message = sr.ReadLine();
                Thread.Sleep(4000);

            }

            Console.WriteLine("Empty message detected");
            ns.Close();
            connectionSocket.Close();
            clientNr--;
            Console.WriteLine("Connection Socket " + clientNr + "Closed ");
        }

        private static TcpListener StartServer(IPAddress ip, int port)
        {
            TcpListener serverSocket = new TcpListener(ip, port);
            serverSocket.Start();
            Console.WriteLine("Server started waiting for connection !!");
            return serverSocket;

        }

        public static IPAddress GetIp()
        {
            string name = "google.com";
            IPAddress[] address = Dns.GetHostEntry(name).AddressList;
            Console.WriteLine("Google id returned by GetHostEntry" + address[0]);
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Console.WriteLine("Local Host Ip : " + ip);
            return ip;
        }

    }
}

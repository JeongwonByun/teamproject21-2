using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BingoServer
{
    class Program
    {
        public static Dictionary<TcpClient, string> clientList = new Dictionary<TcpClient, string>();
        public static Dictionary<string, TcpClient> bingoReadyUser = new Dictionary<string, TcpClient>();
        public static Dictionary<string, string> saveUserBingo = new Dictionary<string, string>();

        public static string[] userList = new string[2];

        public static int[,] bingoA = new int[5, 5];
        public static int[,] bingoB = new int[5, 5];

        public static int playerA_bingoCount { get; set; }
        public static int playerB_bingoCount { get; set; }
        public static string playerA { get; set; }
        public static string playerB { get; set; }
        public static string nextUser { get; set; }
        public static string userID { get; set; }

        public static void sendAll(string msg)
        {
            foreach (var list in clientList) 
            {
                TcpClient client = list.Key as TcpClient;
                NetworkStream stream = client.GetStream();
                StreamWriter sendMsg = new StreamWriter(stream, Encoding.UTF8);

                sendMsg.WriteLine(msg); 
                sendMsg.Flush();
            }
        }

        static void Main(string[] args)
        {
            TcpListener Listener = null;
            TcpClient client = null;

            NetworkStream NS = null;
            StreamReader SR = null;
            StreamWriter SW = null;

            const int PORT = 7777;

            Console.WriteLine("빙고서버가 시작되었습니다.");
            Console.WriteLine("빙고서버 포트 : {0}", PORT);
            Console.WriteLine("----------------------------");

            int tryingCount = 0;

            try
            {
                Listener = new TcpListener(PORT);
                Listener.Start(); 

                while (true)
                {
                    client = Listener.AcceptTcpClient();
                    NS = client.GetStream(); // 소켓에서 메시지를 가져오는 스트림
                    SR = new StreamReader(NS, Encoding.UTF8); // Get message
                    SW = new StreamWriter(NS, Encoding.UTF8); // Send message

                    string GetMessage = SR.ReadLine();
                    string[] MsgResult = GetMessage.Split('|');

                    if (MsgResult[0] == "my-id") 
                    {
                        Console.WriteLine("클라이언트 {0}의 접속이 감지되었습니다. - ID : {1} [{2}]", ((IPEndPoint)client.Client.RemoteEndPoint).ToString(), MsgResult[1], DateTime.Now); // 클라이언트 구별

                        if (clientList.ContainsKey(client) == true || userList[0] == MsgResult[1])
                        {
                            SW.WriteLine("permission-denied"); 
                            SW.Flush();
                            Console.WriteLine("클라이언트 {0}의 연결을 차단했습니다. - ID : {1} [{2}]", ((IPEndPoint)client.Client.RemoteEndPoint).ToString(), MsgResult[1], DateTime.Now); // 클라이언트 구별
                        }
                        else if (clientList.Count >= 2)
                        {
                            SW.WriteLine("#Full#"); 
                            SW.Flush();
                            Console.WriteLine("방이 가득찼습니다. [{0}]", DateTime.Now);
                        }
                        else
                        {
                            SW.WriteLine("permission-granted");
                            Console.WriteLine("접속이 승인되었습니다. [{0}]", DateTime.Now);
                            SW.Flush();
                            userList[tryingCount] = MsgResult[1];
                            tryingCount++;
                        }
                    }
                    else if (MsgResult[0] == "#login#")
                    {
                        clientList.Add(client, MsgResult[1]); 
                        Console.WriteLine("클라이언트 {0}가 로그인하였습니다. - ID : {1} [{2}]", ((IPEndPoint)client.Client.RemoteEndPoint).ToString(), MsgResult[1], DateTime.Now); // 클라이언트 구별

                        sendAll("#MSG#|" + MsgResult[1] + "님이 접속했습니다.");
                        userID = MsgResult[1];
                        Receiver r = new Receiver();
                        r.startClient(client);
                    }

                    Console.WriteLine("현재 유저 수 {0}명 [{1}]", clientList.Count, DateTime.Now);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

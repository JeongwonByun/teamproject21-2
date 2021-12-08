using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;

namespace BingoClient
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        static int myBingoNumber = 1;
        //List<string> myBingoFood = new List<string>(new string[]{
        //    "ㄱ",
        //    "ㄴ",
        //    "ㄷ"
        //});
        //static int myBingoFood = 65;
        static string gameMode = "initializing";
        int[,] bingo = new int[5, 5]; // 빙고 숫자 저장
        string mode = "number";

        NetworkStream NS = null;
        StreamReader SR = null;
        StreamWriter SW = null;
        TcpClient client = null;

        string userID = string.Empty;
        int PORT = 0;
        string IP = string.Empty;

        public MainWindow(string str1, string str2, int num)
        {
            InitializeComponent();

            userID = str1;
            IP = str2;
            PORT = num;
            connect();
        }

        private void btnRezero_Click(object sender, RoutedEventArgs e)
        {
            initializing();
        }

        private void initializing()
        {
            
                myBingoNumber = 1;

                btn1.Content = "";
                btn1.IsEnabled = true;
                btn2.Content = "";
                btn2.IsEnabled = true;
                btn3.Content = "";
                btn3.IsEnabled = true;
                btn4.Content = "";
                btn4.IsEnabled = true;
                btn5.Content = "";
                btn5.IsEnabled = true;
                btn6.Content = "";
                btn6.IsEnabled = true;
                btn7.Content = "";
                btn7.IsEnabled = true;
                btn8.Content = "";
                btn8.IsEnabled = true;
                btn9.Content = "";
                btn9.IsEnabled = true;
                btn10.Content = "";
                btn10.IsEnabled = true;
                btn11.Content = "";
                btn11.IsEnabled = true;
                btn12.Content = "";
                btn12.IsEnabled = true;
                btn13.Content = "";
                btn13.IsEnabled = true;
                btn14.Content = "";
                btn14.IsEnabled = true;
                btn15.Content = "";
                btn15.IsEnabled = true;
                btn16.Content = "";
                btn16.IsEnabled = true;
                btn17.Content = "";
                btn17.IsEnabled = true;
                btn18.Content = "";
                btn18.IsEnabled = true;
                btn19.Content = "";
                btn19.IsEnabled = true;
                btn20.Content = "";
                btn20.IsEnabled = true;
                btn21.Content = "";
                btn21.IsEnabled = true;
                btn22.Content = "";
                btn22.IsEnabled = true;
                btn23.Content = "";
                btn23.IsEnabled = true;
                btn24.Content = "";
                btn24.IsEnabled = true;
                btn25.Content = "";
                btn25.IsEnabled = true;

            btnRandom.IsEnabled = true;
            btnRezero.IsEnabled = true;
            gameMode = "initializing";
        }

        private void btnRandom_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            int[] list = Enumerable.Range(1, 25).OrderBy(o => rand.Next()).ToArray(); // 리스트에 1부터 25까지의 숫자를 넣기

            // 리스트에 넣은 숫자를 각 버튼에 할당
            btn1.Content = list[0];
            btn2.Content = list[1];
            btn3.Content = list[2];
            btn4.Content = list[3];
            btn5.Content = list[4];
            btn6.Content = list[5];
            btn7.Content = list[6];
            btn8.Content = list[7];
            btn9.Content = list[8];
            btn10.Content = list[9];
            btn11.Content = list[10];
            btn12.Content = list[11];
            btn13.Content = list[12];
            btn14.Content = list[13];
            btn15.Content = list[14];
            btn16.Content = list[15];
            btn17.Content = list[16];
            btn18.Content = list[17];
            btn19.Content = list[18];
            btn20.Content = list[19];
            btn21.Content = list[20];
            btn22.Content = list[21];
            btn23.Content = list[22];
            btn24.Content = list[23];
            btn25.Content = list[24];

            int num = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    bingo[i, j] = list[num];
                    num++;
                }
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (gameMode == "game-over") // 게임 끝
            {
                btnStart.Dispatcher.BeginInvoke(((Action)(() => btnStart.Content = "ready")));
                initializing();
            }
            else if (gameMode == "initializing") // 초기화
            {
                int order = bingoCheck();

                if (order == 1) // 빙고판 준비
                {
                    string msg = string.Empty;
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            msg += bingo[i, j].ToString() + ",";
                        }
                    }
                    sendMsg("#ready#|" + msg); // 준비완료 전송

                    btnRezero.IsEnabled = false;
                    btnRandom.IsEnabled = false;
                    btnStart.IsEnabled = false;
                }
            }
        }

        public void sendKey(string str)
        {
            sendMsg("#0rder#|" + str); // 빙고판 번호 보내기
        }

        public void sendMsg(string str)
        {
            SW.WriteLine(str); // 메시지 보내기
            SW.Flush();
        }

        private int bingoCheck()
        {
            // 빙고판 상태 확인 (빙고판이 비어있는가?)
            if (btn1.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn2.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn3.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn4.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn5.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn6.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn7.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn8.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn9.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn10.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn11.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn12.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn13.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn14.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn15.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn16.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn17.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn18.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn19.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn20.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn21.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn22.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn23.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn24.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }
            else if (btn25.Content.ToString() == "")
            {
                UpdateChatBox("빙고판이 준비되지 않았습니다.");
                return 0;
            }

            // 배열에 입력된 빙고판을 넣기
            bingo[0, 0] = Convert.ToInt32(btn1.Content);
            bingo[0, 1] = Convert.ToInt32(btn2.Content);
            bingo[0, 2] = Convert.ToInt32(btn3.Content);
            bingo[0, 3] = Convert.ToInt32(btn4.Content);
            bingo[0, 4] = Convert.ToInt32(btn5.Content);

            bingo[1, 0] = Convert.ToInt32(btn6.Content);
            bingo[1, 1] = Convert.ToInt32(btn7.Content);
            bingo[1, 2] = Convert.ToInt32(btn8.Content);
            bingo[1, 3] = Convert.ToInt32(btn9.Content);
            bingo[1, 4] = Convert.ToInt32(btn10.Content);

            bingo[2, 0] = Convert.ToInt32(btn11.Content);
            bingo[2, 1] = Convert.ToInt32(btn12.Content);
            bingo[2, 2] = Convert.ToInt32(btn13.Content);
            bingo[2, 3] = Convert.ToInt32(btn14.Content);
            bingo[2, 4] = Convert.ToInt32(btn15.Content);

            bingo[3, 0] = Convert.ToInt32(btn16.Content);
            bingo[3, 1] = Convert.ToInt32(btn17.Content);
            bingo[3, 2] = Convert.ToInt32(btn18.Content);
            bingo[3, 3] = Convert.ToInt32(btn19.Content);
            bingo[3, 4] = Convert.ToInt32(btn20.Content);

            bingo[4, 0] = Convert.ToInt32(btn21.Content);
            bingo[4, 1] = Convert.ToInt32(btn22.Content);
            bingo[4, 2] = Convert.ToInt32(btn23.Content);
            bingo[4, 3] = Convert.ToInt32(btn24.Content);
            bingo[4, 4] = Convert.ToInt32(btn25.Content);

            return 1;
        }

        public void UpdateChatBox(string data)
        {
            try
            {
                // 해당 쓰레드가 UI쓰레드인가?
                if (chatBox.Dispatcher.CheckAccess())
                {
                    //UI 쓰레드인 경우
                    chatBox.AppendText(data + Environment.NewLine);
                    chatBox.ScrollToLine(chatBox.LineCount - 1); // 로그창 스크롤 아래로
                }
                else
                {
                    // 작업쓰레드인 경우
                    chatBox.Dispatcher.BeginInvoke((Action)(() => { chatBox.AppendText(data + Environment.NewLine); chatBox.ScrollToLine(chatBox.LineCount - 1); }));
                }
            }catch(Exception e)
            { }            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            disconnect();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string msg = msgBox.Text;
                sendMsg("#MSG#|" + userID + "님 : " + msg); // 메시지 보내기
                msgBox.Text = "";
            }
            catch (Exception A)
            {
                UpdateChatBox("ERR : " + A.Message);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            switch (gameMode)
            {
                case "initializing": // 빙고 판 숫자 채우기
                    {
                        if ((sender as Button).Content.ToString() == "")
                        {
                            if ((sender as Button).Content.ToString() == "")
                            {
                                (sender as Button).Content = myBingoNumber.ToString();
                                myBingoNumber++;
                            }
                        }
                    }
                    break;
                case "my-turn":
                    {
                        sendKey((sender as Button).Content.ToString());
                        gameMode = "other-turn";
                    }
                    break;
            }

        
        }

        public int findLoc(int order)
        {
            // 위치 찾기
            int loc = 0;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (bingo[i, j] == order)
                    {
                        return loc;
                    }
                    loc++;
                }
            }

            return loc;
        }

        public void pushBingo(int order)
        {
            int bingoLoc = findLoc(order);

           
            switch (bingoLoc + 1)
            {
                case 1:
                    {
                        btn1.Dispatcher.BeginInvoke(((Action)(() => btn1.IsEnabled = false)));
                        bingo[0, 0] = 0;
                        break;
                    }
                case 2:
                    {
                        btn2.Dispatcher.BeginInvoke(((Action)(() => btn2.IsEnabled = false)));
                        bingo[0, 1] = 0;
                        break;
                    }
                case 3:
                    {
                        btn3.Dispatcher.BeginInvoke(((Action)(() => btn3.IsEnabled = false)));
                        bingo[0, 2] = 0;
                        break;
                    }
                case 4:
                    {
                        btn4.Dispatcher.BeginInvoke(((Action)(() => btn4.IsEnabled = false)));
                        bingo[0, 3] = 0;
                        break;
                    }
                case 5:
                    {
                        btn5.Dispatcher.BeginInvoke(((Action)(() => btn5.IsEnabled = false)));
                        bingo[0, 4] = 0;
                        break;
                    }
                case 6:
                    {
                        btn6.Dispatcher.BeginInvoke(((Action)(() => btn6.IsEnabled = false)));
                        bingo[1, 0] = 0;
                        break;
                    }
                case 7:
                    {
                        btn7.Dispatcher.BeginInvoke(((Action)(() => btn7.IsEnabled = false)));
                        bingo[1, 1] = 0;
                        break;
                    }
                case 8:
                    {
                        btn8.Dispatcher.BeginInvoke(((Action)(() => btn8.IsEnabled = false)));
                        bingo[1, 2] = 0;
                        break;
                    }
                case 9:
                    {
                        btn9.Dispatcher.BeginInvoke(((Action)(() => btn9.IsEnabled = false)));
                        bingo[1, 3] = 0;
                        break;
                    }
                case 10:
                    {
                        btn10.Dispatcher.BeginInvoke(((Action)(() => btn10.IsEnabled = false)));
                        bingo[1, 4] = 0;
                        break;
                    }
                case 11:
                    {
                        btn11.Dispatcher.BeginInvoke(((Action)(() => btn11.IsEnabled = false)));
                        bingo[2, 0] = 0;
                        break;
                    }
                case 12:
                    {
                        btn12.Dispatcher.BeginInvoke(((Action)(() => btn12.IsEnabled = false)));
                        bingo[2, 1] = 0;
                        break;
                    }
                case 13:
                    {
                        btn13.Dispatcher.BeginInvoke(((Action)(() => btn13.IsEnabled = false)));
                        bingo[2, 2] = 0;
                        break;
                    }
                case 14:
                    {
                        btn14.Dispatcher.BeginInvoke(((Action)(() => btn14.IsEnabled = false)));
                        bingo[2, 3] = 0;
                        break;
                    }
                case 15:
                    {
                        btn15.Dispatcher.BeginInvoke(((Action)(() => btn15.IsEnabled = false)));
                        bingo[2, 4] = 0;
                        break;
                    }
                case 16:
                    {
                        btn16.Dispatcher.BeginInvoke(((Action)(() => btn16.IsEnabled = false)));
                        bingo[3, 0] = 0;
                        break;
                    }
                case 17:
                    {
                        btn17.Dispatcher.BeginInvoke(((Action)(() => btn17.IsEnabled = false)));
                        bingo[3, 1] = 0;
                        break;
                    }
                case 18:
                    {
                        btn18.Dispatcher.BeginInvoke(((Action)(() => btn18.IsEnabled = false)));
                        bingo[3, 2] = 0;
                        break;
                    }
                case 19:
                    {
                        btn19.Dispatcher.BeginInvoke(((Action)(() => btn19.IsEnabled = false)));
                        bingo[3, 3] = 0;
                        break;
                    }
                case 20:
                    {
                        btn20.Dispatcher.BeginInvoke(((Action)(() => btn20.IsEnabled = false)));
                        bingo[3, 4] = 0;
                        break;
                    }
                case 21:
                    {
                        btn21.Dispatcher.BeginInvoke(((Action)(() => btn21.IsEnabled = false)));
                        bingo[4, 0] = 0;
                        break;
                    }
                case 22:
                    {
                        btn22.Dispatcher.BeginInvoke(((Action)(() => btn22.IsEnabled = false)));
                        bingo[4, 1] = 0;
                        break;
                    }
                case 23:
                    {
                        btn23.Dispatcher.BeginInvoke(((Action)(() => btn23.IsEnabled = false)));
                        bingo[4, 2] = 0;
                        break;
                    }
                case 24:
                    {
                        btn24.Dispatcher.BeginInvoke(((Action)(() => btn24.IsEnabled = false)));
                        bingo[4, 3] = 0;
                        break;
                    }
                case 25:
                    {
                        btn25.Dispatcher.BeginInvoke(((Action)(() => btn25.IsEnabled = false)));
                        bingo[4, 4] = 0;
                        break;
                    }
            }
        }

        public void listening()
        {
            string ReceivedMsg = string.Empty;

            while (client.Connected == true)
            {
                try
                {
                    ReceivedMsg = SR.ReadLine();
              
                    byte[] ascii = Encoding.ASCII.GetBytes(ReceivedMsg);
                    int count = 0;
                    foreach (byte b in ascii)
                    {
                        if (count == 0 && (Char)b == '?')
                        {
                            ReceivedMsg = ReceivedMsg.Substring(1);
                            break;
                        }
                        count++;
                    }

                    string[] MsgResult = ReceivedMsg.Split('|');

                    if (MsgResult[0] == "#MSG#") 
                    {
                        UpdateChatBox(MsgResult[1]);
                    }
                    else if (MsgResult[0] == "#0rder#") 
                    {
                        int order = Convert.ToInt32(MsgResult[1]);
                        pushBingo(order);
                    }
                    else if (MsgResult[0] == "#Attack#") 
                    {
                        gameMode = "my-turn";
                    }
                    else if (MsgResult[0] == "#GAME-0VER#") 
                    {
                        gameMode = "game-over";
                        btnStart.Dispatcher.BeginInvoke(((Action)(() => { btnStart.Content = "restart"; btnStart.IsEnabled = true; })));
                    }
                }
                catch (Exception A)
                {
                    UpdateChatBox("ERR listening : " + A);
                }
            }
        }

        void disconnect()
        {
            try
            {
                sendMsg("G00D-BY2"); 
            }
            catch (Exception a)
            {
                Console.WriteLine(a.ToString());
            }
            finally
            {
                if (SW != null)
                    SW.Close();
                if (SR != null)
                    SR.Close();
                if (client != null)
                    client.Close();
                if (NS != null)
                    NS.Close();
            }
        }

        void connect()
        {
            try
            {
                client = new TcpClient(IP, PORT); //client 연결
                UpdateChatBox("서버에 연결되었습니다.");

                NS = client.GetStream(); // 소켓에서 메시지를 가져오는 스트림
                SR = new StreamReader(NS, Encoding.UTF8); // Get message
                SW = new StreamWriter(NS, Encoding.UTF8); // Send message

                Thread listen_thread = new Thread(listening); // 리스닝 쓰레드
                listen_thread.Start();

                sendMsg("#login#|" + userID);
            }
            catch (Exception A)
            {
                UpdateChatBox("ERR2 : " + A.Message);
            }
        }

        private void msgBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key==System.Windows.Input.Key.Return)
            {
                try
                {
                    string msg = msgBox.Text;
                    sendMsg("#MSG#|" + userID + "님 : " + msg); 
                    msgBox.Text = "";
                }
                catch (Exception A)
                {
                    UpdateChatBox("ERR : " + A.Message);
                }
            }
        }
    }
}


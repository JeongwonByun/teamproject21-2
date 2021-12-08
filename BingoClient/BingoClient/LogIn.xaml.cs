using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace BingoClient
{
    /// <summary>
    /// LogIn.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LogIn : Window
    {
        NetworkStream NS = null;
        StreamReader SR = null;
        StreamWriter SW = null;
        TcpClient client = null;
        
        public LogIn()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            string userId = idBox.Text;
            string IP = ipBox.Text;

            if (userId.Length <= 0)
            {
                MessageBoxResult result = MessageBox.Show("ID를 입력해주세요.");
            }
            else if (IP.Length <= 0)
            {
                MessageBoxResult result = MessageBox.Show("IP를 입력해주세요.");
            }
            else if (portBox.Text.Length <= 0)
            {
                MessageBoxResult result = MessageBox.Show("Port번호를 입력해주세요.");
            }
            else
            {
                int PORT = Convert.ToInt32(portBox.Text);
                
                try
                {
                    client = new TcpClient(IP, PORT); //client 연결
                    NS = client.GetStream(); // 소켓에서 메시지를 가져오는 스트림
                    SR = new StreamReader(NS, Encoding.UTF8); // Get message
                    SW = new StreamWriter(NS, Encoding.UTF8); // Send message

                    SW.WriteLine("my-id|" + userId); // ID 보내기
                    SW.Flush();

                    string GetMessage = string.Empty;
                    GetMessage = SR.ReadLine();

                    if (GetMessage == "permission-denied") // 접속 거절시
                    {
                        MessageBoxResult result = MessageBox.Show("ID가 중복되었습니다.");
                    }
                    else if (GetMessage == "#Full#") // 방이 가득찼다면
                    {
                        MessageBoxResult result = MessageBox.Show("방이 가득찼습니다.");
                    }
                    else if(GetMessage == "permission-granted") // 접속 승인시
                    {
                        MainWindow dialog = new MainWindow(userId, IP, PORT);
                        this.Close();
                        dialog.Show();
                    }
                }
                catch(Exception dd)
                {
                    MessageBoxResult result = MessageBox.Show(dd.ToString());
                }
            }
        }
    }
}

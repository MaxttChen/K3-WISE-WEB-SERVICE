using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace DemoForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string url = "base/GetBase/add";
            string newurl = addAsmx(url);
            string url2 = "base";
            string newurl2 = addAsmx(url2);
        }

        public string addAsmx(string path)
        {
            /*
             *-base/getbase/add
             *- / 4 
             * 0-4 : base
             *  11
             *  16- 5
             */
            string newPath ="";
            if (!path.Contains(".asmx"))
            {
                newPath = path.IndexOf('/') >= 0 ? (path.Substring(0, path.IndexOf('/')) + ".asmx" + path.Substring(path.IndexOf('/') , path.Length - path.IndexOf('/'))) : (path + ".asmx");
            }
            return newPath;

        }

        static void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket socket = ar.AsyncState as Socket;
                Socket new_client = socket.EndAccept(ar);  //接收到来自浏览器的代理socket
                                                           //NO.1  并行处理http请求
                socket.BeginAccept(new AsyncCallback(OnAccept), socket); //开始下一次http请求接收   （此行代码放在NO.2处时，就是串行处理http请求，前一次处理过程会阻塞下一次请求处理）

                byte[] recv_buffer = new byte[1024 * 640];
                int real_recv = new_client.Receive(recv_buffer);  //接收浏览器的请求数据
                string recv_request = Encoding.UTF8.GetString(recv_buffer, 0, real_recv);
                Console.WriteLine(recv_request);  //将请求显示到界面

                


                //Resolve(recv_request, new_client);  //解析、路由、处理

                //NO.2  串行处理http请求
            }
            catch
            {

            }
        }
    }
}

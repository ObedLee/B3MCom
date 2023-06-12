using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace B3MCom
{


    public partial class Form1 : Form
    {

        private const char STX = (char)0x31;
        private const char CR = (char)0x0D;
        private const char LF = (char)0x0A;

        private const string OB = "31 4f 42 20 3";
        private const string ETX = "0d 0a";

        private Object lockObj = new Object();
        private string inStream = "";

        private Button activeBtn;

        delegate void TimerEventDelegate();


        public Form1()
        {
            InitializeComponent();
        }

        private void TimerCallback(object status)
        {
            BeginInvoke(new TimerEventDelegate(fGetState));
        }

        private void fSetSerialPort()
        {
            try
            {
                string str = File.ReadAllText(@"setting.txt").Replace("\n", String.Empty);
                string[] setting = str.Split(',');


                for (int i = 0; i < setting.Length; i++)
                {
                    if (setting[i].Split('=')[0] == "PortName")
                        serialPort1.PortName = setting[i].Split('=')[1];

                    else if (setting[i].Split('=')[0] == "BaudRate")
                        serialPort1.BaudRate = Int32.Parse(setting[i].Split('=')[1]);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void fOpenSerialPort()
        {
            if (!serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private void fReceiveSerialData(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                lock (lockObj)
                {
                    BeginInvoke(new EventHandler(fReceiveSerialDataCallback));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void fReceiveSerialDataCallback(object sender, EventArgs e)
        {
            string inData = serialPort1.ReadExisting();
            char[] arr = inData.ToCharArray();

            if (arr.Length <= 0) return;

            if (arr[0] == STX)
                inStream = inData;
            else
                inStream += inData;

            string[] splited = inStream.Split(LF);
            foreach (string s in splited)
            {

                //if (s.Length <= 0) continue;

                //int etxIndex = s.IndexOf(ETX);
                //if (etxIndex <= 0) continue;

                //string cmd = s.Substring(0, etxIndex);


                char[] arr2 = s.ToCharArray();

                if (arr2.Length <= 0) continue;

                int etxIndex = Array.IndexOf(arr2, CR);
                if (etxIndex <= 0) continue;

                //etxIndex = Array.IndexOf(arr2, LF);
                //if (etxIndex <= 0) continue;

                char[] arr3 = arr2.SubArray(0, etxIndex);
                string cmd = new string(arr3);

                tbRX.Text = cmd;
                fCheckState(cmd);
            }
        }

        private void fCheckState(string cmd)
        {
            int lenzNum = 0;

            if (!(int.TryParse(cmd.Replace("1OB ", String.Empty), out lenzNum)))
                    return;

            if (activeBtn != null)
                 activeBtn.BackColor = Color.Gray;


            switch (lenzNum)
            {
                case 1:
                    activeBtn = button1;
                    break;
                case 2:
                    activeBtn = button2;
                    break;
                case 3:
                    activeBtn = button3;
                    break;
                case 4:
                    activeBtn = button4;
                    break;
                case 5:
                    activeBtn = button5;
                    break;
            }

            activeBtn.BackColor = Color.Lime;

        }

        private void fSendSerialData(string data)
        {
            byte[] outData = CSUtil.Str2Hex(data);

            tbTX.Text = Encoding.Default.GetString(outData);

            if (serialPort1.IsOpen)
            {
                serialPort1.Write(outData, 0, outData.Length);
            }
            else
            {
                fOpenSerialPort();
                fSendSerialData(data);
            }
        }

        private void fGetState()
        {
            string cmd = "31 4f 42 3f 0d 0a";

            fSendSerialData(cmd);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            fSetSerialPort();
            fOpenSerialPort();

            string cmd = "31 4c 20 32 0d 0a";
            fSendSerialData(cmd);

            cmd = "31 4e 4f 42 20 31 0d 0a";
            fSendSerialData(cmd);

            System.Threading.Timer timer = new System.Threading.Timer(TimerCallback);
            timer.Change(0, 1);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string lenzNum = "1";
            string cmd = OB + lenzNum + ETX;

            fSendSerialData(cmd);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string lenzNum = "2";
            string cmd = OB + lenzNum + ETX;

            fSendSerialData(cmd);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string lenzNum = "3";
            string cmd = OB + lenzNum + ETX;

            fSendSerialData(cmd);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string lenzNum = "4";
            string cmd = OB + lenzNum + ETX;

            fSendSerialData(cmd);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string lenzNum = "5";
            string cmd = OB + lenzNum + ETX;

            fSendSerialData(cmd);

        }

    }
}


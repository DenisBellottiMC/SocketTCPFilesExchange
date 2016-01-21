using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace SocketTCPFilesExchange
{
    public partial class frmSendFile : Form
    {
        const int BufferSize = 1024;

        public void SendTCP(string M, string IPA, Int32 PortN)
        {
            byte[] SendingBuffer = null;
            TcpClient Client = null;
            lblStatus.Text = "Connecting to Server...";
            NetworkStream netstream = null;

            try
            {
                Client = new TcpClient(IPA, PortN);
                lblStatus.Text = "Connected to Server...";
                netstream = Client.GetStream();
                FileStream Fs = new FileStream(M, FileMode.Open, FileAccess.Read);
                int NoOfPackets = Convert.ToInt32
                    (Math.Ceiling(Convert.ToDouble(Fs.Length) / Convert.ToDouble(BufferSize)));
                prgSendFile.Maximum = NoOfPackets;
                int TotalLength = (int)Fs.Length, CurrentPacketLength, counter = 0;
                for (int i = 0; i < NoOfPackets; i++)
                 {
                     if (TotalLength > BufferSize)
                     {
                         CurrentPacketLength = BufferSize;
                         TotalLength = TotalLength - CurrentPacketLength;
                     }
                     else
                         CurrentPacketLength = TotalLength;
                         SendingBuffer = new byte[CurrentPacketLength];
                         Fs.Read(SendingBuffer, 0, CurrentPacketLength);
                         netstream.Write(SendingBuffer, 0, (int)SendingBuffer.Length);
                         if (prgSendFile.Value >= prgSendFile.Maximum)
                              prgSendFile.Value = prgSendFile.Minimum;
                         prgSendFile.PerformStep();
                     }
                
                     lblStatus.Text=lblStatus.Text+"Sent "+Fs.Length.ToString()+" bytes to the server";
                     Fs.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally 
            {
                netstream.Close();
                Client.Close();
            }
        }

        public frmSendFile()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            IPAddress ipAddress;
            if (IPAddress.TryParse(txtIPAddress.Text, out ipAddress))
            {
               Int32 PortN = 1000;
               SendTCP(lblFileToSend.Text, ipAddress.ToString(), PortN);
            }
            else
            {
                //is not valid ip
            }
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            Stream myStream = null;

            OpenFileDialog selectFile = new OpenFileDialog();

            selectFile.InitialDirectory = "c:\\";
            selectFile.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            selectFile.FilterIndex = 2;
            selectFile.RestoreDirectory = true;

            if (selectFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = selectFile.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            lblFileToSend.Text = selectFile.FileName;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Error: " + ex.Message);
                }
            }

            
        }

    }
}

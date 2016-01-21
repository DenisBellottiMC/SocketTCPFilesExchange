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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace SocketTCPFilesExchange
{
    public partial class frmSendFile : Form
    {
        const int BufferSize = 1024;
        const int ServiceStandardPort = 999;

        public void SendTCP(string M, string IPA, Int32 PortN)
{
    byte[] SendingBuffer = null;
    TcpClient client = null;
    lblStatus.Text = "";
    NetworkStream netstream = null;
    try
    {
         client = new TcpClient(IPA, PortN);
         lblStatus.Text = "Connected to the Server...\n";
         netstream = client.GetStream();
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
    catch (Exception ex)
    {
         Console.WriteLine(ex.Message);
    }
    finally
    {
         netstream.Close();
         client.Close();
    }
} 

        public void ReceiveTCP(int portN)
        {
            TcpListener Listener = null;
            String Status;
            try
            {
                Listener = new TcpListener(IPAddress.Any, portN);
                Listener.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            byte[] RecData = new byte[BufferSize];
            int RecBytes;

            for (; ; )
            {
                TcpClient client = null;
                NetworkStream netstream = null;
                Status = string.Empty;
                try
                {
                    string message = "Accept the Incoming File ";
                    string caption = "Incoming Connection";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result;

                    if (Listener.Pending())
                    {
                        client = Listener.AcceptTcpClient();
                        netstream = client.GetStream();
                        Status = "Connected to a client\n";
                        result = MessageBox.Show(message, caption, buttons);

                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            string SaveFileName = string.Empty;
                            SaveFileDialog DialogSave = new SaveFileDialog();
                            DialogSave.Filter = "All files (*.*)|*.*";
                            DialogSave.RestoreDirectory = true;
                            DialogSave.Title = "Where do you want to save the file?";
                            DialogSave.InitialDirectory = @"C:/";
                            if (DialogSave.ShowDialog() == DialogResult.OK)
                                SaveFileName = DialogSave.FileName;
                            if (SaveFileName != string.Empty)
                            {
                                int totalrecbytes = 0;
                                FileStream Fs = new FileStream
                 (SaveFileName, FileMode.OpenOrCreate, FileAccess.Write);
                                while ((RecBytes = netstream.Read
                     (RecData, 0, RecData.Length)) > 0)
                                {
                                    Fs.Write(RecData, 0, RecBytes);
                                    totalrecbytes += RecBytes;
                                }
                                Fs.Close();
                            }
                            netstream.Close();
                            client.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //netstream.Close();
                }
            }
        }

        public Thread T = null;

        public frmSendFile()
        {
            InitializeComponent();
            ThreadStart Ts = new ThreadStart(StartReceiving);
            
            T = new Thread(Ts);
            T.SetApartmentState(ApartmentState.STA);
            T.Start();
        }

        public void StartReceiving()
        {
            ReceiveTCP(ServiceStandardPort);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            IPAddress ipAddress;
            if (IPAddress.TryParse(txtIPAddress.Text, out ipAddress))
            {
               SendTCP(lblFileToSend.Text, ipAddress.ToString(), (Int32)ServiceStandardPort);
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

        private void txtIPAddress_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

    }
}

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

        public frmSendFile()
        {
            InitializeComponent();
            new Thread(delegate() { ReceiveTCP(ServiceStandardPort); });
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

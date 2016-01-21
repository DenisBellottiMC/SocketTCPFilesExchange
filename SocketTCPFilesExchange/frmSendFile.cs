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
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally 
            {
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
                //valid ip
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

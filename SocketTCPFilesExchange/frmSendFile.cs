using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketTCPFilesExchange
{
    public partial class frmSendFile : Form
    {
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

    }
}

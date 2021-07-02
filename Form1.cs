﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TN.Mobike.ToolLock.Core;
using TN.Mobike.ToolLock.Settings;

namespace TN.Mobike.ToolLock
{
    public partial class Form1 : Form
    {
        public static Label lblMessageP;
        public static ListBox ListBoxP;
        public static RichTextBox RtbMessage;

        public Form1()
        {
            InitializeComponent();
            lblMessageP = lblMessage;
            ListBoxP = listBoxLock;
            RtbMessage = rtbMessageReturn;
            timer1.Enabled = true;
            MinaControl.StartServer(btnConnect, rtbMessageReturn, btnDisconnect, rtbMessageReturn);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(AppSettings.HostService))
            {
                string hostName = Dns.GetHostName();
                var ipAddress = Dns.GetHostByName(hostName).AddressList[0];
                txtIpServer.Text = $"{ipAddress}";
            }
            else
            {
                txtIpServer.Text = AppSettings.HostService;
            }
            txtPortOpen.Text = $"{AppSettings.PortService}";
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            var key = "";
            key = cbbKey.SelectedItem == null ? "S5" : cbbKey.SelectedItem.ToString().Split('-')[0].Trim();

            var item = listBoxLock.SelectedItem;

            var imei = txtImei.Text;

            if (String.IsNullOrEmpty(imei))
            {
                imei = item.ToString();
            }

            MinaControl.UnLock(rtbMessageReturn ,key, imei, true);

            Console.WriteLine(key);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            MinaControl.StartServer(btnConnect, rtbMessageReturn, btnDisconnect, rtbMessageReturn);
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            MinaControl.StopServer(btnDisconnect, btnConnect, rtbMessageReturn);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine(SessionMap.List.Count);
            listBoxLock.DataSource = SessionMap.List;
            listBoxLock.Refresh();
        }
    }
}

﻿
namespace TN.Mobike.ToolLock
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cbbKey = new System.Windows.Forms.ComboBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtIpServer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPortOpen = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.rtbMessageReturn = new System.Windows.Forms.RichTextBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.txtImei = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cbbKey
            // 
            this.cbbKey.FormattingEnabled = true;
            this.cbbKey.Items.AddRange(new object[] {
            "L0 - Mở khóa",
            "D0 - Kiểm tra vị trí",
            "D1 - Tự động gửi vị trí",
            "D1O - Tắt tự động gửi vị trí",
            "S5 - Kiểm tra thông tin",
            "S8 - Đổ chuông",
            "G0 - Kiểm tra phần mềm",
            "I0 - Số ICCID",
            "M0 - Địa chỉ Mac Bluetooth",
            "S0 - Tắt thiết bị",
            "S1 - Khởi động lại thiết bị",
            "C0 - Mở khóa bằng mã RFID",
            "C1 - Cài đặt mã số cho khóa",
            "U0 - Upgrade"});
            this.cbbKey.Location = new System.Drawing.Point(21, 311);
            this.cbbKey.Name = "cbbKey";
            this.cbbKey.Size = new System.Drawing.Size(210, 21);
            this.cbbKey.TabIndex = 0;
            this.cbbKey.Text = "Chọn mã lệnh";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(81, 371);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(86, 32);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "SEND";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtIpServer
            // 
            this.txtIpServer.Location = new System.Drawing.Point(81, 8);
            this.txtIpServer.Name = "txtIpServer";
            this.txtIpServer.Size = new System.Drawing.Size(150, 20);
            this.txtIpServer.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "IP Server :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Port         :";
            // 
            // txtPortOpen
            // 
            this.txtPortOpen.Location = new System.Drawing.Point(81, 39);
            this.txtPortOpen.Name = "txtPortOpen";
            this.txtPortOpen.Size = new System.Drawing.Size(150, 20);
            this.txtPortOpen.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Gửi câu lệnh";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 286);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Mã lệnh gửi xuống khóa";
            // 
            // btnConnect
            // 
            this.btnConnect.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnConnect.Location = new System.Drawing.Point(21, 65);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 32);
            this.btnConnect.TabIndex = 9;
            this.btnConnect.Text = "CONNECT";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // rtbMessageReturn
            // 
            this.rtbMessageReturn.Location = new System.Drawing.Point(312, 42);
            this.rtbMessageReturn.Name = "rtbMessageReturn";
            this.rtbMessageReturn.Size = new System.Drawing.Size(780, 515);
            this.rtbMessageReturn.TabIndex = 10;
            this.rtbMessageReturn.Text = "";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnDisconnect.Location = new System.Drawing.Point(131, 65);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(100, 32);
            this.btnDisconnect.TabIndex = 11;
            this.btnDisconnect.Text = "DISCONNECT";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(309, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Message :";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(371, 15);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(254, 13);
            this.lblMessage.TabIndex = 13;
            this.lblMessage.Text = "*CMDS,OM,861123053530935,000000000000,D0#";
            // 
            // txtImei
            // 
            this.txtImei.Location = new System.Drawing.Point(21, 232);
            this.txtImei.Name = "txtImei";
            this.txtImei.Size = new System.Drawing.Size(210, 20);
            this.txtImei.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 207);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Imei khóa";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(21, 155);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(210, 20);
            this.txtMessage.TabIndex = 16;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1104, 569);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtImei);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.rtbMessageReturn);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPortOpen);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtIpServer);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.cbbKey);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbbKey;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtIpServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPortOpen;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.RichTextBox rtbMessageReturn;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TextBox txtImei;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox txtMessage;
    }
}


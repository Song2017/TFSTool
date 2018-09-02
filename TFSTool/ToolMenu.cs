using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using TFSUtils;

namespace TFSTool
{
    public partial class ToolMenu : Form
    {
        public List<KeyValuePair<string, string>> TFSCredentials { get; private set; }

        public List<KeyValuePair<string, string>> EmailParameters { get; private set; }

        public ToolMenu()
        {
            this.InitializeComponent();
            this.Init();
        }

        private void Init()
        {
            this.InitUI();
            this.InitMethod();
        }

        private void InitUI()
        {
            this.linkLabelCredentials.Links.Add(0, 40, this._TFSSecurityURL);
            this.textPassWord.PasswordChar = '*';
            this.textUserName.Text = Utils.GetConfig("username", "");
            this.textPassWord.Text = Utils.GetConfig("password", "");
            this.textTo.Text = Utils.GetConfig("to", "");
            this.textCC.Text = Utils.GetConfig("cc", "");
            this.textSubject.Text = Utils.GetConfig("subject", "");

            if (textTo.Text.ToStringEx().IsNullOrEmpty())
                textTo.Text = "Guangshun.Song@bhge.com; Bensong2017@hotmail.com";
            if (textCC.Text.ToStringEx().IsNullOrEmpty())
                textCC.Text = "Bensong2017@hotmail.com";
            if (textSubject.Text.ToStringEx().IsNullOrEmpty())
                textSubject.Text = string.Format("VKC2 Released @{0}",DateTime.Now.ToString("yyyy/MM/dd"));
        }

        private void InitMethod()
        {
            this.buttonOK.Click += delegate (object sender, EventArgs e)
            {
                this.TFSCredentials = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("username", this.textUserName.Text.ToStringEx()),
                    new KeyValuePair<string, string>("password", this.textPassWord.Text.ToStringEx())
                };
                this.EmailParameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("to", this.textTo.Text.ToStringEx()),
                    new KeyValuePair<string, string>("cc", this.textCC.Text.ToStringEx()),
                    new KeyValuePair<string, string>("subject", this.textSubject.Text.ToStringEx())
                };
                base.DialogResult = DialogResult.OK;
            };
            this.buttonCancel.Click += delegate (object sender, EventArgs e)
            {
                base.DialogResult = DialogResult.Cancel;
            };
            this.linkLabelCredentials.LinkClicked += delegate (object obj, LinkLabelLinkClickedEventArgs sender)
            {
                Process.Start(new ProcessStartInfo(sender.Link.LinkData.ToStringEx()));
            };
        }

        private string _TFSSecurityURL = "https://identitydivision.visualstudio.com/defaultcollection/_details/security/altcreds";
    }
}

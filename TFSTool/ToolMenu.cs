using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using TFSUtils;

namespace TFSTool
{
    public partial class ToolMenu : BaseForm
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
            this.textUserName.Text = Utils.GetConfig(AppConstants.TFS_USERNAME, "");
            this.textPassWord.Text = Utils.GetConfig(AppConstants.TFS_PASSWORD, "");
            this.textTo.Text = Utils.GetConfig(AppConstants.EMAIL_TO, "");
            this.textCC.Text = Utils.GetConfig(AppConstants.EMAIL_CC, "");
            this.txtUrl.Text = Utils.GetConfig(AppConstants.TFSURL, "");
            this.txtQuery.Text = Utils.GetConfig(AppConstants.TFSQUERY, "");
            this.textSubject.Text = Utils.GetConfig(AppConstants.EMAIL_SUBJECT, "");

            if (textTo.Text.ToStringEx().IsNullOrEmpty())
                textTo.Text = "your email";
            if (textCC.Text.ToStringEx().IsNullOrEmpty())
                textCC.Text = "your email";
            if (txtUrl.Text.ToStringEx().IsNullOrEmpty())
                txtUrl.Text = "http://address:8080/tfs/DefaultCollection";
            if (txtQuery.Text.ToStringEx().IsNullOrEmpty())
                txtQuery.Text = $"SELECT * FROM WorkItems WHERE  [System.TeamProject] = '{Utils.GetConfig(AppConstants.PRONAME) ?? "project name"}'";
            if (textSubject.Text.ToStringEx().IsNullOrEmpty())
                textSubject.Text = string.Format("Subject ~@{0}", DateTime.Now.ToString("yyyy/MM/dd"));
        }

        private void InitMethod()
        {
            this.buttonOK.Click += delegate (object sender, EventArgs e)
            {
                this.TFSCredentials = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(AppConstants.TFS_USERNAME, this.textUserName.Text.ToStringEx()),
                    new KeyValuePair<string, string>(AppConstants.TFS_PASSWORD, this.textPassWord.Text.ToStringEx()),
                    new KeyValuePair<string, string>(AppConstants.TFSURL, this.txtUrl.Text.ToStringEx()),
                    new KeyValuePair<string, string>(AppConstants.TFSQUERY, this.txtQuery.Text.ToStringEx())
                };
                this.EmailParameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(AppConstants.EMAIL_TO, this.textTo.Text.ToStringEx()),
                    new KeyValuePair<string, string>(AppConstants.EMAIL_CC, this.textCC.Text.ToStringEx()),
                    new KeyValuePair<string, string>(AppConstants.EMAIL_SUBJECT, this.textSubject.Text.ToStringEx())
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

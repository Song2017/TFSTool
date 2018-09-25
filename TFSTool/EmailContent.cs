using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using TFSUtils;

namespace TFSTool
{
    public partial class EmailContent : Form
    {
 

        public EmailContent()
        {
            this.InitializeComponent();
            this.Init();
        }

        private void Init()
        {
            InitUI();
            InitMethod();
        }


        private void InitUI()
        {
            txtFromName.Text = Utils.GetConfig("fromname");
            txtPro.Text = Utils.GetConfig("proname");

            string emailHeader = $"<P style='COLOR: #2f5597; MARGIN: 0px'>Hi All,<O:P></O:P></P><BR>" +
                $"<P style='COLOR: #2f5597; MARGIN: 0px'><B><SPAN style='BACKGROUND: yellow; mso-highlight: yellow'>Sprint {Utils.GetConfig("sprintnum")}</SPAN></B></P><BR>" +
                $"<P style='COLOR: #2f5597; MARGIN: 0px'>{txtPro.Text.ToStringEx()} released @ ~{DateTime.Now.ToString("HH:mm MMMM dd")} with script run. &nbsp; &nbsp;<O:P></O:P></P><UL style='MARGIN-TOP: 0in' type=disc><LI style='COLOR: #2f5597; MARGIN: 0px'>vkc2.3.3-20180924.sql<O:P></O:P></LI></UL><BR>";
            string emailFooter = $"<p style='margin: 0 0;color:#2F5597;'>&nbsp;<o:p></o:p></p> <p style='margin: 0 0;color:#2F5597;'>Thanks.<o:p></o:p></p><p style='margin: 0 0;color:#2F5597;'><br>Regards,<o:p></o:p></p>" +
                $"<p style='margin: 0 0;color:#2F5597;'>{txtFromName.Text.ToStringEx()}<o:p></o:p></p>";
            richHeader.Text = Utils.GetConfig("emailheader", emailHeader);
            richFooter.Text = Utils.GetConfig("emailfooter", emailFooter);
        }

        private void InitMethod()
        {
            txtFromName.KeyPress += delegate (object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Return)
                    return;
                string emailFooter = $"<p style='margin: 0 0;color:#2F5597;'>&nbsp;<o:p></o:p></p> <p style='margin: 0 0;color:#2F5597;'>Thanks.<o:p></o:p></p><p style='margin: 0 0;color:#2F5597;'><br>Regards,<o:p></o:p></p>" +
                    $"<p style='margin: 0 0;color:#2F5597;'>{txtFromName.Text.ToStringEx()}<o:p></o:p></p>";

                Utils.SaveConfig("emailfooter", emailFooter);
                richFooter.Text = emailFooter;
                Utils.SaveConfig("fromname", txtFromName.Text.ToStringEx());
            };
            txtPro.KeyPress += delegate (object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Return)
                    return;

                string emailHeader = $"<P style='COLOR: #2f5597; MARGIN: 0px'>Hi All,<O:P></O:P></P><BR>" +
                    $"<P style='COLOR: #2f5597; MARGIN: 0px'><B><SPAN style='BACKGROUND: yellow; mso-highlight: yellow'>Sprint {Utils.GetConfig("sprintnum")}</SPAN></B></P><BR>" +
                    $"<P style='COLOR: #2f5597; MARGIN: 0px'>{txtPro.Text.ToStringEx()} released @ ~{DateTime.Now.ToString("HH:mm MMMM dd")} with script run. &nbsp; &nbsp;<O:P></O:P></P><UL style='MARGIN-TOP: 0in' type=disc><LI style='COLOR: #2f5597; MARGIN: 0px'>vkc2.3.3-20180924.sql<O:P></O:P></LI></UL><BR>";
                Utils.SaveConfig("emailheader", emailHeader);
                richHeader.Text = emailHeader;
                Utils.SaveConfig("proname", txtPro.Text.ToStringEx());
            };

            this.buttonOK.Click += delegate (object sender, EventArgs e)
            {
                Utils.SaveConfig("proname", txtPro.Text);
                Utils.SaveConfig("fromname", txtFromName.Text);
                Utils.SaveConfig("emailheader", richHeader.Text);
                Utils.SaveConfig("emailfooter", richFooter.Text);

                base.DialogResult = DialogResult.OK;
            };
            this.buttonCancel.Click += delegate (object sender, EventArgs e)
            {
                base.DialogResult = DialogResult.Cancel;
            };
        }
    }
}

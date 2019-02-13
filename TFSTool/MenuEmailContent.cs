using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using TFSUtils;

namespace TFSTool
{
    public partial class MenuEmailContent : BaseForm
    {
 

        public MenuEmailContent()
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
            txtFromName.Text = Utils.GetConfig(AppConstants.EMAIL_SENDER,"Sender Name");
            txtPro.Text = Utils.GetConfig(AppConstants.PRONAME, "Pro Name");

            string emailHeader = $"<P style='COLOR: #2f5597; MARGIN: 0px'>Hi All,<O:P></O:P></P><BR>" +
                $"<P style='COLOR: #2f5597; MARGIN: 0px'><B><SPAN style='BACKGROUND: yellow; mso-highlight: yellow'>$$spring_number$$</SPAN></B></P><BR>" +
                $"<P style='COLOR: #2f5597; MARGIN: 0px'>$$pro_name$$ released @ ~$$date$$ with sql script run. &nbsp; &nbsp;<O:P></O:P></P><BR>";
            string emailFooter = $"<p style='margin: 0 0;color:#2F5597;'>&nbsp;<o:p></o:p></p> <p style='margin: 0 0;color:#2F5597;'>Thanks.<o:p></o:p></p><p style='margin: 0 0;color:#2F5597;'><br>Regards,<o:p></o:p></p>" +
                $"<p style='margin: 0 0;color:#2F5597;'>$$from_name$$<o:p></o:p></p>";

            richHeader.Text = Utils.GetConfig(AppConstants.EMAIL_HEADER, 
                emailHeader.Replace("$$spring_number$$", Utils.GetConfig(AppConstants.SPRINTNUM))
                .Replace("$$pro_name$$", Utils.GetConfig(AppConstants.PRONAME, "Pro Name")));
                //.Replace("$$date$$", DateTime.Now.ToString("HH:mm MMMM dd")
            richFooter.Text = Utils.GetConfig(AppConstants.EMAIL_FOOTER, emailFooter.Replace("$$from_name$$", 
                Utils.GetConfig(AppConstants.EMAIL_SENDER, "Sender Name")));
        }

        private void InitMethod()
        {
            txtFromName.KeyPress += delegate (object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Return)
                    return;
                Utils.SaveConfig(AppConstants.EMAIL_SENDER, txtFromName.Text.ToStringEx());

                richFooter.Text = Utils.GetConfig(AppConstants.EMAIL_FOOTER).
                Replace("$$from_name$$", Utils.GetConfig(AppConstants.EMAIL_SENDER, "Sender Name"));
            };
            txtPro.KeyPress += delegate (object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Return)
                    return;

                richHeader.Text = Utils.GetConfig(AppConstants.EMAIL_HEADER).Replace("$$spring_number$$", 
                    Utils.GetConfig(AppConstants.SPRINTNUM))
                    .Replace("$$pro_name$$", Utils.GetConfig(AppConstants.PRONAME, "Pro Name"));
            };

            this.buttonOK.Click += delegate (object sender, EventArgs e)
            {
                Utils.SaveConfig(AppConstants.PRONAME, txtPro.Text);
                Utils.SaveConfig(AppConstants.EMAIL_SENDER, txtFromName.Text);
                Utils.SaveConfig(AppConstants.EMAIL_HEADER, richHeader.Text);
                Utils.SaveConfig(AppConstants.EMAIL_FOOTER, richFooter.Text);

                base.DialogResult = DialogResult.OK;
            };
            this.buttonCancel.Click += delegate (object sender, EventArgs e)
            {
                base.DialogResult = DialogResult.Cancel;
            };
        }
    }
}

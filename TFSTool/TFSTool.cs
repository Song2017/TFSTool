using Helper;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TFSUtils;

namespace TFSTool
{
    public partial class TFSTool : BaseForm
    {
        public TFSTool()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            InitUI();
            InitMethod();

        }

        private void InitUI()
        {
            SetUICredentials();

            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                dateTimePicker.Value = DateTime.Now.AddDays(-3);
            else
                dateTimePicker.Value = DateTime.Now.AddDays(-1);

            dateTimePickerEnd.Value = DateTime.Now;

            txtSprintNum.Text = Utils.GetConfig("sprintnum");
            richTextBox1.Visible = false;
        }


        private void InitMethod()
        {
            bool toEdit = true;
            this.ConfigToolStripMenuItem.Click += delegate (object sender, EventArgs e)
            {
                if (this.SetCredentials())
                {
                    this.SetUICredentials();
                }
            };
            this.loadToolStripMenuItem.Click += delegate { this.SetUICredentials(); };
            this.editEmailToolStripMenuItem.Click += delegate
            {
                richTextBox1.Visible = !richTextBox1.Visible;
                tableLayoutPanel1.Visible = !tableLayoutPanel1.Visible;
                if (toEdit)
                {
                    richTextBox1.Text = webBrowserShow.DocumentText.ToStringEx();
                    toEdit = false;
                }
                else
                {
                    webBrowserShow.DocumentText = richTextBox1.Text.ToStringEx();
                    toEdit = true;
                }
            };
            this.tipsToolStripMenuItem.Click += delegate (object sender, EventArgs e)
            {
                MessageBox.Show(this, "This is Help...", "Tips", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            };

            buttonSend.Click += delegate (object sender, EventArgs e)
            {
                if (textTo.Text.IsNullOrEmpty())
                {
                    MessageBox.Show(this, "Please input To.", "SendEmail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                if (webBrowserShow.DocumentText.IsNullOrEmpty())
                {
                    MessageBox.Show(this, "Please receive details.", "SendEmail", MessageBoxButtons.OK);
                }
                if (SendEmail())
                {
                    MessageBox.Show(this, "Email has been sent. Please check your MailBox.", "SendEmail", MessageBoxButtons.OK);
                    return;
                }
                MessageBox.Show(this, "Error: Email send with an error, please check log.", "SendEmail", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            };

            buttonReceive.Click += delegate
            {
                SetUICredentials();
                PrepareVKWorkItems(null, null);
            };

            txtSprintNum.KeyPress += TxtSprintNum_KeyPress;
        }

        private void TxtSprintNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Return )
                return;

            Utils.SaveConfig("sprintnum", txtSprintNum.Text);
        }

        private bool SendEmail()
        {
            OutlookApplication outlookApplication = new OutlookApplication();
            try
            {
                outlookApplication.SendEmail(OlItemType.olMailItem, textTo.Text, textCC.Text, textSubject.Text,
                    webBrowserShow.DocumentText, OlImportance.olImportanceNormal);
            }
            catch (System.Exception e)
            {
                return false;
            }
            finally
            {
                if (outlookApplication.IsNew)
                {
                    outlookApplication.Dispose();
                }
            }

            return true;
        }

        private void SetUICredentials()
        {
            if (textTo.Text.IsNullOrEmpty())
            {
                textTo.Text = Utils.GetConfig("to", "");
            }
            if (textCC.Text.IsNullOrEmpty())
            {
                textCC.Text = Utils.GetConfig("cc", "");
            }
            if (textSubject.Text.IsNullOrEmpty())
            {
                textSubject.Text = Utils.GetConfig("subject", "");
            }
        }

        private bool SetCredentials()
        {
            try
            {
                using (ToolMenu dialog = new ToolMenu())
                {
                    if (dialog.ShowDialog() != DialogResult.OK)
                    {
                        return false;
                    }
                    foreach (KeyValuePair<string, string> tfs in dialog.TFSCredentials)
                    {
                        if (!tfs.Value.ToStringEx().IsNullOrEmpty())
                        {
                            Utils.SaveConfig(tfs.Key, tfs.Value);
                        }
                    }
                    foreach (KeyValuePair<string, string> email in dialog.EmailParameters)
                    {
                        if (!email.Value.ToStringEx().IsNullOrEmpty())
                        {
                            Utils.SaveConfig(email.Key, email.Value);
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void PrepareVKWorkItems(object sender, EventArgs e)
        {

            this.BeginWait();
            ThreadPool.QueueUserWorkItem(delegate (object arg)
            {
                try
                {
                    TFSOperation tFSOperation = TFSOperation.Instance;
                    if (!SetTFSInstance(ref tFSOperation))
                        return;

                    if (!tFSOperation.ConnectTFS())
                        return;

                    List<VKWorkItem> vKWorkItems = tFSOperation.GetVKWorkItems();
                    if (vKWorkItems == null || vKWorkItems.Count <= 0)
                        return;

                    HandleWorkItems(vKWorkItems);
                }
                finally
                {
                    this.EndWait();
                }
            });
        }

        private bool SetTFSInstance(ref TFSOperation tFSOperation)
        {
            string username = Utils.GetConfig("username");
            string pass = Utils.GetConfig("password");
            if (username.IsNullOrEmpty() || pass.IsNullOrEmpty())
            {
                return false;
            }
            string tfsQuery = Utils.GetConfig("tfsquery");
            string tfsURL = Utils.GetConfig("tfsurl");


            tFSOperation.UserName = username;
            tFSOperation.Password = pass;
            tFSOperation.QuertStr = tfsQuery;
            tFSOperation.URL = tfsURL;

            return true;
        }

        private void HandleWorkItems(List<VKWorkItem> vKWorkItems)
        {
            List<VKWorkItem> vKWorkItemsRtn = new List<VKWorkItem>();
            DateTime dateTime = dateTimePicker.Value;
            DateTime dateTimeEnd = dateTimePickerEnd.Value;
            string sprintNum = txtSprintNum.Text.ToStringEx();

            for (int i = 0; i < vKWorkItems.Count; i++)
            {
                if (vKWorkItems[i].IterationPath.Contains(sprintNum))
                    vKWorkItemsRtn.Add(vKWorkItems[i]);
            }

            //save to local result folder
            vKWorkItemsRtn = GetSpecficDateWorkItems(vKWorkItemsRtn, dateTime, dateTimeEnd);
            vKWorkItemsRtn.SaveWorkItemList();

            //set email content
            vKWorkItemsRtn = GetSpecficDateWorkItems(vKWorkItemsRtn, dateTime, dateTimeEnd, true);
            string strEmailContent = SetEmailContent(vKWorkItemsRtn, sprintNum);
            if (!strEmailContent.IsNullOrEmpty())
                webBrowserShow.DocumentText = strEmailContent.ToStringEx();
        }

        private string SetEmailContent(List<VKWorkItem> vKWorkItemsRtn, string sprintNum)
        {
            StringBuilder body = new StringBuilder();

            body.Append(string.Format("<p style='margin: 0 0;color:#2F5597;'>Hi All,<o:p></o:p></p><br>" +
                "<p style='margin: 0 0;color:#2F5597;'><b><span style='background:yellow;mso-highlight:yellow'>Sprint {0}</span></b></p><br>" +
                "<p style='margin: 0 0;color:#2F5597;'>VKC2 released @ ~{1} with script run. &nbsp; &nbsp;<o:p></o:p></p>" +
                "<ul style='margin-top:0in' type=disc><li style='margin: 0 0;color:#2F5597;'>vkc2.3.3-20180829.sql<o:p></o:p></li></ul><br>" +
                "<p style='margin: 0 0;color:#2F5597;'>Following PBI are finished.<o:p></o:p></p>", sprintNum, DateTime.Now.ToString("HH:mm MMMM dd")));
            body.Append("<table class='MsoNormalTable' width=1393 border = 1 cellspacing=0 cellpadding=0 style='color:#2F5597;'>");
            body.Append("<tr style='height:17.15pt'>");
            body.Append(string.Format("<td width=130 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", "Work Item Type"));
            body.Append(string.Format("<td width=100 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", "ID"));
            body.Append(string.Format("<td width=760 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", "Title"));

            foreach (VKWorkItem wi in vKWorkItemsRtn)
            {
                body.Append("<tr style='height:17.15pt'>");
                body.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.WorkItemType.Replace("Product Backlog Item", "PBI")));
                body.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.ID));
                body.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", wi.Title));
                body.Append("</tr>");
            }
            body.Append("</table>");
            body.Append("<p style='margin: 0 0;color:#2F5597;'>&nbsp;<o:p></o:p></p>" +
                "<p style='margin: 0 0;color:#2F5597;'>Thanks.<o:p></o:p></p><p style='margin: 0 0;color:#2F5597;'><br>Regards,<o:p></o:p></p>" +
                "<p style='margin: 0 0;color:#2F5597;'>Ben<o:p></o:p></p>");

            return body.ToStringEx();
        }

        private List<VKWorkItem> GetSpecficDateWorkItems(List<VKWorkItem> vKWorkItems, DateTime dateTime, DateTime dateTimeEnd, bool isFileterStatus = false)
        {
            List<VKWorkItem> vKWorkItemsRtn = new List<VKWorkItem>();
            if (vKWorkItems == null || dateTime == null)
                return vKWorkItems;

            foreach (VKWorkItem wi in vKWorkItems)
            {
                if (wi.ChangedDate.DayOfYear < dateTime.DayOfYear)
                    continue;
                if (dateTimeEnd != null && (wi.ChangedDate.DayOfYear > dateTimeEnd.DayOfYear))
                    continue;
                if (isFileterStatus && !(wi.State.Equals("Done") || wi.State.Equals("Resolved")))
                    continue;

                vKWorkItemsRtn.Add(wi);
            }

            return vKWorkItemsRtn;
        }

    }
}

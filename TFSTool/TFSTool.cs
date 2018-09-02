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
        string query = "SELECT * FROM WorkItems WHERE [System.TeamProject] = 'test' AND[System.Id] > 23186";
        string _URL = "http://test/tfs/DefaultCollection";

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

            txtSprintNum.Text = "84";
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
            this.editEmailToolStripMenuItem.Click += delegate {
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
            string tfsQuery = Utils.GetConfig("tfsquery", string.Empty, false);
            string tfsURL = Utils.GetConfig("tfsurl", string.Empty, false);


            tFSOperation.UserName = username;
            tFSOperation.Password = pass;
            tFSOperation.QuertStr = query;
            tFSOperation.URL = _URL;

            return true;
        }

        private void HandleWorkItems(List<VKWorkItem> vKWorkItems)
        {
            List<VKWorkItem> vKWorkItemsRtn = new List<VKWorkItem>();
            DateTime dateTime = dateTimePicker.Value;
            string sprintNum = txtSprintNum.Text.ToStringEx();

            for (int i = 0; i < vKWorkItems.Count; i++)
            {
                if (vKWorkItems[i].IterationPath.Contains(sprintNum))
                    vKWorkItemsRtn.Add(vKWorkItems[i]);
            }

            //save to local result folder
            vKWorkItemsRtn = GetSpecficDateWorkItems(vKWorkItemsRtn, dateTime);
            vKWorkItemsRtn.SaveWorkItemList();

            //set email content
            vKWorkItemsRtn = GetSpecficDateWorkItems(vKWorkItemsRtn, dateTime,true);
            string strEmailContent = SetEmailContent(vKWorkItemsRtn, sprintNum);
            if (!strEmailContent.IsNullOrEmpty())
                webBrowserShow.DocumentText = strEmailContent.ToStringEx();
        }

        private string SetEmailContent(List<VKWorkItem> vKWorkItemsRtn, string sprintNum)
        {
            StringBuilder body = new StringBuilder();
             
            body.Append(string.Format("<p class='MsoNormal'>Hi All,<o:p></o:p></p>" +
                "<p class=MsoNormal><b><span style='background:yellow;mso-highlight:yellow'>Sprint {0}</span></b></p>" +
                "<p class=MsoNormal>VKC2 released @ ~{1} with script run. &nbsp; &nbsp;<o:p></o:p></p>" +
                "<ul style='margin-top:0in' type=disc><li class=MsoNormal style='mso-list:l0 level1 lfo3'>vkc2.3.3-20180829.sql<o:p></o:p></li></ul>" +
                "<p class=MsoNormal>Following PBI are finished.<o:p></o:p></p>", sprintNum, DateTime.Now.ToString("HH:mm MMMM dd")));
            body.Append("<table class='MsoNormalTable' width=1393 border = 1 cellspacing=0 cellpadding=0>");
            body.Append("<tr style='height:17.15pt'>");
            body.Append(string.Format("<td width=125 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", "ID"));
            body.Append(string.Format("<td width=194 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", "Work Item Type"));
            body.Append(string.Format("<td width=671 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", "Title"));

            foreach (VKWorkItem wi in vKWorkItemsRtn) {
                body.Append("<tr style='height:17.15pt'>");
                body.Append(string.Format("<td width=125 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.WorkItemType.Replace("Product Backlog Item", "PBI")));
                body.Append(string.Format("<td width=194 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.ID));
                body.Append(string.Format("<td width=671 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", wi.Title)); 
                body.Append("</tr>");
            }
            body.Append("</table>");
            body.Append("<p class='MsoNormal'>&nbsp;<o:p></o:p></p>" +
                "<p class=MsoNormal>Thanks.<o:p></o:p></p><p class=MsoNormal><br>Regards,<o:p></o:p></p>" +
                "<p class=MsoNormal>Ben<o:p></o:p></p>");

            return body.ToStringEx();

        }

        private List<VKWorkItem> GetSpecficDateWorkItems(List<VKWorkItem> vKWorkItems, DateTime dateTime, bool isFileterStatus = false)
        {
            List<VKWorkItem> vKWorkItemsRtn = new List<VKWorkItem>();
            if (vKWorkItems == null || dateTime == null)
                return vKWorkItems;

            foreach (VKWorkItem wi in vKWorkItems)
            {
                if (!wi.ChangedDate.ToShortDateString().Equals(dateTime.ToShortDateString()))
                    continue;
                if (isFileterStatus && !(wi.State.Equals("Done") || wi.State.Equals("Resolved")))
                    continue;

                vKWorkItemsRtn.Add(wi);
            }

            return vKWorkItemsRtn;
        }

    }
}

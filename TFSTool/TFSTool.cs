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
        private List<VKWorkItem> _vKWorkItemsCache = new List<VKWorkItem>();

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
            txtFromName.Text = Utils.GetConfig("fromname");
            textSubject.Text = $"VKC2 released @ ~{DateTime.Now.ToString("yyyy/MM/dd")}";

            tableLayoutPanel2.Visible = false;

            checkedListStatus.SetItemChecked(2, true);
            checkedListStatus.SetItemChecked(3, true);
            checkedListType.SetItemChecked(0, true);
            checkedListType.SetItemChecked(2, true);
            checkedListType.SetItemChecked(4, true);
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
                tableLayoutPanel2.Visible = !tableLayoutPanel2.Visible;
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
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("1. Install Outlook to local machine: avoid local email server prohibit ");
                sb.AppendLine("2. Set Credentials in Menu ");
                sb.AppendLine("3. Select Date ");
                sb.AppendLine("4. Retrive tfs data via button ");
                sb.AppendLine("5. Edit Email content in Menu ");
                sb.AppendLine("6. Send Email via button");

                MessageBox.Show(this, sb.ToStringEx(), "Tips", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            };
            this.aboutToolStripMenuItem.Click += delegate
            {
                MessageBox.Show(this, "this is about...", "About", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                PrepareVKWorkItems();
            };

            btnSaveLocal.Click += delegate
            {
                if (_vKWorkItemsCache == null || _vKWorkItemsCache.Count <= 0)
                {
                    MessageBox.Show(this, "Please get TFS data.", "TFS Data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (_vKWorkItemsCache.SaveWorkItemList())
                {
                    MessageBox.Show(this, "Success: Please find data in folder result.", "TFS Data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                };

            };

            txtSprintNum.KeyPress += delegate (object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Return)
                    return;

                Utils.SaveConfig("sprintnum", txtSprintNum.Text);
            };

            txtFromName.KeyPress += delegate (object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Return)
                    return;

                Utils.SaveConfig("fromname", txtFromName.Text);
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
            textTo.Text = Utils.GetConfig("to", "");
            textCC.Text = Utils.GetConfig("cc", "");
            textSubject.Text = Utils.GetConfig("subject", "");
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

        private void PrepareVKWorkItems()
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
                if (sprintNum.IsNullOrEmpty() || vKWorkItems[i].IterationPath.Contains(sprintNum))
                    vKWorkItemsRtn.Add(vKWorkItems[i]);
            }

            //get vk items by UI 
            _vKWorkItemsCache = GetSpecficDateWorkItems(vKWorkItemsRtn, dateTime, dateTimeEnd);

            //set email content
            vKWorkItemsRtn = GetSpecficDateWorkItems(_vKWorkItemsCache, dateTime, dateTimeEnd);
            string strEmailContent = SetEmailContent(vKWorkItemsRtn, sprintNum);
            if (!strEmailContent.IsNullOrEmpty())
                webBrowserShow.DocumentText = strEmailContent.ToStringEx();
        }

        private string SetEmailContent(List<VKWorkItem> vKWorkItemsRtn, string sprintNum)
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine(string.Format("<HTML><BODY contentEditable='true'><p style='margin: 0 0;color:#2F5597;'>Hi All,<o:p></o:p></p><br>" +
                "<p style='margin: 0 0;color:#2F5597;'><b><span style='background:yellow;mso-highlight:yellow'>Sprint {0}</span></b></p><br>", sprintNum));
            body.AppendLine(string.Format("<p style='margin: 0 0;color:#2F5597;'>VKC2 released @ ~{0} with script run. &nbsp; &nbsp;<o:p></o:p></p>", 
                DateTime.Now.ToString("HH:mm MMMM dd")));
            body.AppendLine(string.Format("<ul style='margin-top:0in' type=disc><li style='margin: 0 0;color:#2F5597;'>vkc2.3.3-{0}.sql<o:p></o:p></li></ul><br>" +
                "<p style='margin: 0 0;color:#2F5597;'>Following PBI are finished.<o:p></o:p></p>", DateTime.Now.AddDays(-1).ToString("yyyyMMdd")));
            body.Append("<table class='MsoNormalTable' width=1393 border = 1 cellspacing=0 cellpadding=0 style='color:#2F5597;'>");
            body.AppendLine("<tr style='height:17.15pt'>");
            body.Append(string.Format("<td width=130 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", "Work Item Type"));
            body.Append(string.Format("<td width=100 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", "ID"));
            body.Append(string.Format("<td width=760 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", "Title"));
            body.AppendLine("</tr>");
            foreach (VKWorkItem wi in vKWorkItemsRtn)
            {
                body.Append("<tr style='height:17.15pt'>");
                body.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.WorkItemType.Replace("Product Backlog Item", "PBI")));
                body.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.ID));
                body.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", wi.Title));
                body.AppendLine("</tr>");
            }
            body.Append("</table>");
            body.Append("<p style='margin: 0 0;color:#2F5597;'>&nbsp;<o:p></o:p></p>" +
                "<p style='margin: 0 0;color:#2F5597;'>Thanks.<o:p></o:p></p><p style='margin: 0 0;color:#2F5597;'><br>Regards,<o:p></o:p></p>" +
                $"<p style='margin: 0 0;color:#2F5597;'>{txtFromName.Text.ToStringEx()}<o:p></o:p></p></BODY></HTML>");

            return body.ToStringEx();
        }

        private List<VKWorkItem> GetSpecficDateWorkItems(List<VKWorkItem> vKWorkItems, DateTime dateTime, DateTime dateTimeEnd)
        {
            List<VKWorkItem> vKWorkItemsRtn = new List<VKWorkItem>();
            if (vKWorkItems == null || dateTime == null)
                return vKWorkItems;

            string status = string.Empty;
            foreach (var item in checkedListStatus.CheckedItems)
                status += item.ToStringEx() + ",";

            bool isFileterStatus = false;
            if (!status.IsNullOrEmpty())
                isFileterStatus = true;

            string type = string.Empty;
            foreach (var item in checkedListType.CheckedItems)
                type += item.ToStringEx() + ",";

            bool isType = false;
            if (!status.IsNullOrEmpty())
                isType = true;


            foreach (VKWorkItem wi in vKWorkItems)
            {
                if (wi.ChangedDate.DayOfYear < dateTime.DayOfYear)
                    continue;
                if (dateTimeEnd != null && (wi.ChangedDate.DayOfYear > dateTimeEnd.DayOfYear))
                    continue;

                if (isFileterStatus && !status.Contains(wi.State))
                    continue;

                if (isType && !type.Contains(wi.WorkItemType))
                    continue;

                if (!type.Contains("Test Case") && wi.Title.ToUpper().StartsWith("TEST"))
                    continue;

                vKWorkItemsRtn.Add(wi);
            }

            return vKWorkItemsRtn;
        }
 
    }
}

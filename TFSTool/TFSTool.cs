using Helper;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.IO;
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
            textSubject.Text = $"{Utils.GetConfig("proname")} released @ ~{DateTime.Now.ToString("yyyy/MM/dd")}";
            tlpText.Visible = false;
            txtOwners.Text = Utils.GetConfig("owners");


            checkedListStatus.SetItemChecked(2, true);
            checkedListStatus.SetItemChecked(3, true);
            checkedListType.SetItemChecked(0, true);
            checkedListType.SetItemChecked(2, true);
            checkedListType.SetItemChecked(4, true);
        }


        private void InitMethod()
        {
            //menu
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
                tlpText.Visible = !tlpText.Visible;
                tlpBrowser.Visible = !tlpBrowser.Visible;
                buttonSend.Enabled = buttonReceive.Enabled = buttonSaveLocal.Enabled = tlpBrowser.Visible;

                if (tlpText.Visible)
                {
                    richTextBox1.Text = webBrowserShow.Document.ActiveElement.InnerHtml.ToStringEx(); 
                }
                else
                {
                    webBrowserShow.DocumentText = $"<HTML><BODY contentEditable='true'>{richTextBox1.Text.ToStringEx()}</BODY></HTML>";
                }
            };
            this.configEmailToolStripMenuItem.Click += delegate {
                using (EmailContent dialog = new EmailContent())
                {
                    if (dialog.ShowDialog() != DialogResult.OK)
                    {
                        MessageBox.Show(this, "Email content configure does not saved.", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
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
                MessageBox.Show(this, "More details please find in ...", "About", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            };

            //Button
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
            buttonSaveLocal.Click += delegate
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

            //Text: Enter save
            txtSprintNum.KeyPress += delegate (object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Return)
                    return;

                Utils.SaveConfig("sprintnum", txtSprintNum.Text);
            };
            txtOwners.KeyPress += delegate (object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Return)
                    return;

                Utils.SaveConfig("owners", txtOwners.Text);
            };

            //check
            chkStatus.CheckedChanged += delegate
            {
                checkedListStatus.AllSelected(chkStatus.Checked);
            };
            chkType.CheckedChanged += delegate
            {
                checkedListType.AllSelected(chkType.Checked);
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
            string tfsQuery = Utils.GetConfig("tfsquery");
            string tfsURL = Utils.GetConfig("tfsurl");
            if (Utils.GetConfig("username").IsNullOrEmpty() || Utils.GetConfig("password").IsNullOrEmpty()
                || tfsQuery.IsNullOrEmpty() || tfsURL.IsNullOrEmpty())
            {
                throw new System.Exception("Please set tfs credential in app.config.");
            }

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

            body.AppendLine($"<HTML><BODY contentEditable='true'> {Utils.GetConfig("emailheader")}");
            body.AppendLine(  $"<p style='margin: 0 0;color:#2F5597;'>Following PBI are finished.<o:p></o:p></p>");
            body.Append("<table class='MsoNormalTable' width=1393 border = 1 cellspacing=0 cellpadding=0 style='color:#2F5597;'>");
            body.AppendLine("<tr style='height:17.15pt'>");
            body.Append(string.Format("<td width=130 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", "Work Item Type"));
            body.Append(string.Format("<td width=100 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", "ID"));
            body.Append(string.Format("<td width=760 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", "Title"));
            body.Append(string.Format("<td width=130 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", "AssignedTo"));
            body.AppendLine("</tr>");
            foreach (VKWorkItem wi in vKWorkItemsRtn)
            {
                body.Append("<tr style='height:17.15pt'>");
                body.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.WorkItemType.Replace("Product Backlog Item", "PBI")));
                body.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.ID));
                body.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", wi.Title));
                body.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.AssignedTo));
                body.AppendLine("</tr>");
            }
            body.Append("</table>");
            body.Append($"{Utils.GetConfig("emailfooter").ToStringEx()}</BODY></HTML>");

            return body.ToStringEx();
        }

        private List<VKWorkItem> GetSpecficDateWorkItems(List<VKWorkItem> vKWorkItems, DateTime dateTime, DateTime dateTimeEnd)
        {
            List<VKWorkItem> vKWorkItemsRtn = new List<VKWorkItem>();
            if (vKWorkItems == null || dateTime == null)
                return vKWorkItems;
            string[] owners = txtOwners.Text.Split(new char[] { ',' });

            foreach (VKWorkItem wi in vKWorkItems)
            {
                if (wi.ChangedDate.DayOfYear < dateTime.DayOfYear)
                    continue;
                if (dateTimeEnd != null && (wi.ChangedDate.DayOfYear > dateTimeEnd.DayOfYear))
                    continue;

                if (checkedListStatus.HasSelected(out string status) && !status.Contains(wi.State))
                    continue;

                if (checkedListType.HasSelected(out string type) && !type.Contains(wi.WorkItemType))
                    continue;

                if (owners.Length > 0)
                {
                    foreach (var owner in owners)
                        if (wi.AssignedTo.ToStringEx().Contains(owner))
                            vKWorkItemsRtn.Add(wi);
                }
                else
                {
                    vKWorkItemsRtn.Add(wi);
                }
            }

            return vKWorkItemsRtn;
        }
 
    }
}

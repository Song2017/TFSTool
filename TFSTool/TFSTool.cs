using Helper;
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
        StringBuilder body = new StringBuilder();
        StringBuilder content = new StringBuilder();


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
            //monday shoud get history from last friday
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                dateTimePicker.Value = DateTime.Now.AddDays(-3);
            else
                dateTimePicker.Value = DateTime.Now;
            dateTimePickerEnd.Value = DateTime.Now;
            DateTime dEnd = new DateTime(this.dateTimePickerEnd.Value.Year, 
                this.dateTimePickerEnd.Value.Month, this.dateTimePickerEnd.Value.Day, 23, 59, 59);
            dateTimePickerEnd.Value = dEnd;

            txtSprintNum.Text = Utils.GetConfig(AppConstants.SPRINTNUM);
            tlpText.Visible = false;
            txtOwners.Text = Utils.GetConfig(AppConstants.OWNERS);

            checkedListStatus.SetItemChecked(10, true);
            checkedListStatus.SetItemChecked(3, true);
            checkedListType.SetItemChecked(0, true);
            checkedListType.SetItemChecked(2, true);
        }

        private void InitMethod()
        {
            //menu
            this.ConfigToolStripMenuItem.Click += delegate (object sender, EventArgs e)
            {
                this.SetCredentials();
            };
            this.configChangeSetsToolStripMenuItem.Click += delegate {
                using (MenuChangeSets dialog = new MenuChangeSets())
                {
                    if (dialog.ShowDialog() != DialogResult.OK)
                    {
                        MessageBox.Show(this, "Change Sets content configure does not saved.", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }

            };
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
                using (MenuEmailContent dialog = new MenuEmailContent())
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
                sb.AppendLine("2. Set Credentials/Email Content/Changesets Path in Menu ");
                sb.AppendLine("3. Config filter condition: Date, Type, Status...");
                sb.AppendLine("4. Retrive TFS data via button ");
                sb.AppendLine("5. Edit Email content and Reload via Menu ");
                sb.AppendLine("6. Send Email via button");
                sb.AppendLine("7. Localize TFS Data via button");
                MessageBox.Show(this, sb.ToStringEx(), "Tips", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            };
            this.aboutToolStripMenuItem.Click += delegate
            {
                MessageBox.Show(this, "More details please find in \r\n  https://github.com/Song2017/TFSTool/blob/master/README.md", 
                    "About", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            };

            //Button
            buttonSend.Click += delegate (object sender, EventArgs e)
            {
                if (Utils.GetConfig(AppConstants.EMAIL_TO, "").IsNullOrEmpty())
                {
                    MessageBox.Show(this, "Please set Email To.", "SendEmail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                PrepareVKWorkItems();
            };
            buttonSaveLocal.Click += delegate
            {
                try
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
                }
                catch (Exception e) {
                    _Log.Error("Localize error: " + e.Message);
                }

            };

            //Text: Enter save
            txtSprintNum.KeyPress += delegate (object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Return)
                    return;

                Utils.SaveConfig(AppConstants.SPRINTNUM, txtSprintNum.Text);
            };
            txtOwners.KeyPress += delegate (object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Return)
                    return;

                Utils.SaveConfig(AppConstants.OWNERS, txtOwners.Text);
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


        private bool SetCredentials()
        {
            try
            {
                using (MenuCredentials dialog = new MenuCredentials())
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

        private bool SendEmail()
        {
            OutlookApplication outlookApplication = new OutlookApplication();
            try
            {
                outlookApplication.SendEmail(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem,
                    Utils.GetConfig(AppConstants.EMAIL_TO, ""),
                    Utils.GetConfig(AppConstants.EMAIL_CC, ""),
                    Utils.GetConfig(AppConstants.EMAIL_SUBJECT, "") + $" @ ~{DateTime.Now.ToString("yyyy / MM / dd")}",
                    webBrowserShow.DocumentText,
                    Microsoft.Office.Interop.Outlook.OlImportance.olImportanceNormal);
            }
            catch (Exception e)
            {
                _Log.Error("Send email error:" + e.Message);
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

        private void PrepareVKWorkItems()
        {

            this.BeginWait();
            ThreadPool.QueueUserWorkItem(delegate (object arg)
            {
                try
                {
                    TFSOperation tFSOperation = TFSOperation.Instance;

                    try
                    {
                        if (!SetTFSInstance(ref tFSOperation))
                            return;

                        if (!tFSOperation.ConnectTFS())
                            return;
                    }
                    catch (Exception e)
                    {
                        base._Log.Error("TFS Operation:" + e.Message);
                    }

                    try
                    {
                        body = new StringBuilder();
                        content = new StringBuilder();

                        if (Utils.GetConfig(AppConstants.CHANGESETS_ENABLE, "F") == "T")
                        {
                            List<ChangeSetItem> changeSets = new List<ChangeSetItem>();
                            string[] strChangeSets = Utils.GetConfig(AppConstants.CHANGESETS_PROS).Split(',');
                            foreach (string pro in strChangeSets)
                            {

                                changeSets = tFSOperation.GetChangeItems(pro);
                                HandleCSItems(changeSets, pro.Replace("$/", string.Empty));

                            }
                        }
                        List<VKWorkItem> vKWorkItems = tFSOperation.GetVKWorkItems();
                        if (vKWorkItems == null || vKWorkItems.Count <= 0)
                            return;

                        HandleWorkItems(vKWorkItems);
                        GetEmailContent();
                    }
                    catch (Exception e)
                    {
                        base._Log.Error("Items Handle error:" + e.Message);
                    }
                } 
                finally
                {
                    this.EndWait();
                }
            });
        }

        private bool SetTFSInstance(ref TFSOperation tFSOperation)
        { 
            string tfsQuery = Utils.GetConfig(AppConstants.TFSQUERY);
            string tfsURL = Utils.GetConfig(AppConstants.TFSURL);
            if (Utils.GetConfig(AppConstants.TFS_USERNAME).IsNullOrEmpty() || Utils.GetConfig(AppConstants.TFS_PASSWORD).IsNullOrEmpty()
                || tfsQuery.IsNullOrEmpty() || tfsURL.IsNullOrEmpty())
            {
                throw new Exception("Please set tfs credential in app.config.");
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
            if (vKWorkItems == null || vKWorkItems.Count <= 0)
                return;

            for (int i = 0; i < vKWorkItems.Count; i++)
            {
                if (sprintNum.IsNullOrEmpty() || vKWorkItems[i].IterationPath.ToLower().Contains(sprintNum.ToLower()))
                    vKWorkItemsRtn.Add(vKWorkItems[i]);
            }

            //get vk items by UI 
            _vKWorkItemsCache = GetSpecficDateWorkItems(vKWorkItemsRtn, dateTime, dateTimeEnd);

            //set email content
            vKWorkItemsRtn = GetSpecficDateWorkItems(_vKWorkItemsCache, dateTime, dateTimeEnd);

            SetEmailContent(vKWorkItemsRtn, sprintNum);
        }

        private void SetEmailContent(List<VKWorkItem> vKWorkItemsRtn, string sprintNum)
        {
            content.AppendLine($"<p style='margin: 0 0;color:#2F5597;'>Following Items are finished. Please Verify!<o:p></o:p></p>");
            content.Append("<table class='MsoNormalTable' width=1100 border = 1 cellspacing=0 cellpadding=0 style='color:#2F5597;'>");
            content.AppendLine("<tr style='height:17.15pt'>");
            content.Append(string.Format("<td width=110 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", "Work Item Type"));
            content.Append(string.Format("<td width=150 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", "AssignedTo"));
            content.Append(string.Format("<td width=80 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", "ID"));
            content.Append(string.Format("<td width=80 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", "State"));
            content.Append(string.Format("<td style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", "Title"));

            content.AppendLine("</tr>");
            foreach (VKWorkItem wi in vKWorkItemsRtn)
            {
                content.Append("<tr style='height:17.15pt'>");
                content.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.WorkItemType.Replace("Product Backlog Item", "PBI")));
                content.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.AssignedTo));
                content.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.ID));
                content.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.State));
                content.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", wi.Title));

                content.AppendLine("</tr>");
            }
            content.Append("</table>");

            content.AppendLine("</br>");
        }

        private void HandleCSItems(List<ChangeSetItem> csItems, string projectName)
        {
            List<ChangeSetItem> csItemsRtn = new List<ChangeSetItem>();
            DateTime dateTime = dateTimePicker.Value;
            DateTime dateTimeEnd = dateTimePickerEnd.Value;
            if (csItems == null || csItems.Count <= 0)
                return;

            for (int i = 0; i < csItems.Count; i++)
            {
                if ( csItems[i].CreationDate >= dateTime && csItems[i].CreationDate <= dateTimeEnd)
                    csItemsRtn.Add(csItems[i]);
            }

            SetChangeSetEmailContent(csItemsRtn, projectName);
        }


        private void GetEmailContent()
        {
            if (Utils.GetConfig(AppConstants.EMAIL_HEADER).IsNullOrEmpty()
                || Utils.GetConfig(AppConstants.EMAIL_FOOTER).IsNullOrEmpty())
            {
                MessageBox.Show(this, "Please set Email Header&Footer.", "SendEmail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            body.AppendLine($"<HTML><BODY contentEditable='true'> " +
                $"{ Utils.GetConfig(AppConstants.EMAIL_HEADER).Replace("$$spring_number$$", Utils.GetConfig(AppConstants.SPRINTNUM)).Replace("$$pro_name$$", Utils.GetConfig(AppConstants.PRONAME, "Pro Name")).Replace("$$date$$", DateTime.Now.ToString("HH:mm MMMM dd"))}");

            body.Append(content);

            body.Append($"{Utils.GetConfig(AppConstants.EMAIL_FOOTER).Replace("$$from_name$$", Utils.GetConfig(AppConstants.EMAIL_SENDER, "Sender Name"))}</BODY></HTML>");

            webBrowserShow.DocumentText = body.ToStringEx();
        }

        private void SetChangeSetEmailContent(List<ChangeSetItem> vKWorkItemsRtn,string projectName)
        {
            content.AppendLine($"<p style='margin: 0 0;color:#2F5597;'>"+ projectName + " ChangeSets<o:p></o:p></p>");
            content.Append("<table class='MsoNormalTable' width=1200 border = 1 cellspacing=0 cellpadding=0 style='color:#2F5597;'>");
            content.AppendLine("<tr style='height:17.15pt'>");
            content.Append(string.Format("<td width=100 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", "ChangeSet Id"));
            content.Append(string.Format("<td width=200 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", "User"));
            content.Append(string.Format("<td width=150 style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", "Time"));
            content.Append(string.Format("<td style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", "Comment"));
            content.AppendLine("</tr>");
            foreach (ChangeSetItem wi in vKWorkItemsRtn)
            {
                content.Append("<tr style='height:17.15pt'>");
                content.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.ChangesetId));
                content.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.OwnerDisplayName));
                content.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0} </span></td>", wi.CreationDate.ToStringEx()));
                content.Append(string.Format("<td   style='padding:0in 0in 0in 0in;height:17.15pt'><span style='font-size:12.0pt'>{0}</span></td>", wi.Comment));
                content.AppendLine("</tr>");
            }
            content.Append("</table>");
            content.AppendLine("</br>");
        }

        private List<VKWorkItem> GetSpecficDateWorkItems(List<VKWorkItem> vKWorkItems, DateTime dateTime, DateTime dateTimeEnd)
        {
            List<VKWorkItem> vKWorkItemsRtn = new List<VKWorkItem>();
            if (vKWorkItems == null || dateTime == null)
                return vKWorkItems;
            string[] owners = txtOwners.Text.Split(new char[] { ',' });

            try
            {
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
                            if (wi.AssignedTo.ToStringEx().ToLower().Contains(owner.ToLower()))
                                vKWorkItemsRtn.Add(wi);
                    }
                    else
                    {
                        vKWorkItemsRtn.Add(wi);
                    }
                }
            }
            catch (Exception e){

            }

            return vKWorkItemsRtn;
        }
 
    }
}

using Helper;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using TFSUtils;

namespace TFSTool
{
    public partial class TFSTool : BaseForm
    {
        string query = "SELECT [System.Id],[System.WorkItemType],[System.Title],[System.AssignedTo],[System.State],[System.Tags], [System.IterationPath] FROM WorkItems WHERE [System.TeamProject] = 'VKC2' AND[System.Id] > 23186";
        string _URL = "http://ogmcshyaptf01.logon.ds.ge.com:8080/tfs/defaultcollection";
        private string currentSprintNumber = "84";

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
        }


        private void InitMethod()
        {
            this.ConfigToolStripMenuItem.Click += delegate (object sender, EventArgs e)
            {
                if (this.SetCredentials())
                {
                    this.SetUICredentials();
                }
            };
            this.loadToolStripMenuItem.Click += delegate { this.SetUICredentials(); };
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

                    SetEmailContent(vKWorkItems);
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
                MessageBox.Show(this, "Please input credential.", "Credential", MessageBoxButtons.OK);
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

        private void SetEmailContent(List<VKWorkItem> vKWorkItems)
        {
            List<VKWorkItem> vKWorkItemsRtn = new List<VKWorkItem>();
            for (int i = 0; i < vKWorkItems.Count; i++)
            {
                if (vKWorkItems[i].IterationPath.Contains(currentSprintNumber))
                    vKWorkItemsRtn.Add(vKWorkItems[i]);
            }

            vKWorkItemsRtn.SaveWorkItemList();

        }
    }
}

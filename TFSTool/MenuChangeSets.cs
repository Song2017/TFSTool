using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TFSUtils;

namespace TFSTool
{
    public partial class MenuChangeSets : BaseForm
    {
        public MenuChangeSets()
        {
            InitializeComponent();
            InitUI();
            InitMethod();
        }

        private void InitUI()
        {
            chkEnableChangesets.Checked = Utils.GetConfig(AppConstants.CHANGESETS_ENABLE, "F") == "T";
            txtChangesetsPro.Text = Utils.GetConfig(AppConstants.CHANGESETS_PROS, "$/path/path A,$/path/path B");
        }
        private void InitMethod()
        {
            this.buttonOK.Click += delegate (object sender, EventArgs e)
            {
                Utils.SaveConfig(AppConstants.CHANGESETS_ENABLE, chkEnableChangesets.Checked ? "T" : "F");
                Utils.SaveConfig(AppConstants.CHANGESETS_PROS, txtChangesetsPro.Text);

                base.DialogResult = DialogResult.OK;
            };
            this.buttonCancel.Click += delegate (object sender, EventArgs e)
            {
                base.DialogResult = DialogResult.Cancel;
            };
        }
    }
}

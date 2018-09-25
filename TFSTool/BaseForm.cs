using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TFSTool
{

    public partial class BaseForm : Form
    {
        protected Logger _Log { get; private set; }
        public virtual ProgressBar ProgressBarControl
        {
            get
            {
                return this.progressBar;
            }
            set
            {
                this.progressBar = value;
            }
        }


        public BaseForm()
        {
            this.InitializeComponent();
            this._Log = LogManager.GetLogger(base.GetType().FullName);
        }

        private ProgressBar progressBar;
    }

}

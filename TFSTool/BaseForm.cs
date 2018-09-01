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
            }

            private ProgressBar progressBar;
        }

}

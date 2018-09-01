using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TFSTool
{
    public static class BaseFormEx
    {
        // Token: 0x0600001C RID: 28 RVA: 0x000025CC File Offset: 0x000007CC
        public static void BeginWait(this BaseForm baseForm)
        {
            baseForm.Invoke(new MethodInvoker(delegate ()
            {
                baseForm.Enabled = false;
                if (baseForm.ProgressBarControl == null)
                {
                    baseForm.ProgressBarControl = new ProgressBar();
                    baseForm.Controls.Add(baseForm.ProgressBarControl);
                    baseForm.ProgressBarControl.Size = new Size(246, 36);
                    baseForm.ProgressBarControl.Name = "progressBar";
                    baseForm.ProgressBarControl.Visible = true;
                    baseForm.ProgressBarControl.Style = ProgressBarStyle.Marquee;
                    baseForm.ProgressBarControl.Location = new Point(baseForm.Width / 2 - baseForm.ProgressBarControl.Width / 2, baseForm.Height / 2 - baseForm.ProgressBarControl.Height);
                    baseForm.ProgressBarControl.BringToFront();
                }
            }));
        }

        // Token: 0x0600001D RID: 29 RVA: 0x00002600 File Offset: 0x00000800
        public static void EndWait(this BaseForm baseForm)
        {
            baseForm.Invoke(new MethodInvoker(delegate ()
            {
                if (baseForm.ProgressBarControl != null)
                {
                    if (baseForm.Controls.Contains(baseForm.ProgressBarControl))
                    {
                        baseForm.Controls.Remove(baseForm.ProgressBarControl);
                    }
                    baseForm.ProgressBarControl.Dispose();
                    baseForm.ProgressBarControl = null;
                    baseForm.Enabled = true;
                }
            }));
        }
    } 
}

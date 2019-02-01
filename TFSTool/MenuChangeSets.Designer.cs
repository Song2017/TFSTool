namespace TFSTool
{
    partial class MenuChangeSets
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtChangesetsPro = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chkEnableChangesets = new System.Windows.Forms.CheckBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtChangesetsPro
            // 
            this.txtChangesetsPro.Location = new System.Drawing.Point(17, 61);
            this.txtChangesetsPro.Name = "txtChangesetsPro";
            this.txtChangesetsPro.Size = new System.Drawing.Size(271, 20);
            this.txtChangesetsPro.TabIndex = 71;
            this.txtChangesetsPro.Text = "A,B";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(14, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(144, 13);
            this.label6.TabIndex = 70;
            this.label6.Text = "ChangeSets Product Names:";
            // 
            // chkEnableChangesets
            // 
            this.chkEnableChangesets.AutoSize = true;
            this.chkEnableChangesets.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkEnableChangesets.Location = new System.Drawing.Point(17, 16);
            this.chkEnableChangesets.Name = "chkEnableChangesets";
            this.chkEnableChangesets.Size = new System.Drawing.Size(121, 17);
            this.chkEnableChangesets.TabIndex = 72;
            this.chkEnableChangesets.Text = "Contain Changesets";
            this.chkEnableChangesets.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(217, 104);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(71, 22);
            this.buttonCancel.TabIndex = 74;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(96, 104);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(71, 22);
            this.buttonOK.TabIndex = 73;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // ChangeSets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 153);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.chkEnableChangesets);
            this.Controls.Add(this.txtChangesetsPro);
            this.Controls.Add(this.label6);
            this.Name = "ChangeSets";
            this.Text = "ChangeSets";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtChangesetsPro;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkEnableChangesets;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
    }
}
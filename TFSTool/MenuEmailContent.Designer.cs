namespace TFSTool
{
    partial class MenuEmailContent
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.richHeader = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.richFooter = new System.Windows.Forms.RichTextBox();
            this.txtPro = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtFromName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(357, 328);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(71, 22);
            this.buttonCancel.TabIndex = 24;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(236, 328);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(71, 22);
            this.buttonOK.TabIndex = 23;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.richHeader);
            this.groupBox2.Location = new System.Drawing.Point(12, 52);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(419, 161);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Email Header";
            // 
            // richHeader
            // 
            this.richHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richHeader.Location = new System.Drawing.Point(3, 16);
            this.richHeader.Name = "richHeader";
            this.richHeader.Size = new System.Drawing.Size(413, 142);
            this.richHeader.TabIndex = 0;
            this.richHeader.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richFooter);
            this.groupBox1.Location = new System.Drawing.Point(12, 219);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(419, 103);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Email Footer";
            // 
            // richFooter
            // 
            this.richFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richFooter.Location = new System.Drawing.Point(3, 16);
            this.richFooter.Name = "richFooter";
            this.richFooter.Size = new System.Drawing.Size(413, 84);
            this.richFooter.TabIndex = 1;
            this.richFooter.Text = "";
            // 
            // txtPro
            // 
            this.txtPro.Location = new System.Drawing.Point(80, 26);
            this.txtPro.Name = "txtPro";
            this.txtPro.Size = new System.Drawing.Size(96, 20);
            this.txtPro.TabIndex = 67;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(17, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 66;
            this.label6.Text = "Pro Name:";
            // 
            // txtFromName
            // 
            this.txtFromName.Location = new System.Drawing.Point(254, 27);
            this.txtFromName.Name = "txtFromName";
            this.txtFromName.Size = new System.Drawing.Size(96, 20);
            this.txtFromName.TabIndex = 69;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(184, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 68;
            this.label4.Text = "From Name:";
            // 
            // MenuEmailContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 360);
            this.Controls.Add(this.txtFromName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPro);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MenuEmailContent";
            this.Text = "Email Content";
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPro;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtFromName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox richHeader;
        private System.Windows.Forms.RichTextBox richFooter;
    }
}
namespace TFSTool
{
    partial class MenuCredentials
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
            this.txtQuery = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabelCredentials = new System.Windows.Forms.LinkLabel();
            this.textPassWord = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textUserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textSubject = new System.Windows.Forms.TextBox();
            this.labelSubject = new System.Windows.Forms.Label();
            this.textCC = new System.Windows.Forms.TextBox();
            this.labelCC = new System.Windows.Forms.Label();
            this.textTo = new System.Windows.Forms.TextBox();
            this.labelTo = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(236, 315);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(71, 22);
            this.buttonCancel.TabIndex = 24;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(115, 315);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(71, 22);
            this.buttonOK.TabIndex = 23;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtQuery);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtUrl);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.linkLabelCredentials);
            this.groupBox2.Controls.Add(this.textPassWord);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textUserName);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(419, 161);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "TFS Authentication Credentials";
            // 
            // txtQuery
            // 
            this.txtQuery.Location = new System.Drawing.Point(59, 121);
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.Size = new System.Drawing.Size(350, 20);
            this.txtQuery.TabIndex = 25;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-2, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "TFSQuery:";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(59, 95);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(350, 20);
            this.txtUrl.TabIndex = 23;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(-3, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "TFSURL:";
            // 
            // linkLabelCredentials
            // 
            this.linkLabelCredentials.AutoSize = true;
            this.linkLabelCredentials.Location = new System.Drawing.Point(6, 16);
            this.linkLabelCredentials.Name = "linkLabelCredentials";
            this.linkLabelCredentials.Size = new System.Drawing.Size(176, 13);
            this.linkLabelCredentials.TabIndex = 21;
            this.linkLabelCredentials.TabStop = true;
            this.linkLabelCredentials.Text = "Alternate authentication credentials ";
            // 
            // textPassWord
            // 
            this.textPassWord.Location = new System.Drawing.Point(59, 69);
            this.textPassWord.Name = "textPassWord";
            this.textPassWord.Size = new System.Drawing.Size(350, 20);
            this.textPassWord.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-2, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "PassWord:";
            // 
            // textUserName
            // 
            this.textUserName.Location = new System.Drawing.Point(59, 43);
            this.textUserName.Name = "textUserName";
            this.textUserName.Size = new System.Drawing.Size(350, 20);
            this.textUserName.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(-3, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "UserName:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textSubject);
            this.groupBox1.Controls.Add(this.labelSubject);
            this.groupBox1.Controls.Add(this.textCC);
            this.groupBox1.Controls.Add(this.labelCC);
            this.groupBox1.Controls.Add(this.textTo);
            this.groupBox1.Controls.Add(this.labelTo);
            this.groupBox1.Location = new System.Drawing.Point(12, 179);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(419, 130);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Email";
            // 
            // textSubject
            // 
            this.textSubject.Location = new System.Drawing.Point(59, 91);
            this.textSubject.Name = "textSubject";
            this.textSubject.Size = new System.Drawing.Size(350, 20);
            this.textSubject.TabIndex = 22;
            // 
            // labelSubject
            // 
            this.labelSubject.AutoSize = true;
            this.labelSubject.Location = new System.Drawing.Point(7, 94);
            this.labelSubject.Name = "labelSubject";
            this.labelSubject.Size = new System.Drawing.Size(46, 13);
            this.labelSubject.TabIndex = 21;
            this.labelSubject.Text = "Subject:";
            // 
            // textCC
            // 
            this.textCC.Location = new System.Drawing.Point(59, 65);
            this.textCC.Name = "textCC";
            this.textCC.Size = new System.Drawing.Size(350, 20);
            this.textCC.TabIndex = 20;
            // 
            // labelCC
            // 
            this.labelCC.AutoSize = true;
            this.labelCC.Location = new System.Drawing.Point(28, 68);
            this.labelCC.Name = "labelCC";
            this.labelCC.Size = new System.Drawing.Size(24, 13);
            this.labelCC.TabIndex = 19;
            this.labelCC.Text = "CC:";
            // 
            // textTo
            // 
            this.textTo.Location = new System.Drawing.Point(59, 19);
            this.textTo.Multiline = true;
            this.textTo.Name = "textTo";
            this.textTo.Size = new System.Drawing.Size(350, 40);
            this.textTo.TabIndex = 18;
            // 
            // labelTo
            // 
            this.labelTo.AutoSize = true;
            this.labelTo.Location = new System.Drawing.Point(28, 22);
            this.labelTo.Name = "labelTo";
            this.labelTo.Size = new System.Drawing.Size(23, 13);
            this.labelTo.TabIndex = 17;
            this.labelTo.Text = "To:";
            // 
            // MenuCredentials
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 360);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MenuCredentials";
            this.Text = "Credentials";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.LinkLabel linkLabelCredentials;
        private System.Windows.Forms.TextBox textPassWord;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textUserName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textSubject;
        private System.Windows.Forms.Label labelSubject;
        private System.Windows.Forms.TextBox textCC;
        private System.Windows.Forms.Label labelCC;
        private System.Windows.Forms.TextBox textTo;
        private System.Windows.Forms.Label labelTo;
        private System.Windows.Forms.TextBox txtQuery;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Label label4;
    }
}
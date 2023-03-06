namespace VCCorp.TikTokCrawler
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btShowDevTool = new Ext.RButton.RJButton();
            this.btKingLive = new Ext.RButton.RJButton();
            this.btStartedTag = new Ext.RButton.RJButton();
            this.btResume = new Ext.RButton.RJButton();
            this.btResumeLastTag = new Ext.RButton.RJButton();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(12, 75);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1220, 589);
            this.panel1.TabIndex = 1;
            // 
            // txtUrl
            // 
            this.txtUrl.Enabled = false;
            this.txtUrl.Location = new System.Drawing.Point(379, 29);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(520, 20);
            this.txtUrl.TabIndex = 10;
            // 
            // btShowDevTool
            // 
            this.btShowDevTool.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(121)))), ((int)(((byte)(111)))));
            this.btShowDevTool.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(121)))), ((int)(((byte)(111)))));
            this.btShowDevTool.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btShowDevTool.BorderRadius = 7;
            this.btShowDevTool.BorderSize = 0;
            this.btShowDevTool.FlatAppearance.BorderSize = 0;
            this.btShowDevTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btShowDevTool.ForeColor = System.Drawing.Color.Transparent;
            this.btShowDevTool.Location = new System.Drawing.Point(257, 18);
            this.btShowDevTool.Name = "btShowDevTool";
            this.btShowDevTool.Size = new System.Drawing.Size(113, 40);
            this.btShowDevTool.TabIndex = 11;
            this.btShowDevTool.Text = "DevTool";
            this.btShowDevTool.TextColor = System.Drawing.Color.Transparent;
            this.btShowDevTool.UseVisualStyleBackColor = false;
            this.btShowDevTool.Click += new System.EventHandler(this.btShowDevTool_Click);
            // 
            // btKingLive
            // 
            this.btKingLive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(61)))), ((int)(((byte)(91)))));
            this.btKingLive.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(61)))), ((int)(((byte)(91)))));
            this.btKingLive.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btKingLive.BorderRadius = 7;
            this.btKingLive.BorderSize = 0;
            this.btKingLive.FlatAppearance.BorderSize = 0;
            this.btKingLive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btKingLive.ForeColor = System.Drawing.Color.White;
            this.btKingLive.Location = new System.Drawing.Point(17, 18);
            this.btKingLive.Name = "btKingLive";
            this.btKingLive.Size = new System.Drawing.Size(102, 40);
            this.btKingLive.TabIndex = 0;
            this.btKingLive.Text = "Kinglive.vn";
            this.btKingLive.TextColor = System.Drawing.Color.White;
            this.btKingLive.UseVisualStyleBackColor = false;
            this.btKingLive.Click += new System.EventHandler(this.btKingLive_Click);
            // 
            // btStartedTag
            // 
            this.btStartedTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(58)))), ((int)(((byte)(52)))));
            this.btStartedTag.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(58)))), ((int)(((byte)(52)))));
            this.btStartedTag.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btStartedTag.BorderRadius = 7;
            this.btStartedTag.BorderSize = 0;
            this.btStartedTag.FlatAppearance.BorderSize = 0;
            this.btStartedTag.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btStartedTag.ForeColor = System.Drawing.Color.White;
            this.btStartedTag.Location = new System.Drawing.Point(123, 18);
            this.btStartedTag.Name = "btStartedTag";
            this.btStartedTag.Size = new System.Drawing.Size(130, 40);
            this.btStartedTag.TabIndex = 0;
            this.btStartedTag.Text = "Bắt đầu crwal";
            this.btStartedTag.TextColor = System.Drawing.Color.White;
            this.btStartedTag.UseVisualStyleBackColor = false;
            this.btStartedTag.Click += new System.EventHandler(this.btStartedTag_Click);
            // 
            // btResume
            // 
            this.btResume.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btResume.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(76)))), ((int)(((byte)(97)))));
            this.btResume.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(76)))), ((int)(((byte)(97)))));
            this.btResume.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btResume.BorderRadius = 7;
            this.btResume.BorderSize = 0;
            this.btResume.FlatAppearance.BorderSize = 0;
            this.btResume.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btResume.ForeColor = System.Drawing.Color.White;
            this.btResume.Location = new System.Drawing.Point(905, 18);
            this.btResume.Name = "btResume";
            this.btResume.Size = new System.Drawing.Size(153, 40);
            this.btResume.TabIndex = 0;
            this.btResume.Text = "Tiếp tục";
            this.btResume.TextColor = System.Drawing.Color.White;
            this.btResume.UseVisualStyleBackColor = false;
            this.btResume.Click += new System.EventHandler(this.btResume_Click);
            // 
            // btResumeLastTag
            // 
            this.btResumeLastTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(126)))), ((int)(((byte)(137)))));
            this.btResumeLastTag.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(126)))), ((int)(((byte)(137)))));
            this.btResumeLastTag.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btResumeLastTag.BorderRadius = 7;
            this.btResumeLastTag.BorderSize = 0;
            this.btResumeLastTag.FlatAppearance.BorderSize = 0;
            this.btResumeLastTag.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btResumeLastTag.ForeColor = System.Drawing.Color.White;
            this.btResumeLastTag.Location = new System.Drawing.Point(1071, 18);
            this.btResumeLastTag.Name = "btResumeLastTag";
            this.btResumeLastTag.Size = new System.Drawing.Size(157, 40);
            this.btResumeLastTag.TabIndex = 9;
            this.btResumeLastTag.Text = "Tiếp tục với tag cuối cùng";
            this.btResumeLastTag.TextColor = System.Drawing.Color.White;
            this.btResumeLastTag.UseVisualStyleBackColor = false;
            this.btResumeLastTag.Click += new System.EventHandler(this.rjButton1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1244, 676);
            this.Controls.Add(this.btShowDevTool);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.btKingLive);
            this.Controls.Add(this.btStartedTag);
            this.Controls.Add(this.btResume);
            this.Controls.Add(this.btResumeLastTag);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Crawl dữ liệu TikTok";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private Ext.RButton.RJButton btResumeLastTag;
        private Ext.RButton.RJButton btResume;
        private Ext.RButton.RJButton btStartedTag;
        private Ext.RButton.RJButton btKingLive;
        private System.Windows.Forms.TextBox txtUrl;
        private Ext.RButton.RJButton btShowDevTool;
    }
}


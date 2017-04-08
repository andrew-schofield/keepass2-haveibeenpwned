namespace HaveIBeenPwned
{
    partial class CheckerPrompt
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.checkOldEntries = new System.Windows.Forms.CheckBox();
            this.expireEntries = new System.Windows.Forms.CheckBox();
            this.layoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.breachCheckerLogo = new System.Windows.Forms.PictureBox();
            this.breachCheckerText = new System.Windows.Forms.Label();
            this.ignoreDeletedEntries = new System.Windows.Forms.CheckBox();
            this.layoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.breachCheckerLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(181, 131);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(262, 131);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // checkOldEntries
            // 
            this.checkOldEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkOldEntries.AutoSize = true;
            this.checkOldEntries.Checked = true;
            this.checkOldEntries.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkOldEntries.Location = new System.Drawing.Point(13, 59);
            this.checkOldEntries.Name = "checkOldEntries";
            this.checkOldEntries.Size = new System.Drawing.Size(331, 17);
            this.checkOldEntries.TabIndex = 2;
            this.checkOldEntries.Text = "Only check entries that have not changed since the breach date";
            this.checkOldEntries.UseVisualStyleBackColor = true;
            // 
            // expireEntries
            // 
            this.expireEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.expireEntries.AutoSize = true;
            this.expireEntries.Location = new System.Drawing.Point(13, 105);
            this.expireEntries.Name = "expireEntries";
            this.expireEntries.Size = new System.Drawing.Size(253, 17);
            this.expireEntries.TabIndex = 3;
            this.expireEntries.Text = "Expire any entries that are found to be breached";
            this.expireEntries.UseVisualStyleBackColor = true;
            // 
            // layoutPanel
            // 
            this.layoutPanel.ColumnCount = 2;
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutPanel.Controls.Add(this.breachCheckerLogo, 0, 0);
            this.layoutPanel.Controls.Add(this.breachCheckerText, 1, 0);
            this.layoutPanel.Location = new System.Drawing.Point(77, 12);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.RowCount = 1;
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutPanel.Size = new System.Drawing.Size(200, 38);
            this.layoutPanel.TabIndex = 4;
            // 
            // breachCheckerLogo
            // 
            this.breachCheckerLogo.Location = new System.Drawing.Point(3, 3);
            this.breachCheckerLogo.Name = "breachCheckerLogo";
            this.breachCheckerLogo.Size = new System.Drawing.Size(32, 32);
            this.breachCheckerLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.breachCheckerLogo.TabIndex = 0;
            this.breachCheckerLogo.TabStop = false;
            // 
            // breachCheckerText
            // 
            this.breachCheckerText.AutoSize = true;
            this.breachCheckerText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.breachCheckerText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.breachCheckerText.Location = new System.Drawing.Point(41, 0);
            this.breachCheckerText.Name = "breachCheckerText";
            this.breachCheckerText.Size = new System.Drawing.Size(156, 38);
            this.breachCheckerText.TabIndex = 1;
            this.breachCheckerText.Text = "Breach Title";
            this.breachCheckerText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ignoreDeletedEntries
            // 
            this.ignoreDeletedEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ignoreDeletedEntries.AutoSize = true;
            this.ignoreDeletedEntries.Checked = true;
            this.ignoreDeletedEntries.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ignoreDeletedEntries.Location = new System.Drawing.Point(13, 82);
            this.ignoreDeletedEntries.Name = "ignoreDeletedEntries";
            this.ignoreDeletedEntries.Size = new System.Drawing.Size(242, 17);
            this.ignoreDeletedEntries.TabIndex = 5;
            this.ignoreDeletedEntries.Text = "Ignore any deleted entries (i.e. in Recycle Bin)";
            this.ignoreDeletedEntries.UseVisualStyleBackColor = true;
            // 
            // CheckerPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 166);
            this.Controls.Add(this.ignoreDeletedEntries);
            this.Controls.Add(this.layoutPanel);
            this.Controls.Add(this.expireEntries);
            this.Controls.Add(this.checkOldEntries);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckerPrompt";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Have I Been Pwned?";
            this.layoutPanel.ResumeLayout(false);
            this.layoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.breachCheckerLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox checkOldEntries;
        private System.Windows.Forms.CheckBox expireEntries;
        private System.Windows.Forms.TableLayoutPanel layoutPanel;
        private System.Windows.Forms.PictureBox breachCheckerLogo;
        private System.Windows.Forms.Label breachCheckerText;
        private System.Windows.Forms.CheckBox ignoreDeletedEntries;
    }
}
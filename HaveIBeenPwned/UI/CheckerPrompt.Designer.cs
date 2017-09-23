using System;
using System.Linq;
using System.Windows.Forms;

namespace HaveIBeenPwned.UI
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
            this.ignoreDeletedEntries = new System.Windows.Forms.CheckBox();
            this.checkAllBreaches = new System.Windows.Forms.CheckBox();
            this.supportedBreachList = new System.Windows.Forms.ComboBox();
            this.checkAllBreachLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(181, 134);
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
            this.cancelButton.Location = new System.Drawing.Point(262, 134);
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
            this.checkOldEntries.Location = new System.Drawing.Point(3, 25);
            this.checkOldEntries.Name = "checkOldEntries";
            this.checkOldEntries.Size = new System.Drawing.Size(319, 16);
            this.checkOldEntries.TabIndex = 2;
            this.checkOldEntries.Text = "Only check entries that have not changed since the breach date";
            this.checkOldEntries.UseVisualStyleBackColor = true;
            // 
            // expireEntries
            // 
            this.expireEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.expireEntries.AutoSize = true;
            this.expireEntries.Location = new System.Drawing.Point(3, 69);
            this.expireEntries.Name = "expireEntries";
            this.expireEntries.Size = new System.Drawing.Size(253, 16);
            this.expireEntries.TabIndex = 3;
            this.expireEntries.Text = "Expire any entries that are found to be breached";
            this.expireEntries.UseVisualStyleBackColor = true;
            // 
            // ignoreDeletedEntries
            // 
            this.ignoreDeletedEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ignoreDeletedEntries.AutoSize = true;
            this.ignoreDeletedEntries.Checked = true;
            this.ignoreDeletedEntries.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ignoreDeletedEntries.Location = new System.Drawing.Point(3, 47);
            this.ignoreDeletedEntries.Name = "ignoreDeletedEntries";
            this.ignoreDeletedEntries.Size = new System.Drawing.Size(242, 16);
            this.ignoreDeletedEntries.TabIndex = 5;
            this.ignoreDeletedEntries.Text = "Ignore any deleted entries (i.e. in Recycle Bin)";
            this.ignoreDeletedEntries.UseVisualStyleBackColor = true;
            // 
            // checkAllBreaches
            // 
            this.checkAllBreaches.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkAllBreaches.AutoSize = true;
            this.checkAllBreaches.Location = new System.Drawing.Point(3, 3);
            this.checkAllBreaches.Name = "checkAllBreaches";
            this.checkAllBreaches.Size = new System.Drawing.Size(167, 16);
            this.checkAllBreaches.TabIndex = 6;
            this.checkAllBreaches.Text = "Check all supported breaches";
            this.checkAllBreaches.UseVisualStyleBackColor = true;
            // 
            // supportedBreachList
            // 
            this.supportedBreachList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.supportedBreachList.DisplayMember = "Text";
            this.supportedBreachList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.supportedBreachList.FormattingEnabled = true;
            this.supportedBreachList.Location = new System.Drawing.Point(134, 15);
            this.supportedBreachList.Name = "supportedBreachList";
            this.supportedBreachList.Size = new System.Drawing.Size(203, 21);
            this.supportedBreachList.TabIndex = 7;
            // 
            // checkAllBreachLabel
            // 
            this.checkAllBreachLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkAllBreachLabel.AutoSize = true;
            this.checkAllBreachLabel.Location = new System.Drawing.Point(12, 18);
            this.checkAllBreachLabel.Name = "checkAllBreachLabel";
            this.checkAllBreachLabel.Size = new System.Drawing.Size(116, 13);
            this.checkAllBreachLabel.TabIndex = 8;
            this.checkAllBreachLabel.Text = "Check breaches using:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.checkAllBreaches, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkOldEntries, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ignoreDeletedEntries, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.expireEntries, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 42);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(325, 86);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // CheckerPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 169);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.checkAllBreachLabel);
            this.Controls.Add(this.supportedBreachList);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckerPrompt";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Have I Been Pwned?";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox checkOldEntries;
        private System.Windows.Forms.CheckBox expireEntries;
        private System.Windows.Forms.CheckBox ignoreDeletedEntries;
        private System.Windows.Forms.CheckBox checkAllBreaches;
        private System.Windows.Forms.ComboBox supportedBreachList;
        private System.Windows.Forms.Label checkAllBreachLabel;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
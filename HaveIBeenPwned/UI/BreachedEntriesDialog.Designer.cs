namespace HaveIBeenPwned.UI
{
    partial class BreachedEntriesDialog
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
            System.Windows.Forms.ColumnHeader titleHeader;
            this.breachedEntryList = new KeePass.UI.CustomListViewEx();
            this.usernameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.urlHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lastModifiedHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.breachName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.breachedHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.closeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            titleHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // titleHeader
            // 
            titleHeader.Text = "Title";
            titleHeader.Width = 115;
            // 
            // breachedEntryList
            // 
            this.breachedEntryList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.breachedEntryList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            titleHeader,
            this.usernameHeader,
            this.urlHeader,
            this.lastModifiedHeader,
            this.breachName,
            this.breachedHeader});
            this.breachedEntryList.FullRowSelect = true;
            this.breachedEntryList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.breachedEntryList.Location = new System.Drawing.Point(12, 12);
            this.breachedEntryList.Name = "breachedEntryList";
            this.breachedEntryList.ShowGroups = false;
            this.breachedEntryList.Size = new System.Drawing.Size(719, 268);
            this.breachedEntryList.TabIndex = 0;
            this.breachedEntryList.UseCompatibleStateImageBehavior = false;
            this.breachedEntryList.View = System.Windows.Forms.View.Details;
            this.breachedEntryList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.breachedEntryList_MouseDoubleClick);
            // 
            // usernameHeader
            // 
            this.usernameHeader.Text = "Username";
            this.usernameHeader.Width = 136;
            // 
            // urlHeader
            // 
            this.urlHeader.Text = "URL";
            this.urlHeader.Width = 128;
            // 
            // lastModifiedHeader
            // 
            this.lastModifiedHeader.Text = "Password Changed";
            this.lastModifiedHeader.Width = 105;
            // 
            // breachName
            // 
            this.breachName.Text = "Breach Name";
            this.breachName.Width = 130;
            // 
            // breachedHeader
            // 
            this.breachedHeader.Text = "Breach Date";
            this.breachedHeader.Width = 80;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Location = new System.Drawing.Point(656, 286);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 291);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Double-click to edit breached entries";
            // 
            // BreachedEntriesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 317);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.breachedEntryList);
            this.MinimizeBox = false;
            this.Name = "BreachedEntriesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Breached Entries - {0} {1} Found";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private KeePass.UI.CustomListViewEx breachedEntryList;
        private System.Windows.Forms.ColumnHeader usernameHeader;
        private System.Windows.Forms.ColumnHeader urlHeader;
        private System.Windows.Forms.ColumnHeader lastModifiedHeader;
        private System.Windows.Forms.ColumnHeader breachedHeader;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader breachName;
    }
}
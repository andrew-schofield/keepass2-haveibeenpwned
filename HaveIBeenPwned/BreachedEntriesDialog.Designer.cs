namespace HaveIBeenPwned
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
            this.breachedEntryList = new System.Windows.Forms.ListView();
            this.usernameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.urlHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lastModifiedHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.breachedHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.closeButton = new System.Windows.Forms.Button();
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
            this.breachedHeader});
            this.breachedEntryList.FullRowSelect = true;
            this.breachedEntryList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.breachedEntryList.Location = new System.Drawing.Point(12, 12);
            this.breachedEntryList.Name = "breachedEntryList";
            this.breachedEntryList.ShowGroups = false;
            this.breachedEntryList.Size = new System.Drawing.Size(568, 268);
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
            // breachedHeader
            // 
            this.breachedHeader.Text = "Breach Date";
            this.breachedHeader.Width = 80;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Location = new System.Drawing.Point(505, 286);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // BreachedEntriesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 317);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.breachedEntryList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "BreachedEntriesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Breached Entries";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView breachedEntryList;
        private System.Windows.Forms.ColumnHeader usernameHeader;
        private System.Windows.Forms.ColumnHeader urlHeader;
        private System.Windows.Forms.ColumnHeader lastModifiedHeader;
        private System.Windows.Forms.ColumnHeader breachedHeader;
        private System.Windows.Forms.Button closeButton;
    }
}
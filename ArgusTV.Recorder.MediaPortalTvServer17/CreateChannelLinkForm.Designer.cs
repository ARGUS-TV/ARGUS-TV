namespace ArgusTV.Recorder.MediaPortalTvServer
{
    partial class CreateChannelLinkForm
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
            this.components = new System.ComponentModel.Container();
            this._channelLabel = new System.Windows.Forms.Label();
            this._channelNameLabel = new System.Windows.Forms.Label();
            this._linkLabel = new System.Windows.Forms.Label();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._clearLinkButton = new System.Windows.Forms.Button();
            this._groupsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._channelsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._groupsListView = new System.Windows.Forms.ListView();
            this._groupNameColumn = new System.Windows.Forms.ColumnHeader();
            this._lvImageList = new System.Windows.Forms.ImageList(this.components);
            this._channelsListView = new System.Windows.Forms.ListView();
            this._channelNameColumn = new System.Windows.Forms.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)(this._groupsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._channelsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // _channelLabel
            // 
            this._channelLabel.AutoSize = true;
            this._channelLabel.Location = new System.Drawing.Point(12, 15);
            this._channelLabel.Name = "_channelLabel";
            this._channelLabel.Size = new System.Drawing.Size(49, 13);
            this._channelLabel.TabIndex = 0;
            this._channelLabel.Text = "Channel:";
            // 
            // _channelNameLabel
            // 
            this._channelNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._channelNameLabel.AutoEllipsis = true;
            this._channelNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._channelNameLabel.Location = new System.Drawing.Point(67, 15);
            this._channelNameLabel.Name = "_channelNameLabel";
            this._channelNameLabel.Size = new System.Drawing.Size(365, 20);
            this._channelNameLabel.TabIndex = 1;
            // 
            // _linkLabel
            // 
            this._linkLabel.AutoSize = true;
            this._linkLabel.Location = new System.Drawing.Point(12, 41);
            this._linkLabel.Name = "_linkLabel";
            this._linkLabel.Size = new System.Drawing.Size(42, 13);
            this._linkLabel.TabIndex = 2;
            this._linkLabel.Text = "Link to:";
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.Location = new System.Drawing.Point(165, 327);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(85, 23);
            this._okButton.TabIndex = 4;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(347, 327);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(85, 23);
            this._cancelButton.TabIndex = 5;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _clearLinkButton
            // 
            this._clearLinkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._clearLinkButton.Location = new System.Drawing.Point(256, 327);
            this._clearLinkButton.Name = "_clearLinkButton";
            this._clearLinkButton.Size = new System.Drawing.Size(85, 23);
            this._clearLinkButton.TabIndex = 8;
            this._clearLinkButton.Text = "Remove Link";
            this._clearLinkButton.UseVisualStyleBackColor = true;
            this._clearLinkButton.Click += new System.EventHandler(this._clearLinkButton_Click);
            // 
            // _groupsListView
            // 
            this._groupsListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._groupsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._groupNameColumn});
            this._groupsListView.FullRowSelect = true;
            this._groupsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this._groupsListView.HideSelection = false;
            this._groupsListView.Location = new System.Drawing.Point(67, 38);
            this._groupsListView.MultiSelect = false;
            this._groupsListView.Name = "_groupsListView";
            this._groupsListView.Size = new System.Drawing.Size(365, 104);
            this._groupsListView.SmallImageList = this._lvImageList;
            this._groupsListView.TabIndex = 11;
            this._groupsListView.UseCompatibleStateImageBehavior = false;
            this._groupsListView.View = System.Windows.Forms.View.Details;
            this._groupsListView.SelectedIndexChanged += new System.EventHandler(this._groupNameListView_SelectedIndexChanged);
            // 
            // _groupNameColumn
            // 
            this._groupNameColumn.Text = "Channel Group";
            this._groupNameColumn.Width = 340;
            // 
            // _lvImageList
            // 
            this._lvImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this._lvImageList.ImageSize = new System.Drawing.Size(1, 18);
            this._lvImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // _channelsListView
            // 
            this._channelsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._channelsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._channelNameColumn});
            this._channelsListView.FullRowSelect = true;
            this._channelsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this._channelsListView.HideSelection = false;
            this._channelsListView.Location = new System.Drawing.Point(67, 148);
            this._channelsListView.MultiSelect = false;
            this._channelsListView.Name = "_channelsListView";
            this._channelsListView.Size = new System.Drawing.Size(365, 161);
            this._channelsListView.SmallImageList = this._lvImageList;
            this._channelsListView.TabIndex = 12;
            this._channelsListView.UseCompatibleStateImageBehavior = false;
            this._channelsListView.View = System.Windows.Forms.View.Details;
            this._channelsListView.DoubleClick += new System.EventHandler(this._channelsListView_DoubleClick);
            // 
            // _channelNameColumn
            // 
            this._channelNameColumn.Text = "MediaPortal Channel";
            this._channelNameColumn.Width = 340;
            // 
            // CreateChannelLinkForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(444, 359);
            this.Controls.Add(this._clearLinkButton);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._linkLabel);
            this.Controls.Add(this._channelNameLabel);
            this.Controls.Add(this._channelLabel);
            this.Controls.Add(this._groupsListView);
            this.Controls.Add(this._channelsListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateChannelLinkForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Channel Link";
            this.Load += new System.EventHandler(this.CreateShareForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._groupsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._channelsBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _channelLabel;
        private System.Windows.Forms.Label _channelNameLabel;
        private System.Windows.Forms.Label _linkLabel;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _clearLinkButton;
        private System.Windows.Forms.BindingSource _groupsBindingSource;
        private System.Windows.Forms.BindingSource _channelsBindingSource;
        private System.Windows.Forms.ListView _groupsListView;
        private System.Windows.Forms.ColumnHeader _groupNameColumn;
        private System.Windows.Forms.ListView _channelsListView;
        private System.Windows.Forms.ColumnHeader _channelNameColumn;
        private System.Windows.Forms.ImageList _lvImageList;
    }
}
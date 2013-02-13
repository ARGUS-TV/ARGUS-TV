namespace ArgusTV.WinForms.UserControls
{
    partial class ChannelGroupControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._channelTypeComboBox = new System.Windows.Forms.ComboBox();
            this._channelGroupsComboBox = new System.Windows.Forms.ComboBox();
            this._channelsLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _channelTypeComboBox
            // 
            this._channelTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._channelTypeComboBox.FormattingEnabled = true;
            this._channelTypeComboBox.Items.AddRange(new object[] {
            "Television",
            "Radio"});
            this._channelTypeComboBox.Location = new System.Drawing.Point(45, 0);
            this._channelTypeComboBox.Name = "_channelTypeComboBox";
            this._channelTypeComboBox.Size = new System.Drawing.Size(75, 21);
            this._channelTypeComboBox.TabIndex = 4;
            this._channelTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._channelTypeComboBox_SelectedIndexChanged);
            // 
            // _channelGroupsComboBox
            // 
            this._channelGroupsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._channelGroupsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._channelGroupsComboBox.FormattingEnabled = true;
            this._channelGroupsComboBox.Location = new System.Drawing.Point(126, 0);
            this._channelGroupsComboBox.Name = "_channelGroupsComboBox";
            this._channelGroupsComboBox.Size = new System.Drawing.Size(150, 21);
            this._channelGroupsComboBox.TabIndex = 5;
            this._channelGroupsComboBox.SelectedIndexChanged += new System.EventHandler(this._channelGroupsComboBox_SelectedIndexChanged);
            // 
            // _channelsLabel
            // 
            this._channelsLabel.AutoSize = true;
            this._channelsLabel.Location = new System.Drawing.Point(0, 4);
            this._channelsLabel.Name = "_channelsLabel";
            this._channelsLabel.Size = new System.Drawing.Size(39, 13);
            this._channelsLabel.TabIndex = 3;
            this._channelsLabel.Text = "Group:";
            // 
            // ChannelGroupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._channelTypeComboBox);
            this.Controls.Add(this._channelGroupsComboBox);
            this.Controls.Add(this._channelsLabel);
            this.Name = "ChannelGroupControl";
            this.Size = new System.Drawing.Size(276, 21);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _channelTypeComboBox;
        private System.Windows.Forms.ComboBox _channelGroupsComboBox;
        private System.Windows.Forms.Label _channelsLabel;
    }
}

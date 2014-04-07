namespace ArgusTV.UI.Console.UserControls
{
    partial class KeepUntilControl
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
            this._keepModeComboBox = new System.Windows.Forms.ComboBox();
            this._keepValueComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // _keepModeComboBox
            // 
            this._keepModeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._keepModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._keepModeComboBox.FormattingEnabled = true;
            this._keepModeComboBox.Items.AddRange(new object[] {
            "Until space is needed",
            "Days",
            "Most recent recordings",
            "Watched recordings",
            "Forever"});
            this._keepModeComboBox.Location = new System.Drawing.Point(49, 0);
            this._keepModeComboBox.Name = "_keepModeComboBox";
            this._keepModeComboBox.Size = new System.Drawing.Size(157, 21);
            this._keepModeComboBox.TabIndex = 2;
            this._keepModeComboBox.SelectedIndexChanged += new System.EventHandler(this._keepModeComboBox_SelectedIndexChanged);
            // 
            // _keepValueComboBox
            // 
            this._keepValueComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._keepValueComboBox.FormattingEnabled = true;
            this._keepValueComboBox.Location = new System.Drawing.Point(0, 0);
            this._keepValueComboBox.Name = "_keepValueComboBox";
            this._keepValueComboBox.Size = new System.Drawing.Size(45, 21);
            this._keepValueComboBox.TabIndex = 3;
            this._keepValueComboBox.SelectedIndexChanged += new System.EventHandler(this._keepValueComboBox_SelectedIndexChanged);
            // 
            // KeepUntilControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._keepValueComboBox);
            this.Controls.Add(this._keepModeComboBox);
            this.MaximumSize = new System.Drawing.Size(9999, 21);
            this.MinimumSize = new System.Drawing.Size(150, 21);
            this.Name = "KeepUntilControl";
            this.Size = new System.Drawing.Size(206, 21);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox _keepModeComboBox;
        private System.Windows.Forms.ComboBox _keepValueComboBox;
    }
}

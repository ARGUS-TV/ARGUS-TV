namespace ArgusTV.UI.Console.UserControls
{
    partial class SchedulePriorityControl
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
            this._schedulePriorityComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // _schedulePriorityComboBox
            // 
            this._schedulePriorityComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._schedulePriorityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._schedulePriorityComboBox.FormattingEnabled = true;
            this._schedulePriorityComboBox.Items.AddRange(new object[] {
            "Very low",
            "Low",
            "Normal",
            "High",
            "Very high"});
            this._schedulePriorityComboBox.Location = new System.Drawing.Point(0, 0);
            this._schedulePriorityComboBox.Name = "_schedulePriorityComboBox";
            this._schedulePriorityComboBox.Size = new System.Drawing.Size(150, 21);
            this._schedulePriorityComboBox.TabIndex = 2;
            // 
            // SchedulePriorityControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._schedulePriorityComboBox);
            this.MaximumSize = new System.Drawing.Size(9999, 21);
            this.MinimumSize = new System.Drawing.Size(50, 21);
            this.Name = "SchedulePriorityControl";
            this.Size = new System.Drawing.Size(150, 21);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox _schedulePriorityComboBox;
    }
}

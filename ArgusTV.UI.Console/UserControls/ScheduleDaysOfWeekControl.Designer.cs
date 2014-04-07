namespace ArgusTV.UI.Console.UserControls
{
    partial class ScheduleDaysOfWeekControl
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
            this._mondayCheckBox = new System.Windows.Forms.CheckBox();
            this._tuesdayCheckBox = new System.Windows.Forms.CheckBox();
            this._wednesdayCheckBox = new System.Windows.Forms.CheckBox();
            this._thursdayCheckBox = new System.Windows.Forms.CheckBox();
            this._fridayCheckBox = new System.Windows.Forms.CheckBox();
            this._saturdayCheckBox = new System.Windows.Forms.CheckBox();
            this._sundayCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _mondayCheckBox
            // 
            this._mondayCheckBox.AutoSize = true;
            this._mondayCheckBox.Location = new System.Drawing.Point(0, 3);
            this._mondayCheckBox.Name = "_mondayCheckBox";
            this._mondayCheckBox.Size = new System.Drawing.Size(41, 17);
            this._mondayCheckBox.TabIndex = 8;
            this._mondayCheckBox.Text = "Mo";
            this._mondayCheckBox.CheckedChanged += new System.EventHandler(this._checkBox_CheckedChanged);
            // 
            // _tuesdayCheckBox
            // 
            this._tuesdayCheckBox.AutoSize = true;
            this._tuesdayCheckBox.Location = new System.Drawing.Point(41, 3);
            this._tuesdayCheckBox.Name = "_tuesdayCheckBox";
            this._tuesdayCheckBox.Size = new System.Drawing.Size(39, 17);
            this._tuesdayCheckBox.TabIndex = 9;
            this._tuesdayCheckBox.Text = "Tu";
            this._tuesdayCheckBox.CheckedChanged += new System.EventHandler(this._checkBox_CheckedChanged);
            // 
            // _wednesdayCheckBox
            // 
            this._wednesdayCheckBox.AutoSize = true;
            this._wednesdayCheckBox.Location = new System.Drawing.Point(80, 3);
            this._wednesdayCheckBox.Name = "_wednesdayCheckBox";
            this._wednesdayCheckBox.Size = new System.Drawing.Size(43, 17);
            this._wednesdayCheckBox.TabIndex = 10;
            this._wednesdayCheckBox.Text = "We";
            this._wednesdayCheckBox.CheckedChanged += new System.EventHandler(this._checkBox_CheckedChanged);
            // 
            // _thursdayCheckBox
            // 
            this._thursdayCheckBox.AutoSize = true;
            this._thursdayCheckBox.Location = new System.Drawing.Point(123, 3);
            this._thursdayCheckBox.Name = "_thursdayCheckBox";
            this._thursdayCheckBox.Size = new System.Drawing.Size(39, 17);
            this._thursdayCheckBox.TabIndex = 11;
            this._thursdayCheckBox.Text = "Th";
            this._thursdayCheckBox.CheckedChanged += new System.EventHandler(this._checkBox_CheckedChanged);
            // 
            // _fridayCheckBox
            // 
            this._fridayCheckBox.AutoSize = true;
            this._fridayCheckBox.Location = new System.Drawing.Point(162, 3);
            this._fridayCheckBox.Name = "_fridayCheckBox";
            this._fridayCheckBox.Size = new System.Drawing.Size(35, 17);
            this._fridayCheckBox.TabIndex = 12;
            this._fridayCheckBox.Text = "Fr";
            this._fridayCheckBox.CheckedChanged += new System.EventHandler(this._checkBox_CheckedChanged);
            // 
            // _saturdayCheckBox
            // 
            this._saturdayCheckBox.AutoSize = true;
            this._saturdayCheckBox.Location = new System.Drawing.Point(197, 3);
            this._saturdayCheckBox.Name = "_saturdayCheckBox";
            this._saturdayCheckBox.Size = new System.Drawing.Size(39, 17);
            this._saturdayCheckBox.TabIndex = 13;
            this._saturdayCheckBox.Text = "Sa";
            this._saturdayCheckBox.CheckedChanged += new System.EventHandler(this._checkBox_CheckedChanged);
            // 
            // _sundayCheckBox
            // 
            this._sundayCheckBox.AutoSize = true;
            this._sundayCheckBox.Location = new System.Drawing.Point(236, 3);
            this._sundayCheckBox.Name = "_sundayCheckBox";
            this._sundayCheckBox.Size = new System.Drawing.Size(39, 17);
            this._sundayCheckBox.TabIndex = 14;
            this._sundayCheckBox.Text = "Su";
            this._sundayCheckBox.CheckedChanged += new System.EventHandler(this._checkBox_CheckedChanged);
            // 
            // ScheduleDaysOfWeekControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._mondayCheckBox);
            this.Controls.Add(this._tuesdayCheckBox);
            this.Controls.Add(this._wednesdayCheckBox);
            this.Controls.Add(this._thursdayCheckBox);
            this.Controls.Add(this._fridayCheckBox);
            this.Controls.Add(this._saturdayCheckBox);
            this.Controls.Add(this._sundayCheckBox);
            this.Name = "ScheduleDaysOfWeekControl";
            this.Size = new System.Drawing.Size(278, 22);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox _mondayCheckBox;
        private System.Windows.Forms.CheckBox _tuesdayCheckBox;
        private System.Windows.Forms.CheckBox _wednesdayCheckBox;
        private System.Windows.Forms.CheckBox _thursdayCheckBox;
        private System.Windows.Forms.CheckBox _fridayCheckBox;
        private System.Windows.Forms.CheckBox _saturdayCheckBox;
        private System.Windows.Forms.CheckBox _sundayCheckBox;

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

using ArgusTV.DataContracts;
using ArgusTV.UI.Process;

namespace ArgusTV.UI.Console.UserControls
{
    public partial class KeepUntilControl : UserControl
    {
        private int _suspendChangedEvent;

        public event EventHandler KeepUntilChanged;

        public KeepUntilControl()
        {
            InitializeComponent();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            _keepModeComboBox.Width = this.Width - _keepModeComboBox.Left;
        }

        public KeepUntilMode KeepUntilMode
        {
            get { return KeepUntilControlUtility.IndexToMode(_keepModeComboBox.SelectedIndex); }
        }

        public int? KeepUntilValue
        {
            get
            {
                if (_keepValueComboBox.SelectedValue != null)
                {
                    return (int)_keepValueComboBox.SelectedValue;
                }
                return null;
            }
        }

        public void SetKeepUntil(KeepUntilMode mode, int? value)
        {
            SetKeepUntil(mode, value, true);
        }

        public void ClearKeepUntil()
        {
            _suspendChangedEvent++;
            try
            {
                _keepModeComboBox.SelectedIndex = -1;
                _keepValueComboBox.DataSource = null;
                _keepValueComboBox.SelectedIndex = -1;
                _keepValueComboBox.Enabled = false;
            }
            finally
            {
                _suspendChangedEvent--;
            }
        }

        private void SetKeepUntil(KeepUntilMode mode, int? value, bool addValueIfNeeded)
        {
            _suspendChangedEvent++;
            try
            {
                DataTable table = KeepUntilControlUtility.CreateValueTable(mode, addValueIfNeeded ? value : null);
                _keepValueComboBox.DataSource = table;
                if (table.Rows.Count == 0)
                {
                    _keepValueComboBox.Enabled = false;
                }
                else
                {
                    _keepValueComboBox.Enabled = true;
                    _keepValueComboBox.DisplayMember = KeepUntilControlUtility.TextColumnName;
                    _keepValueComboBox.ValueMember = KeepUntilControlUtility.ValueColumnName;
                    _keepValueComboBox.SelectedIndex = KeepUntilControlUtility.GetIndexToSelect(table, value);
                }
                _keepModeComboBox.SelectedIndex = KeepUntilControlUtility.ModeToIndex(mode);
            }
            finally
            {
                _suspendChangedEvent--;
            }
        }

        private void _keepModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_keepModeComboBox.SelectedIndex >= 0)
            {
                SetKeepUntil(this.KeepUntilMode, this.KeepUntilValue, false);
                FireKeepUntilChangedEvent();
            }
        }

        private void _keepValueComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_keepValueComboBox.SelectedIndex >= 0)
            {
                FireKeepUntilChangedEvent();
            }
        }

        private void FireKeepUntilChangedEvent()
        {
            if (_suspendChangedEvent == 0
                && KeepUntilChanged != null)
            {
                KeepUntilChanged(this, EventArgs.Empty);
            }
        }
    }
}

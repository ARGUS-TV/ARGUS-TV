/*
 *	Copyright (C) 2007-2014 ARGUS TV
 *	http://www.argus-tv.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */
using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;
using ArgusTV.UI.Process;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace ArgusTV.UI.Console.Panels
{
    public partial class LogPanel : ContentPanel
    {
        private const int _maxLogEntries = 500;

        private string _logsFolderPath;

        public LogPanel()
        {
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Log"; }
        }

        private void LogPanel_Load(object sender, EventArgs e)
        {
            try
            {
                _logsFolderPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"ARGUS TV\Logs");
                _openLogsButton.Visible = Directory.Exists(_logsFolderPath);

                var proxy = new LogServiceProxy();
                FillModulesComboBox(proxy.GetAllModules());
                FillSeverityComboBox();
                SetDefaultSearchCriteria();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public override void OnClosed()
        {
            _logListControl.LogEntries = null;
        }

        private void SetDefaultSearchCriteria()
        {
            _startDatePicker.Value = DateTime.Today;
            _endDatePicker.Value = DateTime.Today;

            _moduleComboBox.Text = String.Empty;
            _severityComboBox.Text = String.Empty;

            _searchButton_Click(this, EventArgs.Empty);
        }

        private void FillModulesComboBox(IEnumerable<string> modules)
        {
            _moduleComboBox.Items.Clear();
            _moduleComboBox.Items.Add(String.Empty);
            foreach (string module in modules)
            {
                _moduleComboBox.Items.Add(module);
            }
        }

        private void FillSeverityComboBox()
        {
            _severityComboBox.Items.Clear();
            _severityComboBox.Items.Add(String.Empty);
            foreach(string severity in Enum.GetNames(typeof(LogSeverity)))
            {
                _severityComboBox.Items.Add(severity);
            }
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                string module = _moduleComboBox.Text.Length > 0 ? _moduleComboBox.Text : null;

                var proxy = new LogServiceProxy();

                LogSeverity? logSeverity = null;
                if (!String.IsNullOrEmpty(_severityComboBox.Text))
                {
                    logSeverity = (LogSeverity)Enum.Parse(typeof(LogSeverity), _severityComboBox.Text, true);
                }

                bool maxEntriesReached;
                var logEntries = proxy.GetLogEntries(_startDatePicker.Value.Date, _endDatePicker.Value.Date.AddDays(1), _maxLogEntries,
                    module, logSeverity, out maxEntriesReached);

                _maxEntriesReachedLabel.Visible = maxEntriesReached;
                if (maxEntriesReached)
                {
                    _maxEntriesReachedLabel.Text = String.Format(CultureInfo.CurrentCulture, "Warning: only the first {0} log messages are displayed, more are available.", logEntries.Count);
                }

                _logListControl.LogEntries = new SortableBindingList<LogEntry>(logEntries);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void _clearButton_Click(object sender, EventArgs e)
        {
            SetDefaultSearchCriteria();
        }

        private void _openLogsButton_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(_logsFolderPath);
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                startInfo.UseShellExecute = true;
                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

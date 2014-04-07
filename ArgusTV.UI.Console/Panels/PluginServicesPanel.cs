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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

using ArgusTV.DataContracts;
using ArgusTV.UI.Process;
using ArgusTV.WinForms;

namespace ArgusTV.UI.Console.Panels
{
    public partial class PluginServicesPanel : ContentPanel
    {
        private SortableBindingList<PluginService> _pluginServices;
        private HashSet<Guid> _changedServiceIds = new HashSet<Guid>();
        private List<PluginService> _deletedServices;

        public PluginServicesPanel()
        {
            InitializeComponent();
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_servicesDataGridView);
            _changedServiceIds.Add(Guid.Empty);
        }

        public override string Title
        {
            get { return "Recorder Plugins"; }
        }

        private void PluginServicesPanel_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                MainForm.SetMenuMode(MainMenuMode.SaveCancel);

                LoadAllPlugins();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public override void OnClosed()
        {
            MainForm.SetMenuMode(MainMenuMode.Normal);
            _servicesBindingSource.DataSource = null;
            _servicesBindingSource.ResetBindings(false);
        }

        private void LoadAllPlugins()
        {
            try
            {
                _deletedServices = new List<PluginService>();
                _pluginServices = new SortableBindingList<PluginService>(MainForm.ControlProxy.GetAllPluginServices(false));
                _servicesBindingSource.DataSource = _pluginServices;
                _servicesBindingSource.Sort = priorityDataGridViewTextBoxColumn.DataPropertyName +  " DESC";
                _servicesBindingSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private PluginService GetSelectedPluginService()
        {
            PluginService pluginService = null;
            if (_servicesDataGridView.SelectedRows.Count > 0)
            {
                pluginService = _servicesDataGridView.SelectedRows[0].DataBoundItem as PluginService;
            }
            return pluginService;
        }

        private void EnableButtons()
        {
            PluginService selectedPluginService = GetSelectedPluginService();
            _deleteButton.Enabled = (selectedPluginService != null);
            _pingButton.Enabled = (selectedPluginService != null && selectedPluginService.IsActive);
            _testSharesButton.Enabled = (selectedPluginService != null && selectedPluginService.IsActive);
        }

        private void _servicesDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(this, _servicesDataGridView.Columns[e.ColumnIndex].HeaderText + ": " + e.Exception.Message, null,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.ThrowException = false;
        }

        private void _servicesDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        public override void OnSave()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                foreach (PluginService pluginService in _pluginServices)
                {
                    if (_changedServiceIds.Contains(pluginService.PluginServiceId))
                    {
                        MainForm.ControlProxy.SavePluginService(pluginService);
                    }
                }

                foreach (PluginService pluginService in _deletedServices)
                {
                    if (pluginService.PluginServiceId != Guid.Empty)
                    {
                        MainForm.ControlProxy.DeletePluginService(pluginService.PluginServiceId);
                    }
                }

                _servicesBindingSource.RemoveSort();
                ClosePanel();
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

        public override void OnCancel()
        {
            _servicesBindingSource.RemoveSort();
            base.OnCancel();
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            PluginService pluginService = GetSelectedPluginService();
            if (pluginService != null)
            {
                _pluginServices.Remove(pluginService);
                _deletedServices.Add(pluginService);
            }
        }

        private void _createNewButton_Click(object sender, EventArgs e)
        {
            _createMenuStrip.Items.Clear();

            foreach (PluginServiceSetting setting in MainForm.PluginServiceSettings)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(setting.FullName);
                menuItem.Tag = setting;
                menuItem.Click += new EventHandler(CreateNewServiceItem_Click);
                _createMenuStrip.Items.Add(menuItem);
            }

            _createMenuStrip.Show(_createNewButton,
                new Point(_createNewButton.Width - 1, _createNewButton.Height - 1), ToolStripDropDownDirection.BelowLeft);
        }

        private void CreateNewServiceItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            PluginServiceSetting setting = (PluginServiceSetting)menuItem.Tag;

            PluginService pluginService = new PluginService();
            pluginService.Name = setting.Name;
            pluginService.IsActive = true;
            pluginService.ServiceInterface = "Recorder";
            pluginService.ServiceUrl = String.Format(CultureInfo.InvariantCulture, "http://localhost:{0}/ArgusTV/Recorder", setting.TcpPort);
            _servicesBindingSource.Add(pluginService);
            _servicesDataGridView.CurrentCell = _servicesDataGridView.Rows[_servicesDataGridView.Rows.Count - 1].Cells[1];
            _servicesDataGridView.BeginEdit(true);
        }

        private void _pingButton_Click(object sender, EventArgs e)
        {
            PluginService pluginService = GetSelectedPluginService();
            if (pluginService != null)
            {
                try
                {
                    MainForm.ControlProxy.PingPluginService(pluginService);
                    MessageBox.Show(this, String.Format(CultureInfo.InvariantCulture, "Server succesfully connected to {0}.", pluginService.Name),
                        _pingButton.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch(Exception ex)
                {
                    string message = ex.Message;
                    if (ex is ArgusTVUnexpectedErrorException
                        || !(ex is ArgusTVException))
                    {
                        message = String.Format(CultureInfo.InvariantCulture, "Server failed to connect to {0}.", pluginService.Name);
                    }
                    MessageBox.Show(this, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void _testSharesButton_Click(object sender, EventArgs e)
        {
            PluginService pluginService = GetSelectedPluginService();
            if (pluginService != null)
            {
                try
                {
                    string message;
                    var recordingShareAccessibilityInfo = MainForm.ControlProxy.AreRecordingSharesAccessible(pluginService);
                    if (recordingShareAccessibilityInfo.Count == 0)
                    {
                        message = String.Format("No shares defined on recorder {0}.", pluginService.Name);
                        MessageBox.Show(this, message, _testSharesButton.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        bool allOK = true;
                        StringBuilder sb = new StringBuilder();
                        foreach (RecordingShareAccessibilityInfo info in recordingShareAccessibilityInfo)
                        {
                            allOK &= info.ShareAccessible;
                            sb.Append(String.Format("{0}: {1}",
                                String.IsNullOrEmpty(info.Share) ? "(undefined)" : info.Share, info.ShareAccessible ? "OK" : "not OK" ) + Environment.NewLine );                            
                        }
                        message = String.Format("Shares defined on recorder {0} are {1}:", pluginService.Name, allOK ? "OK" : "not OK")
                            + Environment.NewLine + Environment.NewLine + sb.ToString();
                        MessageBox.Show(this, message, _testSharesButton.Text, MessageBoxButtons.OK,
                            allOK ? MessageBoxIcon.Information : MessageBoxIcon.Exclamation);
                    }                    
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    if (ex is ArgusTVUnexpectedErrorException
                        || !(ex is ArgusTVException))
                    {
                        message = String.Format(CultureInfo.InvariantCulture, "Server failed to connect to {0}.", pluginService.Name);
                    }
                    MessageBox.Show(this, message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void _servicesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var service = _servicesDataGridView.Rows[e.RowIndex].DataBoundItem as PluginService;
                _changedServiceIds.Add(service.PluginServiceId);
                EnableButtons();
            }
        }
    }
}

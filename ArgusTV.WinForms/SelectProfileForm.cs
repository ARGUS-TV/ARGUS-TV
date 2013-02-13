/*
 *	Copyright (C) 2007-2012 ARGUS TV
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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ArgusTV.ServiceAgents;
using ArgusTV.Client.Common;

namespace ArgusTV.WinForms
{
    public partial class SelectProfileForm : Form
    {
        public SelectProfileForm()
        {
            InitializeComponent();
        }

        private ConnectionProfile _selectedProfile;

        public ConnectionProfile SelectedProfile
        {
            get { return _selectedProfile; }
        }

        private bool _editSelectedProfile;

        public bool EditSelectedProfile
        {
            get { return _editSelectedProfile; }
        }

        private string _selectedProfileName;

        public void SetSelectedProfileName(string name)
        {
            _selectedProfileName = name;
        }

        private void SelectProfileForm_Load(object sender, EventArgs e)
        {
            ReloadProfiles();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.TopMost = true;
            this.TopMost = false;
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _selectedProfile = null;
            if (_profilesDataGridView.SelectedRows.Count > 0)
            {
                ConnectionProfile profile = _profilesDataGridView.SelectedRows[0].DataBoundItem as ConnectionProfile;
                if (profile.ServerSettings != null)
                {
                    _selectedProfile = profile;
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void _profilesDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (_profilesDataGridView.SelectedRows.Count > 0)
            {
                _okButton_Click(this, EventArgs.Empty);
            }
        }

        private void _profilesDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return
                || e.KeyCode == Keys.Enter)
            {
                _profilesDataGridView_DoubleClick(this, EventArgs.Empty);
            }
        }

        private void _profilesDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == _removeProfileColumn.Index)
            {
                IList<ConnectionProfile> list = (IList<ConnectionProfile>)_profilesBindingSource.DataSource;
                ConnectionProfiles.Remove(list[e.RowIndex].Name);
                ReloadProfiles();
            }
            else if (e.ColumnIndex == _editProfileColumn.Index)
            {
                _editSelectedProfile = true;
                _profilesDataGridView_DoubleClick(this, EventArgs.Empty);
            }
        }

        private void ReloadProfiles()
        {
            List<ConnectionProfile> list = new List<ConnectionProfile>(ConnectionProfiles.GetList());
            list.Add(new ConnectionProfile("New connection...", null, false));
            _profilesBindingSource.DataSource = list;
            _profilesBindingSource.ResetBindings(false);

            if (!String.IsNullOrEmpty(_selectedProfileName))
            {
                foreach (DataGridViewRow row in _profilesDataGridView.Rows)
                {
                    ConnectionProfile profile = row.DataBoundItem as ConnectionProfile;
                    if (profile.ServerSettings != null
                        && profile.Name == _selectedProfileName)
                    {
                        row.Selected = true;
                        break;
                    }
                }
            }
        }

        private void _profilesDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            IList<ConnectionProfile> list = (IList<ConnectionProfile>)_profilesBindingSource.DataSource;
            if (e.ColumnIndex >= _editProfileColumn.Index)
            {
                if (e.RowIndex < list.Count - 1)
                {
                    _profilesDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value =
                        (e.ColumnIndex == _editProfileColumn.Index) ? "Edit" : "Remove";
                }
            }
            else if (e.ColumnIndex == 0
                && e.RowIndex == list.Count - 1)
            {
                e.CellStyle.ForeColor = Color.Gray;
                e.CellStyle.SelectionForeColor = Color.Gray;
            }
        }

        private void _profilesDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                IList<ConnectionProfile> list = (IList<ConnectionProfile>)_profilesBindingSource.DataSource;
                if (e.RowIndex < list.Count - 1)
                {
                    Icon icon = null;
                    if (e.ColumnIndex == _editProfileColumn.Index)
                    {
                        icon = Properties.Resources.EditProfile;
                    }
                    else if (e.ColumnIndex == _removeProfileColumn.Index)
                    {
                        icon = Properties.Resources.RemoveProfile;
                    }
                    if (icon != null)
                    {
                        e.Paint(e.ClipBounds, e.PaintParts & ~DataGridViewPaintParts.ContentForeground);
                        Rectangle rect = new Rectangle(e.CellBounds.Left + (e.CellBounds.Width - icon.Width) / 2,
                            e.CellBounds.Top + (e.CellBounds.Height - icon.Height) / 2, icon.Width, icon.Height);
                        e.Graphics.DrawIconUnstretched(icon, rect);
                        e.Handled = true;
                    }
                }
            }
        }
    }
}

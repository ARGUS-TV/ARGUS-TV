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
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;
using ArgusTV.WinForms;

namespace ArgusTV.UI.Console
{
    public partial class SelectProcessingCommandsForm : Form
    {
        public SelectProcessingCommandsForm()
        {
            InitializeComponent();
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_commandsDataGridView);
        }

        private Schedule _schedule;

        public Schedule Schedule
        {
            get { return _schedule; }
            set { _schedule = value; }
        }

        [Obfuscation(Exclude = true)]
        private class CommandConfig
        {
            private bool _runCommand;

            public bool RunCommand
            {
                get { return _runCommand; }
                set { _runCommand = value; }
            }

            private Guid _processingCommandId;

            public Guid ProcessingCommandId
            {
                get { return _processingCommandId; }
                set { _processingCommandId = value; }
            }

            private string _name;

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }
        }

        private void SelectProcessingCommandsForm_Load(object sender, EventArgs e)
        {
            try
            {
                List<CommandConfig> commandConfigs = new List<CommandConfig>();

                var allCommands = Proxies.SchedulerService.GetAllProcessingCommands().Result;
                foreach (ProcessingCommand command in allCommands)
                {
                    CommandConfig commandConfig = new CommandConfig();
                    commandConfig.ProcessingCommandId = command.ProcessingCommandId;
                    commandConfig.Name = command.Name;
                    commandConfig.RunCommand = IsCommandInSchedule(_schedule, command.ProcessingCommandId);
                    commandConfigs.Add(commandConfig);
                }
                _commandsBindingSource.DataSource = commandConfigs;
                _commandsBindingSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsCommandInSchedule(Schedule schedule, Guid processingCommandId)
        {
            foreach (ScheduleProcessingCommand command in schedule.ProcessingCommands)
            {
                if (command.ProcessingCommandId == processingCommandId)
                {
                    return true;
                }
            }
            return false;
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _schedule.ProcessingCommands.Clear();
            foreach (DataGridViewRow row in _commandsDataGridView.Rows)
            {
                CommandConfig commandConfig = row.DataBoundItem as CommandConfig;
                if (commandConfig.RunCommand)
                {
                    _schedule.ProcessingCommands.Add(new ScheduleProcessingCommand()
                    {
                        ProcessingCommandId = commandConfig.ProcessingCommandId,
                        Name = commandConfig.Name
                    });
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

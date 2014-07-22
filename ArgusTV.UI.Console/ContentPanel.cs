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
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console
{
    public class ContentPanel : UserControl
    {
        public ContentPanel()
        {
            _eventsClientId = String.Format("{0}-{1}-d92663a4156c43b2ba5af09d38daf198", // Unique for the UI Console!
                Dns.GetHostName(), System.Environment.GetEnvironmentVariable("SESSIONNAME"));
        }

        private ContentPanel _ownerPanel;

        public ContentPanel OwnerPanel
        {
            get { return _ownerPanel; }
        }

        private DialogResult _dialogResult;

        public DialogResult DialogResult
        {
            get { return _dialogResult; }
            set { _dialogResult = value; }
        }

        public virtual string Title
        {
            get { return String.Empty; }
        }

        private MainForm _mainForm;

        public MainForm MainForm
        {
            get
            {
                if (_mainForm == null)
                {
                    Control control = this.Parent;
                    while (control != null)
                    {
                        MainForm mainForm = control as MainForm;
                        if (mainForm != null)
                        {
                            _mainForm = mainForm;
                            break;
                        }
                        control = control.Parent;
                    }
                }
                return _mainForm;
            }
        }

        public void OpenPanel(ContentPanel ownerPanel)
        {
            _ownerPanel = ownerPanel;
            _ownerPanel.MainForm.OpenContentPanel(this);
        }

        public void ClosePanel()
        {
            ClosePanel(DialogResult.None);
        }

        public void ClosePanel(DialogResult result)
        {
            this.DialogResult = result;
            this.MainForm.CloseContentPanel(this, this.OwnerPanel);
            if (this.OwnerPanel != null)
            {
                this.OwnerPanel.OnChildClosed(this);
            }
        }

        public virtual void OnClosed()
        {
        }

        public virtual void OnChildClosed(ContentPanel childPanel)
        {
        }

        public virtual void OnSave()
        {
        }

        public virtual void OnCancel()
        {
            ClosePanel(DialogResult.Cancel);
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                base.WndProc(ref m);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.MainForm, ex.Message, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ClosePanel(DialogResult.Abort);
            }
        }

        protected override void Dispose(bool disposing)
        {
            CancelListenerTask();
            base.Dispose(disposing);
        }

        #region Event Listener

        private string _eventsClientId;

        private SynchronizationContext _uiSyncContext;
        private bool _eventListenerSubscribed;
        private Task _listenerTask;
        private CancellationTokenSource _listenerCancellationTokenSource;

        protected void StartListenerTask(EventGroup eventGroups)
        {
            _uiSyncContext = SynchronizationContext.Current;

            _listenerCancellationTokenSource = new CancellationTokenSource();
            _listenerTask = new Task(() => HandleArgusTVEvents(_listenerCancellationTokenSource.Token),
                _listenerCancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            _listenerTask.Start();
        }

        private void CancelListenerTask()
        {
            try
            {
                if (_listenerCancellationTokenSource != null)
                {
                    _listenerCancellationTokenSource.Cancel();
                    _listenerTask.Wait();
                }
            }
            catch
            {
            }
            finally
            {
                if (_listenerTask != null)
                {
                    _listenerTask.Dispose();
                    _listenerTask = null;
                }
                if (_listenerCancellationTokenSource != null)
                {
                    _listenerCancellationTokenSource.Dispose();
                    _listenerCancellationTokenSource = null;
                }
            }
        }

        private void HandleArgusTVEvents(CancellationToken cancellationToken)
        {
            for (; ; )
            {
                if (Proxies.IsInitialized)
                {
                    IList<ServiceEvent> events = null;
                    if (!_eventListenerSubscribed)
                    {
                        try
                        {
                            Proxies.CoreService.SubscribeServiceEvents(_eventsClientId, EventGroup.RecordingEvents).Wait();
                            _eventListenerSubscribed = true;
                        }
                        catch
                        {
                        }
                    }
                    if (_eventListenerSubscribed)
                    {
                        try
                        {
                            events = Proxies.CoreService.GetServiceEvents(_eventsClientId, cancellationToken).Result;
                            if (events == null)
                            {
                                _eventListenerSubscribed = false;
                            }
                            else
                            {
                                ProcessEvents(events);
                            }
                        }
                        catch
                        {
                            _eventListenerSubscribed = false;
                        }
                    }
                }
                if (cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(_eventListenerSubscribed ? 0 : 10)))
                {
                    break;
                }
            }

            if (Proxies.IsInitialized
                && _eventListenerSubscribed)
            {
                try
                {
                    Proxies.CoreService.UnsubscribeServiceEvents(_eventsClientId).Wait();
                }
                catch
                {
                }
                _eventListenerSubscribed = false;
            }
        }

        private void ProcessEvents(IList<ServiceEvent> events)
        {
            foreach(var @event in events)
            {
                OnArgusTVEvent(_uiSyncContext, @event);
            }
        }

        protected virtual void OnArgusTVEvent(SynchronizationContext uiSyncContext, ServiceEvent @event)
        {
        }

        #endregion
    }
}

#region Copyright (C) 2005-2009 Team MediaPortal

/* 
 *	Copyright (C) 2005-2009 Team MediaPortal
 *	http://www.team-mediaportal.com
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

#endregion

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.Profile;
using Action = MediaPortal.GUI.Library.Action;

using ForTheRecord.ServiceAgents;
using ForTheRecord.ServiceContracts;
using ForTheRecord.Entities;

namespace ForTheRecord.UI.MediaPortal
{

    #region enum

    public enum TeletextButton
    {
        Red,
        Green,
        Yellow,
        Blue
    }

    #endregion

    /// <summary>
    /// Common class for both teletext windows
    /// </summary>
    public class TvTeletextBase : GUIWindow
    {
        #region gui controls

        [SkinControl(27)]
        protected GUILabelControl lblMessage = null;
        [SkinControl(500)]
        protected GUIImage imgTeletextForeground = null;
        [SkinControl(501)]
        protected GUIImage imgTeletextBackground = null;

        #endregion

        #region variables

        protected Bitmap bmpTeletextPage;
        protected string inputLine = String.Empty;
        protected int currentPageNumber = 0x100;
        protected int currentSubPageNumber;
        protected int receivedPageNumber = 0x100;
        protected int receivedSubPageNumber;
        protected TeletextPage _receivedTeletextPage;
        protected bool _updatingForegroundImage;
        protected bool _updatingBackgroundImage;
        protected bool _waiting;
        protected DateTime _startTime = DateTime.MinValue;
        protected TeletextPageRenderer _renderer = new TeletextPageRenderer();
        protected bool _hiddenMode;
        protected bool _transparentMode;
        protected Thread _updateThread;
        protected bool _updateThreadStop;
        protected int _numberOfRequestedUpdates;
        protected bool _rememberLastValues;
        protected int _percentageOfMaximumHeight;

        #endregion

        #region Property

        public override bool IsTv
        {
            get { return true; }
        }

        #endregion

        #region Initialization methods

        /// <summary>
        /// Initialize the window
        /// </summary>
        /// <param name="fullscreenMode">Indicate, if is fullscreen mode</param>
        protected void InitializeWindow(bool fullscreenMode)
        {
            LoadSettings();
            _numberOfRequestedUpdates = 0;
            // Create an update thread and set it's priority to lowest
            _updateThreadStop = false;
            _updateThread = new Thread(UpdatePage);
            _updateThread.Name = "Teletext Updater";
            _updateThread.Priority = ThreadPriority.BelowNormal;
            _updateThread.IsBackground = true;
            _updateThread.Start();
            lblMessage.Label = "";
            lblMessage.Visible = false;
            // Activate teletext grabbing in the server            
            ForTheRecordMain.Navigator.StartGrabbingTeletext();

            // Set the current page to the index page
            currentPageNumber = 0x100;
            currentSubPageNumber = 0;

            // Remember the start time
            _startTime = DateTime.MinValue;

            // Initialize the render
            _renderer = new TeletextPageRenderer();
            _renderer.TransparentMode = _transparentMode;
            _renderer.FullscreenMode = fullscreenMode;
            _renderer.HiddenMode = _hiddenMode;
            _renderer.PageSelectText = Convert.ToString(currentPageNumber, 16);
            _renderer.PercentageOfMaximumHeight = _percentageOfMaximumHeight;

            // Initialize the images
            if (imgTeletextForeground != null)
            {
                imgTeletextForeground.ColorKey = Color.HotPink.ToArgb();
                _renderer.Width = imgTeletextForeground.Width;
                _renderer.Height = imgTeletextForeground.Height;
            }
            if (imgTeletextBackground != null)
            {
                imgTeletextBackground.ColorKey = Color.HotPink.ToArgb();
                _renderer.Width = imgTeletextBackground.Width;
                _renderer.Height = imgTeletextBackground.Height;
            }
            // Load the mp logo page into teletext data array
            LoadLogoPage();
            // Request an update
            _numberOfRequestedUpdates++;
        }

        /// <summary>
        /// Loads the logo page from the assembly
        /// </summary>
        protected void LoadLogoPage()
        {
            Assembly assm = Assembly.GetExecutingAssembly();
            Stream stream = assm.GetManifestResourceStream("ForTheRecord.UI.MediaPortal.TeletextLogoPage");
            if (stream != null)
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    _receivedTeletextPage = new TeletextPage();
                    byte[] teletextPageBytes = new byte[stream.Length];
                    reader.Read(teletextPageBytes, 0, (int)stream.Length);
                    _receivedTeletextPage.Content = teletextPageBytes;
                    _receivedTeletextPage.PageNumber = 0;
                    _receivedTeletextPage.SubPageNumber = 0;
                }
            }
        }

        #endregion

        #region OnAction

        public override void OnAction(Action action)
        {
            // if we have a keypress or a remote button press then check if it is a number and add it to the inputLine
            char key = (char)0;
            if (action.wID == Action.ActionType.ACTION_KEY_PRESSED)
            {
                if (action.m_key != null)
                {
                    if (action.m_key.KeyChar >= '0' && action.m_key.KeyChar <= '9')
                    {
                        // Get offset to item
                        key = (char)action.m_key.KeyChar;
                    }
                }
                if (key == (char)0)
                {
                    return;
                }
                UpdateInputLine(key);
            }
            switch (action.wID)
            {
                case Action.ActionType.ACTION_REMOTE_RED_BUTTON:
                    // Red teletext button
                    showTeletextButtonPage(TeletextButton.Red);
                    break;
                case Action.ActionType.ACTION_REMOTE_GREEN_BUTTON:
                    // Green teletext button
                    showTeletextButtonPage(TeletextButton.Green);
                    break;
                case Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON:
                    // Yellow teletext button
                    showTeletextButtonPage(TeletextButton.Yellow);
                    break;
                case Action.ActionType.ACTION_REMOTE_BLUE_BUTTON:
                    // Blue teletext button
                    showTeletextButtonPage(TeletextButton.Blue);
                    break;
                case Action.ActionType.ACTION_REMOTE_SUBPAGE_UP:
                    // Subpage up
                    SubpageUp();
                    break;
                case Action.ActionType.ACTION_REMOTE_SUBPAGE_DOWN:
                    // Subpage down
                    SubpageDown();
                    break;
                case Action.ActionType.ACTION_NEXT_TELETEXTPAGE:
                    // Page up
                    PageUp();
                    break;
                case Action.ActionType.ACTION_PREV_TELETEXTPAGE:
                    // Page down
                    PageDown();
                    break;
                case Action.ActionType.ACTION_CONTEXT_MENU:
                    // Show previous window
                    GUIWindowManager.ShowPreviousWindow();
                    break;
                case Action.ActionType.ACTION_SWITCH_TELETEXT_HIDDEN:
                    //Change Hidden Mode
                    _hiddenMode = !_hiddenMode;
                    _renderer.HiddenMode = _hiddenMode;
                    // Rerender the image
                    _numberOfRequestedUpdates++;
                    break;
                case Action.ActionType.ACTION_SHOW_INDEXPAGE:
                    // Index page
                    showNewPage(0x100);
                    break;
            }
            base.OnAction(action);
        }

        #endregion

        #region Navigation methods

        /// <summary>
        /// Selects the next subpage, if possible
        /// </summary>
        protected void SubpageUp()
        {
            if (currentSubPageNumber < 0x79)
            {
                currentSubPageNumber++;
                while ((currentSubPageNumber & 0x0F) > 9)
                {
                    currentSubPageNumber++;
                }
                _numberOfRequestedUpdates++;
                Log.Info("dvb-teletext: select page {0:X} / subpage {1:X}", currentPageNumber, currentSubPageNumber);
            }
        }

        /// <summary>
        /// Selects the previous subpage, if possible
        /// </summary>
        protected void SubpageDown()
        {
            if (currentSubPageNumber > 0)
            {
                currentSubPageNumber--;
                while ((currentSubPageNumber % 0x0F) > 9)
                {
                    currentSubPageNumber--;
                }
                _numberOfRequestedUpdates++;
                Log.Info("dvb-teletext: select page {0:X} / subpage {1:X}", currentPageNumber, currentSubPageNumber);
            }
        }

        /// <summary>
        /// Selects the next page, if possible
        /// </summary>
        protected void PageUp()
        {
            if (currentPageNumber < 0x899)
            {
                currentPageNumber++;
                while ((currentPageNumber & 0x0F) > 9)
                {
                    currentPageNumber++;
                }
                while ((currentPageNumber & 0xF0) > 0x90)
                {
                    currentPageNumber += 16;
                }
                _renderer.PageSelectText = Convert.ToString(currentPageNumber, 16);
                currentSubPageNumber = 0;
                _numberOfRequestedUpdates++;
                Log.Info("dvb-teletext: select page {0:X} / subpage {1:X}", currentPageNumber, currentSubPageNumber);
                inputLine = "";
                return;
            }
        }

        /// <summary>
        /// Selects the previous subpage, if possible
        /// </summary>
        protected void PageDown()
        {
            if (currentPageNumber > 0x100)
            {
                currentPageNumber--;
                while ((currentPageNumber & 0xF0) > 0x90)
                {
                    currentPageNumber -= 16;
                }
                while ((currentPageNumber & 0x0F) > 9)
                {
                    currentPageNumber--;
                }
                _renderer.PageSelectText = Convert.ToString(currentPageNumber, 16);
                currentSubPageNumber = 0;
                _numberOfRequestedUpdates++;
                Log.Info("dvb-teletext: select page {0:X} / subpage {1:X}", currentPageNumber, currentSubPageNumber);
                inputLine = "";
                return;
            }
        }

        /// <summary>
        /// Updates the header and the selected page text
        /// </summary>
        /// <param name="key">Key</param>
        protected void UpdateInputLine(char key)
        {
            Log.Info("dvb-teletext: key received: " + key);
            if (inputLine.Length == 0 && (key == '0' || key == '9'))
            {
                return;
            }
            inputLine += key;
            _renderer.PageSelectText = inputLine;
            if (inputLine.Length == 3)
            {
                // change channel
                currentPageNumber = Convert.ToInt16(inputLine, 16);
                currentSubPageNumber = 0;
                if (currentPageNumber < 0x100)
                {
                    currentPageNumber = 0x100;
                }
                if (currentPageNumber > 0x899)
                {
                    currentPageNumber = 0x899;
                }
                _numberOfRequestedUpdates++;
                Log.Info("dvb-teletext: select page {0:X} / subpage {1:X}", currentPageNumber, currentSubPageNumber);
                inputLine = "";
            }
            _numberOfRequestedUpdates++;
        }

        /// <summary>
        /// Selects a teletext page, based on the teletext button
        /// </summary>
        /// <param name="button"></param>
        protected void showTeletextButtonPage(TeletextButton button)
        {
            if (_receivedTeletextPage != null)
            {
                switch (button)
                {
                    case TeletextButton.Red:
                        showNewPage(_receivedTeletextPage.RedPageNumber.Value);
                        break;
                    case TeletextButton.Green:
                        showNewPage(_receivedTeletextPage.GreenPageNumber.Value);
                        break;
                    case TeletextButton.Yellow:
                        showNewPage(_receivedTeletextPage.YellowPageNumber.Value);
                        break;
                    case TeletextButton.Blue:
                        showNewPage(_receivedTeletextPage.BluePageNumber.Value);
                        break;
                }
            }
        }

        /// <summary>
        /// Displays a new page, with the give page number
        /// </summary>
        /// <param name="hexPage">Page number (hexnumber)</param>
        protected void showNewPage(int? hexPage)
        {
            if (hexPage.HasValue && hexPage >= 0x100 && hexPage <= 0x899)
            {
                currentPageNumber = (int)hexPage;
                _renderer.PageSelectText = Convert.ToString(currentPageNumber, 16);
                currentSubPageNumber = 0;
                inputLine = "";
                _numberOfRequestedUpdates++;
                Log.Info("dvb-teletext: select page {0:X} / subpage {1:X}", currentPageNumber, currentSubPageNumber);
                return;
            }
        }

        #endregion

        #region Update, Process and Redraw

        /// <summary>
        /// Gets called by MP. Rotate the subpages and updates the pages
        /// </summary>
        public override void Process()
        {
            TimeSpan ts = DateTime.Now - _startTime;
            // Only every second, we check
            if (ts.TotalMilliseconds < 1000)
            {
                return;
            }
            // Still waiting for a page, then request an update again
            if (_waiting)
            {
                _numberOfRequestedUpdates++;
                _startTime = DateTime.Now;
                return;
            }
            // Check the rotation speed
            //TimeSpan tsRotation = TVHome.Card.TeletextRotation(currentPageNumber);

            TimeSpan tsRotation = new TimeSpan(0, 0, 10);
            // Should we rotate?
            if (ts.TotalMilliseconds < tsRotation.TotalMilliseconds)
            {
                return;
            }
            // Rotate --> Check the subpagenumber and update time variable
            _startTime = DateTime.Now;

            if (currentPageNumber < 0x100)
            {
                currentPageNumber = 0x100;
            }
            if (currentPageNumber > 0x899)
            {
                currentPageNumber = 0x899;
            }
            int NumberOfSubpages = _receivedTeletextPage == null ? 1 : _receivedTeletextPage.SubPageCount;
            if (currentSubPageNumber < NumberOfSubpages)
            {
                currentSubPageNumber++;
                while ((currentSubPageNumber & 0x0F) > 9)
                {
                    currentSubPageNumber++;
                }
            }
            if (currentSubPageNumber == NumberOfSubpages)
            {
                currentSubPageNumber = 0;
            }

            Log.Info("dvb-teletext page updated. {0:X}/{1:X} total:{2} rotspeed:{3}", currentPageNumber, currentSubPageNumber,
                     NumberOfSubpages, tsRotation.TotalMilliseconds);
            // Request the update
            _numberOfRequestedUpdates++;
        }

        /// <summary>
        /// Method of the update thread
        /// </summary>
        protected void UpdatePage()
        {
            // While not stop the thread, continue
            while (!_updateThreadStop)
            {
                // Is there an update request, than update
                if (_numberOfRequestedUpdates > 0 && !_updateThreadStop)
                {
                    GetNewPage();
                    _numberOfRequestedUpdates--;
                }
                else
                {
                    // Otherwise sleep for 300ms
                    Thread.Sleep(300);
                }
            }
        }

        /// <summary>
        /// Redraws the images
        /// </summary>
        protected void Redraw()
        {
            Log.Info("dvb-teletext redraw()");
            if (bmpTeletextPage != null)
            {
                try
                {
                    // First update the foreground image. Step 1 make it invisible
                    _updatingForegroundImage = true;
                    imgTeletextForeground.IsVisible = false;
                    // Clear the old image
                    Image img = (Image)bmpTeletextPage.Clone();
                    imgTeletextForeground.FileName = "";
                    GUITextureManager.ReleaseTexture("[teletextpage]");
                    // Set the new image and make the image visible again
                    imgTeletextForeground.MemoryImage = img;
                    imgTeletextForeground.FileName = "[teletextpage]";
                    imgTeletextForeground.Centered = false;
                    imgTeletextForeground.KeepAspectRatio = false;
                    imgTeletextForeground.IsVisible = true;
                    _updatingForegroundImage = false;
                    // Update the background image now. Therefor make image invisible
                    _updatingBackgroundImage = true;
                    imgTeletextBackground.IsVisible = false;
                    // Clear the old image
                    Image img2 = (Image)bmpTeletextPage.Clone();
                    imgTeletextBackground.FileName = "";
                    GUITextureManager.ReleaseTexture("[teletextpage2]");
                    // Set the new image and make the image visible again
                    imgTeletextBackground.MemoryImage = img2;
                    imgTeletextBackground.FileName = "[teletextpage2]";
                    imgTeletextBackground.Centered = false;
                    imgTeletextBackground.KeepAspectRatio = false;
                    imgTeletextBackground.IsVisible = true;
                    _updatingBackgroundImage = false;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        /// <summary>
        /// Retrieve the new page from the server
        /// </summary>
        protected void GetNewPage()
        {
            int sub = currentSubPageNumber;
            int maxSubs = _receivedTeletextPage == null ? 1 : _receivedTeletextPage.SubPageCount;
            // Check if the page is available
            if (maxSubs <= 0)
            {
                if (_receivedTeletextPage != null && _receivedTeletextPage.Content != null && !_waiting)
                {
                    bmpTeletextPage = _renderer.RenderPage(_receivedTeletextPage.Content, receivedPageNumber, receivedSubPageNumber);
                    Log.Info("dvb-teletext: received page {0:X} / subpage {1:X}", receivedPageNumber, receivedSubPageNumber);
                }
                Redraw();
                return;
            }
            if (sub >= maxSubs)
            {
                sub = maxSubs - 1;
            }
            // Get the page
            TeletextPage teletextPage = ForTheRecordMain.Navigator.GetTeletextPage(currentPageNumber, sub);

            // Was the page available, then render it. Otherwise render the last page again and update the header line, if
            // it was for the first time
            if (teletextPage != null)
            {
                _receivedTeletextPage = teletextPage;
                receivedPageNumber = currentPageNumber;
                receivedSubPageNumber = currentSubPageNumber;
                bmpTeletextPage = _renderer.RenderPage(_receivedTeletextPage.Content, _receivedTeletextPage.PageNumber, sub);
                _waiting = false;
                Log.Info("dvb-teletext: select page {0:X} / subpage {1:X}", currentPageNumber, currentSubPageNumber);
            }
            else
            {
                if (_receivedTeletextPage != null && _receivedTeletextPage.Content != null && !_waiting)
                {
                    bmpTeletextPage = _renderer.RenderPage(_receivedTeletextPage.Content, receivedPageNumber, receivedSubPageNumber);
                    Log.Info("dvb-teletext: received page {0:X} / subpage {1:X}", receivedPageNumber, receivedSubPageNumber);
                }
                _waiting = true;
            }
            Redraw();
        }

        #endregion

        #region Serialisation

        /// <summary>
        /// Load the settings
        /// </summary>
        protected void LoadSettings()
        {
            using (Settings xmlreader = new Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
            {
                _hiddenMode = xmlreader.GetValueAsBool("mytv", "teletextHidden", false);
                _transparentMode = xmlreader.GetValueAsBool("mytv", "teletextTransparent", false);
                _rememberLastValues = xmlreader.GetValueAsBool("mytv", "teletextRemember", true);
                _percentageOfMaximumHeight = xmlreader.GetValueAsInt("mytv", "teletextMaxFontSize", 100);
            }
        }

        /// <summary>
        /// Store the settings, if the user wants it
        /// </summary>
        protected void SaveSettings()
        {
            if (_rememberLastValues)
            {
                using (Settings xmlreader = new Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
                {
                    xmlreader.SetValueAsBool("mytv", "teletextHidden", _hiddenMode);
                    xmlreader.SetValueAsBool("mytv", "teletextTransparent", _transparentMode);
                }
            }
        }

        #endregion
    }
}
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ArgusTV.Recorder.MediaPortalTvServer
{
    public class ThreeStateTreeView : TreeView
    {
        public event EventHandler<TreeViewEventArgs> BeforeNodeStateChangeByUser;

        public static class ItemState
        {
            public const int Unchecked = 0;
            public const int Checked = 1;
            public const int Mixed = 2;
        }

        private ImageList _internalStateImageList;

        public void InitializeNodesState(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.StateImageIndex = ItemState.Unchecked;
                if (node.Nodes.Count != 0)
                {
                    InitializeNodesState(node.Nodes);
                }
            }
        }

        public void UpdateChildren(TreeNode parent)
        {
            int state = parent.StateImageIndex;
            foreach (TreeNode node in parent.Nodes)
            {
                node.StateImageIndex = state;
                if (node.Nodes.Count != 0)
                {
                    UpdateChildren(node);
                }
            }
        }

        public void UpdateParent(TreeNode child)
        {
            TreeNode parent = child.Parent;
            if (parent != null)
            {
                if (child.StateImageIndex == ItemState.Mixed)
                {
                    parent.StateImageIndex = ItemState.Mixed;
                }
                else if (IsChildrenChecked(parent))
                {
                    parent.StateImageIndex = ItemState.Checked;
                }
                else if (IsChildrenUnchecked(parent))
                {
                    parent.StateImageIndex = ItemState.Unchecked;
                }
                else
                {
                    parent.StateImageIndex = ItemState.Mixed;
                }
                UpdateParent(parent);
            }
        }

        public static bool IsChildrenChecked(TreeNode parent)
        {
            return IsAllChildrenSame(parent, ItemState.Checked);
        }

        public static bool IsChildrenUnchecked(TreeNode parent)
        {
            return IsAllChildrenSame(parent, ItemState.Unchecked);
        }

        public static bool IsAllChildrenSame(TreeNode parent, int state)
        {
            foreach (TreeNode node in parent.Nodes)
            {
                if (node.StateImageIndex != state)
                {
                    return false;
                }
                if (node.Nodes.Count != 0 && !IsAllChildrenSame(node, state))
                {
                    return false;
                }

            }
            return true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (_internalStateImageList == null)
            {
                _internalStateImageList = new ImageList();
                using (Graphics g = base.CreateGraphics())
                {
                    Size glyphSize = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.UncheckedNormal);
                    _internalStateImageList.Images.Add(GetStateImage(CheckBoxState.UncheckedNormal, glyphSize));
                    _internalStateImageList.Images.Add(GetStateImage(CheckBoxState.UncheckedNormal, glyphSize));
                    _internalStateImageList.Images.Add(GetStateImage(CheckBoxState.CheckedNormal, glyphSize));
                    _internalStateImageList.Images.Add(GetStateImage(CheckBoxState.MixedNormal, glyphSize));
                    _internalStateImageList.Images.Add(GetStateImage(CheckBoxState.UncheckedDisabled, glyphSize));
                    _internalStateImageList.Images.Add(GetStateImage(CheckBoxState.CheckedDisabled, glyphSize));
                    _internalStateImageList.Images.Add(GetStateImage(CheckBoxState.MixedDisabled, glyphSize));
                }
            }
            base.StateImageList = _internalStateImageList;

            InitializeNodesState(base.Nodes);
        }

        protected override void WndProc(ref Message m)
        {
            const int TV_FIRST = 0x1100;
            const int TVM_SETIMAGELIST = (TV_FIRST + 9);
            const int TVSIL_STATE = 2;

            if (m.Msg == TVM_SETIMAGELIST)
            {
                if (m.WParam.ToInt32() == TVSIL_STATE && m.LParam != IntPtr.Zero)
                {
                    // pass comctl the original
                    m.LParam = StateImageList.Handle;
                }
            }

            base.WndProc(ref m);
        }

        private static Image GetStateImage(CheckBoxState state, Size imageSize)
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Point pt = new Point((16 - imageSize.Width) / 2, (16 - imageSize.Height) / 2);
                CheckBoxRenderer.DrawCheckBox(g, pt, state);
            }
            return bmp;
        }

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseClick(e);
            if (e.Button == MouseButtons.Left)
            {
                TreeViewHitTestInfo info = base.HitTest(e.Location);
                if (info.Node != null && info.Location == TreeViewHitTestLocations.StateImage)
                {
                    ChangeNodeStateByUser(info.Node, TreeViewAction.ByMouse);
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Space)
            {
                if (base.SelectedNode != null)
                {
                    ChangeNodeStateByUser(base.SelectedNode, TreeViewAction.ByKeyboard);
                }
            }
        }

        public void SetNodeState(TreeNode node, int state)
        {
            node.StateImageIndex = (state == ItemState.Checked) ? ItemState.Unchecked : ItemState.Checked;
            ChangeNodeStateByUser(node, TreeViewAction.Unknown);
        }

        private void ChangeNodeStateByUser(TreeNode node, TreeViewAction action)
        {
            if (this.BeforeNodeStateChangeByUser != null)
            {
                this.BeforeNodeStateChangeByUser(this, new TreeViewEventArgs(node, action));
            }
            switch (node.StateImageIndex)
            {
                case ItemState.Unchecked:
                case ItemState.Mixed:
                    node.StateImageIndex = ItemState.Checked;
                    break;
                case ItemState.Checked:
                    node.StateImageIndex = ItemState.Unchecked;
                    break;
            }
            UpdateChildren(node);
            UpdateParent(node);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            // Swap between enabled and disabled images.
            for (int index = 0; index < 3; index++)
            {
                Image img = _internalStateImageList.Images[0];
                _internalStateImageList.Images.RemoveAt(0);
                _internalStateImageList.Images.Add(img);

            }
        }
    }
}

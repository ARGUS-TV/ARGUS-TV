/*
 *	Copyright (C) 2007-2013 ARGUS TV
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
using System.Windows.Forms;

namespace ArgusTV.WinForms.Controls
{
    public class MultiSelectTreeView : TreeView
    {
        protected List<TreeNode> _selectedNodes = new List<TreeNode>();
        protected TreeNode _firstNode;
        protected TreeNode _lastNode;

        public event EventHandler<EventArgs> SelectionChanged;

        public List<TreeNode> SelectedNodes
        {
            get
            {
                return _selectedNodes;
            }
            set
            {
                RemovePaintFromNodes();
                _selectedNodes.Clear();
                _selectedNodes = value;
                PaintSelectedNodes();
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            PaintSelectedNodes();
            base.OnLostFocus(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            PaintSelectedNodes();
            base.OnGotFocus(e);
        }

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            base.OnBeforeSelect(e);

            bool controlKeyDown = (Control.ModifierKeys == Keys.Control);
            bool shiftKeyDown = (Control.ModifierKeys == Keys.Shift);

            if (controlKeyDown
                && _selectedNodes.Contains(e.Node))
            {
                // Unselect it (let framework know we don't want selection this time)
                e.Cancel = true;

                // Update nodes
                e.Node.BackColor = this.BackColor;
                e.Node.ForeColor = this.ForeColor;
                _selectedNodes.Remove(e.Node);

                if (this.SelectionChanged != null)
                {
                    this.SelectionChanged(this, EventArgs.Empty);
                }
            }
            else
            {
                _lastNode = e.Node;
                if (!shiftKeyDown)
                {
                    _firstNode = e.Node;
                }
            }
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);

            bool controlKeyDown = (ModifierKeys == Keys.Control);
            bool shiftKeyDown = (ModifierKeys == Keys.Shift);

            if (controlKeyDown)
            {
                if (!_selectedNodes.Contains(e.Node)) // new node ?
                {
                    _selectedNodes.Add(e.Node);
                    PaintSelectedNodes();
                }
                else  // not new, remove it from the collection
                {
                    e.Node.BackColor = this.BackColor;
                    e.Node.ForeColor = this.ForeColor;
                    _selectedNodes.Remove(e.Node);
                }
            }
            else
            {
                // SHIFT is pressed
                if (shiftKeyDown)
                {
                    Queue<TreeNode> myQueue = new Queue<TreeNode>();

                    TreeNode uppernode = _firstNode;
                    TreeNode bottomnode = e.Node;
                    // case 1 : begin and end nodes are parent
                    bool isParent = IsParent(_firstNode, e.Node); // is m_firstNode parent (direct or not) of e.Node
                    if (!isParent)
                    {
                        isParent = IsParent(bottomnode, uppernode);
                        if (isParent) // swap nodes
                        {
                            TreeNode t = uppernode;
                            uppernode = bottomnode;
                            bottomnode = t;
                        }
                    }
                    if (isParent)
                    {
                        TreeNode node = bottomnode;
                        while (node != uppernode.Parent)
                        {
                            if (!_selectedNodes.Contains(node)) // new node ?
                            {
                                myQueue.Enqueue(node);
                            }
                            node = node.Parent;
                        }
                    }
                    // case 2 : nor the begin nor the end node are descendant one another
                    else
                    {
                        if ((uppernode.Parent == null && bottomnode.Parent == null) || (uppernode.Parent != null && uppernode.Parent.Nodes.Contains(bottomnode))) // are they siblings ?
                        {
                            int indexUpper = uppernode.Index;
                            int indexBottom = bottomnode.Index;
                            if (indexBottom < indexUpper) // reversed?
                            {
                                TreeNode t = uppernode;
                                uppernode = bottomnode;
                                bottomnode = t;
                                indexUpper = uppernode.Index;
                                indexBottom = bottomnode.Index;
                            }

                            TreeNode node = uppernode;
                            while (indexUpper <= indexBottom)
                            {
                                if (!_selectedNodes.Contains(node)) // new node ?
                                {
                                    myQueue.Enqueue(node);
                                }

                                node = node.NextNode;

                                indexUpper++;
                            }
                        }
                        else
                        {
                            if (!_selectedNodes.Contains(uppernode))
                            {
                                myQueue.Enqueue(uppernode);
                            }
                            if (!_selectedNodes.Contains(bottomnode))
                            {
                                myQueue.Enqueue(bottomnode);
                            }
                        }
                    }

                    _selectedNodes.AddRange(myQueue);

                    PaintSelectedNodes();
                    _firstNode = e.Node; // let us chain several SHIFTs if we like it
                }
                else
                {
                    // in the case of a simple click, just add this item
                    if (_selectedNodes != null && _selectedNodes.Count > 0)
                    {
                        RemovePaintFromNodes();
                        _selectedNodes.Clear();
                    }
                    _selectedNodes.Add(e.Node);
                }
            }
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, EventArgs.Empty);
            }
        }

        protected bool IsParent(TreeNode parentNode, TreeNode childNode)
        {
            if (parentNode == childNode)
            {
                return true;
            }

            TreeNode node = childNode;
            bool found = false;
            while (!found && node != null)
            {
                node = node.Parent;
                found = (node == parentNode);
            }
            return found;
        }

        protected void PaintSelectedNodes()
        {
            foreach (TreeNode n in _selectedNodes)
            {
                n.BackColor = this.Focused ? SystemColors.Highlight : SystemColors.Control;
                n.ForeColor = this.Focused ? SystemColors.HighlightText : SystemColors.ControlText;
            }
        }

        protected void RemovePaintFromNodes()
        {
            if (_selectedNodes.Count > 0)
            {
                foreach (TreeNode n in _selectedNodes)
                {
                    n.BackColor = this.BackColor;
                    n.ForeColor = this.ForeColor;
                }
            }
        }
    }
}

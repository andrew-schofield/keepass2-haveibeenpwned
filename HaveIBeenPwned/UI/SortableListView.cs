using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace HaveIBeenPwned.UI {

    /// <summary>
    /// A ListViewControl with multi column sort and clipboard support.
    /// </summary>
    public class SortableListView: ListView {

        #region Private Types

        [StructLayout(LayoutKind.Sequential)]
        private struct HDITEM {
            public Mask mask;
            public int cxy;
            [MarshalAs(UnmanagedType.LPTStr)] public string pszText;
            public IntPtr hbm;
            public int cchTextMax;
            public Format fmt;
            public IntPtr lParam;
            // _WIN32_IE >= 0x0300 
            public int iImage;
            public int iOrder;
            // _WIN32_IE >= 0x0500
            public uint type;
            public IntPtr pvFilter;
            // _WIN32_WINNT >= 0x0600
            public uint state;

            [Flags]
            public enum Mask {
                Format = 0x4,       // HDI_FORMAT
            };

            [Flags]
            public enum Format {
                SortDown = 0x200,   // HDF_SORTDOWN
                SortUp = 0x400,     // HDF_SORTUP
            };
        };

        #endregion

        #region Private Classes

        /// <summary>
        /// ListViewComparer for the SortableListView with number, DateTime and string comparison for multiple columns.
        /// </summary>
        private class SortableListViewComparer : System.Collections.IComparer {

            /// <summary>
            /// Dictionary with column indices as key and their sort order as value
            /// </summary>
            Dictionary<int, SortOrder> _dic = new Dictionary<int, SortOrder>();

            /// <summary>
            /// Compares two numeric values. One of them might be empty, but then the other one must be a numeric value.
            /// </summary>
            /// <param name="s1">First string of a numeric value.</param>
            /// <param name="s2">Second string of a numeric value.</param>
            /// <param name="i">A negative value if s1 is smaller than s2, zero if both are equal and a positive value if s1 is larger than s2.</param>
            /// <returns>True, if both strings can be converted to a numeric value, otherwise false.</returns>
            private bool CompareDouble(string s1, string s2, out int i) {
                bool b1, b2;
                double dbl1, dbl2;

                // Parse first string
                if(string.IsNullOrEmpty(s1)) {
                    b1 = true;
                    dbl1 = double.MinValue;
                } else
                    b1 = double.TryParse(s1, NumberStyles.Any, CultureInfo.CurrentCulture, out dbl1);

                // Parse second string
                if(string.IsNullOrEmpty(s2)) {
                    b2 = true;
                    dbl2 = double.MinValue;
                } else
                    b2 = double.TryParse(s2, NumberStyles.Any, CultureInfo.CurrentCulture, out dbl2);

                // Calculate result
                if(b1 && b2) {
                    i = dbl1.CompareTo(dbl2);
                    return true;
                } else {
                    i = 0;
                    return false;
                }
            }

            /// <summary>
            /// Compares two <see cref="DateTime"/> values. One of them might be empty, but then the other one must be a numeric value.
            /// </summary>
            /// <param name="s1">First string of a <see cref="DateTime"/> value.</param>
            /// <param name="s2">Second string of a <see cref="DateTime"/> value.</param>
            /// <param name="i">A negative value if s1 is smaller than s2, zero if both are equal and a positive value if s1 is larger than s2.</param>
            /// <returns>True, if both strings can be converted to <see cref="DateTime"/>, otherwise false.</returns>
            private bool CompareDateTime(string s1, string s2, out int i) {
                bool b1, b2;
                DateTime dt1, dt2;

                // Parse first string
                if(string.IsNullOrEmpty(s1)) {
                    b1 = true;
                    dt1 = DateTime.MinValue;
                } else
                    b1 = DateTime.TryParse(s1, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt1);

                // Parse second string
                if(string.IsNullOrEmpty(s2)) {
                    b2 = true;
                    dt2 = DateTime.MinValue;
                } else
                    b2 = DateTime.TryParse(s2, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt2);

                // Calculate result
                if(b1 && b2) {
                    i = dt1.CompareTo(dt2);
                    return true;
                } else {
                    i = 0;
                    return false;
                }
            }

            public SortableListViewComparer(Dictionary<int, SortOrder> dic) {
                _dic = dic;
            }

            /// <summary>
            /// Order of ListViewItems without sorting
            /// </summary>
            public List<ListViewItem> Unordered { get; set; }

            /// <summary>
            /// Compares two ListViewItems
            /// </summary>
            /// <param name="obj1">First ListViewItem</param>
            /// <param name="obj2">Second ListViewItem</param>
            /// <returns>A negative value if obj1 is smaller than obj2, zero if both are equal and a positive value if obj1 is larger than obj2.</returns>
            public int Compare(object obj1, object obj2) {
                int iRes;

                ListViewItem lvi1 = obj1 as ListViewItem;
                ListViewItem lvi2 = obj2 as ListViewItem;

                if(_dic.Count == 0 && this.Unordered != null) {
                    return this.Unordered.IndexOf(lvi1).CompareTo(this.Unordered.IndexOf(lvi2));
                } else {
                    foreach(int iCol in _dic.Keys) {
                        string s1 = iCol < lvi1.SubItems.Count ? s1 = lvi1.SubItems[iCol].Text : s1 = string.Empty;
                        string s2 = iCol < lvi2.SubItems.Count ? s2 = lvi2.SubItems[iCol].Text : s2 = string.Empty;
                        if(CompareDouble(s1, s2, out iRes)) {                               // Compare strings as numbers
                            if(iRes != 0)
                                return _dic[iCol] == SortOrder.Ascending ? iRes : -iRes;
                        } else if(CompareDateTime(s1, s2, out iRes)) {                      // Compare strings as DateTime
                            if(iRes != 0)
                                return _dic[iCol] == SortOrder.Ascending ? iRes : -iRes;
                        } else {                                                            // Compare strings
                            iRes = s1.CompareTo(s2);
                            if(iRes != 0)
                                return _dic[iCol] == SortOrder.Ascending ? iRes : -iRes;
                        }
                    }
                    return 0;
                }
            }
        }

        #endregion

        #region Private Constants

        private const int LVM_FIRST = 0x1000;
        private const int LVM_GETHEADER = LVM_FIRST + 31;

        private const int HDM_FIRST = 0x1200;
        private const int HDM_GETITEM = HDM_FIRST + 11;
        private const int HDM_SETITEM = HDM_FIRST + 12;
        
        #endregion 

        #region Private Declarations

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, ref HDITEM lParam);

        #endregion

        #region Private Variables

        /// <summary>
        /// Dictionary with column indices as key and their sort order as value
        /// </summary>
        Dictionary<int, SortOrder> _dic = new Dictionary<int, SortOrder>();

        /// <summary>
        /// Switch for hiding groups on sort
        /// </summary>
        private bool _bHideGroupsOnSort = true;

        /// <summary>
        /// Buffer for ShowGroups value during sort
        /// </summary>
        private bool _bShowGroups;

        /// <summary>
        /// Sets the sort icon of a column header via SendMessage
        /// </summary>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="order">SortOrder</param>
        private void SetSortIcon(int columnIndex, SortOrder order) {
            IntPtr columnHeader = SendMessage(this.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
            IntPtr columnPtr = new IntPtr(columnIndex);
            HDITEM item = new HDITEM { mask = HDITEM.Mask.Format };

            if(SendMessage(columnHeader, HDM_GETITEM, columnPtr, ref item) == IntPtr.Zero) throw new Win32Exception();

            switch(order) {
                    case SortOrder.Ascending:
                        item.fmt &= ~HDITEM.Format.SortDown;
                        item.fmt |= HDITEM.Format.SortUp;
                        break;
                    case SortOrder.Descending:
                        item.fmt &= ~HDITEM.Format.SortUp;
                        item.fmt |= HDITEM.Format.SortDown;
                        break;
                    case SortOrder.None:
                    item.fmt &= ~HDITEM.Format.SortDown & ~HDITEM.Format.SortUp;
                        break;
                }

            if(SendMessage(columnHeader, HDM_SETITEM, columnPtr, ref item) == IntPtr.Zero) throw new Win32Exception();
        }

        #endregion

        #region Constructors & Destructors

        public SortableListView() {
            this.ListViewItemSorter = new SortableListViewComparer(_dic);
        }

        #endregion

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);

            if(e.Control) {
                switch(e.KeyCode) {
                    case Keys.A:
                        BeginUpdate();
                        if(this.SelectedItems.Count == this.Items.Count)
                            foreach(ListViewItem lvi in this.Items)
                                lvi.Selected = false;
                        else {
                            foreach(ListViewItem lvi in this.Items)
                                lvi.Selected = true;
                        }
                        EndUpdate();
                        break;

                    case Keys.C:
                        if(this.SelectedItems.Count != 0) {
                            StringBuilder sb = new StringBuilder();
                            foreach(ListViewItem item in this.SelectedItems) {
                                foreach(ListViewItem.ListViewSubItem sub in item.SubItems)
                                    sb.Append(sub.Text + "\t");
                                sb.AppendLine();
                            }
                            Clipboard.SetText(sb.ToString());
                        }
                        break;
                }
            }
        }

        protected override void OnColumnClick(ColumnClickEventArgs e) {
            base.OnColumnClick(e);

            // Multiple sort colums with Ctrl-Key only
            if(!ModifierKeys.HasFlag(Keys.Control)) {
                foreach(int iCol in _dic.Keys.ToList())
                    if(iCol != e.Column) {
                        SetSortIcon(iCol, SortOrder.None);
                        _dic.Remove(iCol);
                    }
            }

            // Flip sort order of clicked column
            if(_dic.ContainsKey(e.Column)) {
                if(_dic[e.Column] == SortOrder.Ascending) {
                    _dic[e.Column] = SortOrder.Descending;
                    SetSortIcon(e.Column, SortOrder.Descending);
                } else {
                    _dic.Remove(e.Column);
                    SetSortIcon(e.Column, SortOrder.None);
                }
            } else {
                _dic[e.Column] = SortOrder.Ascending;
                SetSortIcon(e.Column, SortOrder.Ascending);
            }

            // Store unsorted row order on the first sort
            if(_dic.Count != 0) {
                if(((SortableListViewComparer)this.ListViewItemSorter).Unordered == null) {
                    ((SortableListViewComparer)this.ListViewItemSorter).Unordered = new List<ListViewItem>(this.Items.OfType<ListViewItem>());
                    _bShowGroups = this.ShowGroups;
                    if(_bHideGroupsOnSort)
                        this.ShowGroups = false;
                }
                Sort();
            } else {
                if(_bHideGroupsOnSort)
                    this.ShowGroups = _bShowGroups;
                Sort();
                ((SortableListViewComparer)this.ListViewItemSorter).Unordered = null;
            }
        }

        #region Public Properties

        /// <summary>
        /// True, if groups are hidden during sort, false if not.
        /// </summary>
        /// <remarks>
        /// This switch is independant of the <see cref="ShowGroups"/> value.
        /// </remarks>
        public bool HideGroupsOnSort {
            get { return _bHideGroupsOnSort; }
            set { if(_bHideGroupsOnSort != value) {
                    _bHideGroupsOnSort = value;
                    if(_dic.Count != 0) {
                        if(_bHideGroupsOnSort) {
                            _bShowGroups = this.ShowGroups;
                            if(this.ShowGroups) {
                                this.ShowGroups = false;
                                Sort();
                            }
                        } else {
                            if(this.ShowGroups != _bShowGroups) {
                                this.ShowGroups = _bShowGroups;
                                Sort();
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}

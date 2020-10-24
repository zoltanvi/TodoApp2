// Copyright (C) Josh Smith - January 2007
// Bugs fixed by zoltanvi
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WPF.JoshSmith.Adorners;
using WPF.JoshSmith.Controls.Utilities;

namespace WPF.JoshSmith.ServiceProviders.UI
{
    #region ListViewDragDropManager

    /// <summary>
    /// Manages the dragging and dropping of ListViewItems in a ListView.
    /// The ItemType type parameter indicates the type of the objects in
    /// the ListView's items source.  The ListView's ItemsSource must be
    /// set to an instance of ObservableCollection of ItemType, or an
    /// Exception will be thrown.
    /// </summary>
    /// <typeparam name="ItemType">The type of the ListView's items.</typeparam>
    public class ListViewDragDropManager<ItemType> where ItemType : class
    {
        #region Data

        private bool m_CanInitiateDrag;
        private DragAdorner m_DragAdorner;
        private double m_DragAdornerOpacity;
        private int m_IndexToSelect;
        private ItemType m_ItemUnderDragCursor;
        private ListView m_ListView;
        private Point m_PointMouseDown;
        private bool m_ShowDragAdorner;

        #endregion Data

        #region Constructors

        /// <summary>
        /// Initializes a new instance of ListViewDragManager.
        /// </summary>
        public ListViewDragDropManager()
        {
            m_CanInitiateDrag = false;
            m_DragAdornerOpacity = 0.7;
            m_IndexToSelect = -1;
            m_ShowDragAdorner = true;
        }

        /// <summary>
        /// Initializes a new instance of ListViewDragManager.
        /// </summary>
        /// <param name="listView"></param>
        public ListViewDragDropManager(ListView listView)
            : this()
        {
            ListView = listView;
        }

        /// <summary>
        /// Initializes a new instance of ListViewDragManager.
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="dragAdornerOpacity"></param>
        public ListViewDragDropManager(ListView listView, double dragAdornerOpacity)
            : this(listView)
        {
            DragAdornerOpacity = dragAdornerOpacity;
        }

        /// <summary>
        /// Initializes a new instance of ListViewDragManager.
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="showDragAdorner"></param>
        public ListViewDragDropManager(ListView listView, bool showDragAdorner)
            : this(listView)
        {
            ShowDragAdorner = showDragAdorner;
        }

        #endregion Constructors

        #region Public Interface

        #region DragAdornerOpacity

        /// <summary>
        /// Gets/sets the opacity of the drag adorner.  This property has no
        /// effect if ShowDragAdorner is false. The default value is 0.7
        /// </summary>
        public double DragAdornerOpacity
        {
            get { return m_DragAdornerOpacity; }
            set
            {
                if (IsDragInProgress)
                    throw new InvalidOperationException("Cannot set the DragAdornerOpacity property during a drag operation.");

                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException("DragAdornerOpacity", value, "Must be between 0 and 1.");

                m_DragAdornerOpacity = value;
            }
        }

        #endregion DragAdornerOpacity

        #region IsDragInProgress

        /// <summary>
        /// Returns true if there is currently a drag operation being managed.
        /// </summary>
        public bool IsDragInProgress { get; private set; }

        #endregion IsDragInProgress

        #region ListView

        /// <summary>
        /// Gets/sets the ListView whose dragging is managed.  This property
        /// can be set to null, to prevent drag management from occuring.  If
        /// the ListView's AllowDrop property is false, it will be set to true.
        /// </summary>
        public ListView ListView
        {
            get { return m_ListView; }
            set
            {
                if (IsDragInProgress)
                    throw new InvalidOperationException("Cannot set the ListView property during a drag operation.");

                if (m_ListView != null)
                {
                    #region Unhook Events

                    m_ListView.PreviewMouseLeftButtonDown -= listView_PreviewMouseLeftButtonDown;
                    m_ListView.PreviewMouseMove -= listView_PreviewMouseMove;
                    m_ListView.DragOver -= listView_DragOver;
                    m_ListView.DragLeave -= listView_DragLeave;
                    m_ListView.DragEnter -= listView_DragEnter;
                    m_ListView.Drop -= listView_Drop;

                    #endregion Unhook Events
                }

                m_ListView = value;

                if (m_ListView != null)
                {
                    if (!m_ListView.AllowDrop)
                        m_ListView.AllowDrop = true;

                    #region Hook Events

                    m_ListView.PreviewMouseLeftButtonDown += listView_PreviewMouseLeftButtonDown;
                    m_ListView.PreviewMouseMove += listView_PreviewMouseMove;
                    m_ListView.DragOver += listView_DragOver;
                    m_ListView.DragLeave += listView_DragLeave;
                    m_ListView.DragEnter += listView_DragEnter;
                    m_ListView.Drop += listView_Drop;

                    #endregion Hook Events
                }
            }
        }

        #endregion ListView

        #region ProcessDrop [event]

        /// <summary>
        /// Raised when a drop occurs.  By default the dropped item will be moved
        /// to the target index.  Handle this event if relocating the dropped item
        /// requires custom behavior.  Note, if this event is handled the default
        /// item dropping logic will not occur.
        /// </summary>
        public event EventHandler<ProcessDropEventArgs<ItemType>> ProcessDrop;

        #endregion ProcessDrop [event]

        #region ShowDragAdorner

        /// <summary>
        /// Gets/sets whether a visual representation of the ListViewItem being dragged
        /// follows the mouse cursor during a drag operation.  The default value is true.
        /// </summary>
        public bool ShowDragAdorner
        {
            get { return m_ShowDragAdorner; }
            set
            {
                if (IsDragInProgress)
                    throw new InvalidOperationException("Cannot set the ShowDragAdorner property during a drag operation.");

                m_ShowDragAdorner = value;
            }
        }

        #endregion ShowDragAdorner

        #endregion Public Interface

        #region Event Handling Methods

        #region listView_PreviewMouseLeftButtonDown

        private void listView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseOverScrollbar)
            {
                // 4/13/2007 - Set the flag to false when cursor is over scrollbar.
                m_CanInitiateDrag = false;
                return;
            }

            int index = IndexUnderDragCursor;
            m_CanInitiateDrag = index > -1;

            if (m_CanInitiateDrag)
            {
                // Remember the location and index of the ListViewItem the user clicked on for later.
                m_PointMouseDown = MouseUtilities.GetMousePosition(m_ListView);
                m_IndexToSelect = index;
            }
            else
            {
                m_PointMouseDown = new Point(-10000, -10000);
                m_IndexToSelect = -1;
            }
        }

        #endregion listView_PreviewMouseLeftButtonDown

        #region listView_PreviewMouseMove

        private void listView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!CanStartDragOperation)
                return;

            // Select the item the user clicked on.
            if (m_ListView.SelectedIndex != m_IndexToSelect)
            {
                m_ListView.SelectedIndex = m_IndexToSelect;
            }

            // If the item at the selected index is null, there's nothing
            // we can do, so just return;
            if (m_ListView.SelectedItem == null)
                return;

            ListViewItem itemToDrag = GetListViewItem(m_IndexToSelect);
            if (itemToDrag == null)
            {
                return;
            }

            AdornerLayer adornerLayer = ShowDragAdornerResolved ? InitializeAdornerLayer(itemToDrag) : null;

            InitializeDragOperation(itemToDrag);
            PerformDragOperation();
            FinishDragOperation(itemToDrag, adornerLayer);
        }

        #endregion listView_PreviewMouseMove

        #region listView_DragOver

        private void listView_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;

            if (ShowDragAdornerResolved)
                UpdateDragAdornerLocation();

            // Update the item which is known to be currently under the drag cursor.
            int index = IndexUnderDragCursor;
            ItemUnderDragCursor = index < 0 ? null : ListView.Items[index] as ItemType;
        }

        #endregion listView_DragOver

        #region listView_DragLeave

        private void listView_DragLeave(object sender, DragEventArgs e)
        {
            if (!IsMouseOver(m_ListView))
            {
                if (ItemUnderDragCursor != null)
                    ItemUnderDragCursor = null;

                if (m_DragAdorner != null)
                    m_DragAdorner.Visibility = Visibility.Collapsed;
            }
        }

        #endregion listView_DragLeave

        #region listView_DragEnter

        private void listView_DragEnter(object sender, DragEventArgs e)
        {
            if (m_DragAdorner != null && m_DragAdorner.Visibility != Visibility.Visible)
            {
                // Update the location of the adorner and then show it.
                UpdateDragAdornerLocation();
                m_DragAdorner.Visibility = Visibility.Visible;
            }
        }

        #endregion listView_DragEnter

        #region listView_Drop

        private void listView_Drop(object sender, DragEventArgs e)
        {
            if (ItemUnderDragCursor != null)
                ItemUnderDragCursor = null;

            e.Effects = DragDropEffects.None;

            object asd = e.Data.GetData(typeof(ItemType));

            if (!e.Data.GetDataPresent(typeof(ItemType)))
                return;

            // Get the data object which was dropped.
            ItemType data = e.Data.GetData(typeof(ItemType)) as ItemType;
            if (data == null)
                return;

            // Get the ObservableCollection<ItemType> which contains the dropped data object.
            ObservableCollection<ItemType> itemsSource = m_ListView.ItemsSource as ObservableCollection<ItemType>;
            if (itemsSource == null)
                throw new Exception(
                    "A ListView managed by ListViewDragManager must have its ItemsSource set to an ObservableCollection<ItemType>.");

            int oldIndex = itemsSource.IndexOf(data);
            int newIndex = IndexUnderDragCursor;

            if (newIndex < 0)
            {
                // The drag started somewhere else, and our ListView is empty
                // so make the new item the first in the list.
                if (itemsSource.Count == 0)
                    newIndex = 0;

                // The drag started somewhere else, but our ListView has items
                // so make the new item the last in the list.
                else if (oldIndex < 0)
                    newIndex = itemsSource.Count;

                // The user is trying to drop an item from our ListView into
                // our ListView, but the mouse is not over an item, so don't
                // let them drop it.
                else
                    return;
            }

            // Dropping an item back onto itself is not considered an actual 'drop'.
            if (oldIndex == newIndex)
                return;

            if (ProcessDrop != null)
            {
                // Let the client code process the drop.
                ProcessDropEventArgs<ItemType> args = new ProcessDropEventArgs<ItemType>(itemsSource, data, oldIndex, newIndex, e.AllowedEffects);
                ProcessDrop(this, args);
                e.Effects = args.Effects;
            }
            else
            {
                // Move the dragged data object from it's original index to the
                // new index (according to where the mouse cursor is).  If it was
                // not previously in the ListBox, then insert the item.
                if (oldIndex > -1)
                {
                    itemsSource.Move(oldIndex, newIndex);
                }
                else
                {
                    itemsSource.Insert(newIndex, data);
                }

                // Set the Effects property so that the call to DoDragDrop will return 'Move'.
                e.Effects = DragDropEffects.Move;
            }
        }

        #endregion listView_Drop

        #endregion Event Handling Methods

        #region Private Helpers

        #region CanStartDragOperation

        private bool CanStartDragOperation
        {
            get
            {
                if (Mouse.LeftButton != MouseButtonState.Pressed)
                    return false;

                if (!m_CanInitiateDrag)
                    return false;

                if (m_IndexToSelect == -1)
                    return false;

                if (!HasCursorLeftDragThreshold)
                    return false;

                return true;
            }
        }

        #endregion CanStartDragOperation

        #region FinishDragOperation

        private void FinishDragOperation(ListViewItem draggedItem, AdornerLayer adornerLayer)
        {
            // Let the ListViewItem know that it is not being dragged anymore.
            ListViewItemDragState.SetIsBeingDragged(draggedItem, false);

            IsDragInProgress = false;

            if (ItemUnderDragCursor != null)
                ItemUnderDragCursor = null;

            // Remove the drag adorner from the adorner layer.
            if (adornerLayer != null)
            {
                adornerLayer.Remove(m_DragAdorner);
                m_DragAdorner = null;
            }
        }

        #endregion FinishDragOperation

        #region GetListViewItem

        private ListViewItem GetListViewItem(int index)
        {
            if (index == -1 ||
                m_ListView.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated ||
                m_ListView.Items.Count == 0)
            {
                return null;
            }

            return m_ListView.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
        }

        private ListViewItem GetListViewItem(ItemType dataItem)
        {
            if (m_ListView.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;

            return m_ListView.ItemContainerGenerator.ContainerFromItem(dataItem) as ListViewItem;
        }

        #endregion GetListViewItem

        #region HasCursorLeftDragThreshold

        private bool HasCursorLeftDragThreshold
        {
            get
            {
                if (m_IndexToSelect < 0)
                {
                    return false;
                }

                ListViewItem item = GetListViewItem(m_IndexToSelect);

                if (item == null)
                {
                    return false;
                }

                Rect bounds = VisualTreeHelper.GetDescendantBounds(item);
                Point ptInItem = m_ListView.TranslatePoint(m_PointMouseDown, item);

                // In case the cursor is at the very top or bottom of the ListViewItem
                // we want to make the vertical threshold very small so that dragging
                // over an adjacent item does not select it.
                double topOffset = Math.Abs(ptInItem.Y);
                double btmOffset = Math.Abs(bounds.Height - ptInItem.Y);
                double vertOffset = Math.Min(topOffset, btmOffset);

                double width = SystemParameters.MinimumHorizontalDragDistance * 2;
                double height = Math.Min(SystemParameters.MinimumVerticalDragDistance, vertOffset) * 2;
                Size szThreshold = new Size(width, height);

                Rect rect = new Rect(m_PointMouseDown, szThreshold);
                rect.Offset(szThreshold.Width / -2, szThreshold.Height / -2);
                Point ptInListView = MouseUtilities.GetMousePosition(m_ListView);
                return !rect.Contains(ptInListView);
            }
        }

        #endregion HasCursorLeftDragThreshold

        #region IndexUnderDragCursor

        /// <summary>
        /// Returns the index of the ListViewItem underneath the
        /// drag cursor, or -1 if the cursor is not over an item.
        /// </summary>
        private int IndexUnderDragCursor
        {
            get
            {
                int index = -1;
                for (int i = 0; i < m_ListView.Items.Count; ++i)
                {
                    ListViewItem item = GetListViewItem(i);
                    if (IsMouseOver(item))
                    {
                        index = i;
                        break;
                    }
                }
                return index;
            }
        }

        #endregion IndexUnderDragCursor

        #region InitializeAdornerLayer

        private AdornerLayer InitializeAdornerLayer(ListViewItem itemToDrag)
        {
            // Create a brush which will paint the ListViewItem onto
            // a visual in the adorner layer.
            VisualBrush brush = new VisualBrush(itemToDrag);

            // Create an element which displays the source item while it is dragged.
            m_DragAdorner = new DragAdorner(m_ListView, itemToDrag.RenderSize, brush);

            // Set the drag adorner's opacity.
            m_DragAdorner.Opacity = DragAdornerOpacity;

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(m_ListView);
            layer.Add(m_DragAdorner);

            // Save the location of the cursor when the left mouse button was pressed.
            m_PointMouseDown = MouseUtilities.GetMousePosition(m_ListView);

            return layer;
        }

        #endregion InitializeAdornerLayer

        #region InitializeDragOperation

        private void InitializeDragOperation(ListViewItem itemToDrag)
        {
            // Set some flags used during the drag operation.
            IsDragInProgress = true;
            m_CanInitiateDrag = false;

            // Let the ListViewItem know that it is being dragged.
            ListViewItemDragState.SetIsBeingDragged(itemToDrag, true);
        }

        #endregion InitializeDragOperation

        #region IsMouseOver

        private bool IsMouseOver(Visual target)
        {
            // We need to use MouseUtilities to figure out the cursor
            // coordinates because, during a drag-drop operation, the WPF
            // mechanisms for getting the coordinates behave strangely.

            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            Point mousePos = MouseUtilities.GetMousePosition(target);
            return bounds.Contains(mousePos);
        }

        #endregion IsMouseOver

        #region IsMouseOverScrollbar

        /// <summary>
        /// Returns true if the mouse cursor is over a scrollbar in the ListView.
        /// </summary>
        private bool IsMouseOverScrollbar
        {
            get
            {
                Point ptMouse = MouseUtilities.GetMousePosition(m_ListView);
                HitTestResult res = VisualTreeHelper.HitTest(m_ListView, ptMouse);
                if (res == null)
                    return false;

                DependencyObject depObj = res.VisualHit;
                while (depObj != null)
                {
                    if (depObj is ScrollBar)
                        return true;

                    // VisualTreeHelper works with objects of type Visual or Visual3D.
                    // If the current object is not derived from Visual or Visual3D,
                    // then use the LogicalTreeHelper to find the parent element.
                    if (depObj is Visual || depObj is System.Windows.Media.Media3D.Visual3D)
                        depObj = VisualTreeHelper.GetParent(depObj);
                    else
                        depObj = LogicalTreeHelper.GetParent(depObj);
                }

                return false;
            }
        }

        #endregion IsMouseOverScrollbar

        #region ItemUnderDragCursor

        private ItemType ItemUnderDragCursor
        {
            get { return m_ItemUnderDragCursor; }
            set
            {
                if (m_ItemUnderDragCursor == value)
                    return;

                // The first pass handles the previous item under the cursor.
                // The second pass handles the new one.
                for (int i = 0; i < 2; ++i)
                {
                    if (i == 1)
                        m_ItemUnderDragCursor = value;

                    if (m_ItemUnderDragCursor != null)
                    {
                        ListViewItem listViewItem = GetListViewItem(m_ItemUnderDragCursor);
                        if (listViewItem != null)
                            ListViewItemDragState.SetIsUnderDragCursor(listViewItem, i == 1);
                    }
                }
            }
        }

        #endregion ItemUnderDragCursor

        #region PerformDragOperation

        private void PerformDragOperation()
        {
            ItemType focusedItem = GetFocusedItem();
            if (focusedItem != null)
            {
                DragDropEffects allowedEffects = DragDropEffects.Move | DragDropEffects.Link;
                if (DragDrop.DoDragDrop(m_ListView, focusedItem, allowedEffects) != DragDropEffects.None)
                {
                    // The item was dropped into a new location,
                    // so make it the new selected item.
                    m_ListView.SelectedItem = focusedItem;
                }
            }
        }

        /// <summary>
        /// Gets the focused item (selected item) from the list.
        /// Since the ListView selection works weird, this is a workaround for it.
        /// </summary>
        /// <returns>Returns the focused item.</returns>
        private ItemType GetFocusedItem()
        {
            ItemType focusedItem = null;
            foreach (object item in m_ListView.Items)
            {
                ListBoxItem lbi = (ListBoxItem)m_ListView.ItemContainerGenerator.ContainerFromItem(item);
                if (lbi.IsFocused)
                {
                    focusedItem = (ItemType)lbi.Content;
                    break;
                }
            }
            return focusedItem;
        }

        #endregion PerformDragOperation

        #region ShowDragAdornerResolved

        private bool ShowDragAdornerResolved
        {
            get { return ShowDragAdorner && DragAdornerOpacity > 0.0; }
        }

        #endregion ShowDragAdornerResolved

        #region UpdateDragAdornerLocation

        private void UpdateDragAdornerLocation()
        {
            if (m_DragAdorner != null)
            {
                Point ptCursor = MouseUtilities.GetMousePosition(ListView);

                double left = ptCursor.X - m_PointMouseDown.X;

                // 4/13/2007 - Made the top offset relative to the item being dragged.
                ListViewItem itemBeingDragged = GetListViewItem(m_IndexToSelect);
                Point itemLoc = itemBeingDragged.TranslatePoint(new Point(0, 0), ListView);
                double top = itemLoc.Y + ptCursor.Y - m_PointMouseDown.Y;

                m_DragAdorner.SetOffsets(left, top);
            }
        }

        #endregion UpdateDragAdornerLocation

        #endregion Private Helpers
    }

    #endregion ListViewDragDropManager

    #region ListViewItemDragState

    /// <summary>
    /// Exposes attached properties used in conjunction with the ListViewDragDropManager class.
    /// Those properties can be used to allow triggers to modify the appearance of ListViewItems
    /// in a ListView during a drag-drop operation.
    /// </summary>
    public static class ListViewItemDragState
    {
        #region IsBeingDragged

        /// <summary>
        /// Identifies the ListViewItemDragState's IsBeingDragged attached property.
        /// This field is read-only.
        /// </summary>
        public static readonly DependencyProperty IsBeingDraggedProperty =
            DependencyProperty.RegisterAttached(
                "IsBeingDragged",
                typeof(bool),
                typeof(ListViewItemDragState),
                new UIPropertyMetadata(false));

        /// <summary>
        /// Returns true if the specified ListViewItem is being dragged, else false.
        /// </summary>
        /// <param name="item">The ListViewItem to check.</param>
        public static bool GetIsBeingDragged(ListViewItem item)
        {
            return (bool)item.GetValue(IsBeingDraggedProperty);
        }

        /// <summary>
        /// Sets the IsBeingDragged attached property for the specified ListViewItem.
        /// </summary>
        /// <param name="item">The ListViewItem to set the property on.</param>
        /// <param name="value">Pass true if the element is being dragged, else false.</param>
        internal static void SetIsBeingDragged(ListViewItem item, bool value)
        {
            item.SetValue(IsBeingDraggedProperty, value);
        }

        #endregion IsBeingDragged

        #region IsUnderDragCursor

        /// <summary>
        /// Identifies the ListViewItemDragState's IsUnderDragCursor attached property.
        /// This field is read-only.
        /// </summary>
        public static readonly DependencyProperty IsUnderDragCursorProperty =
            DependencyProperty.RegisterAttached(
                "IsUnderDragCursor",
                typeof(bool),
                typeof(ListViewItemDragState),
                new UIPropertyMetadata(false));

        /// <summary>
        /// Returns true if the specified ListViewItem is currently underneath the cursor
        /// during a drag-drop operation, else false.
        /// </summary>
        /// <param name="item">The ListViewItem to check.</param>
        public static bool GetIsUnderDragCursor(ListViewItem item)
        {
            return (bool)item.GetValue(IsUnderDragCursorProperty);
        }

        /// <summary>
        /// Sets the IsUnderDragCursor attached property for the specified ListViewItem.
        /// </summary>
        /// <param name="item">The ListViewItem to set the property on.</param>
        /// <param name="value">Pass true if the element is underneath the drag cursor, else false.</param>
        internal static void SetIsUnderDragCursor(ListViewItem item, bool value)
        {
            item.SetValue(IsUnderDragCursorProperty, value);
        }

        #endregion IsUnderDragCursor
    }

    #endregion ListViewItemDragState

    #region ProcessDropEventArgs

    /// <summary>
    /// Event arguments used by the ListViewDragDropManager.ProcessDrop event.
    /// </summary>
    /// <typeparam name="ItemType">The type of data object being dropped.</typeparam>
    public class ProcessDropEventArgs<ItemType> : EventArgs where ItemType : class
    {
        #region Constructor

        internal ProcessDropEventArgs(
            ObservableCollection<ItemType> itemsSource,
            ItemType dataItem,
            int oldIndex,
            int newIndex,
            DragDropEffects allowedEffects)
        {
            ItemsSource = itemsSource;
            DataItem = dataItem;
            OldIndex = oldIndex;
            NewIndex = newIndex;
            AllowedEffects = allowedEffects;
        }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// The items source of the ListView where the drop occurred.
        /// </summary>
        public ObservableCollection<ItemType> ItemsSource { get; }

        /// <summary>
        /// The data object which was dropped.
        /// </summary>
        public ItemType DataItem { get; }

        /// <summary>
        /// The current index of the data item being dropped, in the ItemsSource collection.
        /// </summary>
        public int OldIndex { get; }

        /// <summary>
        /// The target index of the data item being dropped, in the ItemsSource collection.
        /// </summary>
        public int NewIndex { get; }

        /// <summary>
        /// The drag drop effects allowed to be performed.
        /// </summary>
        public DragDropEffects AllowedEffects { get; } = DragDropEffects.None;

        /// <summary>
        /// The drag drop effect(s) performed on the dropped item.
        /// </summary>
        public DragDropEffects Effects { get; set; } = DragDropEffects.None;

        #endregion Public Properties
    }

    #endregion ProcessDropEventArgs
}
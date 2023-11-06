using System;
using System.Collections.Generic;
using TodoApp2.Common;

namespace TodoApp2.Core
{
    public class Reorderer
    {
        private const long DefaultListOrder = CommonConstants.DefaultListOrder;
        private const long ListOrderInterval = CommonConstants.ListOrderInterval;

        /// <summary>
        /// Re-initializes the ListOrder property of each item in the list according to it's order in the list
        /// </summary>
        /// <param name="itemList"></param>
        public void ResetListOrders(IEnumerable<IReorderable> itemList)
        {
            long current = DefaultListOrder;

            // Update the ListOrder property of the IReorderable items
            foreach (var item in itemList)
            {
                item.ListOrder = current;
                current += ListOrderInterval;
            }
        }

        /// <summary>
        /// Reorders the given item in it's containing collection.
        /// </summary>
        /// <remarks>
        /// The order for every element in the collection may will be overwritten!
        /// The algorithm only modifies a single item until there is room between two item order.
        /// When there is no more room between two item order, every item in the collection gets a
        /// new number as it's order.
        /// </remarks>
        /// <param name="orderedItems">The collection without the item to reorder.</param>
        /// <param name="itemToReorder">The item to reorder.</param>
        /// <param name="newPosition">The item's new position in the collection.</param>
        /// <param name="updateStrategy">The action that is called when each element
        /// in the collection got a new order number. This action should persist the changes.</param>
        public void ReorderItem(List<IReorderable> orderedItems, IReorderable itemToReorder,
            int newPosition, Action<IEnumerable<IReorderable>> updateStrategy)
        {
            // If there is no other item besides the itemToReorder, set the default ListOrder
            if (orderedItems.Count == 0)
            {
                itemToReorder.ListOrder = DefaultListOrder;
                return;
            }

            // If the item moved to the top of the list, calculate the previous order for it
            if (newPosition == 0)
            {
                itemToReorder.ListOrder = GetPreviousListOrder(orderedItems[0].ListOrder);
            }
            // If the item moved to the end of the list, calculate the next order for it
            else if (orderedItems.Count == newPosition)
            {
                itemToReorder.ListOrder = GetNextListOrder(orderedItems[orderedItems.Count - 1].ListOrder);
            }
            // Else the ListOrder should be in the middle between the previous and next order interval
            else
            {
                long previousListOrder = orderedItems[newPosition - 1].ListOrder;
                long nextListOrder = orderedItems[newPosition].ListOrder;
                long newListOrder = previousListOrder + ((nextListOrder - previousListOrder) / 2);

                itemToReorder.ListOrder = newListOrder;

                // If there is no room between 2 existing order, reorder the whole list
                if (newListOrder == previousListOrder || newListOrder == nextListOrder)
                {
                    orderedItems.Insert(newPosition, itemToReorder);
                    ResetListOrders(orderedItems);
                    updateStrategy(orderedItems);
                }
            }
        }

        /// <summary>
        /// Gets the next available ListOrder relative to the provided one
        /// </summary>
        /// <param name="currentListOrder"></param>
        /// <returns></returns>
        public long GetNextListOrder(long currentListOrder)
        {
            return currentListOrder + ListOrderInterval;
        }

        /// <summary>
        /// Gets the previous available ListOrder relative to the provided one
        /// </summary>
        /// <param name="currentListOrder"></param>
        /// <returns></returns>
        public long GetPreviousListOrder(long currentListOrder)
        {
            return currentListOrder - ListOrderInterval;
        }

    }
}

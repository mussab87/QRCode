using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Utility.Toolkit
{
    public static class ListBoxExtension
    {
        public static IEnumerable<ListItem> GetSelectedItems(this ListItemCollection items)
        {
            return items.OfType<ListItem>().Where(item => item.Selected);
        }

        public static void RemoveSelectedItems(this ListBox listBox)
        {
            List<ListItem> itemsToRemove = new List<ListItem>();
            foreach (ListItem listItem in listBox.Items)
            {
                if (listItem.Selected)
                    itemsToRemove.Add(listItem);
            }

            foreach (ListItem listItem in itemsToRemove)
            {
                listBox.Items.Remove(listItem);
            }
        }
    }
}
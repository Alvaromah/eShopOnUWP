using System;

using eShop.UWP.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eShop.UWP.TemplateSelectors
{
    public class NavigationItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommonItem { get; set; }
        public DataTemplate SearchItem { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is ShellSearchItem)
            {
                return SearchItem;
            }
            return CommonItem;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is ShellNavigationItem navItem)
            {
                if (navItem.Label == "Search")
                {
                    return SearchItem;
                }
            }
            return CommonItem;
        }
    }
}

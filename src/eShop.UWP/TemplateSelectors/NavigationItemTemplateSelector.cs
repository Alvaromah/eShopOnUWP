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
        public DataTemplate LogoutItem { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is ShellSearchItem)
            {
                return SearchItem;
            }
            if (item is ShellLogoutItem)
            {
                return LogoutItem;
            }
            return CommonItem;
        }
    }
}

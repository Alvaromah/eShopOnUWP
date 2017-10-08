using System;

namespace eShop.UWP.Models
{
    public class ShellLogoutItem : ShellNavigationItem
    {
        public ShellLogoutItem(string name) : base(name)
        {
        }

        public override bool IsSelected { get => false; set { } }
    }
}

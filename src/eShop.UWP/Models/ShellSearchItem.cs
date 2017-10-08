using System;
using System.Windows.Input;

using GalaSoft.MvvmLight.Command;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eShop.UWP.Models
{
    public class ShellSearchItem : ShellNavigationItem
    {
        private Action<string> _onSearch = null;

        public ShellSearchItem(Action<string> onSearch) : base("Search", "x", null)
        {
            _onSearch = onSearch;
        }

        public override bool IsSelected { get => false; set { } }

        private Visibility _iconVisibility = Visibility.Collapsed;
        public Visibility IconVisibility
        {
            get => _iconVisibility;
            set => Set(ref _iconVisibility, value);
        }

        private string _query = null;
        public string Query
        {
            get => _query;
            set
            {
                Set(ref _query, value);
                if (String.IsNullOrEmpty(value))
                {
                    _onSearch?.Invoke(String.Empty);
                }
            }
        }

        private ICommand _searchCommand = null;
        public ICommand SearchCommand
        {
            get { return _searchCommand ?? (_searchCommand = new RelayCommand<AutoSuggestBoxQuerySubmittedEventArgs>(OnSearch)); }
        }

        private void OnSearch(AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _onSearch?.Invoke(args.QueryText);
        }
    }
}

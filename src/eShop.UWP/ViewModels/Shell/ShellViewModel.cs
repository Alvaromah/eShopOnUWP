using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using GalaSoft.MvvmLight.Command;

using eShop.Cortana;
using eShop.Providers.Contracts;
using eShop.UWP.Helpers;
using eShop.UWP.Models;
using eShop.UWP.ViewModels.Base;
using eShop.UWP.ViewModels.Catalog;
using eShop.UWP.ViewModels.Login;

namespace eShop.UWP.ViewModels.Shell
{
    public class ShellViewModel : CustomViewModelBase
    {
        private readonly CatalogViewModel _catalogViewModel;
        private readonly ICatalogProvider _catalogProvider;

        private ShellNavigationItem _selectedItem;
        private ShellNavigationItem _lastSelectedItem;
        private bool _isPaneOpen = true;
        private ObservableCollection<ShellNavigationItem> _primaryItems = new ObservableCollection<ShellNavigationItem>();
        private ObservableCollection<ShellNavigationItem> _secondaryItems = new ObservableCollection<ShellNavigationItem>();

        public ShellViewModel(CatalogViewModel catalogViewModel, ICatalogProvider catalogProvider)
        {
            _catalogViewModel = catalogViewModel;
            _catalogProvider = catalogProvider;
        }

        public string UserName => ApplicationData.Current.LocalSettings.Values[Constants.HelloUserIdKey] as string ?? string.Empty;


        public string Query
        {
            get => _shellSearchItem.Query;
            set
            {
                _shellSearchItem.Query = value;

                if (string.IsNullOrEmpty(value))
                {
                    _catalogViewModel.Search(string.Empty);
                }
            }
        }

        public ShellNavigationItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                Set(ref _selectedItem, value);
                if (value != null)
                {
                    if (!(value is ShellSearchItem))
                    {
                        Navigate(value);
                    }
                }
            }
        }

        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set
            {
                Set(ref _isPaneOpen, value);
                _shellSearchItem.IconVisibility = value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public ObservableCollection<ShellNavigationItem> PrimaryItems
        {
            get => _primaryItems;
            set => Set(ref _primaryItems, value);
        }

        public ObservableCollection<ShellNavigationItem> SecondaryItems
        {
            get => _secondaryItems;
            set => Set(ref _secondaryItems, value);
        }

        public ICommand LoadedCommand => new RelayCommand(SetInitialPage);

        public ICommand LogoutCommand => new RelayCommand(Logout);

        public ICommand OpenPaneCommand => new RelayCommand(() => IsPaneOpen = !_isPaneOpen);

        public ICommand ItemClickCommand => new RelayCommand<ItemClickEventArgs>((args) =>
        {
            if (args.ClickedItem is ShellSearchItem)
            {
                IsPaneOpen = !_isPaneOpen;
                SelectedItem = _shellDefaultItem;
            }
            else if (args.ClickedItem is ShellLogoutItem)
            {
                Logout();
            }
        });

        public void Initialize(Frame frame)
        {
            NavigationService.Frame.BackStack.Clear();
            NavigationService.Frame = frame;
            NavigationService.Frame.Navigated += NavigationServiceOnNavigated;
            PopulateNavItems();
        }

        public async Task UpdateCatalogTypePhraseList()
        {
            try
            {
                string countryCode = CultureInfo.CurrentCulture.Name.ToLower();
                if (countryCode.Length == 0)
                {
                    countryCode = Constants.DefaultCultureInfoName;
                }

                var commandSetName = string.Format(Constants.CortanaCommandSetName, countryCode);
                if (VoiceCommandDefinitionManager.InstalledCommandDefinitions.TryGetValue(commandSetName, out VoiceCommandDefinition commandDefinitions))
                {
                    var catalogTypes = _catalogProvider.GetCatalogTypes()?.Select(type => type.Type).ToList();
                    await commandDefinitions.SetPhraseListAsync(Constants.CortanaPhraseListName, catalogTypes);

                    Debug.WriteLine("Updating Phrase list for VCDs");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating Phrase list for VCDs: {ex.ToString()}");
            }
        }

        private void SetInitialPage()
        {
            if (Parameter != null && Parameter is CatalogVoiceCommand)
            {
                NavigationService.Navigate(typeof(ItemDetailViewModel).FullName, Parameter);
                Parameter = null;
            }
            else
            {
                SelectedItem = _shellDefaultItem;
            }
        }

        private ShellSearchItem _shellSearchItem = null;
        private ShellNavigationItem _shellDefaultItem = null;

        private void PopulateNavItems()
        {
            _primaryItems.Clear();
            _secondaryItems.Clear();

            _shellSearchItem = new ShellSearchItem(OnSearch);
            _shellDefaultItem = new ShellNavigationItem(Constants.ShellCatalogKey.GetLocalized(), Application.Current.Resources["CatalogIcon"] as string, typeof(CatalogViewModel).FullName);

            _primaryItems.Add(_shellSearchItem);
            _primaryItems.Add(_shellDefaultItem);
            _primaryItems.Add(new ShellNavigationItem(Constants.ShellStatisticsKey.GetLocalized(), Application.Current.Resources["StatisticsIcon"] as string, typeof(StatisticsViewModel).FullName));
            _primaryItems.Add(new ShellNavigationItem(Constants.ShellAddItemKey.GetLocalized(), Application.Current.Resources["AddNewItemIcon"] as string, typeof(ItemDetailViewModel).FullName));

            _secondaryItems.Add(new ShellLogoutItem(UserName));
        }

        private void OnSearch(string query)
        {
            _catalogViewModel.Search(Query ?? String.Empty);
            SelectedItem = _shellDefaultItem;
        }

        private void NavigationServiceOnNavigated(object sender, NavigationEventArgs e)
        {
            if (e != null)
            {
                var vm = NavigationService.GetNameOfRegisteredPage(e.SourcePageType);
                var navigationItem = PrimaryItems?.FirstOrDefault(i => i.ViewModelName == vm);

                if (navigationItem == null)
                {
                    navigationItem = SecondaryItems?.FirstOrDefault(i => i.ViewModelName == vm);
                }

                if (navigationItem != null)
                {
                    ChangeSelected(_lastSelectedItem, navigationItem);
                    _lastSelectedItem = navigationItem;
                }

                SelectedItem = navigationItem;
            }
        }

        private void ChangeSelected(ShellNavigationItem oldValue, ShellNavigationItem newValue)
        {
            if (oldValue != null)
            {
                oldValue.IsSelected = false;
            }
            if (newValue != null)
            {
                newValue.IsSelected = true;
            }
        }

        private void Navigate(object item)
        {
            var navigationItem = item as ShellNavigationItem;

            if (navigationItem != null)
            {
                NavigationService.Navigate(navigationItem.ViewModelName);
            }

            if (navigationItem == null || !navigationItem.ViewModelName.Equals(typeof(CatalogViewModel).FullName) && !string.IsNullOrEmpty(Query))
            {
                Query = string.Empty;
            }
        }

        private void Logout()
        {
            CleanBackStack();
            NavigationService.Frame = Window.Current.Content as Frame;
            NavigationService.Navigate(typeof(LoginViewModel).FullName);
        }
    }
}

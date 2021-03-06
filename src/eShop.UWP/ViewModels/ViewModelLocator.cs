﻿using System;

using Microsoft.Practices.ServiceLocation;

using GalaSoft.MvvmLight.Ioc;

using eShop.UWP.Views;
using eShop.UWP.Services;
using eShop.Providers;
using eShop.Cortana;

namespace eShop.UWP.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register(() => new NavigationServiceEx());
            SimpleIoc.Default.Register<ICatalogProvider, CatalogProvider>();
            SimpleIoc.Default.Register<IOrdersProvider, OrdersProvider>();
            SimpleIoc.Default.Register<VoiceCommandService>();

            Register<LoginViewModel, LoginView>();
            Register<ShellViewModel, ShellView>();
            Register<CatalogViewModel, CatalogView>();
            Register<ItemsGridViewModel, ItemsGridView>();
            Register<ItemsListViewModel, ItemsListView>();
            Register<ItemDetailViewModel, ItemDetailView>();
            Register<SettingsViewModel, SettingsView>();

            Register<StatisticsViewModel, StatisticsView>();
        }

        public NavigationServiceEx NavigationService => ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        public ShellViewModel ShellViewModel => ServiceLocator.Current.GetInstance<ShellViewModel>();

        public LoginViewModel LoginViewModel => ServiceLocator.Current.GetInstance<LoginViewModel>();
        public CatalogViewModel CatalogViewModel => ServiceLocator.Current.GetInstance<CatalogViewModel>();
        public ItemsGridViewModel ItemsGridViewModel => ServiceLocator.Current.GetInstance<ItemsGridViewModel>();
        public ItemsListViewModel ItemsListViewModel => ServiceLocator.Current.GetInstance<ItemsListViewModel>();
        public ItemDetailViewModel ItemDetailViewModel => ServiceLocator.Current.GetInstance<ItemDetailViewModel>();

        public StatisticsViewModel StatisticsViewModel => ServiceLocator.Current.GetInstance<StatisticsViewModel>();
        public SettingsViewModel SettingsViewModel => ServiceLocator.Current.GetInstance<SettingsViewModel>();

        public void Register<VM, V>() where VM : class
        {
            SimpleIoc.Default.Register<VM>();

            NavigationService.Configure(typeof(VM).FullName, typeof(V));
        }
    }
}

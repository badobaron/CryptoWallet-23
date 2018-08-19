﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using NBitcoin;
using Wallet.Core.CoinProviders;
using Wallet.Core.CredentialService;
using Wallet.Presentation.Commands;
using Wallet.Presentation.Model;
using Wallet.Presentation.View.Pages;

namespace Wallet.Presentation.ViewModel
{
    public class BaseViewModel : ObservableObject
    {
        public WalletModel WalletModel { get; set; }
        public ICredentialService CredentialService { get; set; }

        public BaseViewModel(ICredentialService service)
        {
            CredentialService = service;
            WalletModel = new WalletModel();
        }

        public BaseViewModel()
        {
            WalletModel = new WalletModel();
            WalletModel.Mainnet = true;
        }

        public ICommand OpenCreateAccount => new DelegateCommand(OpenCreateAccountPage);
        public ICommand OpenLogin => new DelegateCommand(OpenLoginPage);
        public ICommand LoginCommand => new RelayCommand<Account>(Login);
        public ICommand CreateAccountCommand => new RelayCommand<NewAccount>(CreateAccount);

        private void Login(Account account)
        {
            try
            {
                var mnemonic = CredentialService.UnlockAccount(account.PasswordBox.Password.ToString(), account.AccountName);
                if (mnemonic != "")
                {
                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    if (mainWindow == null) return;
                    var page = new SelectCoinPage(mainWindow.Content);
                    mainWindow.Content = page;
                    WalletModel.WalletName = account.AccountName;
                    account.PasswordBox.Clear();
                }
            }
            catch (Exception e)
            {
                PopUpError = true;
                PopUpErrorMessage = "Wrong password or wallet not found.";
            }
        }

        private void CreateAccount(NewAccount account)
        {
            if (account.RepeatPasswordBox.Password != account.PasswordBox.Password)
            {
                PopUpError = true;
                PopUpErrorMessage = "Passwords mismatch.";
                return;
            }

            try
            {
                CredentialService.CreateAccount(account.PasswordBox.Password.ToString(), account.AccountName);
            }
            catch (Exception e)
            {
                PopUpError = true;
                PopUpErrorMessage = "Wallet name already exists.";
                return;
            }

            PageHelper.FindChild<TextBox>((MainWindow)Application.Current.MainWindow, "WalletName").Text = "";
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null) return;
            var page = new SelectCoinPage(mainWindow.Content);
            mainWindow.Content = page;
            WalletModel.WalletName = account.AccountName;
            account.PasswordBox.Clear();
            account.RepeatPasswordBox.Clear();
        }

        private void OpenCreateAccountPage()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null) return;
            var page = new CreateAccountPage(mainWindow.Content);
            page.DataContext = this;
            mainWindow.Content = page;
        }

        private void OpenLoginPage()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null) return;
            var page = new LoginPage(mainWindow.Content);
            mainWindow.Content = page;
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChangedEvent("Password");
            }
        }
        private string _password;


        public bool PopUpError
        {
            get => _popUpError;
            set
            {
                _popUpError = value;
                RaisePropertyChangedEvent("PopUpError");
            }
        }

        private string _popUpErrorMessage;

        public string PopUpErrorMessage
        {
            get => _popUpErrorMessage;
            set
            {
                _popUpErrorMessage = value;
                RaisePropertyChangedEvent("PopUpErrorMessage");
            }
        }

        private bool _popUpError;
    }
}

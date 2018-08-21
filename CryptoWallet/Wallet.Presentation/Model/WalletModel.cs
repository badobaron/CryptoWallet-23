﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Core.CoinProviders;
using Wallet.Presentation.ViewModel;

namespace Wallet.Presentation.Model
{
    public class WalletModel : ObservableObject
    {
        public WalletModel()
        {
            Mainnet = false;
            Testnet = true;
        }

        public CoinProvider CoinProvider { get; set; }
        public string WalletName { get; set; }

        private bool _mainnet;

        public bool Mainnet
        {
            get => _mainnet;
            set
            {
                CoinProvider?.SetNetwork(value ? NetworkType.MainNet : NetworkType.TestNet);
                _mainnet = value;
                RaisePropertyChangedEvent("Mainnet");
            }
        }

        private bool _testnet;
        public bool Testnet
        {
            get => _testnet;
            set
            {
                CoinProvider?.SetNetwork(value ? NetworkType.TestNet : NetworkType.MainNet);
                _testnet = value;
                RaisePropertyChangedEvent("Testnet");
            }
        }

    }
}

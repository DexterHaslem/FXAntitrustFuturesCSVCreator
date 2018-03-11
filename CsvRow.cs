using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FXAntiTrustFuturesCSVCreator
{
    public enum Side
    {
        Buy,
        Sell
    }

    public enum FxTransactionType
    {
        Spot,
        Forward,
        Swap,
        OTCOption,
        Future,
        OptionOnFuture
    }

    // quick and dirty
    internal class CsvRow : NotifyPropertyChangedBase
    {
        private string _name;
        private string _claimantId;
        private string _brokerFcm;
        private string _exchangeName;
        private string _transactionId;
        private FxTransactionType _fxTransactionType;
        private DateTime _calendarDate;
        private DateTime? _tradeTimestamp;
        private string _timezoneIana;
        private string _exchangeProductCode;
        private string _tradeRate;
        private int _numberOfContracts;
        private string _baseCurrency;
        private string _quotedCurrency;
        private Side _buySell;
        private float _baseAmount;
        private float _contraAmount;
        private DateTime _expiryDate;

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string ClaimantId
        {
            get => _claimantId;
            set
            {
                if (value == _claimantId) return;
                _claimantId = value;
                OnPropertyChanged();
            }
        }

        public string BrokerFcm
        {
            get => _brokerFcm;
            set
            {
                if (value == _brokerFcm) return;
                _brokerFcm = value;
                OnPropertyChanged();
            }
        }

        public string ExchangeName
        {
            get => _exchangeName;
            set
            {
                if (value == _exchangeName) return;
                _exchangeName = value;
                OnPropertyChanged();
            }
        }

        public string TransactionId
        {
            get => _transactionId;
            set
            {
                if (value == _transactionId) return;
                _transactionId = value;
                OnPropertyChanged();
            }
        }

        public FxTransactionType FxTransactionType
        {
            get => _fxTransactionType;
            set
            {
                if (value == _fxTransactionType) return;
                _fxTransactionType = value;
                OnPropertyChanged();
            }
        }

        public DateTime CalendarDate
        {
            get => _calendarDate;
            set
            {
                if (value.Equals(_calendarDate)) return;
                _calendarDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime? TradeTimestamp
        {
            get => _tradeTimestamp;
            set
            {
                if (value.Equals(_tradeTimestamp)) return;
                _tradeTimestamp = value;
                OnPropertyChanged();
            }
        }

        public string TimezoneIana
        {
            get => _timezoneIana;
            set
            {
                if (value == _timezoneIana) return;
                _timezoneIana = value;
                OnPropertyChanged();
            }
        }

        public string ExchangeProductCode
        {
            get => _exchangeProductCode;
            set
            {
                if (value == _exchangeProductCode) return;
                _exchangeProductCode = value;
                OnPropertyChanged();
            }
        }

        public string TradeRate
        {
            get => _tradeRate;
            set
            {
                if (value == _tradeRate) return;
                _tradeRate = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfContracts
        {
            get => _numberOfContracts;
            set
            {
                if (value == _numberOfContracts) return;
                _numberOfContracts = value;
                OnPropertyChanged();
            }
        }

        public string BaseCurrency
        {
            get => _baseCurrency;
            set
            {
                if (value == _baseCurrency) return;
                _baseCurrency = value;
                OnPropertyChanged();
            }
        }

        public string QuotedCurrency
        {
            get => _quotedCurrency;
            set
            {
                if (value == _quotedCurrency) return;
                _quotedCurrency = value;
                OnPropertyChanged();
            }
        }

        public Side BuySell
        {
            get => _buySell;
            set
            {
                if (value == _buySell) return;
                _buySell = value;
                OnPropertyChanged();
            }
        }

        public float BaseAmount
        {
            get => _baseAmount;
            set
            {
                if (value.Equals(_baseAmount)) return;
                _baseAmount = value;
                OnPropertyChanged();
            }
        }

        public float ContraAmount
        {
            get => _contraAmount;
            set
            {
                if (value.Equals(_contraAmount)) return;
                _contraAmount = value;
                OnPropertyChanged();
            }
        }

        public DateTime ExpiryDate
        {
            get => _expiryDate;
            set
            {
                if (value.Equals(_expiryDate)) return;
                _expiryDate = value;
                OnPropertyChanged();
            }
        }

        public static string HeaderRow()
        {
            // note the provided file had space after base amount ... 
            return
                "Name,Claimant ID,Broker,Exchange name,Transaction ID,FX Transaction type,Trade Date,Trade Timestamp,Time zone,Exchange product code,Trade rate,Number of Contracts,Base Currency,Quoted Currency,Buy/Sell,Base Amount ,Contra amount,Expiry date";
        }

        public string ToCsv()
        {
            // http://www.fxantitrustsettlement.com/docs/FX_Electronic_Submission_of_Transaction_Data.pdf
            // hardcoded futures stuff
            var tradeDateStr = CalendarDate.ToString("yyyy-MM-dd");
            // NOTE: expiration date format is not specified in documents. woohoo!
            // lets just match
            var expiryDateStr = ExpiryDate.ToString("yyyy-MM");
            return string.Join(",",
                new string[]
                {
                    Name,
                    ClaimantId,
                    BrokerFcm,
                    ExchangeName,
                    TransactionId,
                    "future",
                    tradeDateStr,
                    "", // trade timestamp
                    TimezoneIana,
                    ExchangeProductCode,
                    TradeRate,
                    NumberOfContracts.ToString("D"),
                    BaseCurrency,
                    QuotedCurrency,
                    BuySell == Side.Buy ? "BUY" : "SELL",
                    BaseAmount.ToString("C"),
                    ContraAmount.ToString(""),
                    expiryDateStr
                });
        }
    }
}
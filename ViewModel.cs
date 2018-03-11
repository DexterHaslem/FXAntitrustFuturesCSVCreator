using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FXAntiTrustFuturesCSVCreator
{
    internal class ViewModel : NotifyPropertyChangedBase
    {
        private string _stickyName;
        private string _stickyClaimantId;
        private string _stickyBrokerName;
        private string _stickyExchangeName;
        private string _stickyExchangeProductCode;
        private CsvRow _activeEditRow;

        public ObservableCollection<CsvRow> Rows { get; set; }
        
        public string StickyName
        {
            get => _stickyName;
            set
            {
                if (value == _stickyName) return;
                _stickyName = value;
                OnPropertyChanged();
            }
        }

        public string StickyClaimantId
        {
            get => _stickyClaimantId;
            set
            {
                if (value == _stickyClaimantId) return;
                _stickyClaimantId = value;
                OnPropertyChanged();
            }
        }

        public string StickyBrokerName
        {
            get => _stickyBrokerName;
            set
            {
                if (value == _stickyBrokerName) return;
                _stickyBrokerName = value;
                OnPropertyChanged();
            }
        }

        public string StickyExchangeName
        {
            get => _stickyExchangeName;
            set
            {
                if (value == _stickyExchangeName) return;
                _stickyExchangeName = value;
                OnPropertyChanged();
            }
        }

        public string StickyExchangeProductCode
        {
            get => _stickyExchangeProductCode;
            set
            {
                if (value == _stickyExchangeProductCode) return;
                _stickyExchangeProductCode = value;
                OnPropertyChanged();
            }
        }

        public CsvRow ActiveEditRow
        {
            get => _activeEditRow;
            set
            {
                if (Equals(value, _activeEditRow)) return;
                _activeEditRow = value;
                OnPropertyChanged();
            }
        }

        public void SaveEditRow()
        {
            Rows.Add(ActiveEditRow);
            // 'sticky' fields maintain values after a row entered
            // a better way to do this would be with a sticky attribute
            // and checking reflection after row added, but this is a throw away tool
            var newRow = new CsvRow();
            ActiveEditRow = newRow;
        }

        public ViewModel(MainWindow owner)
        {
            Rows = new ObservableCollection<CsvRow>();
        }
    }
}

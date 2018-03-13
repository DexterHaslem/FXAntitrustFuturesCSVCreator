using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
        private CsvRow _selectedRow;

        public ObservableCollection<CsvRow> Rows { get; set; }

        public ICommand AddEditRowCommand { get; private set; }
        public ICommand EditRowCommand { get; private set; }
        public ICommand CopyRowCommand { get; private set; }
        public ICommand DeleteRowCommand { get; private set; }
        public ICommand ExportCsvCommand { get; private set; }

        public CsvRow SelectedRow
        {
            get => _selectedRow;
            set
            {
                if (Equals(value, _selectedRow)) return;
                _selectedRow = value;
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
            var newRow = new CsvRow(ActiveEditRow);
            Rows.Add(ActiveEditRow);
            // 'sticky' fields maintain values after a row entered
            // a better way to do this would be with a sticky attribute
            // and checking reflection after row added, but this is a throw away tool
            ActiveEditRow = newRow;
        }

        public ViewModel(MainWindow owner)
        {
            Rows = new ObservableCollection<CsvRow>();
            ActiveEditRow = new CsvRow();
            var myType = typeof(MainWindow);
            AddEditRowCommand = new RoutedCommand("AddEditRow", myType);
            EditRowCommand = new RoutedCommand("EditRow", myType);
            CopyRowCommand = new RoutedCommand("CopyRow", myType);
            DeleteRowCommand = new RoutedCommand("DeleteRow", myType);
            ExportCsvCommand = new RoutedCommand("ExportCsv", myType);

            void CanAddNewRowExec(object sender, CanExecuteRoutedEventArgs e)
            {
                // wew lawd, do some basic checking here, this whol thing is hackery
                e.CanExecute = !string.IsNullOrWhiteSpace(ActiveEditRow?.Name) && 
                               !string.IsNullOrWhiteSpace(ActiveEditRow.ClaimantId) && 
                               !string.IsNullOrWhiteSpace(ActiveEditRow.ExchangeProductCode) && 
                               !string.IsNullOrWhiteSpace(ActiveEditRow.ExchangeName) && 
                               (ActiveEditRow.BuySell == "BUY" || ActiveEditRow.BuySell == "SELL");
            }

            owner.CommandBindings.Add(new CommandBinding(AddEditRowCommand, OnAddEditRow, CanAddNewRowExec));

            void CanExistingExec(object sender, CanExecuteRoutedEventArgs args)
            {
                args.CanExecute = this.SelectedRow != null;
            }

            owner.CommandBindings.Add(new CommandBinding(EditRowCommand, OnEditExistingRow, CanExistingExec));
            owner.CommandBindings.Add(new CommandBinding(CopyRowCommand, OnCopyExistingRow, CanExistingExec));
            owner.CommandBindings.Add(new CommandBinding(DeleteRowCommand, OnDeleteExistingRow, CanExistingExec));
            owner.CommandBindings.Add(new CommandBinding(ExportCsvCommand, OnExportCsv,
                (o, e) => e.CanExecute = Rows.Count > 0));
        }

        private void OnExportCsv(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void OnDeleteExistingRow(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void OnCopyExistingRow(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void OnEditExistingRow(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void OnAddEditRow(object sender, ExecutedRoutedEventArgs e)
        {
            SaveEditRow();
        }
    }
}
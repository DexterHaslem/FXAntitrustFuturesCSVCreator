using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace FXAntiTrustFuturesCSVCreator
{
    internal class ViewModel : NotifyPropertyChangedBase
    {
        private const string BackupFile = "Rows.xml";

        private CsvRow _activeEditRow;
        private CsvRow _selectedRow;
        private readonly MainWindow _owner;

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
            newRow.BuySell = ActiveEditRow.BuySell == "BUY" ? "SELL" : "BUY"; // HACK :(
            ActiveEditRow = newRow;
        }

        public ViewModel(MainWindow owner)
        {
            _owner = owner;
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

            owner.Loaded += OnLoaded;
            owner.Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            Backup();
            _owner.Closing -= OnClosing;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Restore();
            _owner.Loaded -= OnLoaded;
        }

        private string GetCsvString()
        {
            var csv = string.Join(Environment.NewLine, new[] {CsvRow.HeaderRow()}
                .Concat(Rows.Select(r => r.ToCsv())));
            return csv;
        }

        private void OnExportCsv(object sender, ExecutedRoutedEventArgs e)
        {
            var sfd = new SaveFileDialog {Filter = "CSV File (*.csv)|*.csv"};
            var result = sfd.ShowDialog(_owner);
            if (result == true)
                File.WriteAllText(sfd.FileName, GetCsvString());
        }

        private void Backup()
        {
            var ser = new XmlSerializer(typeof(CsvRow[]));
            using (var writer = new XmlTextWriter(BackupFile, Encoding.Unicode) {Formatting = Formatting.Indented})
                ser.Serialize(writer, Rows.ToArray());
        }

        private void Restore()
        {
            var deser = new XmlSerializer(typeof(CsvRow[]));
            using (var stream = File.OpenRead(BackupFile))
            {
                var restoredRows = (CsvRow[]) deser.Deserialize(stream);
                Rows.Clear();
                foreach (var restoredRow in restoredRows)
                {
                    Rows.Add(restoredRow);
                }
            }

            // do this primarily for sticky name / claimant id to transfer
            ActiveEditRow = new CsvRow(Rows.LastOrDefault());
        }

        private void OnDeleteExistingRow(object sender, ExecutedRoutedEventArgs e)
        {
            Rows.Remove(SelectedRow);
        }

        private void OnCopyExistingRow(object sender, ExecutedRoutedEventArgs e)
        {
            ActiveEditRow = new CsvRow(SelectedRow);
        }

        private void OnEditExistingRow(object sender, ExecutedRoutedEventArgs e)
        {
            ActiveEditRow = SelectedRow;
        }

        private void OnAddEditRow(object sender, ExecutedRoutedEventArgs e)
        {
            SaveEditRow();
        }
    }

    public class BuySellFromBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as string == "BUY" ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? "BUY" : "SELL";
        }
    }
}
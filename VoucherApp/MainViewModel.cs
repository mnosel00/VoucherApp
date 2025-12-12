using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VoucherApp.Core.Entities;
using VoucherApp.Core.Interfaces;

namespace VoucherApp
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IVoucherService _voucherService;

        // --- Istniejące właściwości ---
        [ObservableProperty]
        private string _statusMessage = "Gotowy";

        [ObservableProperty]
        private Brush _statusColor = Brushes.Gray;

        // --- Nowe właściwości dla listy i filtrowania ---
        [ObservableProperty]
        private string _filterText;

        private ObservableCollection<Voucher> _allVouchers = new();
        public ObservableCollection<Voucher> FilteredVouchers { get; } = new();

        // --- Nowe właściwości dla formularza dodawania ---
        [ObservableProperty]
        private string _newVoucherCode;

        [ObservableProperty]
        private string _newVoucherDescription;

        // --- Komendy ---
        public IAsyncRelayCommand AddVoucherCommand { get; }
        public IAsyncRelayCommand<Voucher> UseVoucherCommand { get; }
        public IAsyncRelayCommand LoadVouchersCommand { get; }


        public MainViewModel(IVoucherService voucherService)
        {
            _voucherService = voucherService;

            // Używamy AsyncRelayCommand do operacji asynchronicznych
            AddVoucherCommand = new AsyncRelayCommand(AddVoucherAsync);
            UseVoucherCommand = new AsyncRelayCommand<Voucher>(UseVoucherAsync);
            LoadVouchersCommand = new AsyncRelayCommand(LoadVouchersAsync);

            // Nasłuchujemy na zmianę tekstu w filtrze
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(FilterText))
                {
                    FilterVouchers();
                }
            };

            // Automatycznie ładuj dane przy starcie
            LoadVouchersCommand.Execute(null);
        }

        private async Task LoadVouchersAsync()
        {
            var vouchers = await _voucherService.GetAllVouchersAsync();
            _allVouchers = new ObservableCollection<Voucher>(vouchers);
            FilterVouchers();
            StatusMessage = $"Załadowano {_allVouchers.Count} voucherów.";
            StatusColor = Brushes.Green;
        }

        private void FilterVouchers()
        {
            FilteredVouchers.Clear();
            var filtered = string.IsNullOrWhiteSpace(FilterText)
                ? _allVouchers
                // Używamy ShortCode zamiast Code
                : _allVouchers.Where(v => v.ShortCode.Contains(FilterText, System.StringComparison.OrdinalIgnoreCase));

            foreach (var voucher in filtered)
            {
                FilteredVouchers.Add(voucher);
            }
        }

        private async Task AddVoucherAsync()
        {
            if (string.IsNullOrWhiteSpace(NewVoucherCode) || string.IsNullOrWhiteSpace(NewVoucherDescription))
            {
                StatusMessage = "Wypełnij kod i opis nowego vouchera!";
                StatusColor = Brushes.Red;
                return;
            }

            var newVoucher = await _voucherService.CreateVoucherAsync(NewVoucherCode, NewVoucherDescription);
            if (newVoucher != null)
            {
                // Musimy ponownie załadować vouchery, aby mieć pewność, że powiązany RewardTemplate jest dołączony
                await LoadVouchersAsync(); 
                StatusMessage = $"Dodano nowy voucher: {newVoucher.ShortCode}";
                StatusColor = Brushes.Blue;
                NewVoucherCode = string.Empty;
                NewVoucherDescription = string.Empty;
            }
            else
            {
                StatusMessage = $"Voucher o kodzie '{NewVoucherCode}' już istnieje!";
                StatusColor = Brushes.Red;
            }
        }

        private async Task UseVoucherAsync(Voucher voucher)
        {
            if (voucher == null) return;

            // Używamy ShortCode zamiast Code
            var result = MessageBox.Show($"Czy na pewno chcesz wykorzystać voucher '{voucher.ShortCode}'?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _voucherService.UseVoucherAsync(voucher.Id);
                // Używamy IsRedeemed zamiast IsUsed
                voucher.IsRedeemed = true; 
                // Odświeżamy widok, aby pokazać zmianę (np. zmianę koloru lub statusu)
                OnPropertyChanged(nameof(FilteredVouchers));
                StatusMessage = $"Wykorzystano voucher: {voucher.ShortCode}";
                StatusColor = Brushes.DarkGoldenrod;
            }
        }
    }
}
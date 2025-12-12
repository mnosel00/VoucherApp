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

        [ObservableProperty]
        private string _statusMessage = "Gotowy";

        [ObservableProperty]
        private Brush _statusColor = Brushes.Gray;

        [ObservableProperty]
        private string _filterText;

        private ObservableCollection<Voucher> _allVouchers = new();
        public ObservableCollection<Voucher> FilteredVouchers { get; } = new();

        [ObservableProperty]
        private string _newVoucherDescription;

        public IAsyncRelayCommand AddVoucherCommand { get; }
        public IAsyncRelayCommand<Voucher> UseVoucherCommand { get; }
        public IAsyncRelayCommand LoadVouchersCommand { get; }


        public MainViewModel(IVoucherService voucherService)
        {
            _voucherService = voucherService;

            AddVoucherCommand = new AsyncRelayCommand(AddVoucherAsync);
            UseVoucherCommand = new AsyncRelayCommand<Voucher>(UseVoucherAsync);
            LoadVouchersCommand = new AsyncRelayCommand(LoadVouchersAsync);

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(FilterText))
                {
                    FilterVouchers();
                }
            };

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
                : _allVouchers.Where(v => v.ShortCode.Contains(FilterText, System.StringComparison.OrdinalIgnoreCase));

            foreach (var voucher in filtered)
            {
                FilteredVouchers.Add(voucher);
            }
        }

        private async Task AddVoucherAsync()
        {
            if (string.IsNullOrWhiteSpace(NewVoucherDescription))
            {
                StatusMessage = "Wypełnij opis nowego vouchera!";
                StatusColor = Brushes.Red;
                return;
            }

            var newVoucher = await _voucherService.CreateVoucherAsync(NewVoucherDescription);
            if (newVoucher != null)
            {
                await LoadVouchersAsync(); 
                StatusMessage = $"Dodano nowy voucher: {newVoucher.ShortCode}";
                StatusColor = Brushes.Blue;
                NewVoucherDescription = string.Empty;
            }
            else
            {
                StatusMessage = $"Nie udało się utworzyć vouchera.";
                StatusColor = Brushes.Red;
            }
        }

        private async Task UseVoucherAsync(Voucher voucher)
        {
            if (voucher == null) return;

            var result = MessageBox.Show($"Czy na pewno chcesz wykorzystać voucher '{voucher.ShortCode}'?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _voucherService.UseVoucherAsync(voucher.Id);
                voucher.IsRedeemed = true; 
                OnPropertyChanged(nameof(FilteredVouchers));
                StatusMessage = $"Wykorzystano voucher: {voucher.ShortCode}";
                StatusColor = Brushes.DarkGoldenrod;
            }
        }
    }
}
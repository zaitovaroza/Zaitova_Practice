using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZaitovaLibrary.DTO;
using ZaitovaLibrary.Services;

namespace Zaitova.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IPartnerService _partnerService;

        private ObservableCollection<PartnerDto> _partners;
        private ObservableCollection<SalesHistoryDto> _selectedPartnerOrders;
        private PartnerDto? _selectedPartner;
        private bool _isLoading;
        private string _searchText = string.Empty;

        public MainViewModel(IPartnerService partnerService)
        {
            _partnerService = partnerService;
            _partners = new ObservableCollection<PartnerDto>();
            _selectedPartnerOrders = new ObservableCollection<SalesHistoryDto>();

            InitializeCommands();
            LoadPartnersAsync();

            // Подписка на изменение выбранного партнера
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedPartner))
                {
                    LoadSelectedPartnerOrdersAsync();
                }
            };
        }

        public ObservableCollection<PartnerDto> Partners
        {
            get => _partners;
            set => SetProperty(ref _partners, value);
        }

        public ObservableCollection<SalesHistoryDto> SelectedPartnerOrders
        {
            get => _selectedPartnerOrders;
            set => SetProperty(ref _selectedPartnerOrders, value);
        }

        public PartnerDto? SelectedPartner
        {
            get => _selectedPartner;
            set => SetProperty(ref _selectedPartner, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    FilterPartners();
            }
        }

        public ICommand? AddPartnerCommand { get; private set; }
        public ICommand? EditPartnerCommand { get; private set; }
        public ICommand? DeletePartnerCommand { get; private set; }

        private void InitializeCommands()
        {
            AddPartnerCommand = new RelayCommand(ExecuteAddPartner);
            EditPartnerCommand = new RelayCommand(ExecuteEditPartner, () => SelectedPartner != null);
            DeletePartnerCommand = new RelayCommand(ExecuteDeletePartner, () => SelectedPartner != null);
        }

        private async Task LoadPartnersAsync()
        {
            try
            {
                IsLoading = true;
                var partners = await _partnerService.GetAllPartnersAsync();

                Partners.Clear();
                foreach (var partner in partners)
                {
                    Partners.Add(partner);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void LoadSelectedPartnerOrdersAsync()
        {
            if (SelectedPartner == null)
            {
                SelectedPartnerOrders.Clear();
                return;
            }

            try
            {
                var orders = await _partnerService.GetPartnerSalesHistoryAsync(SelectedPartner.Id);
                SelectedPartnerOrders.Clear();
                foreach (var order in orders)
                {
                    SelectedPartnerOrders.Add(order);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterPartners()
        {
            if (string.IsNullOrWhiteSpace(_searchText))
            {
                _ = LoadPartnersAsync();
                return;
            }

            var filtered = Partners.Where(p =>
                p.CompanyName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                (p.DirectorFullname?.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (p.Phone?.Contains(_searchText) ?? false)).ToList();

            Partners.Clear();
            foreach (var partner in filtered)
            {
                Partners.Add(partner);
            }
        }

        private void ExecuteAddPartner()
        {
            var editWindow = new PartnerEditWindow();
            var viewModel = App.ServiceProvider.GetService(typeof(PartnerEditViewModel)) as PartnerEditViewModel;
            viewModel!.InitializeForCreate();
            editWindow.DataContext = viewModel;
            editWindow.Owner = Application.Current.MainWindow;

            if (editWindow.ShowDialog() == true)
            {
                _ = LoadPartnersAsync();
            }
        }

        private void ExecuteEditPartner()
        {
            if (SelectedPartner == null) return;

            var editWindow = new PartnerEditWindow();
            var viewModel = App.ServiceProvider.GetService(typeof(PartnerEditViewModel)) as PartnerEditViewModel;
            viewModel!.InitializeForEdit(SelectedPartner.Id);
            editWindow.DataContext = viewModel;
            editWindow.Owner = Application.Current.MainWindow;

            if (editWindow.ShowDialog() == true)
            {
                _ = LoadPartnersAsync();
            }
        }

        private async void ExecuteDeletePartner()
        {
            if (SelectedPartner == null) return;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить партнера '{SelectedPartner.CompanyName}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    IsLoading = true;
                    await _partnerService.DeletePartnerAsync(SelectedPartner.Id);
                    await LoadPartnersAsync();
                    SelectedPartner = null;

                    MessageBox.Show("Партнер успешно удален", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZaitovaLibrary.DTO;
using ZaitovaLibrary.Services;
using ZaitovaLibrary.Data;
using ZaitovaLibrary.Models;

namespace Zaitova.ViewModels
{
    public class PartnerEditViewModel : ViewModelBase
    {
        private readonly IPartnerService _partnerService;
        private readonly IPartnerTypeRepository _partnerTypeRepository;

        private PartnerCreateUpdateDto _currentPartner;
        private ObservableCollection<PartnerType> _partnerTypes;
        private PartnerType? _selectedType;
        private bool _isEditMode;
        private bool _isLoading;

        public PartnerEditViewModel(
            IPartnerService partnerService,
            IPartnerTypeRepository partnerTypeRepository)
        {
            _partnerService = partnerService;
            _partnerTypeRepository = partnerTypeRepository;
            _currentPartner = new PartnerCreateUpdateDto();
            _partnerTypes = new ObservableCollection<PartnerType>();

            InitializeCommands();

            // Загружаем типы партнеров синхронно
            LoadPartnerTypes();
        }

        public PartnerCreateUpdateDto CurrentPartner
        {
            get => _currentPartner;
            set => SetProperty(ref _currentPartner, value);
        }

        public ObservableCollection<PartnerType> PartnerTypes
        {
            get => _partnerTypes;
            set => SetProperty(ref _partnerTypes, value);
        }

        public PartnerType? SelectedType
        {
            get => _selectedType;
            set
            {
                if (SetProperty(ref _selectedType, value) && value != null)
                {
                    CurrentPartner.TypeId = value.Id;
                }
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string WindowTitle => IsEditMode ? "Редактирование партнера" : "Добавление партнера";

        public ICommand? SaveCommand { get; private set; }
        public ICommand? CancelCommand { get; private set; }

        private void InitializeCommands()
        {
            SaveCommand = new RelayCommand(async () => await SaveAsync(), CanSave);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        // Изменено на синхронный метод
        private void LoadPartnerTypes()
        {
            try
            {
                // Используем Task.Run чтобы не блокировать UI, но дожидаемся результата
                var types = Task.Run(async () => await _partnerTypeRepository.GetAllAsync()).Result;

                PartnerTypes.Clear();
                foreach (var type in types)
                {
                    PartnerTypes.Add(type);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов партнеров: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void InitializeForCreate()
        {
            IsEditMode = false;
            CurrentPartner = new PartnerCreateUpdateDto
            {
                Rating = 0
            };
            SelectedType = PartnerTypes.FirstOrDefault();
        }

        public async void InitializeForEdit(int partnerId)
        {
            try
            {
                IsLoading = true;
                IsEditMode = true;

                var partner = await _partnerService.GetPartnerForEditAsync(partnerId);
                if (partner != null)
                {
                    CurrentPartner = partner;
                    SelectedType = PartnerTypes.FirstOrDefault(t => t.Id == partner.TypeId);
                }
                else
                {
                    MessageBox.Show("Партнер не найден", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    ExecuteCancel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ExecuteCancel();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(CurrentPartner.CompanyName) &&
                   CurrentPartner.TypeId > 0 &&
                   CurrentPartner.Rating >= 0 &&
                   CurrentPartner.Rating <= 100;
        }

        private async Task SaveAsync()
        {
            try
            {
                IsLoading = true;

                if (IsEditMode)
                {
                    await _partnerService.UpdatePartnerAsync(CurrentPartner);
                    MessageBox.Show("Данные партнера успешно обновлены",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    await _partnerService.CreatePartnerAsync(CurrentPartner);
                    MessageBox.Show("Партнер успешно добавлен",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                CloseWindow(true);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Ошибка сохранения: {ex.Message}";

                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nДетали: {ex.InnerException.Message}";
                }

                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ExecuteCancel()
        {
            CloseWindow(false);
        }

        private void CloseWindow(bool result)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = result;
                    window.Close();
                    break;
                }
            }
        }
    }
}
using System.Windows;
using System.Windows.Input;
using Zaitova.ViewModels;
using ZaitovaLibrary.DTO;

namespace Zaitova
{
    public partial class MainWindow : Window
    {
        private MainViewModel? ViewModel => DataContext as MainViewModel;

        public MainWindow()
        {
            InitializeComponent();

            if (App.ServiceProvider != null)
            {
                DataContext = App.ServiceProvider.GetService(typeof(MainViewModel));
            }
        }

        private void PartnerCard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is PartnerDto partner)
            {
                ViewModel!.SelectedPartner = partner;
            }
        }

        private void AddPartnerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.AddPartnerCommand?.Execute(null);
        }

        private void EditPartnerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.EditPartnerCommand?.Execute(null);
        }

        private void DeletePartnerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.DeletePartnerCommand?.Execute(null);
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Мастер пол - Учет партнеров\n" +
                "Версия 1.0\n" +
                "Разработчик: Заитова Роза\n" +
                "Группа: ИПО-41\n\n" +
                "© 2026 Все права защищены",
                "О программе",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
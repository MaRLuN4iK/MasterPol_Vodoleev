using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using MasterPol.Data.Database;
using MasterPol.Data.Models;

namespace MasterPol.App
{
    public partial class MainWindow : Window
    {
        private PartnerRepository _repository;
        private List<Partner> _partners;

        public MainWindow()
        {
            InitializeComponent();
            _repository = new PartnerRepository();
            LoadPartners();
        }
        private void BtnAddSale_Click(object sender, RoutedEventArgs e)
        {
            var selectedPartner = GetSelectedPartner();
            if (selectedPartner != null)
            {
                var addSaleWindow = new AddSaleWindow(selectedPartner.Id, selectedPartner.Name);
                addSaleWindow.Owner = this;
                if (addSaleWindow.ShowDialog() == true)
                {
                    LoadPartners(); // Обновляем список для пересчета скидки
                }
            }
            else
            {
                MessageBox.Show("Выберите партнера для добавления продажи",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void LoadPartners()
        {
            try
            {
                TxtStatus.Text = "Загрузка данных...";
                _partners = await _repository.GetAllPartnersAsync();

                PartnersListView.ItemsSource = _partners.Select(p => new PartnerViewModel
                {
                    Id = p.Id,
                    PartnerTypeName = p.PartnerType?.Name ?? "Не указан",
                    Name = p.Name,
                    DirectorName = string.IsNullOrEmpty(p.DirectorName) ? "Не указан" : p.DirectorName,
                    Phone = string.IsNullOrEmpty(p.Phone) ? "Не указан" : p.Phone,
                    Email = string.IsNullOrEmpty(p.Email) ? "Не указан" : p.Email,
                    Rating = p.Rating,
                    Discount = p.CalculateDiscount()
                }).ToList();

                TxtPartnerCount.Text = _partners.Count.ToString();
                TxtStatus.Text = "Готово";
                TxtLastUpdate.Text = DateTime.Now.ToString("HH:mm:ss");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}\n\nПроверьте подключение к базе данных.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                TxtStatus.Text = "Ошибка загрузки";
            }
        }

        private PartnerViewModel GetSelectedPartner()
        {
            return PartnersListView.SelectedItem as PartnerViewModel;
        }

        private void BtnAddPartner_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new PartnerEditWindow();
            editWindow.Owner = this;
            if (editWindow.ShowDialog() == true)
            {
                LoadPartners();
            }
        }

        private void BtnEditPartner_Click(object sender, RoutedEventArgs e)
        {
            var selectedPartner = GetSelectedPartner();
            if (selectedPartner != null)
            {
                EditPartner(selectedPartner.Id);
            }
            else
            {
                MessageBox.Show("Выберите партнера для редактирования",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDeletePartner_Click(object sender, RoutedEventArgs e)
        {
            var selectedPartner = GetSelectedPartner();
            if (selectedPartner != null)
            {
                DeletePartner(selectedPartner);
            }
            else
            {
                MessageBox.Show("Выберите партнера для удаления",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            var selectedPartner = GetSelectedPartner();
            if (selectedPartner != null)
            {
                ShowPartnerHistory(selectedPartner.Id, selectedPartner.Name);
            }
            else
            {
                MessageBox.Show("Выберите партнера для просмотра истории",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadPartners();
        }

        private void PartnersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedPartner = GetSelectedPartner();
            if (selectedPartner != null)
            {
                EditPartner(selectedPartner.Id);
            }
        }

        private async void EditPartner(int partnerId)
        {
            try
            {
                var fullPartner = await _repository.GetPartnerByIdAsync(partnerId);
                if (fullPartner != null)
                {
                    var editWindow = new PartnerEditWindow(fullPartner);
                    editWindow.Owner = this;
                    if (editWindow.ShowDialog() == true)
                    {
                        LoadPartners();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных партнера: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowPartnerHistory(int partnerId, string partnerName)
        {
            var historyWindow = new SaleHistoryWindow(partnerId, partnerName);
            historyWindow.Owner = this;
            historyWindow.ShowDialog();
        }

        private async void DeletePartner(PartnerViewModel viewModel)
        {
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить партнера '{viewModel.Name}'?\n\n" +
                "Будет удалена также вся история продаж этого партнера.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    TxtStatus.Text = "Удаление...";
                    var success = await _repository.DeletePartnerAsync(viewModel.Id);
                    if (success)
                    {
                        LoadPartners();
                        MessageBox.Show("Партнер успешно удален",
                            "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить партнера. Возможно, у него есть связанные данные.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    TxtStatus.Text = "Готово";
                }
            }
        }
    }

    // Класс для отображения в списке
    public class PartnerViewModel
    {
        public int Id { get; set; }
        public string PartnerTypeName { get; set; }
        public string Name { get; set; }
        public string DirectorName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Rating { get; set; }
        public decimal Discount { get; set; }
    }

    // Конвертер для цвета скидки
    public class DiscountToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is decimal discount)
            {
                if (discount >= 15) return new SolidColorBrush(Colors.Green);
                if (discount >= 10) return new SolidColorBrush(Colors.Blue);
                if (discount >= 5) return new SolidColorBrush(Colors.Orange);
                return new SolidColorBrush(Colors.Gray);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
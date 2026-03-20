using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using MasterPol.Data.Database;
using MasterPol.Data.Models;

namespace MasterPol.App
{
    public partial class AddSaleWindow : Window
    {
        private PartnerRepository _repository;
        private int _partnerId;
        private string _partnerName;
        private List<Product> _products;

        public AddSaleWindow(int partnerId, string partnerName)
        {
            InitializeComponent();
            _repository = new PartnerRepository();
            _partnerId = partnerId;
            _partnerName = partnerName;
            TxtPartnerInfo.Text = partnerName;
            DatePickerSale.SelectedDate = DateTime.Now;
            LoadProducts();

            // Подписка на изменение выбора продукции
            CmbProduct.SelectionChanged += CmbProduct_SelectionChanged;
            TxtQuantity.TextChanged += TxtQuantity_TextChanged;
        }

        private async void LoadProducts()
        {
            try
            {
                _products = await _repository.GetAllProductsAsync();
                CmbProduct.ItemsSource = _products.Select(p => new
                {
                    Id = p.Id,
                    DisplayName = $"{p.Name} - {p.MinPartnerPrice:N2} ₽"
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки продукции: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CmbProduct_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CmbProduct.SelectedValue != null)
            {
                var selectedProduct = _products?.FirstOrDefault(p => p.Id == (int)CmbProduct.SelectedValue);
                if (selectedProduct != null)
                {
                    TxtArticle.Text = selectedProduct.Article;
                    TxtPrice.Text = $"{selectedProduct.MinPartnerPrice:N2} ₽";
                    UpdateTotalPreview();
                }
            }
            else
            {
                TxtArticle.Text = "Выберите продукцию для отображения артикула";
                TxtPrice.Text = "0.00 ₽";
                TxtTotalPreview.Text = "0.00 ₽";
            }
        }

        private void TxtQuantity_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateTotalPreview();
        }

        private void UpdateTotalPreview()
        {
            if (CmbProduct.SelectedValue != null && !string.IsNullOrWhiteSpace(TxtQuantity.Text))
            {
                var selectedProduct = _products?.FirstOrDefault(p => p.Id == (int)CmbProduct.SelectedValue);
                if (selectedProduct != null && int.TryParse(TxtQuantity.Text, out int quantity) && quantity > 0)
                {
                    decimal total = selectedProduct.MinPartnerPrice * quantity;
                    TxtTotalPreview.Text = $"{total:N2} ₽";
                    return;
                }
            }
            TxtTotalPreview.Text = "0.00 ₽";
        }

        private void TxtQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private bool ValidateFields()
        {
            if (CmbProduct.SelectedValue == null)
            {
                MessageBox.Show("Выберите продукцию", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                CmbProduct.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtQuantity.Text))
            {
                MessageBox.Show("Введите количество", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtQuantity.Focus();
                return false;
            }

            if (!int.TryParse(TxtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Количество должно быть положительным целым числом", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtQuantity.Focus();
                return false;
            }

            if (DatePickerSale.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату продажи", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                DatePickerSale.Focus();
                return false;
            }

            if (DatePickerSale.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("Дата продажи не может быть в будущем", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                DatePickerSale.Focus();
                return false;
            }

            return true;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

            try
            {
                var saleHistory = new SaleHistory
                {
                    PartnerId = _partnerId,
                    ProductId = (int)CmbProduct.SelectedValue,
                    Quantity = int.Parse(TxtQuantity.Text),
                    SaleDate = DatePickerSale.SelectedDate ?? DateTime.Now
                };

                bool success = await _repository.AddSaleHistoryAsync(saleHistory);
                if (success)
                {
                    MessageBox.Show("Продажа успешно добавлена",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Не удалось добавить продажу", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
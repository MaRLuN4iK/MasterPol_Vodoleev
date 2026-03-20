using System;
using System.Windows;
using MasterPol.Data.Database;
using MasterPol.Data.Models;

namespace MasterPol.App
{
    public partial class PartnerEditWindow : Window
    {
        private PartnerRepository _repository;
        private Partner _currentPartner;
        private bool _isEditMode;

        public PartnerEditWindow()
        {
            InitializeComponent();
            _repository = new PartnerRepository();
            _isEditMode = false;
            IconText.Text = "➕";
            LoadPartnerTypes();
        }

        public PartnerEditWindow(Partner partner)
        {
            InitializeComponent();
            _repository = new PartnerRepository();
            _currentPartner = partner;
            _isEditMode = true;
            TblHeader.Text = "Редактирование партнера";
            IconText.Text = "✎";
            LoadPartnerTypes();
            LoadPartnerData();
        }

        private async void LoadPartnerTypes()
        {
            try
            {
                var types = await _repository.GetAllPartnerTypesAsync();
                CmbPartnerType.ItemsSource = types;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов партнеров: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPartnerData()
        {
            if (_currentPartner != null)
            {
                CmbPartnerType.SelectedValue = _currentPartner.PartnerTypeId;
                TxtName.Text = _currentPartner.Name;
                TxtAddress.Text = _currentPartner.LegalAddress;
                TxtINN.Text = _currentPartner.INN;
                TxtDirector.Text = _currentPartner.DirectorName;
                TxtPhone.Text = _currentPartner.Phone;
                TxtEmail.Text = _currentPartner.Email;
                TxtRating.Text = _currentPartner.Rating.ToString();
            }
        }

        private bool ValidateFields()
        {
            if (CmbPartnerType.SelectedValue == null)
            {
                MessageBox.Show("Выберите тип партнера", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                CmbPartnerType.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Введите наименование компании", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtRating.Text))
            {
                MessageBox.Show("Введите рейтинг", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtRating.Focus();
                return false;
            }

            if (!int.TryParse(TxtRating.Text, out int rating))
            {
                MessageBox.Show("Рейтинг должен быть целым числом", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtRating.Focus();
                return false;
            }

            if (rating < 0)
            {
                MessageBox.Show("Рейтинг не может быть отрицательным", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtRating.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                if (!TxtEmail.Text.Contains("@") || !TxtEmail.Text.Contains("."))
                {
                    MessageBox.Show("Введите корректный email адрес", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtEmail.Focus();
                    return false;
                }
            }

            return true;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

            try
            {
                if (!_isEditMode)
                {
                    _currentPartner = new Partner();
                }

                _currentPartner.PartnerTypeId = (int)CmbPartnerType.SelectedValue;
                _currentPartner.Name = TxtName.Text.Trim();
                _currentPartner.LegalAddress = string.IsNullOrWhiteSpace(TxtAddress.Text) ? null : TxtAddress.Text.Trim();
                _currentPartner.INN = string.IsNullOrWhiteSpace(TxtINN.Text) ? null : TxtINN.Text.Trim();
                _currentPartner.DirectorName = string.IsNullOrWhiteSpace(TxtDirector.Text) ? null : TxtDirector.Text.Trim();
                _currentPartner.Phone = string.IsNullOrWhiteSpace(TxtPhone.Text) ? null : TxtPhone.Text.Trim();
                _currentPartner.Email = string.IsNullOrWhiteSpace(TxtEmail.Text) ? null : TxtEmail.Text.Trim();
                _currentPartner.Rating = int.Parse(TxtRating.Text);

                bool success;
                if (_isEditMode)
                {
                    success = await _repository.UpdatePartnerAsync(_currentPartner);
                }
                else
                {
                    success = await _repository.AddPartnerAsync(_currentPartner);
                }

                if (success)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Не удалось сохранить данные", "Ошибка",
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
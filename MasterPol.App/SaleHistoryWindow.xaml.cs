using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using MasterPol.Data.Database;
using MasterPol.Data.Models;

namespace MasterPol.App
{
    public partial class SaleHistoryWindow : Window
    {
        private PartnerRepository _repository;
        private int _partnerId;
        private string _partnerName;

        // Свойства для графика
        public SeriesCollection SeriesCollection { get; set; }
        public string[] MonthLabels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public SaleHistoryWindow(int partnerId, string partnerName)
        {
            InitializeComponent();
            _repository = new PartnerRepository();
            _partnerId = partnerId;
            _partnerName = partnerName;
            TblHeader.Text = $"История продаж: {partnerName}";

            // Инициализация графика
            InitializeChart();

            // Загрузка данных
            LoadHistory();
        }

        private void InitializeChart()
        {
            // Названия месяцев на русском
            MonthLabels = new[]
            {
                "Янв", "Фев", "Мар", "Апр", "Май", "Июн",
                "Июл", "Авг", "Сен", "Окт", "Ноя", "Дек"
            };

            // Форматирование значений на оси Y
            YFormatter = value => value.ToString("N0") + " ₽";

            // Создаем коллекцию для графика
            SeriesCollection = new SeriesCollection();

            // Устанавливаем контекст данных для привязки
            DataContext = this;
        }

        private async void LoadHistory()
        {
            try
            {
                // Получаем историю продаж
                var history = await _repository.GetPartnerSaleHistoryAsync(_partnerId);

                // Заполняем таблицу
                HistoryListView.ItemsSource = history.Select(h => new
                {
                    h.SaleDate,
                    ProductName = h.Product?.Name ?? "Неизвестно",
                    ProductArticle = h.Product?.Article ?? "",
                    h.Quantity,
                    Price = h.Product?.MinPartnerPrice ?? 0,
                    Total = h.Quantity * (h.Product?.MinPartnerPrice ?? 0)
                }).ToList();

                // Общая сумма
                decimal totalSum = history.Sum(h => h.Quantity * (h.Product?.MinPartnerPrice ?? 0));
                TxtTotal.Text = $"{totalSum:N2} ₽";

                // Обновляем график
                UpdateChart(history);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки истории продаж: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateChart(List<SaleHistory> history)
        {
            try
            {
                // Очищаем старые данные
                SeriesCollection.Clear();

                // Группируем продажи по месяцам
                var salesByMonth = new decimal[12];

                // Берем данные за текущий год
                int currentYear = DateTime.Now.Year;
                var currentYearSales = history.Where(h => h.SaleDate.Year == currentYear);

                foreach (var sale in currentYearSales)
                {
                    int monthIndex = sale.SaleDate.Month - 1; // 0 = Январь
                    salesByMonth[monthIndex] += sale.Quantity * (sale.Product?.MinPartnerPrice ?? 0);
                }

                // Создаем серию для графика (столбцы)
                var columnSeries = new ColumnSeries
                {
                    Title = "Продажи",
                    Values = new ChartValues<decimal>(salesByMonth),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB")),
                    StrokeThickness = 0,
                    DataLabels = true,
                    LabelPoint = point => point.Y.ToString("N0") + " ₽",
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Colors.White)
                };

                SeriesCollection.Add(columnSeries);

                // Принудительное обновление графика
                SalesChart.Series = SeriesCollection;
                SalesChart.AxisX[0].Labels = MonthLabels;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при построении графика: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnAddSale_Click(object sender, RoutedEventArgs e)
        {
            var addSaleWindow = new AddSaleWindow(_partnerId, _partnerName);
            addSaleWindow.Owner = this;
            if (addSaleWindow.ShowDialog() == true)
            {
                LoadHistory(); // Перезагружаем историю после добавления
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
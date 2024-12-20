using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MedicalInfo.MedicalSystemDataSetTableAdapters;

namespace MedicalInfo
{
    public partial class GlavAndinWindow : Window
    {
        private TreatmentRecordTableAdapter treatmentRecordsAdapter = new TreatmentRecordTableAdapter();

        public GlavAndinWindow(int id)
        {
            InitializeComponent();
            Loaded += (s, e) => DrawBarChart();
        }

        private void DrawBarChart()
        {
            var data = GetDataFromAdapter();

            if (data == null || !data.Any())
            {
                MessageBox.Show("Нет данных для отображения.");
                return;
            }

            double canvasHeight = ChartCanvas.ActualHeight - 20; // Оставляем место сверху
            double canvasWidth = ChartCanvas.ActualWidth - 50;  // Оставляем место для оси Y
            double barSpacing = 10;
            double barWidth = (canvasWidth - barSpacing * (data.Count - 1)) / data.Count;

            double maxValue = data.Values.Max();
            int ySteps = 5; // Количество отметок на оси Y
            double stepValue = Math.Ceiling(maxValue / ySteps); // Округляем шаг до целого числа

            ChartCanvas.Children.Clear();

            // Отрисовка шкалы Y
            for (int step = 0; step <= ySteps; step++)
            {
                double yValue = step * stepValue;
                double yPosition = canvasHeight - (yValue / maxValue) * canvasHeight;

                // Горизонтальная линия шкалы
                var line = new Line
                {
                    X1 = 50,
                    X2 = canvasWidth + 50,
                    Y1 = yPosition,
                    Y2 = yPosition,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };

                // Текст отметки
                var text = new TextBlock
                {
                    Text = yValue.ToString("0"), // Уникальное значение на каждой отметке
                    FontSize = 10,
                    Foreground = Brushes.Black
                };

                Canvas.SetLeft(text, 5); // Текст рядом с линией
                Canvas.SetTop(text, yPosition - 10);

                ChartCanvas.Children.Add(line);
                ChartCanvas.Children.Add(text);
            }

            // Отрисовка столбцов
            int i = 0;
            foreach (var entry in data)
            {
                // Высота столбца
                double barHeight = (entry.Value / maxValue) * canvasHeight;

                // Прямоугольник для столбца
                var bar = new Rectangle
                {
                    Width = barWidth,
                    Height = barHeight,
                    Fill = Brushes.SkyBlue
                };

                Canvas.SetLeft(bar, 50 + i * (barWidth + barSpacing));
                Canvas.SetTop(bar, canvasHeight - barHeight);

                // Текст с датой (вертикально)
                var dateText = new TextBlock
                {
                    Text = entry.Key,
                    FontSize = 10,
                    Width = 100,
                    TextAlignment = TextAlignment.Center,
                    RenderTransform = new RotateTransform(-90), // Поворот текста
                    RenderTransformOrigin = new Point(0.5, 0.5)
                };

                Canvas.SetLeft(dateText, 50 + i * (barWidth + barSpacing) + (barWidth / 2) - 50);
                Canvas.SetTop(dateText, canvasHeight + 10);

                // Добавляем столбцы и подписи к датам
                ChartCanvas.Children.Add(bar);
                ChartCanvas.Children.Add(dateText);

                i++;
            }
        }


        private Dictionary<string, int> GetDataFromAdapter()
        {
            var data = new Dictionary<string, int>();

            try
            {
                // Получаем данные из базы через адаптер
                var treatmentRecordsTable = treatmentRecordsAdapter.GetData();

                // Группируем данные по дате и подсчитываем записи
                var groupedData = treatmentRecordsTable.AsEnumerable()
                    .GroupBy(row => row.Field<DateTime>("date").ToString("yyyy-MM-dd"))
                    .Select(group => new
                    {
                        Date = group.Key,
                        Count = group.Count()
                    });

                foreach (var entry in groupedData)
                {
                    data[entry.Date] = entry.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении данных: {ex.Message}");
            }

            return data;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PreparationsWindow window = new PreparationsWindow();
            Close();
            window.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DoctorsWindow window = new DoctorsWindow();
            Close();
            window.Show();
        }
    }
}

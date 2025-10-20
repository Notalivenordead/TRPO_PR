using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
namespace _222_Busin.Pages
{
    public partial class DiagrammPage : Page
    {
        public DiagrammPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var context = Entities.GetContext();
                CmbUser.ItemsSource = context.User.ToList();
                CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType)).Cast<SeriesChartType>();
                CmbDiagram.SelectedItem = SeriesChartType.Column;

                if (ChartPayments.ChartAreas.Count == 0)
                    ChartPayments.ChartAreas.Add(new ChartArea("Main"));

                if (ChartPayments.Series.Count == 0)
                {
                    var currentSeries = new Series("Платежи")
                    {
                        IsValueShownAsLabel = true,
                        ChartType = SeriesChartType.Column
                    };
                    ChartPayments.Series.Add(currentSeries);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var context = Entities.GetContext();

                if (CmbUser.SelectedItem is User currentUser && CmbDiagram.SelectedItem is SeriesChartType currentType)
                {
                    Series currentSeries = ChartPayments.Series.FirstOrDefault();
                    if (currentSeries == null) return;

                    currentSeries.ChartType = currentType;
                    currentSeries.Points.Clear();

                    var categoriesList = context.Category.ToList();
                    foreach (var category in categoriesList)
                    {
                        var sum = context.Payment
                            .Where(p => p.UserID == currentUser.ID && p.CategoryID == category.ID)
                            .Sum(p => (decimal?)p.Price * p.Num) ?? 0;

                        currentSeries.Points.AddXY(category.Name, (double)sum);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении диаграммы: {ex.Message}");
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы точно хотите сформировать отчёт в Excel?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                try
            {
                var context = Entities.GetContext();

                if (CmbUser.SelectedItem == null)
                {
                    MessageBox.Show("Выберите пользователя для экспорта!");
                    return;
                }

                var user = CmbUser.SelectedItem as User;
                var allUsers = context.User.ToList().OrderBy(u => u.FIO).ToList();

                var excelApp = new Excel.Application();
                excelApp.SheetsInNewWorkbook = allUsers.Count;
                var workbook = excelApp.Workbooks.Add();

                for (int i = 0; i < allUsers.Count; i++)
                {
                    int startRowIndex = 1;
                    var worksheet = workbook.Worksheets[i + 1];
                    worksheet.Name = allUsers[i].FIO.Length > 31 ?
                        allUsers[i].FIO.Substring(0, 31) : allUsers[i].FIO;

                    // Заголовки
                    worksheet.Cells[1, 1] = "Дата платежа";
                    worksheet.Cells[1, 2] = "Название";
                    worksheet.Cells[1, 3] = "Стоимость";
                    worksheet.Cells[1, 4] = "Количество";
                    worksheet.Cells[1, 5] = "Сумма";

                    var headerRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 5]];
                    headerRange.Font.Bold = true;
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    startRowIndex++;

                    var userCategories = allUsers[i].Payment
                        .OrderBy(p => p.Date)
                        .GroupBy(p => p.Category)
                        .OrderBy(g => g.Key?.Name);

                    foreach (var categoryGroup in userCategories)
                    {
                        if (categoryGroup.Key == null) continue;

                        // Заголовок категории
                        var categoryRange = worksheet.Range[
                            worksheet.Cells[startRowIndex, 1],
                            worksheet.Cells[startRowIndex, 5]];
                        categoryRange.Merge();
                        categoryRange.Value = categoryGroup.Key.Name;
                        categoryRange.Font.Italic = true;
                        categoryRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        startRowIndex++;

                        // Данные платежей
                        foreach (var payment in categoryGroup)
                        {
                            worksheet.Cells[startRowIndex, 1] = payment.Date.ToString("dd.MM.yyyy");
                            worksheet.Cells[startRowIndex, 2] = payment.Name;
                            worksheet.Cells[startRowIndex, 3] = (double)payment.Price;
                            worksheet.Cells[startRowIndex, 4] = payment.Num;
                            worksheet.Cells[startRowIndex, 5].Formula =
                                $"=C{startRowIndex}*D{startRowIndex}";
                            startRowIndex++;
                        }

                        // Итог по категории
                        var sumRange = worksheet.Range[
                            worksheet.Cells[startRowIndex, 1],
                            worksheet.Cells[startRowIndex, 4]];
                        sumRange.Merge();
                        sumRange.Value = "ИТОГО:";
                        sumRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                        worksheet.Cells[startRowIndex, 5].Formula =
                            $"=SUM(E{startRowIndex - categoryGroup.Count()}:E{startRowIndex - 1})";

                        var totalRange = worksheet.Range[
                            worksheet.Cells[startRowIndex, 1],
                            worksheet.Cells[startRowIndex, 5]];
                        totalRange.Font.Bold = true;
                        startRowIndex++;
                        startRowIndex++; // Пустая строка
                    }

                    worksheet.Columns.AutoFit();
                }

                excelApp.Visible = true;
                MessageBox.Show("Экспорт в Excel завершен успешно!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в Excel: {ex.Message}");
            }
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы точно хотите сформировать отчёт в Word?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                try
            {
                var context = Entities.GetContext();
                var allUsers = context.User.ToList();
                var allCategories = context.Category.ToList();

                var wordApp = new Word.Application();
                var document = wordApp.Documents.Add();

                // Добавляем общий заголовок
                var titleParagraph = document.Paragraphs.Add();
                var titleRange = titleParagraph.Range;
                titleRange.Text = "Отчет по платежам пользователей";
                titleRange.set_Style("Заголовок 1");
                titleRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                titleRange.InsertParagraphAfter();

                foreach (var user in allUsers)
                {
                    // Заголовок с ФИО пользователя
                    var userParagraph = document.Paragraphs.Add();
                    var userRange = userParagraph.Range;
                    userRange.Text = user.FIO;
                    userRange.set_Style("Заголовок 2");
                    userRange.InsertParagraphAfter();

                    // Таблица платежей
                    if (allCategories.Any())
                    {
                        var tableParagraph = document.Paragraphs.Add();
                        var tableRange = tableParagraph.Range;
                        var paymentsTable = document.Tables.Add(tableRange,
                            allCategories.Count + 1, 2);

                        paymentsTable.Borders.InsideLineStyle =
                            paymentsTable.Borders.OutsideLineStyle =
                            Word.WdLineStyle.wdLineStyleSingle;

                        // Заголовки таблицы
                        paymentsTable.Cell(1, 1).Range.Text = "Категория";
                        paymentsTable.Cell(1, 2).Range.Text = "Сумма расходов";

                        var headerRange = paymentsTable.Rows[1].Range;
                        headerRange.Bold = 1;
                        headerRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                        // Данные таблицы
                        for (int i = 0; i < allCategories.Count; i++)
                        {
                            var category = allCategories[i];
                            paymentsTable.Cell(i + 2, 1).Range.Text = category.Name;

                            var sum = user.Payment
                                .Where(p => p.CategoryID == category.ID)
                                .Sum(p => p.Price * p.Num);

                            paymentsTable.Cell(i + 2, 2).Range.Text =
                                sum.ToString("N2") + " руб.";
                        }

                        paymentsTable.Range.ParagraphFormat.SpaceAfter = 12;
                    }

                    // Разрыв страницы между пользователями (кроме последнего)
                    if (user != allUsers.Last())
                    {
                        document.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);
                    }
                }

                wordApp.Visible = true;
                MessageBox.Show("Экспорт в Word завершен успешно!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в Word: {ex.Message}");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminPage());
        }
    }
}
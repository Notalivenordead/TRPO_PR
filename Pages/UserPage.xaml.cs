using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace _222_Busin.Pages
{
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();
            UpdateUsers();
        }

        private void UpdateUsers()
        {
            var users = Entities.GetContext().User.ToList();

            // Фильтрация по ФИО
            if (!string.IsNullOrWhiteSpace(fioFilterTextBox.Text))
                users = users.Where(x => x.FIO != null && x.FIO.ToLower().Contains(fioFilterTextBox.Text.ToLower())).ToList();

            // Только админы
            if (onlyAdminCheckBox.IsChecked == true)
                users = users.Where(x => x.Role == "Admin").ToList();

            // Сортировка
            if (sortComboBox.SelectedIndex == 0)
                users = users.OrderBy(x => x.FIO).ToList();
            else
                users = users.OrderByDescending(x => x.FIO).ToList();

            ListUser.ItemsSource = users;
        }

        private void fioFilterTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateUsers();
        private void sortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateUsers();
        private void onlyAdminCheckBox_Checked(object sender, RoutedEventArgs e) => UpdateUsers();
        private void onlyAdminCheckBox_Unchecked(object sender, RoutedEventArgs e) => UpdateUsers();
        private void clearFiltersButton_Click_1(object sender, RoutedEventArgs e)
        {
            fioFilterTextBox.Text = "";
            sortComboBox.SelectedIndex = 0;
            onlyAdminCheckBox.IsChecked = false;
            UpdateUsers();
        }
    }

    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                    return new BitmapImage(new Uri("pack://application:,,,/images/default.jpg"));

                string imageName = value.ToString();

                if (imageName.StartsWith("http") || imageName.Contains(":\\"))
                    return new BitmapImage(new Uri(imageName));

                return new BitmapImage(new Uri($"pack://application:,,,/images/{imageName}"));
            }
            catch
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/default.jpg"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

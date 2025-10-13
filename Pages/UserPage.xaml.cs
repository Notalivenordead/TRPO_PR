using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
}

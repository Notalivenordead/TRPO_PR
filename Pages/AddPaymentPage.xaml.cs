using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace _222_Busin.Pages
{
    public partial class AddPaymentPage : Page
    {
        private Payment _currentPayment = new Payment();

        public AddPaymentPage(Payment selectedPayment)
        {
            InitializeComponent();

            CBCategory.ItemsSource = Entities.GetContext().Category.ToList();
            CBUser.ItemsSource = Entities.GetContext().User.ToList();

            if (selectedPayment != null)
                _currentPayment = selectedPayment;

            DataContext = _currentPayment;

            if (_currentPayment.ID == 0)
                _currentPayment.Date = DateTime.Now;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (_currentPayment.UserID == 0)
                errors.AppendLine("Выберите пользователя!");
            if (_currentPayment.CategoryID == 0)
                errors.AppendLine("Выберите категорию!");
            if (string.IsNullOrWhiteSpace(_currentPayment.Name))
                errors.AppendLine("Укажите название платежа!");
            if (_currentPayment.Date == null)
                errors.AppendLine("Укажите дату!");
            if (_currentPayment.Num <= 0)
                errors.AppendLine("Количество должно быть больше 0!");
            if (_currentPayment.Price <= 0)
                errors.AppendLine("Цена должна быть больше 0!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (_currentPayment.ID == 0)
                Entities.GetContext().Payment.Add(_currentPayment);

            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TBPaymentName.Text = "";
            TBCount.Text = "";
            TBPrice.Text = "";
            DPDate.SelectedDate = DateTime.Now;
            CBCategory.SelectedIndex = -1;
            CBUser.SelectedIndex = -1;
        }
    }
}
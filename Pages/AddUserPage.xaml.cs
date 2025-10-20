using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Security.Cryptography;

namespace _222_Busin.Pages
{
    public partial class AddUserPage : Page
    {
        private User _currentUser = new User();

        public AddUserPage(User selectedUser)
        {
            InitializeComponent();
            if (selectedUser != null)
                _currentUser = selectedUser;
            DataContext = _currentUser;
            if (!string.IsNullOrWhiteSpace(_currentUser.Role))
            {
                cmbRole.Text = _currentUser.Role;
            }
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentUser.Login)) errors.AppendLine("Укажите логин!");
            if (string.IsNullOrWhiteSpace(TBPass.Text)) errors.AppendLine("Укажите пароль!");
            if (string.IsNullOrWhiteSpace(cmbRole.Text)) errors.AppendLine("Выберите роль!");
            else _currentUser.Role = cmbRole.Text;
            if (string.IsNullOrWhiteSpace(_currentUser.FIO)) errors.AppendLine("Укажите ФИО");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Проверка пароля только для новых пользователей
            if (_currentUser.ID == 0 || !string.IsNullOrEmpty(TBPass.Text))
            {
                string password = TBPass.Text;

                if (password.Length < 6)
                {
                    MessageBox.Show("Пароль слишком короткий, должно быть минимум 6 символов!");
                    return;
                }

                bool en = true;
                bool number = false;
                foreach (char c in password)
                {
                    if (c >= '0' && c <= '9') number = true;
                    else if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))) en = false;
                }

                if (!en)
                {
                    MessageBox.Show("Используйте только английскую раскладку!");
                    return;
                }

                if (!number)
                {
                    MessageBox.Show("Добавьте хотя бы одну цифру!");
                    return;
                }

                // Хешируем пароль перед сохранением
                _currentUser.Password = GetHash(password);
            }

            if (_currentUser.ID == 0)
                Entities.GetContext().User.Add(_currentUser);

            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                NavigationService?.GoBack();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TBLogin.Text = "";
            TBPass.Text = "";
            cmbRole.SelectedIndex = -1;
            TBFio.Text = "";
            TBPhoto.Text = "";
        }
    }
}
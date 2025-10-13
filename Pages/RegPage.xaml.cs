using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace _222_Busin.Pages
{
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
            comboBxRole.SelectedIndex = 0;
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtbxLog.Text) || string.IsNullOrEmpty(txtbxFIO.Text) ||
                string.IsNullOrEmpty(passBxFrst.Password) || string.IsNullOrEmpty(passBxScnd.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            var db = Entities.GetContext();
            var existing = db.User.AsQueryable().AsNoTracking().FirstOrDefault(u => u.Login == txtbxLog.Text);
            if (existing != null)
            {
                MessageBox.Show("Пользователь с таким логином уже существует!");
                return;
            }

            // Проверки пароля
            if (passBxFrst.Password.Length < 6)
            {
                MessageBox.Show("Пароль слишком короткий, должно быть минимум 6 символов!");
                return;
            }

            bool en = true;
            bool number = false;
            foreach (char c in passBxFrst.Password)
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

            if (passBxFrst.Password != passBxScnd.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            // Создаём пользователя
            var userObject = new User
            {
                FIO = txtbxFIO.Text,
                Login = txtbxLog.Text,
                Password = GetHash(passBxFrst.Password),
                Role = (comboBxRole.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "User",
                Photo = null
            };

            db.User.Add(userObject);
            db.SaveChanges();

            MessageBox.Show("Пользователь успешно зарегистрирован!");
            txtbxLog.Clear();
            passBxFrst.Clear();
            passBxScnd.Clear();
            comboBxRole.SelectedIndex = 0;
            txtbxFIO.Clear();
        }
    }
}

using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace _222_Busin.Pages
{
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
        }

        private string GetHash(string s)
        {
            using (var sha = SHA1.Create())
                return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(s)).Select(x => x.ToString("X2")));
        }

        private void ButtonChange_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLogin.Text) ||
                string.IsNullOrWhiteSpace(TxtOldPass.Password) ||
                string.IsNullOrWhiteSpace(TxtNewPass.Password) ||
                string.IsNullOrWhiteSpace(TxtNewPassRepeat.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            var db = Entities.GetContext();
            var user = db.User.FirstOrDefault(x => x.Login == TxtLogin.Text);
            if (user == null)
            {
                MessageBox.Show("Пользователь не найден!");
                return;
            }

            if (user.Password != GetHash(TxtOldPass.Password))
            {
                MessageBox.Show("Старый пароль неверен!");
                return;
            }

            if (TxtNewPass.Password.Length < 6)
            {
                MessageBox.Show("Минимальная длина пароля — 6 символов!");
                return;
            }

            if (TxtNewPass.Password != TxtNewPassRepeat.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            user.Password = GetHash(TxtNewPass.Password);
            db.SaveChanges();
            MessageBox.Show("Пароль успешно изменён!");
            NavigationService?.GoBack();
        }
    }
}

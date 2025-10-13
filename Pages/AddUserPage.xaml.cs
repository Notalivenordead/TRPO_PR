using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

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

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentUser.Login)) errors.AppendLine("Укажите логин!");
            if (string.IsNullOrWhiteSpace(_currentUser.Password)) errors.AppendLine("Укажите пароль!");
            if (string.IsNullOrWhiteSpace(cmbRole.Text)) errors.AppendLine("Выберите роль!");
            else _currentUser.Role = cmbRole.Text;
            if (string.IsNullOrWhiteSpace(_currentUser.FIO)) errors.AppendLine("Укажите ФИО");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
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

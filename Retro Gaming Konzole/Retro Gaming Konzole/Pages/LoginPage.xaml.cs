using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Retro_Gaming_Konzole.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void seePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            passwordTextBox.Text = passwordBox.Password;
            if (passwordTextBox.Visibility == Visibility.Hidden)
            {
                passwordTextBox.Visibility = Visibility.Visible;
                passwordBox.Visibility = Visibility.Hidden;
            }
            else if (passwordTextBox.Visibility == Visibility.Visible) 
            {
                passwordTextBox.Visibility = Visibility.Hidden;
                passwordBox.Visibility = Visibility.Visible;
            }
        }

        public (string, string) sendData()
        {
            string password = passwordBox.Password;
            passwordBox.Password = passwordTextBox.Text = string.Empty; //prilikom logina, brisemo password iz memorije jer je to senzitivna vrednost
            return (usernameTextBox.Text, password);
        }

        public void clearInput() 
        { 
          usernameTextBox.Text = passwordBox.Password = passwordTextBox.Text = string.Empty; 
        }


    }
}

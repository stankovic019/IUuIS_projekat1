using Domain.Enums;
using Domain.Helpers;
using Domain.Models;
using Domain.Services;
using Notification.Wpf;
using Retro_Gaming_Konzole.Pages;
using Services.LoginService;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Retro_Gaming_Konzole
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotificationManager notificationManager;
        public ILoginService loginService;
        public User user;
        public LoginPage loginPage;
        public DataTablePage dataTablePage;

        public MainWindow()
        {
            InitializeComponent();
            loginService = new LoginService();
            notificationManager = new NotificationManager();
            loginPage = new LoginPage();
            MainFrame.Content = loginPage; //prvo treba prikazati LoginPage
            aGameBoyButton.Content = "Clear\nInput"; //dodato je ovde radi prelaza teksta u novi red, estetski

        }

        public void SendToastNotification(string title, string message, NotificationType type)
        {
            ShowToastNotification(new ToastNotification(title, message, type)); 
        }

        public void ShowToastNotification(ToastNotification toastNotification)
        {
            notificationManager.Show(toastNotification.Title, toastNotification.Message, toastNotification.Type, "WindowNotificationArea");
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {

            if(MainFrame.Content is LoginPage page)
            {
                string username, password;
                (username, password) = page.sendData();

                bool retVal;
                LoginErrors error;

                (retVal, user, error) = loginService.login(username, password);

                if (!retVal)
                {
                    if (error == LoginErrors.username)
                        SendToastNotification("Error", "Username is incorrect.", NotificationType.Error);
                    else if (error == LoginErrors.password)
                        SendToastNotification("Error", "Password is incorrect.", NotificationType.Error);

                    SendToastNotification("Try again.", "", NotificationType.Information);
                }
                else
                {
                    SendToastNotification($"Welcome {user.username}!", "Login success", NotificationType.Success);
                    dataTablePage = new DataTablePage(); //ako se uspesno logovao, otvara mi novi data table page, bilo da je tek usao u aplikaciju, ili se izlogovao, da bi mogle da se primene izmene itd
                    MainFrame.Content = dataTablePage;
                    exitButton.Content = "Log out";
                }
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {

            if (MainFrame.Content is LoginPage)
            {
                if (MessageBox.Show("Do you want to exit the application?", "Leaving?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    this.Close();
                
            }
            else
            {
                MainFrame.Content = loginPage;
                SendToastNotification($"Goodbye {user.username}!", "Logged out.", NotificationType.Information);
                exitButton.Content = "Exit";
            }
        }

        private void aGameBoyButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is LoginPage)
            { 
                loginPage.clearInput(); 
            }
        }
    }
}
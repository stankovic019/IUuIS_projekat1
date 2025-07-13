using Domain.Enums;
using Domain.Helpers;
using Domain.Models;
using Domain.Services;
using Microsoft.Win32;
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

        public ObservableCollection<RetroConsole> retroConsoles;
        public DataIO serializer = new DataIO();

        #region pages
        private Page currentPage;
        public LoginPage loginPage;
        public DataTablePage dataTablePage;
        public AddConsolePage addConsolePage;
        #endregion

        public List<string> pages = new List<string>
        {
            new string("DataTablePage"),
            new string("AddConsolePage")
        };

        public MainWindow()
        {
            InitializeComponent();
            loginService = new LoginService();
            notificationManager = new NotificationManager();
            loginPage = new LoginPage();
            MainFrame.Content = loginPage; //prvo treba prikazati LoginPage
            currentPage = loginPage;
            retroConsoles = serializer.DeSerializeObject<ObservableCollection<RetroConsole>>("RetroConsoles.xml");
            if(retroConsoles == null)
            {
                retroConsoles = new ObservableCollection<RetroConsole>();
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setButtonLabels();
        }

        public void SendToastNotification(string title, string message, NotificationType type)
        {
            ShowToastNotification(new ToastNotification(title, message, type)); 
        }

        public void ShowToastNotification(ToastNotification toastNotification)
        {
            notificationManager.Show(toastNotification.Title, toastNotification.Message, toastNotification.Type, "WindowNotificationArea");
        }

        private  void startButton_Click(object sender, RoutedEventArgs e)
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
                    dataTablePage = new DataTablePage(); //ako se uspesno logovao, otvara mi novi data table page, bilo da je tek usao u aplikaciju, ili se izlogovao, da bi mogle da se primene izmene itd
                    MainFrame.Content = dataTablePage;
                    currentPage = dataTablePage;
                    setButtonLabels();
                    SendToastNotification($"Welcome {user.username}!", "Login success", NotificationType.Success);
                }
                return;
            }

            if(MainFrame.Content is AddConsolePage)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Image Files | *.jpg; *.png;";
                ofd.InitialDirectory = "C:\\";
                ofd.Title = "Please import your Console image";

                if(ofd.ShowDialog() == true)
                {
                    addConsolePage.consoleImgPathTextBox.Text = ofd.FileName;
                    SendToastNotification("Import", "Image imported.", NotificationType.Information);
                }
                else
                {
                    SendToastNotification("Warning", "Failed to import image.", NotificationType.Warning);
                }
            }


        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {

            if (MainFrame.Content is LoginPage)
            {
                this.Close();
            }
            

            if(MainFrame.Content is DataTablePage) 
            {
                MainFrame.Content = loginPage;
                currentPage = loginPage;
                SendToastNotification($"Goodbye {user.username}!", "Logged out.", NotificationType.Information);
                setButtonLabels();
            }

            if(MainFrame.Content is AddConsolePage)
            {
                if(MessageBox.Show("Do you want to abort adding a new console?", "Abort", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    dataTablePage = new DataTablePage();
                    MainFrame.Content = dataTablePage;
                    currentPage = dataTablePage;
                    SendToastNotification("Aborted", "New console adding aborted", NotificationType.Notification);
                    setButtonLabels(); 
                }   
            }

        }

        private void aGameBoyButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is LoginPage)
            { 
                loginPage.clearInput(); 
            }

            if (MainFrame.Content is DataTablePage) {

                addConsolePage = new AddConsolePage();
                MainFrame.Content = addConsolePage;
                currentPage = addConsolePage;
                setButtonLabels();
            }

            if (MainFrame.Content is AddConsolePage)
            {
                RetroConsole console = addConsolePage.sendData();

                if (console != null)
                {
                    retroConsoles.Add(console);
                    dataTablePage = new DataTablePage();
                    MainFrame.Content = dataTablePage;
                    currentPage = dataTablePage;
                    setButtonLabels();
                }
            }
        }
        private void bGameBoyButton_Click(object sender, RoutedEventArgs e)
        {
           
            if (MainFrame.Content is DataTablePage)
            {
                ObservableCollection<RetroConsole> removedConsoles = new ObservableCollection<RetroConsole>();
                int num = 0;
                foreach (RetroConsole console in retroConsoles)
                    if (console.IsSelected)
                    {
                        num++;
                        removedConsoles.Add(console);
                    }

                foreach (RetroConsole console in removedConsoles)
                    retroConsoles.Remove(console);

                if (num > 0)
                    SendToastNotification("Success", $"Successfully removed {num} rows.", NotificationType.Success);
                else
                    SendToastNotification("Error", "Can't remove rows if none is selected.", NotificationType.Error);
                
            }

            if (MainFrame.Content is AddConsolePage) {
                
                if(MessageBox.Show("Do you want to clear all the data from cells?\n" +
                    "WARNING: THIS IS IRREVERSIBLE!", "Clear all", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    addConsolePage.clearAll();
                    SendToastNotification("Cleared", "All cells clear.", NotificationType.Notification);
                }
            }

        }

        private void upGameBoyButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is DataTablePage) {
                //PORADI NA SELECT ALL, OSTALO ZA SAD RADI
               // dataTablePage.selectOrDeselectAll(); 
            }
        }

        private void SaveDataAsXML()
        {
            serializer.SerializeObject<ObservableCollection<RetroConsole>>(retroConsoles, "RetroConsoles.xml");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (MessageBox.Show("Do you want to exit the application?", "Leaving?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SaveDataAsXML();
            }
            else
            {
                e.Cancel = true;
            }

        }


        private void setButtonLabels()
        {
            if(currentPage is LoginPage)
            {
                startButton.Content = "Log in";
                exitButton.Content = "Exit";
                aGameBoyButton.Content = "Clear\nInput";
                bGameBoyButton.Content = "B";
                return;
            }

            if(currentPage is DataTablePage)
            {
                startButton.Content = "Start";
                exitButton.Content = "Log out";
                aGameBoyButton.Content = "Add";
                bGameBoyButton.Content = "Remove";
                return;
            }

            if(currentPage is AddConsolePage)
            {
                startButton.Content = "Import";
                exitButton.Content = "Abort";
                aGameBoyButton.Content = "Add";
                bGameBoyButton.Content = "Clear\nAll";
                return;

            }

        }

        
    }
}
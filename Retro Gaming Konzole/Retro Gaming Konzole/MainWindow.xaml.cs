using Domain.Enums;
using Domain.Helpers;
using Domain.Models;
using Domain.Services;
using Microsoft.Win32;
using Notification.Wpf;
using Retro_Gaming_Konzole.Pages;
using Services.LoginService;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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
        public EditConsolePage editConsolePage;
        public ViewConsolePage viewConsolePage;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            loginService = new LoginService();
            notificationManager = new NotificationManager();
            loginPage = new LoginPage();
            MainFrame.Content = loginPage; //prvo treba prikazati LoginPage
            currentPage = loginPage;
            retroConsoles = serializer.DeSerializeObject<ObservableCollection<RetroConsole>>("RetroConsoles.xml");
            if (retroConsoles == null)
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

        public void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is LoginPage page)
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

            if (MainFrame.Content is DataTablePage)
            {
                if (user.role == UserRole.Admin)
                {
                    if (!dataTablePage.allSelected)
                    {
                        dataTablePage.selectAll();
                        SendToastNotification("Select All", $"All rows ({retroConsoles.Count}) selected.", NotificationType.Notification);
                        startButton.Content = "Deselect All";
                    }
                    else
                    {
                        dataTablePage.deselectAll();
                        SendToastNotification("Deselect All", $"All rows ({retroConsoles.Count}) deselected.", NotificationType.Notification);
                        startButton.Content = "Select All";
                    }
                }
                else
                    SendToastNotification("\"Select All\" Not permitted", $"You don't have permission to select rows.", NotificationType.Error);

            }

            if (MainFrame.Content is AddConsolePage)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Image Files | *.jpg; *.png;";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                ofd.Title = "Please import your Console image";

                if (ofd.ShowDialog() == true)
                {
                    addConsolePage.consoleImgPathTextBox.Text = ofd.FileName;
                    SendToastNotification("Import", "Image imported.", NotificationType.Information);
                }
                else
                {
                    SendToastNotification("Warning", "Failed to import image.", NotificationType.Warning);
                }
            }

            if (MainFrame.Content is EditConsolePage)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Image Files | *.jpg; *.png;";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                ofd.Title = "Please import your Console image";

                if (ofd.ShowDialog() == true)
                {
                    editConsolePage.consoleImgPathTextBox.Text = ofd.FileName;
                    SendToastNotification("Import", "Image imported.", NotificationType.Information);
                }
                else
                {
                    SendToastNotification("Warning", "Failed to import image.", NotificationType.Warning);
                }
            }

            if (MainFrame.Content is ViewConsolePage)
            {
                dataTablePage = new DataTablePage();
                MainFrame.Content = dataTablePage;
                currentPage = dataTablePage;
                setButtonLabels();
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {

            if (MainFrame.Content is LoginPage)
            {
                this.Close();
            }


            if (MainFrame.Content is DataTablePage)
            {
                if (MessageBox.Show("Do you want to log out?", "Log out?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    MainFrame.Content = loginPage;
                    currentPage = loginPage;
                    SendToastNotification($"Goodbye {user.username}!", "Logged out.", NotificationType.Information);
                    setButtonLabels();
                }
            }

            if (MainFrame.Content is AddConsolePage)
            {
                if (MessageBox.Show("Do you want to abort adding a new console?", "Abort", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    dataTablePage = new DataTablePage();
                    MainFrame.Content = dataTablePage;
                    currentPage = dataTablePage;
                    SendToastNotification("Aborted", "New console adding aborted", NotificationType.Notification);
                    setButtonLabels();
                }
            }

            if (MainFrame.Content is EditConsolePage)
            {
                if (MessageBox.Show("Do you want to abort adding a new console?", "Abort", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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

            if (MainFrame.Content is DataTablePage)
            {
                if (user.role == UserRole.Admin)
                {
                    addConsolePage = new AddConsolePage();
                    MainFrame.Content = addConsolePage;
                    currentPage = addConsolePage;
                    setButtonLabels();
                }
                else
                    SendToastNotification("\"Add\" Not permitted", "You don't have permission to add new consoles.", NotificationType.Error);
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

            if (MainFrame.Content is EditConsolePage)
            {
                bool retVal = editConsolePage.returnData();

                if (retVal)
                {
                    dataTablePage = new DataTablePage();
                    MainFrame.Content = dataTablePage;
                    currentPage = dataTablePage;
                    setButtonLabels();
                }
            }
        }
        private void bGameBoyButton_Click(object sender, RoutedEventArgs e)
        {

            if (MainFrame.Content is LoginPage)
            {

                SendToastNotification("About Application", "Created by Dimitrije Stankovic, PR81/2022\n" +
                                                "FTN, Novi Sad, 2024/25.", NotificationType.Information);
            }


            if (MainFrame.Content is DataTablePage)
            {
                if (user.role == UserRole.Admin)
                {
                    ObservableCollection<RetroConsole> removedConsoles = new ObservableCollection<RetroConsole>();
                    foreach (RetroConsole console in retroConsoles)
                        if (console.IsSelected)
                            removedConsoles.Add(console);

                    if (removedConsoles.Count > 0)
                    {
                        if (MessageBox.Show($"Are you sure you want to delete {removedConsoles.Count} {(removedConsoles.Count == 1 ? "row" : "rows")}?\n" +
                            $"WARNING: THIS IS IRREVERSIBLE!", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            int num = 0;
                            foreach (RetroConsole console in removedConsoles)
                            {
                                try
                                {
                                    File.Delete(console.rtfPath);
                                    retroConsoles.Remove(console);
                                    num++;
                                }
                                catch (Exception ex)
                                {
                                    SendToastNotification("\"RTF\" Error", $"Can't delete {console.name}.rtf file.\n" +
                                                                             $"Maybe it's used by another process.", NotificationType.Error);
                                }
                            }
                            if (num > 0)
                                SendToastNotification("Success", $"Successfully removed {num} {(num == 1 ? "row" : "rows")}.", NotificationType.Success);
                        }
                    }
                    else
                        SendToastNotification("Error", "Can't remove rows if none is selected.", NotificationType.Error);
                }
                else
                    SendToastNotification("\"Remove\" Not permitted", "You do not have permission to delete rows from the table.", NotificationType.Error);
            }

            if (MainFrame.Content is AddConsolePage)
            {

                if (MessageBox.Show("Do you want to clear all the data from cells?\n" +
                    "WARNING: THIS IS IRREVERSIBLE!", "Clear all", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    addConsolePage.clearAll();
                    SendToastNotification("Cleared", "All cells clear.", NotificationType.Notification);
                }
            }

            if (MainFrame.Content is EditConsolePage)
            {

                if (MessageBox.Show("Do you want to clear all the data from cells?\n" +
                    "WARNING: THIS IS IRREVERSIBLE!", "Clear all", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    editConsolePage.clearAll();
                    SendToastNotification("Cleared", "All cells clear.", NotificationType.Notification);
                }
            }
        }

        public void hyperLink(RetroConsole _console)
        {

            if (user.role == UserRole.Admin)
            {
                try
                {
                    editConsolePage = new EditConsolePage(_console);
                    MainFrame.Content = editConsolePage;
                    currentPage = editConsolePage;
                    setButtonLabels();
                }
                catch (Exception e)
                {
                    SendToastNotification("\"RTF\" Error", $"Can't open {_console.name}.rtf file.\n" +
                                                                             $"Maybe it's used by another process.", NotificationType.Error);
                }
            }
            else
            {
                try
                {
                    viewConsolePage = new ViewConsolePage(_console);
                    MainFrame.Content = viewConsolePage;
                    currentPage = viewConsolePage;
                    setButtonLabels();
                }
                catch (Exception e)
                {
                    SendToastNotification("\"RTF\" Error", $"Can't open {_console.name}.rtf file.\n" +
                                                                             $"Maybe it's used by another process.", NotificationType.Error);
                }

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
            if (currentPage is LoginPage)
            {
                startButton.Content = "Log in";
                startButton.ToolTip = "Log into application";
                exitButton.Content = "Exit";
                exitButton.ToolTip = "Exit application";
                aGameBoyButton.Content = "Clear\nInput";
                aGameBoyButton.ToolTip = "Clear all text from username and password boxes";
                bGameBoyButton.Content = "About";
                bGameBoyButton.ToolTip = "About application";
                return;
            }

            if (currentPage is DataTablePage)
            {
                startButton.Content = "Select All";
                startButton.ToolTip = "Select or Deselect all rows in table";
                exitButton.Content = "Log out";
                exitButton.ToolTip = "Log out from application";
                aGameBoyButton.Content = "Add";
                aGameBoyButton.ToolTip = "Add new row";
                bGameBoyButton.Content = "Remove";
                bGameBoyButton.ToolTip = "Remove selected rows from table";
                return;
            }

            if (currentPage is AddConsolePage)
            {
                startButton.Content = "Import";
                startButton.ToolTip = "Import image";
                exitButton.Content = "Abort";
                exitButton.ToolTip = "Discard all and go back to the table page";
                aGameBoyButton.Content = "Add";
                aGameBoyButton.ToolTip = "Add new console to the table";
                bGameBoyButton.Content = "Clear\nAll";
                bGameBoyButton.ToolTip = "Clears all text boxes";
                return;

            }

            if (currentPage is EditConsolePage)
            {
                startButton.Content = "Import";
                startButton.ToolTip = "Import image";
                exitButton.Content = "Abort";
                exitButton.ToolTip = "Discard all and go back to the table page";
                aGameBoyButton.Content = "Save";
                aGameBoyButton.ToolTip = "Save data and override the console";
                bGameBoyButton.Content = "Clear\nAll";
                bGameBoyButton.ToolTip = "Clears all text boxes";
                return;
            }

            if (currentPage is ViewConsolePage)
            {
                startButton.Content = "Exit Preview";
                startButton.ToolTip = "Exit preview and go back to the table page";
                exitButton.Content = "";
                exitButton.ToolTip = "";
                aGameBoyButton.Content = "";
                aGameBoyButton.ToolTip = "";
                bGameBoyButton.Content = "";
                bGameBoyButton.ToolTip = "";
                return;
            }

        }


    }
}
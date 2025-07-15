using Domain.Models;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for EditConsolePage.xaml
    /// </summary>
    public partial class EditConsolePage : Page
    {
        private RetroConsole retroConsole;
        MainWindow mainWindow;
        public EditConsolePage(RetroConsole retroConsole)
        {
            InitializeComponent();
            this.retroConsole = retroConsole;
            mainWindow = (MainWindow)Application.Current.MainWindow;
            LoadSystemColors();
            FontFamilyComboBox.SelectedIndex = 51;
            FontSizeComboBox.SelectedIndex = 2;
            FontColorComboBox.SelectedIndex = 7;

            consoleNameTextBox.Text = retroConsole.name;
            consoleReleaseYearTextBox.Text = retroConsole.consoleReleaseYear.ToString(); 
            consoleImgPathTextBox.Text = retroConsole.imgPath;

            if (File.Exists(retroConsole.imgPath))
            {
                previewImage.Source = new BitmapImage(new Uri(retroConsole.imgPath));
            }

            if (File.Exists(retroConsole.rtfPath))
            {
                using (FileStream fs = new FileStream(retroConsole.rtfPath, FileMode.Open, FileAccess.Read))
                {
                    var range = new TextRange(EditorRichTextBox.Document.ContentStart, EditorRichTextBox.Document.ContentEnd);
                    range.Load(fs, DataFormats.Rtf);
                }
            }
        }

        private void LoadSystemColors()
        {
            FontColorComboBox.ItemsSource = typeof(Colors)
                .GetProperties()
                .Select(p => new SolidColorBrush((Color)p.GetValue(null)));
        }

        //change font
        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem != null && !EditorRichTextBox.Selection.IsEmpty)
            {
                
                EditorRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, FontFamilyComboBox.SelectedItem);
            }
        }

        //change size
        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeComboBox.SelectedItem != null && !EditorRichTextBox.Selection.IsEmpty)
            {
                ComboBoxItem selectedItem = FontSizeComboBox.SelectedItem as ComboBoxItem;

                if (selectedItem != null && double.TryParse(selectedItem.Content.ToString(), out double size))
                   
                    EditorRichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, size);
            }

        }

        //change color
        private void FontColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontColorComboBox.SelectedItem != null)
            {
                Brush brush = (Brush)FontColorComboBox.SelectedItem;

                if (!EditorRichTextBox.Selection.IsEmpty)
                {
                    EditorRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
                }
            }
        }

        //word count
        private void EditorRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = new TextRange(EditorRichTextBox.Document.ContentStart, EditorRichTextBox.Document.ContentEnd).Text;
            int wordCount = Regex.Matches(text, @"\b\p{L}+\b").Count; //regex: from edge to edge, and counting alphanumeric signs without blanks
            WordCountTextBlock.Text = $"Words: {wordCount}";
        }


        private void EditorRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object fontBold = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            BoldToggleButton.IsChecked = (fontBold != DependencyProperty.UnsetValue) && (fontBold.Equals(FontWeights.Bold));

            object fontItalic = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            ItalicToggleButton.IsChecked = (fontItalic != DependencyProperty.UnsetValue) && (fontItalic.Equals(FontStyles.Italic));

            object fontUnderline = EditorRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineToggleButton.IsChecked = (fontUnderline != DependencyProperty.UnsetValue) && (fontUnderline.Equals(TextDecorations.Underline));

            object fontFamily = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            FontFamilyComboBox.SelectedItem = fontFamily;

            object fontSize = EditorRichTextBox.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            FontSizeComboBox.SelectedItem = fontSize;

            object fontColor = EditorRichTextBox.Selection.GetPropertyValue(TextElement.ForegroundProperty);
            FontColorComboBox.SelectedItem = fontColor;


        }

        public void clearAll()
        {
            consoleNameTextBox.Text = consoleReleaseYearTextBox.Text = consoleImgPathTextBox.Text = string.Empty;
            TextRange range = new TextRange(EditorRichTextBox.Document.ContentStart, EditorRichTextBox.Document.ContentEnd);
            EditorRichTextBox.Document.Blocks.Clear();
        }

        public bool returnData()
        {
            bool oneOrMoreError = false;

            if (consoleNameTextBox.Text == string.Empty)
            {
                mainWindow.SendToastNotification("\"Name\" Error", "Console must have name.", Notification.Wpf.NotificationType.Error);
                oneOrMoreError = true;
            }

            if (consoleReleaseYearTextBox.Text == string.Empty)
            {
                mainWindow.SendToastNotification("\"Release Year\" Error", "Console must have release year.", Notification.Wpf.NotificationType.Error);
                oneOrMoreError = true;
            }

            if (!Regex.IsMatch(consoleReleaseYearTextBox.Text, "^\\d+$"))
            {
                mainWindow.SendToastNotification("\"Release Year\" Error", "Console release year format is wrong.", Notification.Wpf.NotificationType.Error);
                oneOrMoreError = true;
            }

            if (consoleImgPathTextBox.Text == string.Empty)
            {
                mainWindow.SendToastNotification("\"Image\" Error", "Image is not selected.", Notification.Wpf.NotificationType.Error);
                oneOrMoreError = true;
            }

            TextRange range = new TextRange(EditorRichTextBox.Document.ContentStart, EditorRichTextBox.Document.ContentEnd);

            if (string.IsNullOrEmpty(range.Text.ToString().Trim()))
            {
                mainWindow.SendToastNotification("\"Rich Text Box\" Error", "Rich Text Box can't be empty.", Notification.Wpf.NotificationType.Error);
                oneOrMoreError = true;
            }

            if (!oneOrMoreError)
            {
                string path = $"rtfs\\{consoleNameTextBox.Text}.rtf";

                //if the name is the same, so is the name of the file
                if (consoleNameTextBox.Text == retroConsole.name || !File.Exists(path)) 
                {
                    try
                    {
                        //try to delete old rtf file
                        File.Delete(retroConsole.rtfPath); 
                    }
                    catch (Exception ex) 
                    {
                        mainWindow.SendToastNotification("\"RTF\" Error", $"Can't delete {retroConsole.name}.rtf file.\n" +
                                                                             $"Maybe it's used by another process.", NotificationType.Error);
                        return false;
                    }
                    

                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        range.Save(fs, DataFormats.Rtf);
                    }

                    retroConsole.name = consoleNameTextBox.Text;
                    retroConsole.imgPath = consoleImgPathTextBox.Text;
                    retroConsole.rtfPath = path;
                    retroConsole.consoleReleaseYear = Convert.ToInt32(consoleReleaseYearTextBox.Text);

                    mainWindow.SendToastNotification("Success", "The console fields have been edited successfully.", Notification.Wpf.NotificationType.Success);

                    return true;
                }
                else
                    mainWindow.SendToastNotification("\"Name\" Error", "Console with the same new name already exists. Please, change it.", Notification.Wpf.NotificationType.Error);
            }


            return false;


        }

        private void consoleImgPathTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            mainWindow.startButton_Click(this, new RoutedEventArgs());
        }

        private void consoleImgPathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                previewImage.Source = new BitmapImage(new Uri(consoleImgPathTextBox.Text, UriKind.Absolute));
            }
            catch (Exception ex)
            {
                previewImage.Source = null;
            }
        }

    }
}

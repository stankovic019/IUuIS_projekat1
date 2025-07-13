using Domain.Helpers;
using Domain.Models;
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
    /// Interaction logic for AddConsolePage.xaml
    /// </summary>
    public partial class AddConsolePage : Page
    {
        MainWindow mainWindow;

        public AddConsolePage()
        {
            InitializeComponent();
            LoadSystemColors();
            FontFamilyComboBox.SelectedIndex = 51;
            FontSizeComboBox.SelectedIndex = 2;
            FontColorComboBox.SelectedIndex = 7;
            mainWindow = (MainWindow)Application.Current.MainWindow;
        }

        //ucitavam sve sistemske boje
        private void LoadSystemColors()
        {
            FontColorComboBox.ItemsSource = typeof(Colors)
                .GetProperties()
                .Select(p => new SolidColorBrush((Color)p.GetValue(null)));
        }

        //menjam font
        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem != null && !EditorRichTextBox.Selection.IsEmpty)
            {
                //ako je nesto selektovano, menjamo mu font
                EditorRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, FontFamilyComboBox.SelectedItem);
            }
        }

        //menjam velicinu
        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeComboBox.SelectedItem != null && !EditorRichTextBox.Selection.IsEmpty)
            {
                ComboBoxItem selectedItem = FontSizeComboBox.SelectedItem as ComboBoxItem;

                if (selectedItem != null && double.TryParse(selectedItem.Content.ToString(), out double size))
                    //ako je nesto selektovano primenjujemo na njemu
                    EditorRichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, size);
            }

        }

        //menjam boju
        private void FontColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontColorComboBox.SelectedItem != null)
            {
                Brush brush = (Brush)FontColorComboBox.SelectedItem;

                if (!EditorRichTextBox.Selection.IsEmpty)
                {
                    // ako je nešto selektovano, menjamo boju toga
                    EditorRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
                }
            }
        }

        //brojim reci
        private void EditorRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = new TextRange(EditorRichTextBox.Document.ContentStart, EditorRichTextBox.Document.ContentEnd).Text;
            int wordCount = Regex.Matches(text, @"\b\p{L}+\b").Count; //regex znaci da krene "od ivice do ivice" (\b) i broji sve znakove koji su slova, brojevi ili _ (\p{L})
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
            consoleNameTextBox.Text = consoleImgPathTextBox.Text = string.Empty;
            consoleExpirienceComboBox.SelectedIndex = -1;
            TextRange range = new TextRange(EditorRichTextBox.Document.ContentStart, EditorRichTextBox.Document.ContentEnd);
            EditorRichTextBox.Document.Blocks.Clear();
        }


        public RetroConsole sendData()
        {
            RetroConsole retroConsole = null;
            bool oneOrMoreError = false;

            if(consoleNameTextBox.Text == string.Empty)
            {
                mainWindow.SendToastNotification("Error", "Console must have name.", Notification.Wpf.NotificationType.Error);
                oneOrMoreError = true;
            }

            if (consoleExpirienceComboBox.SelectedIndex == -1)
            {
                mainWindow.SendToastNotification("Error", "You must choose Console Expirience.", Notification.Wpf.NotificationType.Error);
                oneOrMoreError = true;
            }

            if (consoleImgPathTextBox.Text == string.Empty) {
                mainWindow.SendToastNotification("Error", "You must choose picture", Notification.Wpf.NotificationType.Error);
                oneOrMoreError = true;
            }

            if (!oneOrMoreError) 
            {
                TextRange range = new TextRange(EditorRichTextBox.Document.ContentStart, EditorRichTextBox.Document.ContentEnd);
                string path = $"{consoleNameTextBox.Text}.rtf";

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    range.Save(fs, DataFormats.Rtf);
                }

                retroConsole = new RetroConsole(consoleNameTextBox.Text, consoleImgPathTextBox.Text, path, consoleExpirienceComboBox.SelectedIndex + 1);

                mainWindow.SendToastNotification("Success", "The console was successfully added to the table", Notification.Wpf.NotificationType.Success);
            }

            return retroConsole;

        }



    }
}

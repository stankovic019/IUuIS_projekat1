using Domain.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Retro_Gaming_Konzole.Pages
{
    /// <summary>
    /// Interaction logic for ViewConsolePage.xaml
    /// </summary>
    public partial class ViewConsolePage : Page
    {
        private RetroConsole retroConsole;
        MainWindow mainWindow;

        public ViewConsolePage(RetroConsole retroConsole)
        {
            InitializeComponent();
            this.retroConsole = retroConsole;

            consoleNameTextBlock.Text = retroConsole.name;
            consoleReleaseYearTextBlock.Text = retroConsole.consoleReleaseYear.ToString();
            dateCreatedTextBlock.Text = retroConsole.date;

            int wordCount = 0;



            if (File.Exists(retroConsole.imgPath))
            {
                previewImage.Source = new BitmapImage(new Uri(retroConsole.imgPath));
            }

            if (File.Exists(retroConsole.rtfPath))
            {

                //we need rich text box because of the "Document" property that he has, so we first
                //need to open rtb to load all the data and then that document to send to viewer

                RichTextBox tempRtb = new RichTextBox();
                using (FileStream fs = new FileStream(retroConsole.rtfPath, FileMode.Open, FileAccess.Read))
                {
                    TextRange range = new TextRange(tempRtb.Document.ContentStart, tempRtb.Document.ContentEnd);
                    range.Load(fs, DataFormats.Rtf);
                }

                // A FlowDocument instance can only be hosted in a single control at a time.
                // This approach creates a new FlowDocument by copying the content,
                // so it is not tied to any previous visual element.
                FlowDocument newDoc = new FlowDocument();
                TextRange sourceRange = new TextRange(tempRtb.Document.ContentStart, tempRtb.Document.ContentEnd);
                TextRange targetRange = new TextRange(newDoc.ContentStart, newDoc.ContentEnd);

                using (MemoryStream stream = new MemoryStream())
                {
                    sourceRange.Save(stream, DataFormats.Xaml);
                    stream.Position = 0;
                    targetRange.Load(stream, DataFormats.Xaml);
                }

                rtfViewer.Document = newDoc;
            }
            string text = new TextRange(rtfViewer.Document.ContentStart, rtfViewer.Document.ContentEnd).Text;
            wordCount = Regex.Matches(text, @"\b\p{L}+\b").Count; //regex: from edge to edge, and counting alphanumeric signs without blanks
            WordCountTextBlock.Text = $"Words: {wordCount}";
        }
    }
}

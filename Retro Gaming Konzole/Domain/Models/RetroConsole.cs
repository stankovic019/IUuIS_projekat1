using System.ComponentModel;

namespace Domain.Models
{
    [Serializable]
    public class RetroConsole : INotifyPropertyChanged
    {

        public string name { get; set; } //tekstualno polje koje nije deo rtf
        public string imgPath { get; set; } //put ka slici
        public string rtfPath { get; set; } //put ka rtf
        public int consoleReleaseYear { get; set; } //brojevno polje koje mora uneti korisnik
        public string date { get; set; } //datum dodavanja

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public RetroConsole()
        {
        }

        public RetroConsole(string name, string imgPath, string rtfPath, int consoleReleaseYear)
        {
            this.name = name;
            this.imgPath = imgPath;
            this.rtfPath = rtfPath;
            this.consoleReleaseYear = consoleReleaseYear;
            this.date = DateTime.Now.ToString("dd.MM.yyyy.");
            this.IsSelected = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}

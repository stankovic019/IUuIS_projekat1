using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [Serializable]
    public class Console
    {

        public string name { get; set; } //tekstualno polje koje nije deo rtf
        public string imgPath { get; set; } //put ka slici
        public string rtfPath { get; set; } //put ka rtf
        public int consoleExpirience { get; set; } //brojevno polje, u ovom slucaju ocena gejming iskustva nad konzolom (brojevi 1-5; 1-lose iskustvo; 5-odlicno)
        public string date {  get; set; } //datum dodavanja

        public Console()
        {
        }

        public Console(string name, string imgPath, string rtfPath, int consoleExpirience)
        {
            this.name = name;
            this.imgPath = imgPath;
            this.rtfPath = rtfPath;
            this.consoleExpirience = consoleExpirience;
            this.date = DateTime.Now.Date.ToString();
        }
    }
}

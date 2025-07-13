using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [Serializable]
    public class RetroConsole 
    {

        
        public bool IsSelected { get; set; }
        public string name { get; set; } //tekstualno polje koje nije deo rtf
        public string imgPath { get; set; } //put ka slici
        public string rtfPath { get; set; } //put ka rtf
        public int consoleExpirience { get; set; } //brojevno polje, u ovom slucaju ocena gejming iskustva nad konzolom (brojevi 1-5; 1-lose iskustvo; 5-odlicno)
        public string expirience { get; set; }
        public string date {  get; set; } //datum dodavanja

        public RetroConsole()
        {
        }

        public RetroConsole(string name, string imgPath, string rtfPath, int consoleExpirience)
        {
            this.IsSelected = false;
            this.name = name;
            this.imgPath = imgPath;
            this.rtfPath = rtfPath;
            this.consoleExpirience = consoleExpirience;
            this.date = DateTime.Now.Date.ToString().Split(" ")[0];

        }

    }
}

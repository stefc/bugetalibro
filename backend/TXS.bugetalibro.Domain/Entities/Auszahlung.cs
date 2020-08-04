using System;
using TXS.bugetalibro.Domain.ValueObjects;

namespace TXS.bugetalibro.Domain.Entities
{    
    public class Auszahlung : Buchung
    {
        public Kategorie Kategorie { get;  private set; }
        public string Notiz { get;  private set; }

        public Auszahlung(DateTime datum, decimal betrag, Kategorie kategorie, string notiz) : base(datum, betrag)
        {
            this.Kategorie = kategorie ?? throw new ArgumentNullException(nameof(kategorie));
            this.Notiz = notiz;
        }
    }
}

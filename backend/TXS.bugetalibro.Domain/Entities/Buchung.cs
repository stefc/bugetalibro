using System;

namespace TXS.bugetalibro.Domain.Entities
{    
    public abstract class Buchung
    {
        public Guid Id { get; private set; }
        public DateTime Datum { get;  private set; }
        public decimal Betrag { get;  private set; }

        private Buchung(){}
        public Buchung(DateTime datum, decimal betrag)
        {
            if (betrag <= 0m)
                throw new ArgumentOutOfRangeException(nameof(betrag), "Betrag muss > 0 sein");
            
            if (datum.TimeOfDay.Milliseconds > 0)
                throw new ArgumentOutOfRangeException(nameof(datum), "Nur Datümer ohne Zeitanteil zulässig");

            this.Id = Guid.NewGuid();
            this.Datum = datum;
            this.Betrag = betrag;
        }
    }
}

using System;

namespace TXS.bugetalibro.Domain.Entities
{
    public abstract class Buchung
    {
        public Buchung(DateTime datum, decimal betrag)
        {
            if (betrag <= 0m)
                throw new ArgumentOutOfRangeException(nameof(betrag), "Betrag muss > 0 sein");

            if (datum.TimeOfDay.TotalMilliseconds > 0)
                throw new ArgumentOutOfRangeException(nameof(datum), "Nur Datümer ohne Zeitanteil zulässig");

            this.Id = Guid.NewGuid();
            this.Datum = datum;
            this.Betrag = betrag;
        }

        public Guid Id { get; }
        public DateTime Datum { get; }
        public decimal Betrag { get; }
    }
}

using System;

namespace TXS.bugetalibro.Domain.Entities
{
    public class Einzahlung : Buchung
    {
        public Einzahlung(DateTime datum, decimal betrag)
            : base(datum, betrag)
        {
        }
    }
}

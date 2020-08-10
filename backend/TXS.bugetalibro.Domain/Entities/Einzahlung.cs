using System;

namespace TXS.bugetalibro.Domain.Entities
{
    public class Einzahlung : Buchung
    {
        private Einzahlung() {}
        
        public Einzahlung(DateTime datum, decimal betrag)
            : base(datum, betrag)
        {
        }
    }
}

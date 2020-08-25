using System;
using System.Linq;
using TXS.bugetalibro.Domain.Entities;

namespace TXS.bugetalibro.Domain.Logic
{
    public class BalanceQueryFacade
    {
        private readonly IQueryable<Einzahlung> einzahlungen;
        private readonly IQueryable<Auszahlung> auszahlungen;

        public BalanceQueryFacade(IQueryable<Einzahlung> einzahlungen, IQueryable<Auszahlung> auszahlungen) 
        {
            this.einzahlungen = einzahlungen;
            this.auszahlungen = auszahlungen;
        }
        
        public decimal GetBalanceAt(DateTime date)
            => this.einzahlungen.Where( x => x.Datum < date ).Sum( x => x.Betrag) 
            -  this.auszahlungen.Where( x => x.Datum < date ).Sum( x => x.Betrag);


        public decimal GetCredits((DateTime start, DateTime end) range)
            => this.einzahlungen.Where( x => (x.Datum >= range.start) && (x.Datum <= range.end)).Sum( x => x.Betrag);
    }
}

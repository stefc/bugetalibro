using System;
using System.Linq;
using TXS.bugetalibro.Domain.Entities;

namespace TXS.bugetalibro.Domain.Facades
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
    }
}

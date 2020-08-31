using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Echo;
using MediatR;
using TXS.bugetalibro.Application.Models;
using TXS.bugetalibro.Application.UseCases;

namespace TXS.bugetalibro.ConsoleApp.Commands
{
    using static Echo.Process;

    [Verb("übersicht", HelpText = "Hilfe Text zur Übersicht")]
    public class ÜbersichtCommand : BaseCommand
    {
        [Value(0, MetaName = "monat", HelpText = "Monat", Required = false)]
        
        public int? Monat { get; set; }

        [Value(1, MetaName = "jahr", HelpText = "Jahr", Required = false)]
        public int? Jahr { get; set; }

        internal override async Task ExecuteAsync(IMediator mediator, ProcessId logger, CancellationToken cancellationToken)
        {
            var übersicht = await mediator.Send(new GetÜbersicht.Request { Monat = this.Monat, Jahr = this.Jahr }, cancellationToken);
            ToViewModel(übersicht).Select( _ => tell(logger, _));
        }

        private IEnumerable<string> ToViewModel(ÜberblickModel übersicht)
        {
            var output = ImmutableList.Create<(string,decimal)>()
                .Add(("Kassenbestand (Monatsanfang)", übersicht.StartSaldo))
                .Add(("Einzahlungen", übersicht.SummeEinzahlungen))
                .Add(("Kassenbestand (Monatsende)", übersicht.EndSaldo))
                .Where( ((string _, decimal amount) x) => x.amount >= 0);

            int maxCaption = output.Max( ((string caption, decimal _) x) => x.caption.Length);
            int maxAmount = output.Max( ((string _, decimal amount) x) => x.amount.ToString("C").Length);

            int widthSeparator = 3;
            var separator = new String(' ',widthSeparator);
            int width = maxCaption + widthSeparator + maxAmount;
           
            return output
                .Select( ((string caption, decimal amount) x) => 
                    new StringBuilder(width)
                        .Append(x.caption.Align(maxCaption))
                        .Append(separator)
                        .Append(x.amount.ToString("C").Align(-maxAmount))
                        .ToString())
                .Prepend( new String('-', width))
                .Prepend( new DateTime(übersicht.Jahr, übersicht.Monat, 1).ToString("MMMM yyyy"));
        }
    }
}

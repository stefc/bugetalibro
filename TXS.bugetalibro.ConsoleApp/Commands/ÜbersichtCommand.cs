using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using MediatR;
using TXS.bugetalibro.Application.UseCases;
using TXS.Shared.Extensions;

namespace TXS.bugetalibro.ConsoleApp.Commands
{
    [Verb("übersicht", HelpText = "Hilfe Text zur Übersicht")]
    public class ÜbersichtCommand : BaseCommand
    {
        [Value(0, MetaName = "monat", HelpText = "Monat", Required = false)]
        
        public int? Monat { get; set; }

        [Value(1, MetaName = "jahr", HelpText = "Jahr", Required = false)]
        public int? Jahr { get; set; }

        internal override async Task ExecuteAsync(IMediator mediator, CancellationToken cancellationToken)
        {
            var request = new GetÜbersicht.Request { Monat = this.Monat, Jahr = this.Jahr };
            var übersicht = await mediator.Send(request, cancellationToken);

            var date = new DateTime(übersicht.Jahr, übersicht.Monat, 1);
            Console.WriteLine($"{date:MMMM yyyy}");

            var output = ImmutableList.Create<(string,decimal)>()
                .Add(("Kassenbestand (Monatsanfang)", übersicht.StartSaldo))
                .Add(("Einzahlungen", übersicht.SummeEinzahlungen))
                .Add(("Kassenbestand (Monatsende)", übersicht.EndSaldo))
                .Where( ((string _, decimal amount) x) => x.amount >= 0);

            int maxCaption = output.Max( ((string caption, decimal _) x) => x.caption.Length);
            int maxAmount = output.Max( ((string _, decimal amount) x) => x.amount.ToString("C").Length);

            Console.WriteLine(new String('-', maxCaption + maxAmount + 1));
            output.ForEach(((string caption, decimal amount) x) => 
                Console.WriteLine($"{x.caption.Align(maxCaption)} {x.amount.ToString("C").Align(-maxAmount)}"));
        }
    }
}

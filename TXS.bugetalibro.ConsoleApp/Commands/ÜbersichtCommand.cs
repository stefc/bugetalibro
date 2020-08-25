using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using MediatR;
using TXS.bugetalibro.Application.UseCases;

namespace TXS.bugetalibro.ConsoleApp.Commands
{
    [Verb("übersicht", HelpText = "Hilfe Text zur Übersicht")]
    public class ÜbersichtCommand : BaseCommand
    {
        [Option('m', "monat", HelpText = "Monat")]
        public int? Monat { get; set; }

        [Option('j', "jahr", HelpText = "Jahr")]
        public int? Jahr { get; set; }

        internal override async Task ExecuteAsync(IMediator mediator, CancellationToken cancellationToken)
        {
            var request = new GetÜbersicht.Request { Monat = this.Monat, Jahr = this.Jahr };
            var übersicht = await mediator.Send(request, cancellationToken);

            var date = new DateTime(übersicht.Jahr, übersicht.Monat, 1);
            Console.WriteLine($"{date:MMMM yyyy}");
            Console.WriteLine(new String('-', 20));
            
            Console.WriteLine($"Kassenbestand (Monatsanfang): {übersicht.StartSaldo:C}");
            if (übersicht.SummeEinzahlungen > 0m)
            {
                Console.WriteLine($"Einzahlungen: {übersicht.SummeEinzahlungen:C}");
            }
            Console.WriteLine($"Kassenbestand (Monatsende): {übersicht.EndSaldo:C}");
        }
    }
}

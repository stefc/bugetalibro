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
        internal override async Task ExecuteAsync(IMediator mediator, CancellationToken cancellationToken)
        {
            var request = new GetÜbersicht.Request { };
            var übersicht = await mediator.Send(request, cancellationToken);

            var date = new DateTime(übersicht.Jahr, übersicht.Monat, 1);
            Console.WriteLine(date.ToString("MMMM yyyy"));
            Console.WriteLine(new String('-',20));
            Console.WriteLine($"Kassenbestand (Monatsanfang): {übersicht.StartSaldo.ToString("C2", CultureInfo.CreateSpecificCulture("de-DE"))}");
            Console.WriteLine($"Kassenbestand (Monatsende):   {übersicht.EndSaldo.ToString("C2", CultureInfo.CreateSpecificCulture("de-DE"))}");
        }
    }
}

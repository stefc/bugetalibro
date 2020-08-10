using System;
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

            Console.WriteLine("TBD - Ausgabe der Übersicht");
        }
    }
}

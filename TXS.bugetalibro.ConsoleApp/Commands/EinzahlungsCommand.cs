using System;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using MediatR;
using TXS.bugetalibro.Application;
using TXS.bugetalibro.Application.UseCases;

namespace TXS.bugetalibro.ConsoleApp.Commands
{
    [Verb("einzahlung", HelpText = "Hilfe Text zur Einzahlung")]
    public class EinzahlungsCommand : BaseCommand
    {
        [Option('d', "datum",
            HelpText = "Einzahlungsdatum (format yyyy-MM-dd z.B.: --datum=2018-05-31 oder -d 2018-05-31")]
        public DateTime? Datum { get; set; }

        [Option('b', "betrag", HelpText = "Betrag der eingezahlt werden soll.")]
        public decimal Betrag { get; set; }

        internal override async Task Execute(IMediator mediator, CancellationToken cancellationToken)
        {
            var request = new CreateEinzahlung.Request {Betrag = this.Betrag, Datum = this.Datum ?? DateTime.Today};
            var kassenbestand = await mediator.Send(request, cancellationToken);

            Console.WriteLine($"Kassenbestand: {kassenbestand:N2} EUR");
        }
    }
}

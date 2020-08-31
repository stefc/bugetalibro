using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Echo;
using MediatR;
using TXS.bugetalibro.Application.UseCases;

namespace TXS.bugetalibro.ConsoleApp.Commands
{
    using static Echo.Process;

    [Verb("einzahlung", HelpText = "Hilfe Text zur Einzahlung")]
    public class EinzahlungsCommand : BaseCommand
    {
        [Option('d', "datum",
            HelpText = "Einzahlungsdatum (format yyyy-MM-dd z.B.: --datum=2018-05-31 oder -d 2018-05-31")]
        public DateTime? Datum { get; set; }

        [Value(0, MetaName = "betrag", HelpText = "Betrag der eingezahlt werden soll.", Required = true)]
        public decimal Betrag { get; set; }

        internal override async Task ExecuteAsync(IMediator mediator, ProcessId logger, CancellationToken cancellationToken)
        {
            var request = new CreateEinzahlung.Request {Betrag = this.Betrag, Datum = this.Datum ?? DateTime.Today};
            var kassenbestand = await mediator.Send(request, cancellationToken);
            ToViewModel(kassenbestand).Select( _ => tell(logger, _));
        }

        private IEnumerable<string> ToViewModel(decimal saldo)
        {
            yield return $"{"Kassenbestand".Align(20)} {saldo.ToString("C").Align(-20)}";
        }
    }
}

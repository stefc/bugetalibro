using System;
using System.Threading.Tasks;
using CommandLine;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TXS.bugetalibro.Application;

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

        internal override async Task Execute(IServiceProvider serviceProvider)
        {
            var request = new CreateEinzahlung.Request {Betrag = this.Betrag, Datum = this.Datum ?? DateTime.Today};
            await serviceProvider.GetService<IMediator>().Send(request);
        }
    }
}

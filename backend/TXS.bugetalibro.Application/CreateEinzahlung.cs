
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TXS.bugetalibro.Domain.Entities;

namespace TXS.bugetalibro.Application
{   
    public static class CreateEinzahlung
	{
		public class Request : IRequest<decimal>
		{
            public decimal Betrag { get; set; }
            public DateTime? Date { get; set; }
        }

		internal class Handler : IRequestHandler<Request, decimal>
		{
			public Task<decimal> Handle(Request request, CancellationToken cancellationToken)
            {
                var einzahlungsDatum = request.Date ?? DateTime.Now;
                var einzahlung = new Einzahlung(einzahlungsDatum, request.Betrag);

                // using (var scope = this.unitOfWork.BeginWrite())
                // {
                // 	// scope.Instituts.Insert(request.Id, new { request.More });
                // 	await scope.CompleteAsync();
                // 	return Unit.Value;
                // }

                var kassenbestand = 0m;
                return Task.FromResult(kassenbestand);
            }
		}
	}
}

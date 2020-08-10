using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using TXS.bugetalibro.Domain.Entities;

namespace TXS.bugetalibro.Application.UseCases
{
    public static class CreateEinzahlung
    {
        public class Request : IRequest<decimal>
        {
            public decimal Betrag { get; set; }
            public DateTime Datum { get; set; }
        }

        internal class Handler : IRequestHandler<Request, decimal>
        {
            public Task<decimal> Handle(Request request, CancellationToken cancellationToken)
            {
                var einzahlung = new Einzahlung(request.Datum, request.Betrag);

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

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                this.RuleFor(req => req.Betrag).GreaterThan(0m);
                this.RuleFor(req => req.Datum).NotEmpty();
            }
        }
    }
}

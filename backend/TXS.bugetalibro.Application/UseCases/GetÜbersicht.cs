using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Application.Models;

namespace TXS.bugetalibro.Application.UseCases
{
    public static class GetÜbersicht
    {
        public class Request : IRequest<ÜberblickModel>
        { 
            public int? Monat { get; set; }
            public int? Jahr { get; set; }
        }

        internal class Handler : IRequestHandler<Request, ÜberblickModel>
        {
            //private readonly IUnitOfWork unitOfWork;
            private readonly IDateProvider dateProvider;

            public Handler(IDateProvider dateProvider /*, IUnitOfWork unitOfWork*/)
            {
                this.dateProvider = dateProvider;
               // this.unitOfWork = unitOfWork;
            }

            public Task<ÜberblickModel> Handle(Request request, CancellationToken cancellationToken)
            {
                // using (var scope = this.unitOfWork.Begin())
                {
                    var hasDatum = request.Monat.HasValue && request.Jahr.HasValue; 

                    var datum = hasDatum ? 
                        new DateTime(request.Jahr.Value, request.Monat.Value, 1) : this.dateProvider.Today;
                    
                    var überblickModel = new ÜberblickModel() { 
                        Monat = datum.Month, 
                        Jahr = datum.Year 
                    };
                    return Task.FromResult(überblickModel);
                }
            }
        }
    }
}

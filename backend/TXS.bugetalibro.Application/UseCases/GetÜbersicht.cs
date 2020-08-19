using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Application.Contracts.Data;
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
            private readonly IDataStore dataStore;
            private readonly IDateProvider dateProvider;

            public Handler(IDateProvider dateProvider, IDataStore dataStore)
            {
                this.dateProvider = dateProvider;
                this.dataStore = dataStore;
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

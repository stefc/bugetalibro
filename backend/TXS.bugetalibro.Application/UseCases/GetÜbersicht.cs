using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TXS.bugetalibro.Application.Models;

namespace TXS.bugetalibro.Application.UseCases
{
    public static class GetÜbersicht
    {
        public class Request : IRequest<ÜberblickModel>
        { }

        internal class Handler : IRequestHandler<Request, ÜberblickModel>
        {
            //private readonly IUnitOfWork unitOfWork;

            public Handler(/*IUnitOfWork unitOfWork*/)
            {
               // this.unitOfWork = unitOfWork;
            }

            public Task<ÜberblickModel> Handle(Request request, CancellationToken cancellationToken)
            {
                // using (var scope = this.unitOfWork.Begin())
                {
                    var überblickModel = new ÜberblickModel();
                    
                    return Task.FromResult(überblickModel);
                }
            }
        }
    }
}

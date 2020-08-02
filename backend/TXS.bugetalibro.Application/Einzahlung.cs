
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TXS.bugetalibro.Application.Contracts.Data;

namespace TXS.bugetalibro.Application
{   
    public class CreateEinzahlung
	{
		public class Request : IRequest
		{
			public int Id { get; set; }
			public string More { get; set; }
		}

		internal class Handler : IRequestHandler<Request>
		{
			private readonly IUnitOfWork unitOfWork;

			public Handler(IUnitOfWork unitOfWork)
			{
				this.unitOfWork = unitOfWork;
			}

			public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
			{
				using (var scope = this.unitOfWork.BeginWrite())
				{
					// scope.Instituts.Insert(request.Id, new { request.More });
					await scope.CompleteAsync();
					return Unit.Value;
				}
			}
		}

		
	}

}

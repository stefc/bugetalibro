using System;
using System.Threading.Tasks;

namespace TXS.bugetalibro.Application.Contracts.Data
{
	public interface IUnitOfWork
	{
		IUnitOfWorkScope Begin();
		IUnitOfWorkWriteScope BeginWrite();
	}

	public interface IUnitOfWorkScope : IDisposable
	{
	}

	public interface IUnitOfWorkWriteScope : IUnitOfWorkScope
	{
		Task<int> CompleteAsync();
	}
}

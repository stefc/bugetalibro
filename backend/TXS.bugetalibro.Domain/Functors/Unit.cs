using System;
using Unit = System.ValueTuple;

namespace TXS.bugetalibro.Domain.Functors
{
	public static partial class F
	{
		public static Unit unit() => default(Unit);
		public static Func<T, T> identity<T>() => x => x;
	}
}

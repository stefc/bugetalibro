// https://gist.github.com/dadhi/59cfc698f6dc6e31b722cd804aae185a
//  
// Monadic IO implementation, can be reused, published to NuGet, etc.
//-------------------------------------------------------------------
using System;
using Unit = System.ValueTuple;

namespace TXS.bugetalibro.Domain.Functors
{
    using static F;

    public interface IO<A>
    {
        IO<B> Bind<B>(Func<A, IO<B>> f);
    }

    public sealed class Return<A> : IO<A>
    {
        public readonly A Result;
        public Return(A a) => Result = a;

        public IO<B> Bind<B>(Func<A, IO<B>> f) => f(Result);
    }

    public class IO<I, O, A> : IO<A>
    {
        public readonly I Input;
        public readonly Func<O, IO<A>> Next;
        public IO(I input, Func<O, IO<A>> next) => (Input, Next) = (input, next);

        public IO<B> Bind<B>(Func<A, IO<B>> f) => new IO<I, O, B>(Input, r => Next(r).Bind(f));
    }

    public static class IOMonad
    {
        public static IO<A> Lift<A>(this A a) =>
            new Return<A>(a);

        public static IO<B> Select<A, B>(this IO<A> m, Func<A, B> f) =>
            m.Bind(a => f(a).Lift());

        public static IO<C> SelectMany<A, B, C>(this IO<A> m, Func<A, IO<B>> f, Func<A, B, C> project) =>
            m.Bind(a => f(a).Bind(b => project(a, b).Lift()));
    }

    public static partial class LinqExtensions
    {
        public static IO<R> ToIO<I, R>(this I input) => new IO<I, R, R>(input, IOMonad.Lift);
        public static IO<Unit> ToIO<I>(this I input) => input.ToIO<I, Unit>();

        public static IO<A> Ignore<I, A>(this IO<I, Unit, A> x) => x.Next(unit());

        public static IO<A> As<I, O, A>(this IO<I, O, A> x, Func<I, O> process) => x.Next(process(x.Input));

        public static IO<A> As<I, A>(this IO<I, Unit, A> x, Action<I> process)
        {
            process(x.Input);
            return x.Ignore();
        }
    }
}

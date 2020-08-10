using System;
using System.Threading.Tasks;

namespace TXS.bugetalibro.ConsoleApp.Commands
{
    /// <summary>
    ///     Basisklasse, die ein Konsolenkommando beschreibt
    /// </summary>
    public abstract class BaseCommand
    {
        /// <summary>
        ///     Ausführen eines CLI-Kommandos
        /// </summary>
        /// <param name="serviceProvider"></param>
        internal abstract Task Execute(IServiceProvider serviceProvider);
    }
}

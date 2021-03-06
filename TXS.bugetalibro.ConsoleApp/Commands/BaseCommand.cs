﻿using System.Threading;
using System.Threading.Tasks;
using Echo;
using MediatR;

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
        internal abstract Task ExecuteAsync(IMediator mediator, ProcessId logger, CancellationToken cancellationToken);
    }
}

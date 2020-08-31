using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using CommandLine;
using FluentValidation;
using MediatR;
using TXS.bugetalibro.ConsoleApp.Commands;
using Echo;

namespace TXS.bugetalibro.ConsoleApp.Infrastructure
{

    using static Echo.Process;

    internal class CommandLauncher
    {
        private readonly IMediator mediator;

        private readonly ProcessId logger;
        private readonly Type[] commandTypes;
        private readonly StringWriter helpWriter;
        private readonly Parser commandParser;

        public CommandLauncher(IMediator mediator)
        {
            this.mediator = mediator;
            this.logger = spawn<string>("logger", Console.WriteLine);
            this.helpWriter = new StringWriter();
            this.commandTypes = this.GetType().Assembly.GetTypes()
                .Where(p => p.IsClass && !p.IsNested && p.GetCustomAttributes(typeof(VerbAttribute), false).Any())
                .ToArray();
            this.commandParser = new Parser(settings =>
            {
                settings.HelpWriter = this.helpWriter;
                settings.IgnoreUnknownArguments = true;
            });
        }

        public async Task ExecuteCommandAsync(string[] args, CancellationToken cancellationToken)
        {
            try
            {
                await this.commandParser
                    .ParseArguments(args, this.commandTypes)
                    .WithNotParsed(this.Help)
                    .WithParsedAsync<BaseCommand>(cmd => cmd.ExecuteAsync(this.mediator, this.logger, cancellationToken));
            }
            catch (ValidationException e)
            {
                tell(this.logger, e.Message);
            }
        }

        private void Help(IEnumerable<Error> errors)
        {
            if (errors.Any(e => e.Tag == ErrorType.VersionRequestedError))
            {
                var version = Assembly.GetEntryAssembly()?
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                    .InformationalVersion ?? "0.0.0.0";

                tell(this.logger, $"Version: {version}");
            }
            else
                tell(this.logger, this.helpWriter);
        }
    }
}

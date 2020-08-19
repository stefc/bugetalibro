using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TXS.bugetalibro.ConsoleApp.Commands;

namespace TXS.bugetalibro.ConsoleApp.Infrastructure
{
    internal class CommandLauncher
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly Type[] commandTypes;
        private readonly StringWriter helpWriter;
        private readonly Parser commandParser;

        public CommandLauncher(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
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
                using (var scope = this.scopeFactory.CreateScope()) {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await this.commandParser
                        .ParseArguments(args, this.commandTypes)
                        .WithNotParsed(this.Help)
                        .WithParsedAsync<BaseCommand>(cmd => cmd.ExecuteAsync(mediator, cancellationToken));
                }
            }
            catch (ValidationException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        private void Help(IEnumerable<Error> errors)
        {
            if (errors.Any(e => e.Tag == ErrorType.VersionRequestedError))
            {
                var version = Assembly.GetEntryAssembly()?
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                    .InformationalVersion ?? "0.0.0.0";
                
                Console.WriteLine($"Version: {version}");
            }
            else
                Console.WriteLine(this.helpWriter);
        }        
    }
}

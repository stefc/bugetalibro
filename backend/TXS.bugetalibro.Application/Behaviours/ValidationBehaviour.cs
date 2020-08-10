﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using TXS.Shared.Extensions;

namespace TXS.bugetalibro.Application.Behaviours
{
    internal class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext(request);

            var failures = this.validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(failure => failure != null)
                .ToReadOnly();

            if (failures.Any())
                throw new ValidationException($"{this.BuildFailureMessage(failures)}", failures);

            return next();
        }

        private string BuildFailureMessage(IEnumerable<ValidationFailure> failures)
        {
            return string.Join(";", failures.Select(x => x.ErrorMessage));
        }
    }
}

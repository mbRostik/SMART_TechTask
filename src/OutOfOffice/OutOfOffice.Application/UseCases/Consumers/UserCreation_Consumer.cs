using MassTransit;
using MediatR;
using MessageBus.Messages;
using Microsoft.Extensions.Logging;
using OutOfOffice.Application.UseCases.Commands;
using OutOfOffice.Domain;
using OutOfOffice.Domain.Employees;
using OutOfOffice.Domain.Employees.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menu.Application.UseCases.Consumers
{
    public class UserCreation_Consumer : IConsumer<UserCreationEvent>
    {
        private readonly IMediator mediator;
        public UserCreation_Consumer(IMediator _mediator)
        {
            mediator = _mediator;
        }
        public async Task Consume(ConsumeContext<UserCreationEvent> context)
        {
            Console.WriteLine($"Successfully consumed UserCreationEvent");

            UnRegisteredUser temp = new UnRegisteredUser
            { 
                Id = context.Message.UserId,
                UserName=context.Message.UserName
            };
            await mediator.Send(new CreateUnRegisteredUserCommand(temp));
        }
    }
}

using System;
using System.Reflection;
using System.Threading.Tasks;
using Actio.Common.Commands;
using Actio.Common.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using RawRabbit.Instantiation;
using RawRabbit.Pipe;

namespace Actio.Common.RabbitMq
{
  public static class Extensions
  {
    public static Task WithCommandHandlerAsync<TCommand>(this IBusClient bus, ICommandHandler<TCommand> handler) where TCommand : ICommand
    => bus.SubscribeAsync<TCommand>(msg => handler.HandleAsync(msg),
        ctx => ctx.UseConsumerConfiguration(cfg =>
        cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<TCommand>())))
    );

    public static Task WithEventHandlerAsync<TEvent>(this IBusClient bus, IEventHandler<TEvent> handler) where TEvent : IEvent
    => bus.SubscribeAsync<TEvent>(msg => handler.HandleAsync(msg),
        ctx => ctx.UseConsumerConfiguration(cfg =>
        cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<TEvent>())))
    );

    private static string GetQueueName<T>()
    {
      return $"{Assembly.GetEntryAssembly().GetName()}/{typeof(T).Name}";
    }

    public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
      if (configuration == null) throw new ArgumentNullException(nameof(configuration));

      var options = new RabbitMqOptions();
      var section = configuration.GetSection("RabbitMq");
      section.Bind(options);

      using (var client = RawRabbitFactory.CreateSingleton(new RawRabbitOptions
      {
        ClientConfiguration = options,
        DependencyInjection = ioc => ioc.AddSingleton(LoggingFactory.ApplicationLogger),
        Plugins = p => p
          .UseStateMachine()
          .UseGlobalExecutionId()
      }))
      {
        services.AddSingleton<IBusClient>(_ => client);
      }
    }
  }
}
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using InvestimentosRendaVariavel.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
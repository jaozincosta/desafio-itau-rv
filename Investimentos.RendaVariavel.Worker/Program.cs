using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using InvestimentosRendaVariavel.Worker;
using InvestimentosRendaVariavel.Services;
using System.Net.Http;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<CotacaoExternaService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

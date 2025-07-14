using System;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using InvestimentosRendaVariavel.Worker;
using InvestimentosRendaVariavel.Services;
using InvestimentosRendaVariavel.DbContexto;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connectionString = builder.GetConnectionString("DefaultConnection");

        services.AddHttpClient<CotacaoExternaService>();
        services.AddDbContext<InvestimentoContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();

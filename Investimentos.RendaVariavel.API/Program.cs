using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using InvestimentosRendaVariavel.Services;
using InvestimentosRendaVariavel.DbContexto;

var builder = WebApplication.CreateBuilder(args);

//Adiciona servi�os da aplica��o
builder.Services.AddControllers();
builder.Services.AddHttpClient<CotacaoExternaService>();

//Adiciona o DbContext (ajuste a connection string conforme necess�rio)
builder.Services.AddDbContext<InvestimentoContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

//Registra o InvestimentoService
builder.Services.AddScoped<InvestimentoService>();

// Configura Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Investimentos Renda Vari�vel - API",
        Version = "v1"
    });
});

var app = builder.Build();

//Configura pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

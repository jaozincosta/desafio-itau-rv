using Microsoft.OpenApi.Models;
using InvestimentosRendaVariavel.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços da aplicação
builder.Services.AddControllers();
builder.Services.AddHttpClient<CotacaoExternaService>();

// Configura Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Investimentos Renda Variável - API",
        Version = "v1"
    });
});

var app = builder.Build();

// Configura pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

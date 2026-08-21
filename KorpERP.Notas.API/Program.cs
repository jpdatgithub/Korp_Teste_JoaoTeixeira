using MassTransit;
using KorpERP.Notas.API.Persistence;
using Microsoft.EntityFrameworkCore;
using KorpERP.Notas.API.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<NotasDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProdutoCriadoConsumer>();
    x.AddConsumer<ProdutoAtualizadoConsumer>();
    x.AddConsumer<EstoqueAtualizadoConsumer>();
    x.AddConsumer<ProdutoDesativadoConsumer>();
    x.AddConsumer<ProcessamentoDeNotaConcluidoConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

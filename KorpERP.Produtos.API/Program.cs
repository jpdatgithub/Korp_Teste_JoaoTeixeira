using KorpERP.Produtos.API.Persistence;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using KorpERP.Produtos.API.Interfaces;
using KorpERP.Produtos.API.Infrastructure;
using KorpERP.Produtos.API.Services;
using KorpERP.Produtos.API.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IProdutoService, ProdutoService>();

builder.Services.AddDbContext<ProdutosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<NotaFiscalProcessadaConsumer>();

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

builder.Services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

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

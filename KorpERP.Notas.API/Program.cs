using MassTransit;
using KorpERP.Notas.API.Persistence;
using Microsoft.EntityFrameworkCore;
using KorpERP.Notas.API.Consumers;
using KorpERP.Notas.API.Interfaces;
using KorpERP.Notas.API.Services;
using KorpERP.Notas.API.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDevelopment", policy =>
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddScoped<IProdutoProjectionService, ProdutoProjectionService>();
builder.Services.AddScoped<INotasService, NotasService>();
builder.Services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

builder.Services.AddDbContext<NotasDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProdutoCriadoConsumer>();
    x.AddConsumer<ProdutoAtualizadoConsumer>();
    x.AddConsumer<EstoqueAtualizadoConsumer>(consumer =>
    {
        consumer.UseDelayedRedelivery(redelivery =>
            redelivery.Intervals(
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(3)));

        consumer.UseMessageRetry(retry =>
            retry.Immediate(2));
    });
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

app.UseCors("AngularDevelopment");

app.UseAuthorization();

app.MapControllers();

app.Run();

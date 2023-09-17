using Microsoft.Extensions.Configuration;
using Producer.ConfigModel;
using Producer.MessageQueque;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region RabbitMQ configurations

builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection(RabbitMQConfig.SectionName));

builder.Services.AddSingleton<IMessageQuequeProducer, RabbitMQProducer>();
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

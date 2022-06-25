using ShopApi.Data;
using ShopApi.Repositories;
using ShopApi.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Context>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>(); 

builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

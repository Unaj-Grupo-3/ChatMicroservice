using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Custom
var connectionString = "";
Console.WriteLine(Directory.GetCurrentDirectory());
var gab = "C:\\Users\\Gabo\\Documents\\Backup\\unaj\\ProyectoDeSoftware_1\\2023-Primer-cuatri\\Grupal\\AppDeCitas\\UserMicroService2\\Template2\\Template2";
var fran = @"C:\Users\LopezFranco\Desktop\Proyecto Sofware 2023\ExpressoDelasDiez\UserMicroService2\Template2\Template2";

if (Directory.GetCurrentDirectory() == gab)
{
    connectionString =
        builder.Configuration["ConnectionString2"];
}
if (Directory.GetCurrentDirectory() == fran)
{
    connectionString =
        builder.Configuration["DefaultConnection"];
}
else
{
    // MSSQL running locally
    connectionString = builder.Configuration["ConnectionString"];
}

Console.WriteLine(connectionString);
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

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

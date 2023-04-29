using Application.Interface;
using Application.Interfaces;
using Application.UseCases;
using Infrastructure.Commands;
using Infrastructure.Persistence;
using Infrastructure.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(policy =>
{
    policy.AddDefaultPolicy(options => options.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Expreso de las diez - Microservicio de Chat", Version = "v1" });

    //Boton Autorize (Swagger)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Jwt Authorization",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

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


builder.Services.AddDbContext<ChatAppContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddTransient<IMessageCommands, MessageCommands>();
builder.Services.AddTransient<IMessageQueries, MessageQueries>();
builder.Services.AddTransient<IMessageServices, MessageServices>();

builder.Services.AddTransient<IChatCommands, ChatCommands>();
builder.Services.AddTransient<IChatQueries, ChatQueries>();
builder.Services.AddTransient<IChatServices, ChatServices>();

builder.Services.AddTransient<ITokenServices, TokenServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
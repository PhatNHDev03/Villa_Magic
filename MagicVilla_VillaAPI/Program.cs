

using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Reflection;
using MagicVilla_VillaAPI.Repository.IRepository;
using MagicVilla_VillaAPI.Repository;
/*using Serilog;*/


var builder = WebApplication.CreateBuilder(args);
// config log dung seriLog vao cehck log
// tao logg o day log debug tao ra file text
// rollingInterval thoi gian ma no chay la no se sua dung hoai trong 1 ngay
/*var logFilePath = "Log/villaLog.txt";
if (File.Exists(logFilePath))
{
    File.Delete(logFilePath); // Xóa file log cũ trước khi tạo logger mới
}

Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
    .WriteTo.File("Log/villaLog.txt",rollingInterval:RollingInterval.Day).CreateLogger();


builder.Host.UseSerilog();*/



builder.Services.AddDbContext<ApplicationDbContext>(o => {
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
});
// Add services to the container.
builder.Services.AddControllers();
// add service patch vao 
// dotnet Microsoft.aspNetcore.JsonPatch
//          Microsoft.AspNetCore.Mvc.NewtonsoftJson
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddScoped<IVillaRepository,VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
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

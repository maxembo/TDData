using System.Net.Mime;
using TDData.Settings;
using Microsoft.Net.Http.Headers;
using Serilog;
using TDData.Mappings;
using TDData.Middleware;
using TDData.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.Configure<DaDataSettings>(builder.Configuration.GetSection("DaData"));
builder.Services.AddAutoMapper(typeof(AddressProfile));
builder.Services.AddScoped<AddressService>();

builder.Services.AddHttpClient<AddressService>(client =>
{
    var settings = builder.Configuration.GetSection("DaData").Get<DaDataSettings>();
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            var uri = new Uri(origin);
            return uri.Host == "localhost";
        });
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.Run();
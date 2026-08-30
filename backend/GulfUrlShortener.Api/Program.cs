using GulfUrlShortener.Api.Application;
using GulfUrlShortener.Api.Application.Interfaces;
using GulfUrlShortener.Api.Application.Options;
using GulfUrlShortener.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddCors(options => options.AddPolicy("frontend", policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.Configure<ShortUrlOptions>(builder.Configuration.GetSection("ShortUrl"));
builder.Services.AddSingleton<ILinkRepository, InMemoryLinkRepository>();
builder.Services.AddSingleton<IShortCodeGenerator, RandomShortCodeGenerator>();
builder.Services.AddSingleton<LinkService>();

var app = builder.Build();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.UseExceptionHandler();
app.UseCors("frontend");
app.MapControllers();
app.Run();

public partial class Program { }

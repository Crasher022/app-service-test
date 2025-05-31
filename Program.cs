var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello from .NET 8 running on Azure App Service!");
app.MapGet("/", () => "This is release V 1.1");
app.MapGet("/", () => "We are on our second deployment");

app.Run();
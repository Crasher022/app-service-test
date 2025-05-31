var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => @"
    Hello from .NET 8 running on Azure App Service!
    This is release V 2.0 Final
");


app.Run();

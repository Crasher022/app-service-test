var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => @"
    <html>
        <body>
            <h2>Hello from .NET 8 running on Azure App Service!</h2>
            <p>This is release V 1.1</p>
            <p>We are on our second deployment</p>
        </body>
    </html>
");

app.Run();

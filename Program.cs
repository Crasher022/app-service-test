
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

bool burning = false;
var cts = new CancellationTokenSource();

app.MapGet("/", () => Results.Content(@"
    <html>
    <head>
        <title>CPU Burn Test</title>
        <script>
            async function startBurn() {
                await fetch('/start');
            }
            async function stopBurn() {
                await fetch('/stop');
            }
            async function updateCPU() {
                const res = await fetch('/cpu');
                const text = await res.text();
                document.getElementById('cpu').innerText = text + '%';
            }
            setInterval(updateCPU, 1000);
        </script>
    </head>
    <body>
        <h2>Hello from .NET 8 running on Azure App Service!</h2>
        <p>This is release V 1.3</p>
        <p>We are on our third deployment</p>

        <h2>CPU Burn Test App</h2>
        <button onclick='startBurn()'>Start Burn</button>
        <button onclick='stopBurn()'>Stop Burn</button>
        <p>CPU Usage: <span id='cpu'>...</span></p>
    </body>
    </html>", "text/html")
);

app.MapGet("/start", () => {
    if (!burning)
    {
        burning = true;
        cts = new CancellationTokenSource();
        _ = Task.Run(() => {
            while (!cts.Token.IsCancellationRequested)
            {
                var sw = Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < 100) { }
                Thread.Sleep(50); // Prevent 100% pegging
            }
        });
    }
    return Results.Ok("Burn started");
});

app.MapGet("/stop", () => {
    if (burning)
    {
        burning = false;
        cts.Cancel();
    }
    return Results.Ok("Burn stopped");
});

app.MapGet("/cpu", () => {
    var process = Process.GetCurrentProcess();
    var startTime = DateTime.UtcNow;
    var startCpuUsage = process.TotalProcessorTime;
    Thread.Sleep(500); // measurement window
    var endTime = DateTime.UtcNow;
    var endCpuUsage = process.TotalProcessorTime;

    var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
    var totalMsPassed = (endTime - startTime).TotalMilliseconds;
    var cpuUsageTotal = (cpuUsedMs / (Environment.ProcessorCount * totalMsPassed)) * 100;

    return Math.Round(cpuUsageTotal, 2);
});


app.Run();

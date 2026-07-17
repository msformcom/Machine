using ExeLocal;
using Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region Four
// Créer un objet FourOptions en désérialisant la section FourOptions
var fourOptions = builder.Configuration.GetRequiredSection("FourOptions").Get<FourOptions>();
builder.Services.AddSingleton(fourOptions!);

builder.Services.AddLogging(builder =>

{

    var logger = new LoggerConfiguration().MinimumLevel.Debug()
                                                  .WriteTo.EventLog("Application")
                                                //.WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                                                .CreateLogger();

    builder.AddSerilog();

});

// Constructeur de FourLocal => besoin de FourOptions
builder.Services.AddSingleton<IFour, FourLocal>(s =>
{
    var f=new FourLocal(s.GetRequiredService<FourOptions>(),s.GetService<ILogger<FourLocal>>());
    f.ValeurValide = (c => c < 10);
    return f;
});

#endregion
var app = builder.Build();


app.MapGet("/", () => "Hello World!");
app.MapGet("/GetTemperature", async (IFour four) =>
{
    return await four.GetTemperature();
});
app.MapGet("/GetHistorique", async (IFour four) =>
{
    var a = DateTime.Now;
    var b = $"Date : {a:yyyy-MM-dd}";
    b = String.Format("Date : {0:yyyy-MM-dd} {1:C}", a, 12);
    return await four.GetHistorique();
});

app.Run();

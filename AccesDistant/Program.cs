using ExeLocal;
using Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

#region Four
// Créer un objet FourOptions en désérialisant la section FourOptions
var fourOptions = builder.Configuration.GetRequiredSection("FourOptions").Get<FourOptions>();
builder.Services.AddSingleton(fourOptions!);

// Constructeur de FourLocal => besoin de FourOptions
builder.Services.AddSingleton<IFour, FourLocal>();

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

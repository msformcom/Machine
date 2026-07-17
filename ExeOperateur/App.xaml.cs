using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;
using ExeLocal;
using FourDistant;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExeOperateur
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }
        static App()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            #region Config
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            serviceCollection.AddSingleton<IConfiguration>(config);
            #endregion

            #region FourLocal
            // Créer un objet FourOptions en désérialisant la section FourOptions
            //var fourOptions = config.GetRequiredSection("FourOptions").Get<FourOptions>();
            //serviceCollection.AddSingleton(fourOptions!);

            //// Constructeur de FourLocal => besoin de FourOptions
            //serviceCollection.AddSingleton<IFour, FourLocal>();

            #endregion

            #region FourWeb

            serviceCollection.AddSingleton<HttpClient>(s=> {
                // Cette fonction sert à créer le sincleton lorsqu'il est demandé
                // On dispose à ce moment là du IServiceProvider
                var uri = s.GetRequiredService<IConfiguration>().GetSection("FourHttpUrl").Value;
                var client = new HttpClient();
                client.BaseAddress = new Uri(uri);
                return client;
            });
                    // FourWeb doit avoir un HttpClient
                 serviceCollection.AddSingleton<IFour, FourWeb>();
            #endregion

            Services = serviceCollection.BuildServiceProvider();


        }
    }

}

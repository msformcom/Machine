using System.Configuration;
using System.Data;
using System.Windows;
using ExeLocal;
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
            serviceCollection.AddSingleton(config);
            #endregion

            #region Four
            // Créer un objet FourOptions en désérialisant la section FourOptions
            var fourOptions = config.GetRequiredSection("FourOptions").Get<FourOptions>();
            serviceCollection.AddSingleton(fourOptions!);

            // Constructeur de FourLocal => besoin de FourOptions
            serviceCollection.AddSingleton<IFour, FourLocal>();

            #endregion
            
            Services = serviceCollection.BuildServiceProvider();


        }
    }

}

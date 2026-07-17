using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ExeOperateur
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var four = App.Services.GetRequiredService<IFour>();
            var temp = four.GetTemperature().ContinueWith(t =>
            {
                var temp = t.Result;
            });
            four.GetHistorique().ContinueWith(t =>
            {
       
                    this.Historique = t.Result;
            

            });
        }

        // async Task exempleAsync()
        // {
        //     var a=await FAsync();
        //     Console.WriteLine(a);

        //         }
        // void exemple()
        // {
        //     FAsync().ContinueWith(t=>{
        //             var a=t.result;
        //             Console.WriteLine(a);
        //     });


        //         }



        public IEnumerable<IHistoriqueItem> Historique { get; set; }
    }
}
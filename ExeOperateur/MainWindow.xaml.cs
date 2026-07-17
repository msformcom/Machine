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
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ExeOperateur
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [INotifyPropertyChanged]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
           
            InitializeComponent();
            // je considère la class codebehind comme ViewModel
            this.DataContext = this;
            DispatcherTimer t = new DispatcherTimer();
            t.Interval = TimeSpan.FromSeconds(5);
           
            t.Tick += (o,e)=>ReadDatas();
            t.Start();
            ReadDatas();
   
        }



        void ReadDatas()
        {
            var four = App.Services.GetRequiredService<IFour>();
            four.GetTemperature().ContinueWith(t =>
            {
                this.LastTemp = t.Result;
            });
            four.GetHistorique().ContinueWith(t =>
            {
                var result = t.Result.ToList();
                this.Historique = t.Result.OrderByDescending(c=>c.Date).Take(20).ToList();
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

        [ObservableProperty]
        private Double? _LastTemp;

        [ObservableProperty]
        private IEnumerable<IHistoriqueItem> _Historique;
    }
}
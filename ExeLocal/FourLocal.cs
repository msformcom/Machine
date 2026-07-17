using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.Extensions.Logging;

namespace ExeLocal
{
    public class FourLocal : IFour
    {
        public int MaxInvalidValuesBeforeLogging = 3;
        private readonly FourOptions options;
        private readonly ILogger<FourLocal>? logger;

        // Flag évitant l'accès concurrent au fichier
        private Object FlagFichier = new Object();

        public FourLocal(FourOptions options, ILogger<FourLocal>? logger=null)
        {
            //Timer t = new Timer(new TimerCallback(o => GetTemperature()),null,0,1000);
            this.options = options;
            this.logger = logger;
        }

        public Func<double, bool> ValeurValide { get; set; }


        public  Task<IEnumerable<IHistoriqueItem>> GetHistorique()
        {
            return Task.Run(async() =>
            {
                await Task.Delay(200);
                lock (FlagFichier)
                {
                    EnsureFileExist();
                    using (var f = File.Open(Path.Combine(options.Directory, options.FileName), FileMode.Open, FileAccess.Read))
                    {
                        using (var sr = new StreamReader(f))
                        {
                            var chaine = sr.ReadToEnd();

                            var lignes = chaine.Split("\r\n")
                                        .Where(c => !String.IsNullOrWhiteSpace(c))
                                        .Select(c => JsonSerializer.Deserialize<HistoriqueItem>(c)!)
                                        .Select(c => (IHistoriqueItem)c);
                                        

                            return lignes;
                        }
                    }
                }
            });


        }

        void EnsureFileExist()
        {
            if (!Directory.Exists(options.Directory))
            {
                Directory.CreateDirectory(options.Directory);
            }
            if (!File.Exists(Path.Combine(options.Directory, options.FileName)))
            {
                File.Create(Path.Combine(options.Directory, options.FileName)).Close();
            }
        }

        HistoriqueItem SaveTempInHistory(Double temp)
        {
            // Attendre l'accès exclusif à l'objet
            lock (FlagFichier)
            {
                EnsureFileExist();
                using (var f = File.Open(Path.Combine(options.Directory, options.FileName), FileMode.Append))
                {
                    // f sera fermé en sortant de ce bloc de code (interface IDisposable)

                    using (var sw = new StreamWriter(f))
                    {
                        var histoItem = new HistoriqueItem() { Date = DateTime.Now, Valeur = temp };
                        if (ValeurValide != null)
                        {
                            histoItem.Valid = ValeurValide(temp);
                        }
                        var chaine = System.Text.Json.JsonSerializer.Serialize(histoItem);
                        sw.WriteLine(chaine);
                        return histoItem;
                    }
                    ;

                }
            }

        }

        Queue<IHistoriqueItem> LastThree = new Queue<IHistoriqueItem>();

        public Task<double> GetTemperature()
        {

            return Task.Run(async () =>
            {
                await Task.Delay(200);
                var r = new Random();
                var temp = r.NextDouble() * 100;
                var item= SaveTempInHistory(temp);
                LastThree.Enqueue(item);
                if (LastThree.Count() > MaxInvalidValuesBeforeLogging)
                {
                    LastThree.Dequeue();

                    if (LastThree.Count(c => c.Valid == false) >= MaxInvalidValuesBeforeLogging)
                    {
                        if (logger != null) logger.LogCritical($"{MaxInvalidValuesBeforeLogging} Mesures invalides");
                    }
                }


                return temp;
            });

        }
    }
}

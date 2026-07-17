using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace ExeLocal
{
    public class FourLocal : IFour
    {
        private readonly FourOptions options;
        // Flag évitant l'accès concurrent au fichier
        private Object FlagFichier = new Object();

        public FourLocal(FourOptions options)
        {
            //Timer t = new Timer(new TimerCallback(o => GetTemperature()),null,0,1000);
            this.options = options;
        }

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
                                .Select(c => c.Split(";"))

                                .Select(c => new HistoriqueItem()
                                {
                                    Date = DateTime.Parse(c[0]),
                                    Valeur = Double.Parse(c[1])
                                } as IHistoriqueItem);

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

        void SaveTempInHistory(Double temp)
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
                        sw.Write(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        sw.Write(";");
                        sw.WriteLine(temp);
                    }
                    ;

                }
            }

        }

        public Task<double> GetTemperature()
        {

            return Task.Run(async () =>
            {
                await Task.Delay(200);
                var r = new Random();
                var temp = r.NextDouble() * 100;
                SaveTempInHistory(temp);
                return temp;
            });

        }
    }
}

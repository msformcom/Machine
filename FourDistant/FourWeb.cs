using System.Net.Http.Json;
using Interfaces;

namespace FourDistant
{
    public class FourWeb : IFour
    {
        private readonly HttpClient client;

        public FourWeb(HttpClient client)
        {
            this.client = client;
        }
        public async Task<IEnumerable<IHistoriqueItem>> GetHistorique()
        {
            var reponse = await client.GetAsync("GetHistorique");
            // Lecture et déserialisation du stream en un double
            var o = await reponse.Content.ReadFromJsonAsync<IEnumerable<HistoriqueItem>>();
            return o; 
        }

        public async Task<double> GetTemperature()
        {
            // Attente du stream de réponse de la part du serveur
            var reponse = await client.GetAsync("GetTemperature");
            // Lecture et déserialisation du stream en un double
            var o = await reponse.Content.ReadFromJsonAsync<double>();
            return o;
        }
    }
}

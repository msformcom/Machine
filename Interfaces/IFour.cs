namespace Interfaces
{
    public interface IFour
    {
        Task<Double> GetTemperature();
        Task<IEnumerable<IHistoriqueItem>> GetHistorique();

    }
}

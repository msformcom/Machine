namespace Interfaces
{
    public interface IFour
    {
        Task<Double> GetTemperature();
        Task<IEnumerable<IHistoriqueItem>> GetHistorique();

        Func<double, bool> ValeurValide { get; set; }

    }
}

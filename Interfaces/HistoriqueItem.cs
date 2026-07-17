
using Interfaces;


public class HistoriqueItem : IHistoriqueItem
{
    public DateTime Date { get; set; } = DateTime.Now;
    public Double Valeur { get; set; }
    public bool? Valid { get; set; } = null;
}

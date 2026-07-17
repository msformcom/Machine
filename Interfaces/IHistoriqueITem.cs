namespace Interfaces
{
    public interface IHistoriqueItem
    {
        public DateTime Date { get; set; }
        public Double Valeur { get; set; }
        public bool? Valid { get; set; } 
    }
}
namespace UAS_Inventoris.Models
{
    public class Barang
    {
        public int IdBarang { get; set; }
        public string NamaBarang { get; set; }
        public decimal HargaBarang { get; set; }
        public int Stok { get; set; }
        public int KategoriId { get; set; }
    }
}

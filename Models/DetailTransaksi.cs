public class DetailTransaksi
{
    public int DetailId { get; set; }
    public int TransaksiId { get; set; }
    public int BarangId { get; set; }
    public string NamaBarang { get; set; }
    public int Jumlah { get; set; }
    public decimal HargaSatuan { get; set; }
    public decimal Total { get; set; }
}

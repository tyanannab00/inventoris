using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using MySql.Data.MySqlClient;
using MySqlConnector;
using System.Collections.Generic;
using UAS_Inventoris.Models;
using MySqlCommand = MySql.Data.MySqlClient.MySqlCommand;
using MySqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using iText.Kernel;
using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Properties;
using iText.Layout.Element;


public class BarangController : Controller
{
    private readonly string _connectionString = "Server=localhost;Port=3307;Database=inventaris;Uid=root;";

    public IActionResult DaftarBarang()
    {
        List<Barang> daftarBarang = new List<Barang>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            var query = "SELECT * FROM tbl_barang";

            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Barang barang = new Barang
                        {
                            IdBarang = Convert.ToInt32(reader["id_barang"]),
                            NamaBarang = reader["nama_barang"].ToString(),
                            HargaBarang = Convert.ToDecimal(reader["harga_barang"]),
                            Stok = Convert.ToInt32(reader["stok"]),
                            KategoriId = Convert.ToInt32(reader["kategori_id"])
                        };

                        daftarBarang.Add(barang);
                    }
                }
            }
        }

        return View("~/Views/Home/DaftarBarang.cshtml", daftarBarang);
    }

    public IActionResult Edit(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            var query = "SELECT * FROM tbl_barang WHERE id_barang = @id";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Barang barang = new Barang
                        {
                            IdBarang = Convert.ToInt32(reader["id_barang"]),
                            NamaBarang = reader["nama_barang"].ToString(),
                            HargaBarang = Convert.ToDecimal(reader["harga_barang"]),
                            Stok = Convert.ToInt32(reader["stok"]),
                            KategoriId = Convert.ToInt32(reader["kategori_id"])
                        };

                        return View("~/Views/Home/Edit.cshtml", barang);
                    }
                }
            }
        }

        return NotFound(); // Jika data barang tidak ditemukan
    }

    public IActionResult Update(Barang barang)
    {
        if (ModelState.IsValid)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var query = "UPDATE tbl_barang SET nama_barang = @nama, harga_barang = @harga, stok = @stok, kategori_id = @kategori WHERE id_barang = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nama", barang.NamaBarang);
                    command.Parameters.AddWithValue("@harga", barang.HargaBarang);
                    command.Parameters.AddWithValue("@stok", barang.Stok);
                    command.Parameters.AddWithValue("@kategori", barang.KategoriId);
                    command.Parameters.AddWithValue("@id", barang.IdBarang);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction(nameof(DaftarBarang)); // Redirect ke halaman Daftar Barang setelah penyimpanan berhasil
        }

        return View(barang); // Kembali ke view Edit jika model tidak valid
    }

    public IActionResult Tambah()
    {
        return View("~/Views/Home/Tambah.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Tambah(Barang barang)
    {
        if (ModelState.IsValid)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var query = "INSERT INTO tbl_barang (nama_barang, harga_barang, stok, kategori_id) VALUES (@nama, @harga, @stok, @kategori)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nama", barang.NamaBarang);
                    command.Parameters.AddWithValue("@harga", barang.HargaBarang);
                    command.Parameters.AddWithValue("@stok", barang.Stok);
                    command.Parameters.AddWithValue("@kategori", barang.KategoriId);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction(nameof(DaftarBarang)); // Redirect ke halaman Daftar Barang setelah penyimpanan berhasil
        }

        return View(barang); // Kembali ke view Tambah jika model tidak valid
    }

    public IActionResult Delete(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            var query = "DELETE FROM tbl_barang WHERE id_barang = @id";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }

        return RedirectToAction(nameof(DaftarBarang)); // Redirect kembali ke halaman daftar barang setelah penghapusan berhasil
    }
    public IActionResult GeneratePDF()
    {
        List<Barang> daftarBarang = new List<Barang>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            var query = "SELECT * FROM tbl_barang";

            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Barang barang = new Barang
                        {
                            IdBarang = Convert.ToInt32(reader["id_barang"]),
                            NamaBarang = reader["nama_barang"].ToString(),
                            HargaBarang = Convert.ToDecimal(reader["harga_barang"]),
                            Stok = Convert.ToInt32(reader["stok"]),
                            KategoriId = Convert.ToInt32(reader["kategori_id"])
                        };

                        daftarBarang.Add(barang);
                    }
                }
            }
        }

        // Buat dokumen PDF baru
        MemoryStream memoryStream = new MemoryStream();
        PdfWriter writer = new PdfWriter(memoryStream);
        PdfDocument pdf = new PdfDocument(writer);
        Document doc = new Document(pdf);

        // Tambahkan konten ke dokumen PDF
        Table table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth();

        // Tambahkan header tabel
        table.AddHeaderCell("ID Barang");
        table.AddHeaderCell("Nama Barang");
        table.AddHeaderCell("Harga Barang");
        table.AddHeaderCell("Stok");
        table.AddHeaderCell("Kategori ID");

        // Tambahkan isi tabel dari daftarBarang
        foreach (var barang in daftarBarang)
        {
            table.AddCell(barang.IdBarang.ToString());
            table.AddCell(barang.NamaBarang);
            table.AddCell(barang.HargaBarang.ToString());
            table.AddCell(barang.Stok.ToString());
            table.AddCell(barang.KategoriId.ToString());
        }

        doc.Add(table);
        doc.Close();

        // Kirim file PDF sebagai respons
        return File(memoryStream.ToArray(), "application/pdf", "DaftarBarang.pdf");
    }

    public IActionResult FormTransaksi()
    {
        return View("~/Views/Home/FormTransaksi.cshtml");
    }

    public IActionResult GetDaftarNamaBarang()
    {
        List<string> daftarNamaBarang = new List<string>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            var query = "SELECT nama_barang FROM tbl_barang";

            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string namaBarang = reader["nama_barang"].ToString();
                        daftarNamaBarang.Add(namaBarang);
                    }
                }
            }
        }

        return Json(daftarNamaBarang);
    }

    public IActionResult GetDetailBarang(string namaBarang)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            var query = $"SELECT stok, harga_barang FROM tbl_barang WHERE nama_barang = '{namaBarang}'";

            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var detailBarang = new
                        {
                            stok = Convert.ToInt32(reader["stok"]),
                            hargaSatuan = Convert.ToDecimal(reader["harga_barang"])
                        };
                        reader.Close(); // Tutup DataReader setelah selesai membaca
                        return Json(detailBarang);
                    }
                }
            }
        }

        return NotFound();
    }

    [HttpPost]
    public IActionResult BuatTransaksi(DetailTransaksi transaksi)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            // Ambil informasi barang dari tabel tbl_barang
            string getBarangQuery = $"SELECT id_barang, harga_barang, stok FROM tbl_barang WHERE nama_barang = '{transaksi.NamaBarang}'";
            using (MySqlCommand getBarangCommand = new MySqlCommand(getBarangQuery, connection))
            {
                using (var reader = getBarangCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int barangId = Convert.ToInt32(reader["id_barang"]);
                        decimal hargaSatuan = Convert.ToDecimal(reader["harga_barang"]);
                        int stok = Convert.ToInt32(reader["stok"]);

                        // Update nilai transaksi dengan data yang didapat dari frontend
                        transaksi.BarangId = barangId;
                        transaksi.HargaSatuan = hargaSatuan;
                        transaksi.Total = transaksi.Jumlah * hargaSatuan;

                        // Tutup DataReader setelah selesai membaca
                        reader.Close();

                        // Mengurangi stok barang
                        string updateStokQuery = $"UPDATE tbl_barang SET stok = stok - {transaksi.Jumlah} WHERE id_barang = {barangId}";
                        using (MySqlCommand updateStokCommand = new MySqlCommand(updateStokQuery, connection))
                        {
                            updateStokCommand.ExecuteNonQuery();
                        }

                        // Insert informasi transaksi ke dalam tabel tbl_transaksi
                        // Mendefinisikan pernyataan SQL dengan parameter
                        string insertTransaksiQuery = "INSERT INTO tbl_transaksi (date, total) VALUES (@Date, @Total)";

                        using (MySqlCommand insertTransaksiCommand = new MySqlCommand(insertTransaksiQuery, connection))
                        {
                            // Mengatur parameter dengan tipe data yang sesuai
                            insertTransaksiCommand.Parameters.AddWithValue("@Date", DateTime.Now.ToString("yyyy-MM-dd")); // Format tanggal YYYY-MM-DD
                            insertTransaksiCommand.Parameters.AddWithValue("@Total", transaksi.Total);

                            // Melakukan eksekusi pernyataan SQL
                            insertTransaksiCommand.ExecuteNonQuery();
                        }


                        // Ambil id transaksi yang baru saja diinsert
                        string getLastInsertedIdQuery = "SELECT LAST_INSERT_ID()";
                        using (MySqlCommand getLastInsertedIdCommand = new MySqlCommand(getLastInsertedIdQuery, connection))
                        {
                            int transaksiId = Convert.ToInt32(getLastInsertedIdCommand.ExecuteScalar());

                            // Memasukkan detail transaksi ke dalam tabel tbl_detail_transaksi
                            string insertDetailTransaksiQuery = "INSERT INTO tbl_detail_transaksi (transaksi_id, barang_id, nama_barang, jumlah, harga_satuan, total) VALUES (@TransaksiId, @BarangId, @NamaBarang, @Jumlah, @HargaSatuan, @Total)";
                            using (MySqlCommand insertDetailTransaksiCommand = new MySqlCommand(insertDetailTransaksiQuery, connection))
                            {
                                // Set parameter untuk nilai-nilai yang akan dimasukkan
                                insertDetailTransaksiCommand.Parameters.AddWithValue("@TransaksiId", transaksiId);
                                insertDetailTransaksiCommand.Parameters.AddWithValue("@BarangId", transaksi.BarangId);
                                insertDetailTransaksiCommand.Parameters.AddWithValue("@NamaBarang", transaksi.NamaBarang);
                                insertDetailTransaksiCommand.Parameters.AddWithValue("@Jumlah", transaksi.Jumlah);
                                insertDetailTransaksiCommand.Parameters.AddWithValue("@HargaSatuan", transaksi.HargaSatuan);
                                insertDetailTransaksiCommand.Parameters.AddWithValue("@Total", transaksi.Total);

                                int rowsAffected = insertDetailTransaksiCommand.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    return Ok("Transaksi berhasil");
                                }
                                else
                                {
                                    return BadRequest("Gagal membuat transaksi");
                                }
                            }
                        }

                    }
                    else
                    {
                        reader.Close(); // Tutup DataReader jika barang tidak ditemukan
                        return NotFound("Barang tidak ditemukan");
                    }
                }
            }
        }
    }


}



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


}


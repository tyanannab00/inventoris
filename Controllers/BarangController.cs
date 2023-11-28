using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using MySql.Data.MySqlClient;
using MySqlConnector;
using System.Collections.Generic;
using UAS_Inventoris.Models;
using MySqlCommand = MySql.Data.MySqlClient.MySqlCommand;
using MySqlConnection = MySql.Data.MySqlClient.MySqlConnection;

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


}
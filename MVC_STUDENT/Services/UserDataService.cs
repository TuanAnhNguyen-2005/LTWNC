using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using MVC_STUDENT.Models;

namespace MVC_STUDENT.Services  // ĐỔI THÀNH namespace của project bạn
{
    public class UserDataService
    {
        private readonly string _connectionString;

        public UserDataService()
        {
            try
            {
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "appsettings.json"
                );

                filePath = Path.GetFullPath(filePath);

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Không tìm thấy appsettings.json tại: {filePath}");
                }

                string json = File.ReadAllText(filePath);
                dynamic config = JsonConvert.DeserializeObject(json);

                _connectionString = config?.ConnectionStrings?.DefaultConnection;

                if (string.IsNullOrEmpty(_connectionString))
                {
                    throw new InvalidOperationException("Không tìm thấy 'DefaultConnection' trong appsettings.json");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi đọc cấu hình: {ex.Message}", ex);
            }
        }
        public LoginResult LoginUser(string email, string password)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = @"
                SELECT nd.MaNguoiDung, nd.HoTen, nd.Email, vt.TenVaiTro
                FROM NguoiDung nd
                INNER JOIN VaiTro vt ON nd.MaVaiTro = vt.MaVaiTro
                WHERE LOWER(LTRIM(RTRIM(nd.Email))) = LOWER(@Email)
                  AND nd.MatKhau = @MatKhau
                  AND nd.TrangThai = 1";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email.Trim());
                        cmd.Parameters.AddWithValue("@MatKhau", password); // Hiện tại lưu plain text

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string tenVaiTro = reader.GetString(3);

                                return new LoginResult
                                {
                                    Success = true,
                                    UserId = reader.GetInt32(0),
                                    FullName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Role = MapRoleToEnglish(tenVaiTro)
                                };
                            }
                        }
                    }
                }

                return new LoginResult { Success = false };
            }
            catch (Exception)
            {
                return new LoginResult { Success = false };
            }
        }

        private string MapRoleToEnglish(string tenVaiTro)
        {
            switch (tenVaiTro?.ToLower())
            {
                case "sinh viên":
                    return "Student";
                case "giảng viên":
                    return "Teacher";
                case "admin":
                    return "Admin";
                default:
                    return "Student";
            }
        }

        public bool RegisterUser(string fullName, string email, string password, string role = "Student",
            string phoneNumber = null)
        {
            try
            {
                int maVaiTro = GetMaVaiTroByRole(role);

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Kiểm tra email trùng
                    string checkSql = "SELECT COUNT(*) FROM NguoiDung WHERE Email = @Email";
                    using (var cmd = new SqlCommand(checkSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        if ((int)cmd.ExecuteScalar() > 0)
                            return false;
                    }

                    // Insert mới
                    string insertSql = @"
                        INSERT INTO NguoiDung (HoTen, Email, MatKhau, MaVaiTro, SoDienThoai, NgayTao, TrangThai)
                        VALUES (@HoTen, @Email, @MatKhau, @MaVaiTro, @SoDienThoai, GETDATE(), 1)";

                    using (var cmd = new SqlCommand(insertSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@HoTen", fullName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@MatKhau", password); // Cảnh báo: nên hash sau
                        cmd.Parameters.AddWithValue("@MaVaiTro", maVaiTro);
                        cmd.Parameters.AddWithValue("@SoDienThoai", (object)phoneNumber ?? DBNull.Value);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception)
            {
                throw; // Để controller bắt lỗi
            }
        }

        private int GetMaVaiTroByRole(string role)
        {
            if (string.IsNullOrEmpty(role))
                return 3; // Student

            switch (role.ToLower())
            {
                case "teacher":
                case "giảng viên":
                    return 2;
                case "student":
                case "sinh viên":
                default:
                    return 3;
            }
        }

        // Thêm hàm test connection nếu muốn debug
        public bool TestConnection(out string error)
        {
            error = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
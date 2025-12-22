using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using MVC_ADMIN.Helpers;
using Newtonsoft.Json;

namespace MVC_ADMIN.Services
{
    /// <summary>
    /// Service để thao tác với database cho User
    /// </summary>
    public class UserDataService
    {
        private readonly string _connectionString;

        public UserDataService()
        {
            try
            {
                // Đọc từ file appsettings.json trong chính thư mục project MVC_ADMIN
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "appsettings.json"
                );


                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Không tìm thấy file appsettings.json tại: {filePath}. Hãy copy file vào thư mục MVC_ADMIN.");
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
                throw new InvalidOperationException($"Lỗi đọc appsettings.json: {ex.Message}", ex);
            }
        }
        /// <summary>
        /// Quick test helper to verify DB connectivity (useful for diagnostics)
        /// </summary>
        public bool TestConnection(out string error)
        {
            error = null;
            try
            {
                var builder = new SqlConnectionStringBuilder(_connectionString)
                {
                    ConnectTimeout = 5
                };
                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message + (ex.InnerException != null ? " | Inner: " + ex.InnerException.Message : "");
                return false;
            }
        }

        /// <summary>
        /// Đăng ký user mới
        /// </summary>
        public bool RegisterUser(string fullName, string email, string password, string role, 
            string phoneNumber = null, string gender = null, string address = null, DateTime? dateOfBirth = null)
        {
            try
            {
                // Map role string sang MaVaiTro
                int maVaiTro = GetMaVaiTroByRole(role);

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Kiểm tra email đã tồn tại chưa
                    string checkEmailSql = "SELECT COUNT(*) FROM NguoiDung WHERE Email = @Email";
                    using (var checkCmd = new SqlCommand(checkEmailSql, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            return false; // Email đã tồn tại
                        }
                    }

                    // Insert user mới với đầy đủ thông tin
                    string insertSql = @"
                        INSERT INTO NguoiDung (HoTen, Email, MatKhau, MaVaiTro, SoDienThoai, GioiTinh, DiaChi, NgaySinh, NgayTao, TrangThai)
                        VALUES (@HoTen, @Email, @MatKhau, @MaVaiTro, @SoDienThoai, @GioiTinh, @DiaChi, @NgaySinh, GETDATE(), 1)";

                    using (var cmd = new SqlCommand(insertSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@HoTen", (object)fullName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@MatKhau", password); // Lưu plain text (nên hash trong production)
                        cmd.Parameters.AddWithValue("@MaVaiTro", maVaiTro);
                        cmd.Parameters.AddWithValue("@SoDienThoai", (object)phoneNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@GioiTinh", (object)gender ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DiaChi", (object)address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NgaySinh", (object)dateOfBirth ?? DBNull.Value);

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (SqlException) // let caller inspect SQL errors where appropriate
            {
                throw;
            }
            catch (Exception)
            {
                // rethrow to preserve stack for controller to report/log
                throw;
            }
        }

        /// <summary>
        /// Đăng nhập user
        /// </summary>
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
                        WHERE LTRIM(RTRIM(nd.Email)) = LTRIM(RTRIM(@Email))
                          AND LTRIM(RTRIM(nd.MatKhau)) = LTRIM(RTRIM(@MatKhau))
                          AND nd.TrangThai = 1";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@MatKhau", password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new LoginResult
                                {
                                    Success = true,
                                    UserId = reader.GetInt32(0),
                                    FullName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Role = MapRoleToEnglish(reader.GetString(3))
                                };
                            }
                        }
                    }
                }

                return new LoginResult { Success = false };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging in: {ex.Message}");
                return new LoginResult { Success = false };
            }
        }

        /// <summary>
        /// Lấy MaVaiTro từ role string
        /// </summary>
        private int GetMaVaiTroByRole(string role)
        {
            switch (role?.ToLower())
            {
                case "admin":
                    return 1;
                case "teacher":
                case "giảng viên":
                    return 2;
                case "student":
                case "sinh viên":
                default:
                    return 3;
            }
        }

        /// <summary>
        /// Map role tiếng Việt sang tiếng Anh
        /// </summary>
        private string MapRoleToEnglish(string role)
        {
            switch (role?.ToLower())
            {
                case "admin":
                    return "Admin";
                case "giảng viên":
                    return "Teacher";
                case "sinh viên":
                    return "Student";
                default:
                    return role;
            }
        }

        /// <summary>
        /// Tạo user mẫu nếu chưa tồn tại
        /// </summary>
        public bool CreateSampleUser()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Kiểm tra email đã tồn tại chưa
                    string checkEmailSql = "SELECT COUNT(*) FROM NguoiDung WHERE Email = @Email";
                    using (var checkCmd = new SqlCommand(checkEmailSql, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", "thanhtu98912@gmail.com");
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            return true; // Đã tồn tại
                        }
                    }

                    // Insert user mẫu (Sinh viên)
                    string insertSql = @"
                        INSERT INTO NguoiDung (HoTen, Email, MatKhau, MaVaiTro, NgayTao, TrangThai)
                        VALUES (@HoTen, @Email, @MatKhau, @MaVaiTro, GETDATE(), 1)";

                    using (var cmd = new SqlCommand(insertSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@HoTen", "Thành Tú");
                        cmd.Parameters.AddWithValue("@Email", "thanhtu98912@gmail.com");
                        cmd.Parameters.AddWithValue("@MatKhau", "12345");
                        cmd.Parameters.AddWithValue("@MaVaiTro", 3); // Sinh viên

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating sample user: {ex.Message}");
                return false;
            }
        }
    }

    /// <summary>
    /// Kết quả đăng nhập
    /// </summary>
    public class LoginResult
    {
        public bool Success { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}


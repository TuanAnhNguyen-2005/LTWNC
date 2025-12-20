using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using MVC_Teacher.Helpers;
using MVC_Teacher.Models;
using Newtonsoft.Json;
using System.IO;

namespace MVC_Teacher.Services
{
    /// <summary>
    /// Service để thao tác với database cho User (MVC_Teacher)
    /// Provides CRUD for Teacher and login/register operations.
    /// </summary>
    public class UserDataService
    {
        private readonly string _connectionString;

        public UserDataService()
        {
            try
            {
                // Đọc file appsettings.json ngay trong thư mục project MVC hiện tại (cùng cấp Web.config)
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Không tìm thấy file appsettings.json tại: {filePath}. Hãy tạo file này trong thư mục project MVC.");
                }

                string json = File.ReadAllText(filePath);
                dynamic config = JsonConvert.DeserializeObject(json);

                _connectionString = config?.ConnectionStrings?.DefaultConnection;

            _connectionString = string.IsNullOrEmpty(conn)
                ? "Data Source=LEPS\\NGUYENHUYNH;Initial Catalog=NenTangHocLieu;Persist Security Info=True;User ID=sa;Password=123;TrustServerCertificate=True;MultipleActiveResultSets=True;"
                : conn;
        }

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

        public bool RegisterUser(string fullName, string email, string password, string role,
            string phoneNumber = null, string gender = null, string address = null, DateTime? dateOfBirth = null)
        {
            try
            {
                int maVaiTro = GetMaVaiTroByRole(role);

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string checkEmailSql = "SELECT COUNT(*) FROM NguoiDung WHERE Email = @Email";
                    using (var checkCmd = new SqlCommand(checkEmailSql, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0) return false;
                    }

                    string insertSql = @"
                        INSERT INTO NguoiDung (HoTen, Email, MatKhau, MaVaiTro, SoDienThoai, GioiTinh, DiaChi, NgaySinh, NgayTao, TrangThai)
                        VALUES (@HoTen, @Email, @MatKhau, @MaVaiTro, @SoDienThoai, @GioiTinh, @DiaChi, @NgaySinh, GETDATE(), 1)";

                    using (var cmd = new SqlCommand(insertSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@HoTen", (object)fullName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", email);
                        // Hash password before storing
                        var hashed = PasswordHasher.HashPassword(password);
                        cmd.Parameters.AddWithValue("@MatKhau", hashed);
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
            catch (SqlException)
            {
                throw;
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
                        SELECT nd.MaNguoiDung, nd.HoTen, nd.Email, nd.MatKhau, vt.TenVaiTro
                        FROM NguoiDung nd
                        LEFT JOIN VaiTro vt ON nd.MaVaiTro = vt.MaVaiTro
                        WHERE nd.Email = @Email AND nd.TrangThai = 1";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var storedHash = reader.IsDBNull(3) ? null : reader.GetString(3);
                                if (!string.IsNullOrEmpty(storedHash) && PasswordHasher.VerifyPassword(password, storedHash))
                                {
                                    return new LoginResult
                                    {
                                        Success = true,
                                        UserId = reader.GetInt32(0),
                                        FullName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                        Email = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                        Role = reader.IsDBNull(4) ? "" : MapRoleToEnglish(reader.GetString(4))
                                    };
                                }
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

        private int GetMaVaiTroByRole(string role)
        {
            switch (role?.ToLower())
            {
                case "admin": return 1;
                case "teacher":
                case "giảng viên": return 2;
                case "student":
                case "sinh viên":
                default: return 3;
            }
        }

        private string MapRoleToEnglish(string role)
        {
            switch (role?.ToLower())
            {
                case "admin": return "Admin";
                case "giảng viên": return "Teacher";
                case "sinh viên": return "Student";
                default: return role;
            }
        }

        public bool CreateSampleUser()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Ensure default teacher user exists
                    string checkTeacherSql = "SELECT COUNT(*) FROM NguoiDung WHERE Email = @Email";
                    using (var checkCmd = new SqlCommand(checkTeacherSql, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", "teacher123@gmail.com");
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count == 0)
                        {
                            string insertTeacherSql = @"
                                INSERT INTO NguoiDung (HoTen, Email, MatKhau, MaVaiTro, NgayTao, TrangThai)
                                VALUES (@HoTen, @Email, @MatKhau, @MaVaiTro, GETDATE(), 1)";

                            using (var cmd = new SqlCommand(insertTeacherSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@HoTen", "Teacher Default");
                                cmd.Parameters.AddWithValue("@Email", "teacher123@gmail.com");
                                cmd.Parameters.AddWithValue("@MatKhau", PasswordHasher.HashPassword("123456"));
                                cmd.Parameters.AddWithValue("@MaVaiTro", 2); // Teacher role
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    // Ensure sample student still exists
                    string checkEmailSql = "SELECT COUNT(*) FROM NguoiDung WHERE Email = @Email";
                    using (var checkCmd = new SqlCommand(checkEmailSql, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", "thanhtu98912@gmail.com");
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            return true; // Existing users ok
                        }
                    }

                    string insertSql = @"
                        INSERT INTO NguoiDung (HoTen, Email, MatKhau, MaVaiTro, NgayTao, TrangThai)
                        VALUES (@HoTen, @Email, @MatKhau, @MaVaiTro, GETDATE(), 1)";

                    using (var cmd = new SqlCommand(insertSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@HoTen", "Thành Tú");
                        cmd.Parameters.AddWithValue("@Email", "thanhtu98912@gmail.com");
                        cmd.Parameters.AddWithValue("@MatKhau", PasswordHasher.HashPassword("12345"));
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

        public Teacher GetTeacherByEmail(string email)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "SELECT MaNguoiDung, HoTen, Email, MatKhau, SoDienThoai, GioiTinh, DiaChi, NgaySinh, TrangThai FROM NguoiDung WHERE Email = @Email";
                    cmd.Parameters.AddWithValue("@Email", email ?? string.Empty);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Teacher
                            {
                                TeacherId = reader.GetInt32(0),
                                FullName = reader.IsDBNull(1) ? null : reader.GetString(1),
                                Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                                PasswordHash = reader.IsDBNull(3) ? null : reader.GetString(3),
                                PhoneNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Gender = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Address = reader.IsDBNull(6) ? null : reader.GetString(6),
                                DateOfBirth = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
                                IsActive = reader.IsDBNull(8) ? true : reader.GetInt32(8) == 1,
                                CreatedDate = DateTime.Now // placeholder, table may have NgayTao
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTeacherByEmail error: {ex.Message}");
            }
            return null;
        }

        public int GetLoginCountForTeacher(int teacherId)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "SELECT COUNT(*) FROM LoginHistory WHERE TeacherId = @TeacherId";
                    cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                    var obj = cmd.ExecuteScalar();
                    return obj != null ? Convert.ToInt32(obj) : 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetLoginCountForTeacher error: {ex.Message}");
                return 0;
            }
        }

        public void UpdateTeacher(Teacher t)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "UPDATE NguoiDung SET HoTen=@HoTen, SoDienThoai=@SoDienThoai, DiaChi=@DiaChi WHERE MaNguoiDung=@Id";
                    cmd.Parameters.AddWithValue("@HoTen", t.FullName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SoDienThoai", t.PhoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChi", t.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", t.TeacherId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateTeacher error: {ex.Message}");
            }
        }
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
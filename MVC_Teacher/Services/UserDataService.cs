using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using MVC_Teacher.Models;

namespace MVC_Teacher.Services
{
    public class UserDataService
    {
        private readonly string _connectionString;

        public UserDataService()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== MVC_TEACHER: KHỞI TẠO UserDataService ===");

                // ƯU TIÊN ĐỌC TỪ WEB.CONFIG (giống mvc_admin và mvc_student)
                _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

                System.Diagnostics.Debug.WriteLine("1. Đang thử đọc từ Web.config...");

                if (!string.IsNullOrEmpty(_connectionString))
                {
                    System.Diagnostics.Debug.WriteLine($"   THÀNH CÔNG: Connection String từ Web.config.");
                    System.Diagnostics.Debug.WriteLine($"   Giá trị: {_connectionString}");
                    return;
                }

                // Fallback: đọc từ appsettings.json
                System.Diagnostics.Debug.WriteLine($"2. Không tìm thấy trong Web.config. Thử từ appsettings.json...");
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                System.Diagnostics.Debug.WriteLine($"   Đường dẫn: {filePath}");
                System.Diagnostics.Debug.WriteLine($"   File tồn tại: {File.Exists(filePath)}");

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    dynamic config = JsonConvert.DeserializeObject(json);
                    _connectionString = config?.ConnectionStrings?.DefaultConnection;
                    System.Diagnostics.Debug.WriteLine($"   Đọc từ JSON: {_connectionString}");
                }

                if (string.IsNullOrEmpty(_connectionString))
                {
                    throw new InvalidOperationException("KHÔNG TÌM THẤY CHUỐI KẾT NỐI.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== LỖI KHỞI TẠO SERVICE (MVC_TEACHER) ===");
                System.Diagnostics.Debug.WriteLine($"{ex.ToString()}");
                throw;
            }
        }

        public LoginResult LoginUser(string email, string password)
        {
            System.Diagnostics.Debug.WriteLine($"\n=== MVC_TEACHER: BẮT ĐẦU ĐĂNG NHẬP ===");
            System.Diagnostics.Debug.WriteLine($"Email: '{email}'");
            System.Diagnostics.Debug.WriteLine($"Password độ dài: {password?.Length}");

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                System.Diagnostics.Debug.WriteLine("LỖI: Connection String rỗng!");
                return new LoginResult { Success = false };
            }

            System.Diagnostics.Debug.WriteLine($"Connection String: {_connectionString}");

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    System.Diagnostics.Debug.WriteLine("Đang mở kết nối Database...");
                    connection.Open();
                    System.Diagnostics.Debug.WriteLine($"   THÀNH CÔNG! Database: {connection.Database}");

                    // DÙNG CÂU TRUY VẤN ĐƠN GIẢN (giống mvc_admin và mvc_student đã chạy được)
                    string sql = @"
                        SELECT TOP 1 nd.MaNguoiDung, nd.HoTen, nd.Email, nd.MatKhau, nd.MaVaiTro
                        FROM NguoiDung nd
                        WHERE nd.Email = @Email 
                          AND nd.MatKhau = @MatKhau
                          AND nd.TrangThai = 1";

                    System.Diagnostics.Debug.WriteLine($"Thực thi truy vấn: {sql}");

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@MatKhau", password); // LƯU Ý: Mật khẩu plain text

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int maVaiTro = reader.GetInt32(4); // Cột MaVaiTro
                                string roleName = MapRoleIdToName(maVaiTro);

                                System.Diagnostics.Debug.WriteLine($"   TÌM THẤY USER!");
                                System.Diagnostics.Debug.WriteLine($"   ID: {reader.GetInt32(0)}, Tên: {reader.GetString(1)}, Vai trò: {roleName}");

                                return new LoginResult
                                {
                                    Success = true,
                                    UserId = reader.GetInt32(0),
                                    FullName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Role = roleName
                                };
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"   KHÔNG TÌM THẤY USER khớp!");
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"=== LỖI SQL (MVC_TEACHER) ===");
                System.Diagnostics.Debug.WriteLine($"Số lỗi: {sqlEx.Number}, Message: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== LỖI TỔNG QUÁT (MVC_TEACHER) ===");
                System.Diagnostics.Debug.WriteLine($"{ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine("=== KẾT THÚC ĐĂNG NHẬP (THẤT BẠI) ===");
            return new LoginResult { Success = false };
        }

        private string MapRoleIdToName(int maVaiTro)
        {
            switch (maVaiTro)
            {
                case 1: return "Admin";
                case 2: return "Teacher";
                case 3: return "Student";
                default: return "Unknown";
            }
        }

        // Giữ nguyên các phương thức khác nếu cần
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

        /// <summary>
        /// Đăng ký user mới - CHO MVC_TEACHER
        /// </summary>
        public bool RegisterUser(string fullName, string email, string password, string role = "Teacher", string phoneNumber = null)
        {
            System.Diagnostics.Debug.WriteLine($"\n=== MVC_TEACHER: ĐĂNG KÝ USER ===");
            System.Diagnostics.Debug.WriteLine($"Họ tên: {fullName}, Email: {email}, Role: {role}");

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
                        int count = (int)cmd.ExecuteScalar();
                        System.Diagnostics.Debug.WriteLine($"Kiểm tra email trùng: {count} user(s)");

                        if (count > 0)
                        {
                            System.Diagnostics.Debug.WriteLine("   LỖI: Email đã tồn tại!");
                            return false;
                        }
                    }

                    // Insert mới
                    string insertSql = @"
                        INSERT INTO NguoiDung (HoTen, Email, MatKhau, MaVaiTro, SoDienThoai, NgayTao, TrangThai)
                        VALUES (@HoTen, @Email, @MatKhau, @MaVaiTro, @SoDienThoai, GETDATE(), 1)";

                    using (var cmd = new SqlCommand(insertSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@HoTen", fullName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@MatKhau", password);
                        cmd.Parameters.AddWithValue("@MaVaiTro", maVaiTro);
                        cmd.Parameters.AddWithValue("@SoDienThoai", (object)phoneNumber ?? DBNull.Value);

                        int result = cmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"Kết quả insert: {result} row(s) affected");

                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== LỖI ĐĂNG KÝ USER ===");
                System.Diagnostics.Debug.WriteLine($"{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lấy MaVaiTro từ role string - CHO MVC_TEACHER
        /// </summary>
        private int GetMaVaiTroByRole(string role)
        {
            if (string.IsNullOrEmpty(role))
                return 2; // Mặc định là Teacher cho MVC_TEACHER

            switch (role.ToLower())
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

        ///         /// <summary>
        /// Lấy thông tin Teacher bằng email - DÀNH CHO MVC_TEACHER
        /// </summary>
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
                                CreatedDate = DateTime.Now
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

        /// <summary>
        /// Lấy số lần đăng nhập của Teacher - DÀNH CHO MVC_TEACHER
        /// </summary>
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

        /// <summary>
        /// Cập nhật thông tin Teacher - DÀNH CHO MVC_TEACHER
        /// </summary>
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

        /// <summary>
        /// Đăng ký user mới với đầy đủ tham số - DÀNH CHO MVC_TEACHER
        /// </summary>
        public bool RegisterUser(string fullName, string email, string password, string role,
            string phoneNumber = null, string gender = null, string address = null, DateTime? dateOfBirth = null)
        {
            System.Diagnostics.Debug.WriteLine($"\n=== MVC_TEACHER: ĐĂNG KÝ USER (8 tham số) ===");
            System.Diagnostics.Debug.WriteLine($"Họ tên: {fullName}, Email: {email}, Role: {role}");

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
                        int count = (int)cmd.ExecuteScalar();
                        System.Diagnostics.Debug.WriteLine($"Kiểm tra email trùng: {count} user(s)");

                        if (count > 0)
                        {
                            System.Diagnostics.Debug.WriteLine("   LỖI: Email đã tồn tại!");
                            return false;
                        }
                    }

                    // Insert mới với đầy đủ thông tin
                    string insertSql = @"
                        INSERT INTO NguoiDung (HoTen, Email, MatKhau, MaVaiTro, SoDienThoai, GioiTinh, DiaChi, NgaySinh, NgayTao, TrangThai)
                        VALUES (@HoTen, @Email, @MatKhau, @MaVaiTro, @SoDienThoai, @GioiTinh, @DiaChi, @NgaySinh, GETDATE(), 1)";

                    using (var cmd = new SqlCommand(insertSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@HoTen", (object)fullName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@MatKhau", password);
                        cmd.Parameters.AddWithValue("@MaVaiTro", maVaiTro);
                        cmd.Parameters.AddWithValue("@SoDienThoai", (object)phoneNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@GioiTinh", (object)gender ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DiaChi", (object)address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NgaySinh", (object)dateOfBirth ?? DBNull.Value);

                        int result = cmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"Kết quả insert: {result} row(s) affected");

                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== LỖI ĐĂNG KÝ USER (8 tham số) ===");
                System.Diagnostics.Debug.WriteLine($"{ex.Message}");
                throw;
            }
        }
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
                        // Tạo tài khoản Teacher mẫu
                        checkCmd.Parameters.AddWithValue("@Email", "teacher@hoclieu.edu.vn");
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count == 0)
                        {
                            // Insert user mẫu (Teacher)
                            string insertSql = @"
                                INSERT INTO NguoiDung (HoTen, Email, MatKhau, MaVaiTro, NgayTao, TrangThai)
                                VALUES (@HoTen, @Email, @MatKhau, @MaVaiTro, GETDATE(), 1)";

                            using (var cmd = new SqlCommand(insertSql, connection))
                            {
                                cmd.Parameters.AddWithValue("@HoTen", "Giáo Viên Mẫu");
                                cmd.Parameters.AddWithValue("@Email", "teacher@hoclieu.edu.vn");
                                cmd.Parameters.AddWithValue("@MatKhau", "teacher123");
                                cmd.Parameters.AddWithValue("@MaVaiTro", 2); // Teacher

                                int result = cmd.ExecuteNonQuery();
                                System.Diagnostics.Debug.WriteLine($"Đã tạo teacher mẫu: {result > 0}");
                            }
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating sample user: {ex.Message}");
                return false;
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
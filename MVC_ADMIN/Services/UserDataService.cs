using System;
using System.Configuration; // QUAN TRỌNG: Để đọc Web.config
using System.Data;
using System.Data.SqlClient;
using System.IO;
using MVC_ADMIN.Helpers;
using Newtonsoft.Json;

namespace MVC_ADMIN.Services
{
    public class UserDataService
    {
        private readonly string _connectionString;

        public UserDataService()
        {
            try
            {
                // ĐỌC appsettings.json từ thư mục gốc solution (cùng cấp với LTWNC)
                // Khi chạy, BaseDirectory = ...\MVC_ADMIN\bin\Debug\
                // → lên 1 cấp ".." → ...\MVC_ADMIN\
                // → nhưng file appsettings.json nằm ở cấp cao hơn (LTWNC)
                // → thực tế chỉ cần lên 1 cấp từ bin là tới thư mục chứa appsettings (theo bạn xác nhận)
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "appsettings.json");

                // Chuẩn hóa đường dẫn để tránh lỗi
                filePath = Path.GetFullPath(filePath);

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Không tìm thấy file appsettings.json tại: {filePath}. Hãy kiểm tra lại vị trí file.");
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

        public LoginResult LoginUser(string email, string password)
        {
            System.Diagnostics.Debug.WriteLine($"\n=== BẮT ĐẦU ĐĂNG NHẬP ===");
            System.Diagnostics.Debug.WriteLine($"Email nhập: '{email}'");
            System.Diagnostics.Debug.WriteLine($"Password nhập (độ dài {password?.Length}): '{password}'");

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                System.Diagnostics.Debug.WriteLine("LỖI: Connection String bị rỗng.");
                return new LoginResult { Success = false };
            }

            System.Diagnostics.Debug.WriteLine($"Sử dụng Connection String: {_connectionString}");

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // THỬ MỞ KẾT NỐI
                    System.Diagnostics.Debug.WriteLine("Đang thử mở kết nối Database...");
                    connection.Open();
                    System.Diagnostics.Debug.WriteLine($"   THÀNH CÔNG! Kết nối đã mở. Database: {connection.Database}");

                    // THỰC THI TRUY VẤN (ĐÃ ĐƠN GIẢN HÓA)
                    // Tạm bỏ LTRIM/RTRIM và chỉ kiểm tra với bảng NguoiDung trước
                    string sql = @"
                        SELECT TOP 1 nd.MaNguoiDung, nd.HoTen, nd.Email, nd.MatKhau, nd.MaVaiTro
                        FROM NguoiDung nd
                        WHERE nd.Email = @Email 
                          AND nd.MatKhau = @MatKhau
                          AND nd.TrangThai = 1";

                    System.Diagnostics.Debug.WriteLine($"Đang thực thi truy vấn: {sql}");

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@MatKhau", password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Lấy MaVaiTro và map sang tên Role
                                int maVaiTro = reader.GetInt32(4); // Cột MaVaiTro
                                string roleName = MapRoleIdToName(maVaiTro);

                                System.Diagnostics.Debug.WriteLine($"   TÌM THẤY USER!");
                                System.Diagnostics.Debug.WriteLine($"   ID: {reader.GetInt32(0)}, Tên: {reader.GetString(1)}, Vai trò (ID): {maVaiTro}, Vai trò (Tên): {roleName}");

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
                                System.Diagnostics.Debug.WriteLine($"   KHÔNG TÌM THẤY USER nào khớp với email và mật khẩu này.");
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"=== LỖI SQL KHI ĐĂNG NHẬP ===");
                System.Diagnostics.Debug.WriteLine($"Số lỗi: {sqlEx.Number}");
                System.Diagnostics.Debug.WriteLine($"Thông báo: {sqlEx.Message}");
                System.Diagnostics.Debug.WriteLine($"Server: {sqlEx.Server}");
                // Đây là thông tin cực kỳ quan trọng!
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== LỖI TỔNG QUÁT KHI ĐĂNG NHẬP ===");
                System.Diagnostics.Debug.WriteLine($"{ex.ToString()}");
            }

            System.Diagnostics.Debug.WriteLine("=== KẾT THÚC ĐĂNG NHẬP (THẤT BẠI) ===");
            return new LoginResult { Success = false };
        }

        private string MapRoleIdToName(int maVaiTro)
        {
            // Map trực tiếp từ ID, không cần JOIN
            switch (maVaiTro)
            {
                case 1: return "Admin";
                case 2: return "Teacher";
                case 3: return "Student";
                default: return "Unknown";
            }
        }
        // ===== CÁC PHƯƠNG THỨC KHÁC - GIỮ NGUYÊN TỪ CODE CŨ =====

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
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
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

    public class LoginResult
    {
        public bool Success { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
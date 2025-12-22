using System;
using System.Configuration; // THÊM DÒNG NÀY
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using MVC_STUDENT.Models;

namespace MVC_STUDENT.Services
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
            System.Diagnostics.Debug.WriteLine($"\n=== MVC_STUDENT: BẮT ĐẦU ĐĂNG NHẬP ===");
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

                    // DÙNG CÂU TRUY VẤN ĐƠN GIẢN (giống mvc_admin đã chạy được)
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
                        cmd.Parameters.AddWithValue("@MatKhau", password);

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
                System.Diagnostics.Debug.WriteLine($"=== LỖI SQL (MVC_STUDENT) ===");
                System.Diagnostics.Debug.WriteLine($"Số lỗi: {sqlEx.Number}, Message: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== LỖI TỔNG QUÁT (MVC_STUDENT) ===");
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
        /// Đăng ký user mới - CHO MVC_STUDENT
        /// </summary>
        public bool RegisterUser(string fullName, string email, string password, string role = "Student", string phoneNumber = null)
        {
            System.Diagnostics.Debug.WriteLine($"\n=== MVC_STUDENT: ĐĂNG KÝ USER ===");
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
                throw; // Để controller bắt lỗi
            }
        }

        /// <summary>
        /// Lấy MaVaiTro từ role string - CHO MVC_STUDENT
        /// </summary>
        private int GetMaVaiTroByRole(string role)
        {
            if (string.IsNullOrEmpty(role))
                return 3; // Mặc định là Student

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
                    return 3; // Luôn mặc định là Student cho MVC_STUDENT
            }
        }
    }
}
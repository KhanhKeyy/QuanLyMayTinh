using System.Security.Cryptography;
using System.Text;

namespace QuanLyBanMayVT.Common
{
    /// <summary>
    /// Tiện ích băm và kiểm tra mật khẩu bằng SHA-256.
    /// Không cần NuGet — dùng thư viện có sẵn trong .NET.
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Chuyển mật khẩu thuần sang chuỗi SHA-256 (chữ thường, 64 ký tự).
        /// Ví dụ: Hash("123456") → "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92"
        /// </summary>
        public static string Hash(string plainPassword)
        {
            byte[] bytes  = SHA256.HashData(Encoding.UTF8.GetBytes(plainPassword));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        /// <summary>
        /// Kiểm tra mật khẩu thuần có khớp với hash đã lưu trong DB không.
        /// </summary>
        public static bool Verify(string plainPassword, string storedHash)
        {
            string hash = Hash(plainPassword);
            return string.Equals(hash, storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}

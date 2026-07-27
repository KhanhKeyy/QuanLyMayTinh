namespace QuanLyBanMayVT
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Khắc phục triệt để lỗi mã hóa font tiếng Việt và tự nạp lại dữ liệu linh kiện chuẩn Unicode
            QuanLyBanMayVT.DataAccess.DataSeeder.FixFontAndSeedData();

            Application.Run(new frmDangNhap());
        }
    }
}
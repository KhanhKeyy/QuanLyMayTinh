using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyBanMayVT.Models;

namespace QuanLyBanMayVT.Common
{
    public static class GioHangManager
    {
        private static readonly List<GioHangItem> _items = new();

        public static List<GioHangItem> Items => _items;

        public static int TongSoLuong => _items.Sum(i => i.SoLuong);
        public static decimal TongTien => _items.Sum(i => i.ThanhTien);
        public static string TongTienFormatted => TongTien.ToString("N0") + " đ";

        public static void ThemVaoGio(SanPham sp, int soLuong = 1)
        {
            if (sp == null || sp.SoLuongTon <= 0) return;

            var existing = _items.FirstOrDefault(i => i.SanPham.MaSanPham == sp.MaSanPham);
            if (existing != null)
            {
                existing.SoLuong += soLuong;
                if (existing.SoLuong > sp.SoLuongTon) existing.SoLuong = sp.SoLuongTon;
            }
            else
            {
                _items.Add(new GioHangItem
                {
                    SanPham = sp,
                    SoLuong = Math.Min(soLuong, Math.Max(1, sp.SoLuongTon))
                });
            }
        }

        public static void CapNhatSoLuong(int maSanPham, int soLuongMoi)
        {
            var item = _items.FirstOrDefault(i => i.SanPham.MaSanPham == maSanPham);
            if (item != null)
            {
                if (soLuongMoi <= 0)
                {
                    _items.Remove(item);
                }
                else
                {
                    item.SoLuong = Math.Min(soLuongMoi, Math.Max(1, item.SanPham.SoLuongTon));
                }
            }
        }

        public static void XoaKhoiGio(int maSanPham)
        {
            _items.RemoveAll(i => i.SanPham.MaSanPham == maSanPham);
        }

        public static void XoaTatCa()
        {
            _items.Clear();
        }
    }
}

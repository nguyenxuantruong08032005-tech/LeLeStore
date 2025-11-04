using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeLeStore
{
    public enum UserRole
    {
        QuanLy,
        BanHang,
        Kho
    }


    public static class UserRoleExtensions
    {
        public static bool TryParse(string value, out UserRole role)
        {
            role = UserRole.QuanLy;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string normalized = value.Trim()
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty)
                .ToUpperInvariant();

            switch (normalized)
            {
                case "QUANLY":
                    role = UserRole.QuanLy;
                    return true;
                case "BANHANG":
                    role = UserRole.BanHang;
                    return true;
                case "KHO":
                    role = UserRole.Kho;
                    return true;
                default:
                    return false;
            }
        }

        public static string ToDisplayName(this UserRole role)
        {
            switch (role)
            {
                case UserRole.QuanLy:
                    return "Quản lý";
                case UserRole.BanHang:
                    return "Bán hàng";
                case UserRole.Kho:
                    return "Kho";
                default:
                    return role.ToString();
            }
        }
    }
}

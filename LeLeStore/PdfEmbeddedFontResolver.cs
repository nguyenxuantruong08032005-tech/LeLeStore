using System;
using System.IO;
using PdfSharp.Fonts;

namespace LeLeStore
{
    public class PdfEmbeddedFontResolver : IFontResolver
    {
        // Định danh nội bộ cho 2 font
        private const string DejaVuSansRegularId = "DejaVuSans#Regular";
        private const string DejaVuSansBoldId = "DejaVuSans#Bold";

        // Đăng ký global trong Program.cs: PdfEmbeddedFontResolver.RegisterGlobal();
        public static void RegisterGlobal()
        {
            if (GlobalFontSettings.FontResolver == null)
            {
                GlobalFontSettings.FontResolver = new PdfEmbeddedFontResolver();
            }
        }

        // Trả về "ID font" tương ứng với family + style
        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            // Không dùng italic, nên gộp chung
            var name = familyName?.Trim().ToLowerInvariant();

            if (name == "dejavusans" || name == "sans-serif" || name == "arial")
            {
                if (isBold)
                    return new FontResolverInfo(DejaVuSansBoldId);

                return new FontResolverInfo(DejaVuSansRegularId);
            }

            // Font lạ -> fallback về DejaVuSans
            return new FontResolverInfo(DejaVuSansRegularId);
        }

        // Trả về bytes của font theo ID phía trên
        public byte[] GetFont(string faceName)
        {
            switch (faceName)
            {
                case DejaVuSansRegularId:
                    return Properties.Resources.DejaVuSans;          // tên resource trong hình

                case DejaVuSansBoldId:
                    return Properties.Resources.DejaVuSans_Bold;      // tên resource trong hình

                default:
                    throw new ArgumentException("Unknown font: " + faceName);
            }
        }
    }
}

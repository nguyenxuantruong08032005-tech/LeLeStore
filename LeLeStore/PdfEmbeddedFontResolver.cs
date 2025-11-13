using System;
using System.IO;
using PdfSharp.Fonts;

namespace LeLeStore
{
    internal sealed class PdfEmbeddedFontResolver : IFontResolver
    {
        private const string RegularFontKey = "dejavusans#r";
        private const string BoldFontKey = "dejavusans#b";
        private const string FamilyName = "DejaVu Sans";

        private readonly byte[] _regularFontData;
        private readonly byte[] _boldFontData;

        public PdfEmbeddedFontResolver()
        {
            _regularFontData = LoadFontData("DejaVuSans.ttf");
            _boldFontData = LoadFontData("DejaVuSans-Bold.ttf");
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (string.IsNullOrWhiteSpace(familyName))
            {
                return new FontResolverInfo(RegularFontKey);
            }

            if (familyName.Equals(FamilyName, StringComparison.OrdinalIgnoreCase) ||
                familyName.Equals("DejaVuSans", StringComparison.OrdinalIgnoreCase))
            {
                return new FontResolverInfo(isBold ? BoldFontKey : RegularFontKey);
            }

            return null;
        }

        public byte[] GetFont(string faceName)
        {
            switch (faceName)
            {
                case RegularFontKey:
                    return _regularFontData;
                case BoldFontKey:
                    return _boldFontData;
                default:
                    throw new InvalidOperationException($"Không tìm thấy dữ liệu phông chữ cho khóa '{faceName}'.");
            }
        }

        private static byte[] LoadFontData(string fileName)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
            var searchPaths = new[]
            {
                Path.Combine(baseDirectory, "Resources", "Fonts", fileName),
                Path.Combine(baseDirectory, "Fonts", fileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, fileName)
            };

            foreach (var path in searchPaths)
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    return File.ReadAllBytes(path);
                }
            }

            var resourceName = $"LeLeStore.Resources.Fonts.{fileName}";
            var assembly = typeof(PdfEmbeddedFontResolver).Assembly;
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }

            throw new FileNotFoundException($"Không thể tải phông chữ '{fileName}'.", fileName);
        }
    }
}
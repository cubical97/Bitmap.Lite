using System;
using System.Collections.Generic;
using System.IO;

namespace Bitmap.Lite
{
    /// <summary>
    /// Decodes nameof(Header) and pixels dana of bitmap image type.
    /// </summary>
    public class Bitmap
    {
        /// <summary>
        /// Header of bitmap image.
        /// </summary>
        public BitmapHeader Header { get; private set; }
        /// <summary>
        /// Array of pixels.
        /// </summary>
        public Pixel[] Pixels { get; private set; }

        private Bitmap(BitmapHeader header, Pixel[] pixels)
        {
            Header = header;
            Pixels = pixels;
        }

        public static Bitmap FromStream(Stream stream)
        {
            stream.Position = 0;
            var bitmapHeader = BitmapHeader.FromStream(stream);

            if (bitmapHeader.Compression != 0)
                throw new InvalidDataException("Compressed images are not supported.");

            if (bitmapHeader.BitsPerPixel != 24 && bitmapHeader.BitsPerPixel != 32)
                throw new InvalidDataException($"Color depth of ({bitmapHeader.BitsPerPixel}) is not supported. Only 24 and 32 bit color depth is supported by this converter.");

            stream.Position = bitmapHeader.Offset;
            var pixels = new List<Pixel>();

            for (var i_width = 0; i_width < bitmapHeader.WidthPx; i_width++)
            {
                for (var i_heigth = 0; i_heigth < bitmapHeader.HeightPx; i_heigth++)
                {
                    pixels.Add(new Pixel
                    {
                        B = (byte)stream.ReadByte(),
                        G = (byte)stream.ReadByte(),
                        R = (byte)stream.ReadByte(),
                        A = (byte)(bitmapHeader.BitsPerPixel == 32 ? stream.ReadByte() : 0)
                    });
                }
                for (var j_padding = 0; j_padding < bitmapHeader.PaddingAfterEachLine; j_padding++)
                    _ = stream.ReadByte();
            }

            return new Bitmap(bitmapHeader, pixels.ToArray());
        }

        public static Bitmap FromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found at {filePath}.");
            var fileExtension = Path.GetExtension(filePath);

            if (fileExtension != ".bmp" && fileExtension != ".dib")
                throw new FormatException($"File extension '{fileExtension}' is not supported.");

            using var file = File.OpenRead(filePath);
            var bitmapAI = FromStream(file);

            file.Dispose();

            return bitmapAI;
        }
    }
}

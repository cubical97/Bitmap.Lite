using System;
using System.Collections.Generic;
using System.IO;

namespace Bitmap.Lite
{
    public class BitmapHeader
    {
        // Minimal length of header in bytes (54).
        private const int BUFFER_SIZE = 54;

        /// <summary>
        /// Magic identifier
        /// </summary>
        public ushort NumericType { get; private set; }
        /// <summary>
        /// Magic identifier
        /// </summary>
        public char[] Type { get; private set; }
        /// <summary>
        /// File size in bytes
        /// </summary>
        public uint Size { get; private set; }
        /// <summary>
        /// Not used
        /// </summary>
        public ushort Reserved1 { get; private set; }
        /// <summary>
        /// Not used
        /// </summary>
        public ushort Reserved2 { get; private set; }
        /// <summary>
        /// Offset to image data in bytes from beginning of file (54 bytes)
        /// </summary>
        public uint Offset { get; private set; }
        /// <summary>
        /// DIB Header size in bytes (40 bytes)
        /// </summary>
        public uint DibHeaderSize { get; private set; }
        /// <summary>
        /// Width of the image
        /// </summary>
        public int WidthPx { get; private set; }
        /// <summary>
        /// Height of image
        /// </summary>
        public int HeightPx { get; private set; }
        /// <summary>
        /// Number of color planes
        /// </summary>
        public ushort NumPlanes { get; private set; }
        /// <summary>
        /// Bits per pixel
        /// </summary>
        public ushort BitsPerPixel { get; private set; }
        /// <summary>
        /// Compression type
        /// </summary>
        public uint Compression { get; private set; }
        /// <summary>
        /// Image size in bytes
        /// </summary>
        public uint ImageSizeBytes { get; private set; }
        /// <summary>
        /// Pixels per meter
        /// </summary>
        public int XResolutionPpm { get; private set; }
        /// <summary>
        /// Pixels per meter
        /// </summary>
        public int YResolutionPpm { get; private set; }
        /// <summary>
        /// Number of colors
        /// </summary>
        public uint NumColors { get; private set; }
        /// <summary>
        /// Important colors
        /// </summary>
        public uint ImportantColors { get; private set; }
        /// <summary>
        /// Number of empty bytes saved at the end of pixels row.
        /// </summary>
        public int PaddingAfterEachLine { get => BitsPerPixel == 32 ? 0 : WidthPx % 4; }

        public static BitmapHeader FromStream(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (stream.Length < BUFFER_SIZE)
                throw new ArgumentException($"Length of {nameof(stream)} must be at least {BUFFER_SIZE} bytes");

            byte[] buffer = new byte[BUFFER_SIZE];
            _ = stream.Read(buffer, 0, buffer.Length);

            return new BitmapHeader(buffer);
        }

        public static BitmapHeader FromByteArray(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data);
            if (data.Length < BUFFER_SIZE)
                throw new ArgumentException($"Length of {nameof(data)} must be at least {BUFFER_SIZE} bytes");

            return new BitmapHeader(data);
        }

        public byte[] ToByteArray()
        {
            var buffer = new List<byte>();

            buffer.AddRange([(byte)Type[0], (byte)Type[1]]);
            buffer.AddRange(BitConverter.GetBytes(Size));
            buffer.AddRange(BitConverter.GetBytes(Reserved1));
            buffer.AddRange(BitConverter.GetBytes(Reserved2));
            buffer.AddRange(BitConverter.GetBytes(Offset));
            buffer.AddRange(BitConverter.GetBytes(DibHeaderSize));
            buffer.AddRange(BitConverter.GetBytes(WidthPx));
            buffer.AddRange(BitConverter.GetBytes(HeightPx));
            buffer.AddRange(BitConverter.GetBytes(NumPlanes));
            buffer.AddRange(BitConverter.GetBytes(BitsPerPixel));
            buffer.AddRange(BitConverter.GetBytes(Compression));
            buffer.AddRange(BitConverter.GetBytes(ImageSizeBytes));
            buffer.AddRange(BitConverter.GetBytes(XResolutionPpm));
            buffer.AddRange(BitConverter.GetBytes(YResolutionPpm));
            buffer.AddRange(BitConverter.GetBytes(NumColors));
            buffer.AddRange(BitConverter.GetBytes(ImportantColors));

            for (int i = BUFFER_SIZE; i < Offset; i++)
                buffer.Add(0);

            return buffer.ToArray();
        }

        public Stream ToStream()
        {
            return new MemoryStream(ToByteArray());
        }

        private BitmapHeader(byte[] data)
        {
            Type = [(char)data[0], (char)data[1]];
            NumericType = BitConverter.ToUInt16([data[0], data[1]]);
            Size = BitConverter.ToUInt32([data[2], data[3], data[4], data[5]]);
            Reserved1 = BitConverter.ToUInt16([data[6], data[7]]);
            Reserved2 = BitConverter.ToUInt16([data[8], data[9]]);
            Offset = BitConverter.ToUInt32([data[10], data[11], data[12], data[13]]);
            DibHeaderSize = BitConverter.ToUInt32([data[14], data[15], data[16], data[17]]);
            WidthPx = BitConverter.ToInt32([data[18], data[19], data[20], data[21]]);
            HeightPx = BitConverter.ToInt32([data[22], data[23], data[24], data[25]]);
            NumPlanes = BitConverter.ToUInt16([data[26], data[27]]);
            BitsPerPixel = BitConverter.ToUInt16([data[28], data[29]]);
            Compression = BitConverter.ToUInt32([data[30], data[31], data[32], data[33]]);
            ImageSizeBytes = BitConverter.ToUInt32([data[34], data[35], data[36], data[37]]);
            XResolutionPpm = BitConverter.ToInt32([data[38], data[39], data[40], data[41]]);
            YResolutionPpm = BitConverter.ToInt32([data[42], data[43], data[44], data[45]]);
            NumColors = BitConverter.ToUInt32([data[46], data[47], data[48], data[49]]);
            ImportantColors = BitConverter.ToUInt32([data[50], data[51], data[52], data[53]]);
        }
    }
}

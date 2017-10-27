using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;

namespace MakePdf
{
    class ImageHelper
    {
        const int DestWidth = 2048;

        static readonly Regex rxDateTime = new Regex("P_(?<year>\\d{4})(?<month>\\d{2})(?<day>\\d{2})_(?<hour>\\d{2})(?<minute>\\d{2})\\d{2}");

        static Image GetImage(string file)
        {
            var source = Image.FromFile(file);

            var ratio = (double)DestWidth / source.Width;
            var destHeight = (int)(ratio * source.Height);

            var dest = new Bitmap(DestWidth, destHeight);
            dest.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            using (var graphics = Graphics.FromImage(dest))
            {
                graphics.InterpolationMode = InterpolationMode.Low;
                graphics.DrawImage(source, new Rectangle(0, 0, DestWidth, destHeight));
            }

            return dest;
        }

        internal static MemoryStream GetCompressed(string file)
        {
            using (var image = DrawText(file, GetImage(file)))
            {
                var ms = new MemoryStream();

                var encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 90L);
                image.Save(ms, GetImageCodeInfo("image/jpeg"), encoderParameters);

                ms.Position = 0;
                return ms;
            }
        }

        static Image DrawText(string file, Image image)
        {
            using (var graphics = Graphics.FromImage(image))
            using (var brush = new SolidBrush(Color.Red))
            using (var font = new Font("Helvetica", DestWidth / 34.133F))
            {
                graphics.DrawString(GetDateTime(file), font, brush, new PointF(DestWidth / 1.463F, DestWidth / 2.048F));
            }

            return image;
        }

        static string GetDateTime(string file)
        {
            var mc = rxDateTime.Match(Path.GetFileNameWithoutExtension(file));
            if (!mc.Success)
                return "???";
            return $"{mc.Groups["day"].Value}/{mc.Groups["month"].Value}/{mc.Groups["year"].Value} {mc.Groups["hour"].Value}:{mc.Groups["minute"].Value}";
        }

        static ImageCodecInfo GetImageCodeInfo(string mimeType)
        {
            var info = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in info)
            {
                if (ici.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase))
                    return ici;
            }
            return null;
        }
    }
}

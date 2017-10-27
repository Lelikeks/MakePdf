using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace MakePdf
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(@"D:\Drive\000", "*.jpg");
            if (files.Length == 0)
                return;

            var image = Image.GetInstance(ImageHelper.GetCompressed(files[0]));

            using (var stream = File.Create(@"D:\Drive\000\Фото.pdf"))
            using (var document = new Document(new Rectangle(image.Width, image.Height), 0, 0, 0, 0))
            {
                PdfWriter.GetInstance(document, stream);
                document.Open();

                document.Add(image);

                for (int i = 1; i < files.Length; i++)
                {
                    image = Image.GetInstance(ImageHelper.GetCompressed(files[i]));
                    document.Add(image);
                }
            }
        }
    }
}

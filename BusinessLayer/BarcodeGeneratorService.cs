using System;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Rendering;
using ZXing.Windows.Compatibility; 

namespace BusinessLayer
{
    public class BarcodeGeneratorService
    {
        public byte[] GenerateBarcode(string sku)
        {
            var barcodeWriter = new BarcodeWriter<Bitmap>
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 150,
                    Margin = 2
                },
                Renderer = new BitmapRenderer()
            };

            // Barkod resmini oluştur
            Bitmap barcodeBitmap = barcodeWriter.Write(sku);

            // Bitmap'i byte[] olarak döndür
            using (var memoryStream = new MemoryStream())
            {
                barcodeBitmap.Save(memoryStream, ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }
    }
}

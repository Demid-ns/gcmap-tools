using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.Services
{
    public class ImageCompessor
    {
        //Вызываем метод для сжатия картинки
        public async Task CompressAsync(Stream imageStream, Stream fileStream, int quality)
        {
            using var image = new MagickImage(imageStream)
            {
                //Формат картинки
                Format = MagickFormat.Jpeg,
                //Передаваемое в сигнатуре итоговое качество сжатия
                Quality = quality
            };
            await image.WriteAsync(fileStream);
        }
    }
}

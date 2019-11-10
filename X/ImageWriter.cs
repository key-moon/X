//#undef DEBUG
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

#if DEBUG
public static class ImageWriter
{
    static List<Bitmap> bmps = new List<Bitmap>();
    public static void Add(Bitmap bmp) => bmps.Add(bmp);
    public static void Add(Image image) => Add(ConvertToBitMap(image));

    const int mergin = 10;
    public static void Add(Image[] images)
    {
        var h = images.Max(x => x.H);
        var w = images.Sum(x => x.W) + mergin * (images.Length - 1);
        var bitmap = CreateFilledBitmap(h, w, Color.Gray);

        int xOffset = 0;
        int yOffset = 0;
        foreach (var image in images)
        {
            yOffset = (h - image.H) / 2;

            for (int y = 0; y < image.H; y++)
                for (int x = 0; x < image.W; x++)
                    bitmap.SetPixel(xOffset + x, yOffset + y, image[y, x] ? Color.Black : Color.White);

            xOffset += image.W + mergin;
        }
        Add(bitmap);
    }

    public static void Write(string name)
    {
        var h = bmps.Sum(x => x.Height) + mergin * (bmps.Count - 1);
        var w = bmps.Max(x => x.Width);
        Bitmap res = CreateFilledBitmap(h, w, Color.DarkGray);
        int xOffset = 0;
        int yOffset = 0;
        foreach (var bmp in bmps)
        {
            xOffset = (w - bmp.Width) / 2;
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                    res.SetPixel(xOffset + x, yOffset + y, bmp.GetPixel(x, y));
            yOffset += bmp.Height + mergin;
            bmp.Dispose();
        }
        res.Save(Secret.ProjectPath + $@"\img\{name}.png");
        res.Dispose();
        bmps.Clear();
    }

    private static Bitmap ConvertToBitMap(Image image)
    {
        Bitmap bitmap = new Bitmap(image.W, image.H);
        for (int i = 0; i < image.H; i++)
            for (int j = 0; j < image.W; j++)
                bitmap.SetPixel(j, i, image[i, j] ? Color.Black : Color.White);
        return bitmap;
    }

    private static Bitmap CreateFilledBitmap(int h, int w, Color color)
    {
        var bitmap = new Bitmap(w, h);
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                bitmap.SetPixel(x, y, color);
        return bitmap;
    }
}
#else
public static class ImageWriter
{
    static List<Bitmap> bmps = new List<Bitmap>();
    public static void Add(Bitmap bmp) { }
    public static void Add(Image image) { }

    const int mergin = 10;
    public static void Add(Image[] images) { }

    public static void Write(string name) { }
}
#endif

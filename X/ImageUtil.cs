#if DEBUG
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

public static class ImageUtil
{
    public static Bitmap JoinHorizontal(this Image[] images, int mergin)
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
        return bitmap;
    }
    public static void Save(this Image image, string name)
    {
        using (Bitmap bmp = ConvertToBitMap(image))
        {
            bmp.Save(Secret.ProjectPath + $@"\img\{name}_{Solver.CaseNum}.png");
        }
    }
    public static void SaveWithName(this Bitmap bmp, string name)
    {
        bmp.Save(Secret.ProjectPath + $@"\img\{name}_{Solver.CaseNum}.png");
    }
    public static Bitmap ConvertToBitMap(Image image)
    {
        Bitmap bitmap = new Bitmap(image.W, image.H);
        for (int i = 0; i < image.H; i++)
            for (int j = 0; j < image.W; j++)
                bitmap.SetPixel(j, i, image[i, j] ? Color.Black : Color.White);
        return bitmap;
    }
    public static Bitmap CreateFilledBitmap(int h, int w, Color color)
    {
        var bitmap = new Bitmap(w, h);
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                bitmap.SetPixel(x, y, color);
        return bitmap;
    }
}
#endif

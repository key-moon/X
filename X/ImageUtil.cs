#if DEBUG
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

public static class ImageUtil
{
    public static void Save(this Image image, string name)
    {
        using (Bitmap bmp = ConvertToBitMap(image))
        {
            bmp.Save(Secret.ProjectPath + $@"\img\{name}_{Solver.CaseNum}.png");
        }
    }
    public static Bitmap ConvertToBitMap(Image image)
    {
        Bitmap bitmap = new Bitmap(image.W, image.H);
        for (int i = 0; i < image.H; i++)
            for (int j = 0; j < image.W; j++)
                bitmap.SetPixel(j, i, image[i, j] ? Color.Black : Color.White);
        return bitmap;
    }
}
#endif

using System;
using System.Linq;
#if DEBUG
using System.Drawing;
#endif

public class X
{
    public static void Main()
    {
        int testCaseNum = int.Parse(Console.ReadLine());
        int[] wh = Console.ReadLine().Split().Select(int.Parse).ToArray();
        int w = wh[0];
        int h = wh[1];
        bool[][] pixels = Enumerable.Repeat(0, h).Select(_ => Console.ReadLine().Select(x => x == '#').ToArray()).ToArray();
        Image image = new Image(h, w, pixels);
        using (Bitmap bmp = ImageUtil.ConvertToBitMap(image))
        {
            bmp.Save(Secret.ProjectPath + @"\img\default.png");
        }
        int res = new Solver().Solve(image);
        Console.WriteLine(res);
    }
}

public class Image
{
    public readonly int H;
    public readonly int W;
    bool[] pixels;
    public Image(int h, int w, bool[][] pixels)
    {
        H = h;
        W = w;
        this.pixels = new bool[H * W];
        int ptr = 0;
        for (int i = 0; i < H; i++)
            for (int j = 0; j < W; j++)
                this.pixels[ptr++] = pixels[i][j];
    }
    public bool this[int y, int x]
    {
        get { return pixels[y * H + x]; }
        set { pixels[y * H + x] = value; }
    }
}

public class Solver
{
    public int Solve(Image image)
    {
        new NoiseCleaner().Clean(image);
        return 0;
    }
}

public class NoiseCleaner
{
    public NoiseCleaner() { }

    public void Clean(Image image)
    {
        SquareCountFilter(image, 1, 5);
        SquareCountFilter(image, 1, 5);
        using (Bitmap bmp = ImageUtil.ConvertToBitMap(image))
        {
            bmp.Save(Secret.ProjectPath + @"\img\cleaned.png");
        }
    }

    private void SquareCountFilter(Image image, int radius, int threshold)
    {
        int[] counts = new int[image.H * image.W];

        for (int y = radius; y < image.H - radius; y++)
            for (int x = radius; x < image.W - radius; x++)
                if (image[y, x]) 
                    for (int dy = -radius; dy <= radius; dy++)
                        for (int dx = -radius; dx <= radius; dx++)
                            counts[(y + dy) * image.H + (x + dx)]++;
        int ptr = 0;
        for (int i = 0; i < image.H; i++)
            for (int j = 0; j < image.W; j++)
                if (counts[ptr++] >= threshold) image[i, j] = true;
                else image[i, j] = false;
    }
}

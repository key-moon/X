using System;
using System.IO;
using System.Linq;
#if DEBUG
using System.Drawing;
#endif

public class X
{
    public static void Main()
    {
        Case testCase = Case.Parse(Console.In);
        int res = Solver.Solve(testCase);
        Console.WriteLine(res);
    }
}

public class Case
{
    public int Num;
    public Image Image;
    public static Case Parse(TextReader reader)
    {
        int testCaseNum = int.Parse(reader.ReadLine());
        int[] wh = reader.ReadLine().Split().Select(int.Parse).ToArray();
        int w = wh[0];
        int h = wh[1];
        bool[][] pixels = Enumerable.Repeat(0, h).Select(_ => reader.ReadLine().Select(x => x == '#').ToArray()).ToArray();
        Image image = new Image(h, w, pixels);
        return new Case() { Num = testCaseNum, Image = image };
    }
}

public class Image
{
    public static string Prefix;
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
        get { return pixels[y * W + x]; }
        set { pixels[y * W + x] = value; }
    }
    public void Save(string name)
    {
        using (Bitmap bmp = ImageUtil.ConvertToBitMap(this))
        {
            bmp.Save(Secret.ProjectPath + $@"\img\{name}_{Prefix}.png");
        }
    }
}

public static class Solver
{
    public static int Solve(Case testCase)
    {
        Image.Prefix = testCase.Num.ToString();

        NoiseCleaner.Clean(testCase.Image);


        return 0;
    }
}

public static class NoiseCleaner
{
    public static void Clean(Image image)
    {
        SquareCountFilter(image, 1, 5);
        SquareCountFilter(image, 1, 7);
        image.Save("cleaned");
    }

    private static void SquareCountFilter(Image image, int radius, int threshold)
    {
        int[] counts = new int[image.H * image.W];

        for (int y = radius; y < image.H - radius; y++)
            for (int x = radius; x < image.W - radius; x++)
                if (image[y, x]) 
                    for (int dy = -radius; dy <= radius; dy++)
                        for (int dx = -radius; dx <= radius; dx++)
                            counts[(y + dy) * image.W + (x + dx)]++;
        int ptr = 0;
        for (int i = 0; i < image.H; i++)
            for (int j = 0; j < image.W; j++)
                if (counts[ptr++] >= threshold) image[i, j] = true;
                else image[i, j] = false;
    }
}

public class Tokenizer
{
    
}

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

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
    public Image(int h, int w)
    {
        H = h;
        W = w;
        pixels = new bool[H * W];
    }
    public Image(int h, int w, bool[][] pixels) : this(h, w)
    {
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
    public Image Trim(int minY, int maxY, int minX, int maxX)
    {
        var newH = maxY - minY + 1;
        var newW = maxX - minX + 1;
        var res = new Image(newH, newW);
        for (int y = 0; y < newH; y++)
            for (int x = 0; x < newW; x++)
                res[y, x] = this[minY + y, minX + x];
        return res;
    }
}

public static class Solver
{
    public static int Solve(Case testCase)
    {
        Image image = testCase.Image;
        ImageWriter.Add(image);
        image.Clean();
        ImageWriter.Add(image);
        var separated = image.Separate().ToArray();
        ImageWriter.Add(separated);

        var parser = new CharactorParser();
        string parsed = "";
        foreach (var charImg in separated)
            parsed += parser.Parse(charImg);
 
        ImageWriter.Add(parsed.Select(x => FontData.Data[x]).ToArray());
        ImageWriter.Write(testCase.Num.ToString());

        int evalRes = FormulaEvaluator.Eval(parsed);
        return evalRes;
    }
}

public static class NoiseCleaner
{
    public static void Clean(this Image image)
    {
        SquareCountFilter(image, 1, 5);
        SquareCountFilter(image, 1, 7);
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

public static class Separator
{
    public static IEnumerable<Image> Separate(this Image image)
    {
        int minX = int.MaxValue;
        int maxX = 0;
        int minY = int.MaxValue;
        int maxY = 0;
        for (int x = 0; x < image.W; x++)
        {
            bool hasPixel = false;
            for (int y = 0; y < image.H; y++)
                if (image[y, x])
                {
                    hasPixel = true;
                    maxY = Math.Max(maxY, y);
                    minY = Math.Min(minY, y);
                }
            if (hasPixel)
            {
                minX = Math.Min(minX, x);
                maxX = x;
            }
            else if (minX < x)
            {
                yield return image.Trim(minY, maxY, minX, maxX);
                minX = int.MaxValue;
                maxX = 0;
                minY = int.MaxValue;
                maxY = 0;
            }
        }
    }
}

public class CharactorParser
{
    public char Parse(Image charImage) => '8';
}

public static class FormulaEvaluator
{
    public static int Eval(string expression) => 0;
}

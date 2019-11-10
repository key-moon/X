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
    public double Ratio => (double)H / W;
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
    public static void Clean(this Image image, int filter1 = 5, int filter2 = 6)
    {
        SquareCountFilter(image, 1, filter1);
        SquareCountFilter(image, 1, filter2);
    }

    public static void SquareCountFilter(this Image image, int radius, int threshold)
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
        if (minX < image.W)
            yield return image.Trim(minY, maxY, minX, maxX);
    }
}

public static class RotateReducer
{
    static double[] SinCache = Enumerable.Range(0, 31).Select(x => Math.Sin(x * Math.PI / 180)).ToArray();
    static double[] CosCache = Enumerable.Range(0, 31).Select(x => Math.Cos(x * Math.PI / 180)).ToArray();

    private static double Sin(int degree)
    {
        var sign = Math.Sign(degree);
        return sign * SinCache[Math.Abs(degree)];
    }

    private static double Cos(int degree)
    {
        return CosCache[Math.Abs(degree)];
    }

    public static Image ReduceRotate(this Image image, int range)
    {
        Image minImg = image;
        for (int degree = -range; degree <= range; degree++)
        {
            var img = image.Rotate(degree).Separate().First();
            if (img.Ratio > minImg.Ratio) minImg = img;
        }
        return minImg;
    }

    public static Image Rotate(this Image image, int degree)
    {
        int xOffset = (int)Math.Ceiling(Math.Max(0, image.H * Sin(degree)));
        int yOffset = (int)Math.Ceiling(Math.Max(0, image.W * Sin(-degree)));

        int w = (int)Math.Ceiling(
                    image.W * Cos(Math.Abs(degree)) +
                    image.H * Sin(Math.Abs(degree))
                ) + 4;
        int h = (int)Math.Ceiling(
                    image.W * Sin(Math.Abs(degree)) +
                    image.H * Cos(Math.Abs(degree))
                ) + 4;

        var res = new Image(h, w);

        for (int y = 0; y < image.H; y++)
        {
            for (int x = 0; x < image.W; x++)
            {
                var newX = (int)Math.Round(x * Cos(degree) - y * Sin(degree)) + xOffset;
                var newY = (int)Math.Round(y * Cos(degree) + x * Sin(degree)) + yOffset;
                res[newY, newX] = image[y, x];
            }
        }

        return res;
    }
}

public class CharactorParser
{
    int depth = 0;
    //   +-*/ 
    // ) +-*/
    // ( 0-9
    NextChar NextChar = NextChar.OpenBracketOrNum;
    public char Parse(Image charImage)
    {
        var res = Classify(charImage);
        ChangeState(res);
        return res;
    }

    public char Classify(Image charImage)
    {
        if ((NextChar & NextChar.OpenBracket) == NextChar.OpenBracket)
            if (IsOpenBracket(charImage)) return '(';
        if ((NextChar & NextChar.CloseBracket) == NextChar.CloseBracket)
            if (IsCloseBracket(charImage)) return ')';
        if ((NextChar & NextChar.Num) == NextChar.Num)
            return ClassifyNumber(charImage);
        if ((NextChar & NextChar.Operator) == NextChar.Operator)
            return ClassifyOperator(charImage);
        throw new Exception();
    }

    private void ChangeState(char c)
    {
        if (IsNumChar(c) || IsCloseBracketChar(c))
        {
            if (IsCloseBracketChar(c)) depth--;
            NextChar = depth == 0 ? NextChar.Operator : NextChar.CloseBracketOrOperator;
        }
        else if (IsOperatorChar(c) || IsOpenBracketChar(c))
        {
            if (IsOpenBracketChar(c)) depth++;
            NextChar = NextChar.OpenBracketOrNum;
        }
    }

    private bool IsNumChar(char c) => '0' <= c && c <= '9';
    private bool IsOperatorChar(char c) => c == '+' || c == '-' || c == '*' || c == '/';
    private bool IsOpenBracketChar(char c) => c == '(';
    private bool IsCloseBracketChar(char c) => c == ')';

    const double BracketRatioMinThreshold = 2.0;
    const double BracketRatioMaxThreshold = 4.0;

    private bool IsOpenBracket(Image img)
    {
        var reduced = img.ReduceRotate(30);
        if (reduced.Ratio < BracketRatioMinThreshold) return false;
        reduced = reduced.Separate().First();
        var edgeCenter = reduced.VerticalCutCenter(0);
        return reduced.Ratio < BracketRatioMaxThreshold &&
            Math.Abs(edgeCenter - 0.5) < 0.15;
    }

    private bool IsCloseBracket(Image img)
    {
        var reduced = img.ReduceRotate(30);
        if (reduced.Ratio < BracketRatioMinThreshold) return false;
        reduced = reduced.Separate().First();
        var edgeCenter = reduced.VerticalCutCenter(1);
        return reduced.Ratio < BracketRatioMaxThreshold &&
            Math.Abs(edgeCenter - 0.5) < 0.2;
    }

    private char ClassifyOperator(Image img)
    {
        return '*';
    }

    private char ClassifyNumber(Image img)
    {
        return '0';
    }
}

[Flags]
enum NextChar
{
    Num = 1,
    Operator = 2,
    OpenBracket = 4,
    CloseBracket = 8,
    OpenBracketOrNum = OpenBracket | Num,
    CloseBracketOrOperator = CloseBracket | Operator
}

public static class Cut
{
    public static int VerticalCutCount(this Image image, double pos)
    {
        bool last = false;
        int count = 0;
        int x = (int)Math.Round((image.W - 1) * pos);
        for (int i = 0; i < image.H; i++)
        {
            if (!last && image[i, x])
                count++;
            last = image[i, x];
        }
        return count;
    }
    public static int HorizontalCutCount(this Image image, double pos)
    {
        bool last = false;
        int count = 0;
        int y = (int)Math.Round((image.H - 1) * pos);
        for (int i = 0; i < image.W; i++)
        {
            if (!last && image[y, i])
                count++;
            last = image[y, i];
        }
        return count;
    }
    public static double VerticalCutCenter(this Image image, double pos)
    {
        List<int> verts = new List<int>();
        int x = (int)Math.Round((image.W - 1) * pos);
        for (int i = 0; i < image.H; i++)
            if (image[i, x])
                verts.Add(i);
        return verts.Count == 0 ? 0.5 : verts.Average() / image.H;
    }
    public static double HorizontalCutCenter(this Image image, double pos)
    {
        List<int> verts = new List<int>();
        int y = (int)Math.Round((image.H - 1) * pos);
        for (int i = 0; i < image.W; i++)
            if (image[y, i])
                verts.Add(i);
        
        return verts.Count == 0 ? 0.5 : verts.Average() / image.W;
    }
}

public static class FormulaEvaluator
{
    public static int Eval(string expression) => 0;
}

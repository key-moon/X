using System;
using System.Linq;

public class X
{
    public static void Main()
    {
        int testCaseNum = int.Parse(Console.ReadLine());
        int[] hw = Console.ReadLine().Split().Select(int.Parse).ToArray();
        int h = hw[0];
        int w = hw[1];
        bool[][] pixels = Enumerable.Repeat(0, h).Select(_ => Console.ReadLine().Select(x => x == '#').ToArray()).ToArray();
        Image image = new Image(h, w, pixels);
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

    }
}

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
        bool[][] picture = Enumerable.Repeat(0, h).Select(_ => Console.ReadLine().Select(x => x == '#').ToArray()).ToArray();
        //solve
        int res = 0;
        Console.WriteLine(res);
    }
}


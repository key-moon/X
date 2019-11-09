#if !DEBUG
public enum Color
{
    Black,
    White
}
public class Bitmap
{
    public Bitmap(int x, int y) { }
    public void SetPixel(int x, int y, Color color) { }
    public void Save(string path) { }
}
#endif

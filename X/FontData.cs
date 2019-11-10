using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class FontData
{
    public static Dictionary<char, Image> Data = Directory.GetFiles(Secret.ProjectPath + "/font").ToDictionary
        (
            x => FileNameToChar(Path.GetFileName(x)),
            x => Case.Parse(new StreamReader(x)).Image
        );

    public static char FileNameToChar(string filename)
    {
        filename = filename.Split('.').First();
        if (filename == "div") return '/';
        if (filename == "times") return '*';
        return filename[0];
    }
}

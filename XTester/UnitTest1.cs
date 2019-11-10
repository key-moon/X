using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;

using NUnit.Framework;

public class Tests
{
    const string InputCaseDirectoryPath = Secret.ProjectPath + @"/cases/in";
    const string OutputCaseDirectoryPath = Secret.ProjectPath + @"/cases/out";
    
    static IEnumerable<string> CaseNames => Directory.GetFiles(InputCaseDirectoryPath).Select(Path.GetFileName);

    static IEnumerable<TestCaseData> Cases =>
        CaseNames.Select(CaseName =>
        {
            var testCase = Case.Parse(new StreamReader(Path.Combine(InputCaseDirectoryPath, CaseName)));
            var parsed = new StreamReader(Path.Combine(OutputCaseDirectoryPath, CaseName)).ReadToEnd().Trim();
            return new TestCaseData(testCase, parsed).SetName(testCase.Num.ToString()).Returns(true);
        });

    [SetUp]
    public void Setup()
    {

    }

    [TestCaseSource("Cases")]
    public bool Test1(Case testCase, string ans)
    {
        var res = new StringBuilder();
        Console.SetOut(new StringWriter(res));
        Solver.Solve(testCase);
        var parsed = res.ToString().Trim();
        if (ans.Zip(
            parsed,
            (x, y) => x == y || char.IsNumber(x))
            .All(x => x))
            return true;
        throw new Exception($"\nans:{ans}\nres:{parsed}");
    }
}

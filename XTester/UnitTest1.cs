using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

using NUnit.Framework;

public class Tests
{
    const string InputCaseDirectoryPath = Secret.ProjectPath + @"/cases";
    
    static IEnumerable<string> CaseNames => Directory.GetFiles(InputCaseDirectoryPath).Select(Path.GetFileName);

    static IEnumerable<TestCaseData> Cases =>
        CaseNames.Select(CaseName =>
        {
            var testCase = Case.Parse(new StreamReader(Path.Combine(InputCaseDirectoryPath, CaseName)));
            return new TestCaseData(testCase).SetName(testCase.Num.ToString());
        });

    [SetUp]
    public void Setup()
    {

    }

    [TestCaseSource("Cases")]
    public void Test1(Case testCase)
    {
        new Solver().Solve(testCase);
        Assert.Pass();
    }
}
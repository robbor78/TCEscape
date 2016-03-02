using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TCEscape;

namespace UnitTestProject1
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestMethod1()
    {
      string[][] allHarmful = new string[][]
      {
        new string[] { "" },
        new string[] { "500 0 0 500" }
      };
      string[][] allDeadly = new string[][]
      {
        new string[] { "" },
        new string[] { "0 0 0 0" }
      };

      int[] allLives = new int[] { 1, 1000 };

      int length = allHarmful.Length;
      for (int i = 1; i < length; i++)
      {
        string[] harmful = allHarmful[i];
        string[] deadly = allDeadly[i];

        Program p = new Program();
        int actual = p.lowest(harmful, deadly);

        int expected = allLives[i];

        Assert.AreEqual(expected, actual);
      }

    }
  }
}

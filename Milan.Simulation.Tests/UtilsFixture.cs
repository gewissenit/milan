#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Linq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
// ReSharper disable JoinDeclarationAndInitializer

namespace Milan.Simulation.Tests
{
  [TestFixture]
  public class UtilsFixture
  {
    private readonly string[] otherNames = new[]
                                           {
                                             "", "b", "c"
                                           };

    private void TestAllCases(string previousIncrement, string expectedEnd)
    {
      StringEmpty(previousIncrement, expectedEnd);
      StringWithSingleChar(previousIncrement, expectedEnd);
      StringWithMoreChars(previousIncrement, expectedEnd);
      StringWithSpacesInBetween(previousIncrement, expectedEnd);
      StringEndingWithSpace(previousIncrement, expectedEnd);
      StringEndingWithSpaces(previousIncrement, expectedEnd);
      StringEndingWithOpenParenthesis(previousIncrement, expectedEnd);
      StringEndingWithClosingParenthesis(previousIncrement, expectedEnd);
      StringEndingWithEmptyParentheses(previousIncrement, expectedEnd);
      StringEndingWithTextInParenthesis(previousIncrement, expectedEnd);
    }

    private void StringEmpty(string previousIncrement, string expectedEnd)
    {
      Test(string.Empty, previousIncrement, expectedEnd);
    }

    private void StringWithSingleChar(string previousIncrement, string expectedEnd)
    {
      Test("", previousIncrement, expectedEnd);
    }

    private void StringWithMoreChars(string previousIncrement, string expectedEnd)
    {
      Test("moreChars", previousIncrement, expectedEnd);
    }

    private void StringWithSpacesInBetween(string previousIncrement, string expectedEnd)
    {
      Test("spaces in between", previousIncrement, expectedEnd);
    }

    private void StringEndingWithSpace(string previousIncrement, string expectedEnd)
    {
      Test("space at the end ", previousIncrement, expectedEnd);
    }

    private void StringEndingWithSpaces(string previousIncrement, string expectedEnd)
    {
      Test("space at the end   ", previousIncrement, expectedEnd);
    }

    private void StringEndingWithOpenParenthesis(string previousIncrement, string expectedEnd)
    {
      Test("open parenthesis at the end (", previousIncrement, expectedEnd);
    }

    private void StringEndingWithClosingParenthesis(string previousIncrement, string expectedEnd)
    {
      Test("closing parenthesis at the end )", previousIncrement, expectedEnd);
    }

    private void StringEndingWithEmptyParentheses(string previousIncrement, string expectedEnd)
    {
      Test("empty pair of parentheses at the end ()", previousIncrement, expectedEnd);
    }

    private void StringEndingWithTextInParenthesis(string previousIncrement, string expectedEnd)
    {
      Test("text in parenthesis at the end (_)", previousIncrement, expectedEnd);
    }


    private static void Test(string name, string previousIncrement, string expectedEnd)
    {
      var input = string.Format("{0}{1}", name, previousIncrement);
      var expectedResult = string.Format("{0}{1}", name, expectedEnd);

      var result = JsonStore.Utils.GetIncrementedName(input);

      Assert.AreEqual(expectedResult, result);
    }

    [Test]
    public void GetIncrementedName___Result_Ends_With_First_Increment_If_Input_Was_Not_Incremented_Yet()
    {
      var previousIncrement = string.Empty; //<---- name was not incremented
      const string expectedEnd = " (1)";

      TestAllCases(previousIncrement, expectedEnd);
    }

    [Test]
    public void GetIncrementedName___Result_Ends_With_Next_Increment_If_Input_Was_Incremented_Before()
    {
      const string previousIncrement = " (1)";
      const string expectedEnd = " (2)";
      TestAllCases(previousIncrement, expectedEnd);
    }

    [Test]
    public void GetUniqueName___Returns_Given_Name_If_Name_Is_Not_Present_In_OtherNames()
    {
      const string name = "d";

      var result = JsonStore.Utils.GetUniqueName(name, otherNames);
      Assert.AreEqual(name, result);
    }

    [Test]
    public void GetUniqueName___Returns_Modified_Name_If_Name_Is_Present_In_OtherNames()
    {
      const string name = "b";
      var result = JsonStore.Utils.GetUniqueName(name, otherNames);
      Assert.IsFalse(otherNames.Any(n => n == result));
    }
  }
}

// ReSharper restore JoinDeclarationAndInitializer
// ReSharper restore InconsistentNaming
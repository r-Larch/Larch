using System;
using System.Collections;
using System.Collections.Generic;
using Larch.Host.Parser;
using NUnit.Framework;


namespace Larch.Host.Test {
    [TestFixture]
    public class FilterTester {
        [Test]
        public void FilterTest() {
            const string value = "test.www";

            Console.WriteLine("## Normal");
            var filter = new Filter("test.www", CampareType.WildCard, CompareMode.CaseIgnore);
            Test(filter, value);


            Console.WriteLine("## WildCard");
            filter = new Filter("test.???", CampareType.WildCard, CompareMode.CaseIgnore);
            Test(filter, value);


            Console.WriteLine("## Regex");
            filter = new Filter("test.[w]{3}", CampareType.Regex, CompareMode.CaseIgnore);
            Test(filter, value);
        }


        private void Test(Filter filter, string value) {
            var matches = filter.GetMatch(value, "");
            var table = new Table();
            table.Create(1, 1, "matches", matches.Matches);
            Console.WriteLine($"{value}|=> Count: {value.Length}");
            ConsoleEx.PrintHighlighted(value, matches);
            Console.WriteLine();
        }
    }
}
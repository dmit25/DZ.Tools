using System;
using DZ.Tools.Interfaces;
using NUnit.Framework;

namespace DZ.Tools.Tests
{
    [TestFixture]
    public class StringTagsWorkerTests
    {
        [Test]
        public void CanParseText()
        {
            var worker = new StringTagsWorker(
                new[]
                {
                    "a", "b", "ab", "cdf"
                },
                "a");
            var result = worker.Parser.Parse("<a>1</a> <b>2</b>  <ab>3</ab>, <cdf><a>4</a></cdf>");
            var tags = result.Tags;
            tags.Count.AssertEqualTo(5);
            tags[0].Type.AssertEqualTo("a");
            tags[1].Type.AssertEqualTo("b");
            tags[2].Type.AssertEqualTo("ab");
            result.ClearedText.Substring(tags[3].Begin, tags[3].End - tags[3].Begin).AssertEqualTo("4");
        }

        [Test]
        public void CanCompareTwoModels()
        {
            var worker = new StringTagsWorker(
                new[]
                {
                    "a", "b", "ab", "cdf", "u"
                },
                "u");
            var etalon = worker.Parser.Parse("<a>1</a> <b>2</b>  <ab>3</ab>, <cdf><a>4</a></cdf> <a>5</a>");
            var actual = worker.Parser.Parse("<a>1</a> <b>2</b>  <ab>3</ab>, <cdf>4</cdf> 5");
            var result = actual.Tags.CompareTo(
                etalon.Tags,
                TagsMatchers<string>.Strict,
                worker.Values,
                worker.Undefined);
            result.Render().ToConsole();
            //1) <a>4</a> expected but <cdf>4<cdf> was found
            //2) <a>5</a> expected but <null> was found
            result.Mismatches["a"].Count.AssertEqualTo(2);
            result.Mismatches["a"][0].ExpectedType.AssertEqualTo("a");
            result.Mismatches["a"][0].ActualType.AssertEqualTo("cdf");
            result.Mismatches["a"][1].ExpectedType.AssertEqualTo("a");
            result.Mismatches["a"][1].ActualType.AssertEqualTo("u");
            result.Mismatches["a"][1].Actual.AssertEqualTo(null);
        }
    }
}
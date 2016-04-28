using System.IO;
using NUnit.Framework;

namespace DZ.Tools.Tests
{
    [TestFixture]
    public class HtmlParserTests
    {
        private static readonly NERWorker Worker = new NERWorker();

        private const string ModelInput =
            "<P attr1=\"val1\">word1</P> word2. <O attr2=\"val2\">word3 <E gate:annotMaxId=\"4648\">word4</E></O> word5 <R>word6 \"<P>word7 word8</P>\" <O>word9</O> word10</R> <O><L>word11</L> word12</O> <P>word13</P><O>word14</O>";
        private const string ModelHtml =
            "<P>word1</P> word2. <O>word3 <E>word4</E></O> word5 <R>word6 \"<P>word7 word8</P>\" <O>word9</O> word10</R> <O><L>word11</L> word12</O> <P>word13</P><O>word14</O>";

        [Test]
        public void CanParseCorrectModel()
        {
            var cleartext =
                "word1 word2. word3 word4 word5 word6 \"word7 word8\" word9 word10 word11 word12 word13word14";
            var model = Worker.Parser.Parse(ModelInput);
            Assert.AreEqual(10, model.Tags.Count);
            Assert.AreEqual(cleartext, model.ClearedText);
            AssertAreEqual(model.Tags[0], 0, 5, TNER.P);
            AssertAreEqual(model.Tags[1], 19, 24, TNER.E);
            AssertAreEqual(model.Tags[2], 13, 24, TNER.O);
            AssertAreEqual(model.Tags[3], 38, 49, TNER.P);
            AssertAreEqual(model.Tags[4], 51, 56, TNER.O);
            AssertAreEqual(model.Tags[5], 31, 63, TNER.R);
            AssertAreEqual(model.Tags[6], 64, 70, TNER.L);
            AssertAreEqual(model.Tags[7], 64, 77, TNER.O);
            AssertAreEqual(model.Tags[8], 78, 84, TNER.P);
            AssertAreEqual(model.Tags[9], 84, 90, TNER.O);
        }

        [Test]
        public void CanRenderModelCorrectly()
        {
            var model = Worker.Parser.Parse(ModelInput);
            var render = model.Render(Worker.Renderer);
            Assert.AreEqual(ModelHtml, render);
        }

        private void AssertAreEqual(Tag<TNER> entity, int start, int end, TNER type)
        {
            Assert.AreEqual(start, entity.Begin);
            Assert.AreEqual(end, entity.End);
            Assert.AreEqual(type, entity.Type);
        }

        /// <summary>
        /// Converts xml model representation into html representation
        /// </summary>
        [Test, Ignore("Script")]
        public void Convert()
        {
            foreach (var filePath in Directory.EnumerateFiles(@"E:\tmp\Corpora"))
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var model = Worker.Parser.Parse(File.ReadAllText(filePath));
                File.WriteAllText(Path.Combine(@"E:\tmp\Corpora\res", fileName + ".txt"), model.Render(Worker.Renderer));
            }
        }
    }
}
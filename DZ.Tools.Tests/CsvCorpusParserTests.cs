using System.IO;
using NUnit.Framework;

namespace DZ.Tools.Tests
{
    [TestFixture]
    public class CsvCorpusParserTests
    {
        private static readonly NERWorker Worker = new NERWorker();
        private static readonly HtmlRenderer<TNER> _Renderer = new HtmlRenderer<TNER>((t, s) => t.Type.ToString(), (t1, t2) => t1.CompareTo(t2));

        [Test]
        public void CanParseCsvIntoCorrectModel()
        {
            const string input =
                @"The O
139th I-ORG
was O
formed O
at O
Camp I-LOC
Howe I-LOC
, O
near O
Pittsburgh I-LOC
, O
on O
September O
1 O
, O
1862 O
. O

Founding O
member O
Kojima I-PER
Minoru I-PER
played O
guitar O
on O
Good O
Day O
, O
and O
Wardanceis O
cover O
of O
a O
song O
by O
UK I-LOC
post O
punk O
industrial O
band O
Killing I-ORG
Joke I-ORG
. O";
            var parser = new CsvEntitiesParser();
            var c = parser.Parse(input);
            Assert.That(c.Render(_Renderer), Is.EqualTo("The <O>139th</O> was formed at <L>Camp Howe</L> , near <L>Pittsburgh</L> , on September 1 , 1862 . \nFounding member <P>Kojima Minoru</P> played guitar on Good Day , and Wardanceis cover of a song by <L>UK</L> post punk industrial band <O>Killing Joke</O> . "));

        }

        [Test, Ignore("Script")]
        public void ConvertCsvToHtml()
        {
            var parser = new CsvEntitiesParser();
            var c = parser.Parse(File.ReadAllText(@"C:\code\NLPGIT\Master\Sources\Release\corpus_s.txt"));
            File.WriteAllText(@"E:\corpus2.txt", c.Render(Worker.Renderer));
        }
    }
}
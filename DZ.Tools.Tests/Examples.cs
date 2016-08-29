using System;
using NUnit.Framework;

namespace DZ.Tools.Tests
{
    [TestFixture]
    public class Examples
    {
        [Test]
        public void StringExample()
        {
            var worker = new StringTagsWorker(
                tagsNames: new[] { "Org", "Geo", "Per", "O" }
                , undefinedTagName: "O");
            var expected = worker.Parser.Parse("<Per>Jack</Per> finally visited <Org>McDonald's</Org>, then was sent to the <Geo>Hospital</Geo>.");
            var actual = worker.Parser.Parse("<Per>Jack</Per> finally visited <Per>McDonald's</Per>, then was <Org>sent</Org> to the Hospital.");
            var report = actual.Tags.CompareTo(expected.Tags, TagsMatchers<string>.Strict, worker.Values, worker.Undefined);
            Console.WriteLine(report.Render());

            var text = actual.ClearedText;
            Console.WriteLine(report.RenderMatchesAndMismatches(t => text.Substring(t.Begin, t.End - t.Begin)));
        }

        enum Type { O, Org, Geo, Per }
        class EnumWorker : EnumsTagWorker<Type>
        {
            public EnumWorker() : base(
                undefinedValue: Type.O,
                valuesComparer: (t1, t2) => t1.CompareTo(t2))
            {
                Types.Add("Location", Type.Geo);
            }
        }

        [Test]
        public void EnumExample()
        {
            var worker = new EnumWorker();
            var expected = worker.Parser.Parse("<Per>Jack</Per> finally visited <Org>McDonald's</Org>, then was sent to the <Location>Hospital</Location>.");
            var actual = worker.Parser.Parse("<Per>Jack</Per> finally visited <Per>McDonald's</Per>, then was <Org>sent</Org> to the Hospital.");
            var report = actual.Tags.CompareTo(expected.Tags, TagsMatchers<Type>.Strict, worker.Values, worker.Undefined);
            Console.WriteLine(report.Render());

            var text = actual.ClearedText;
            Console.WriteLine(report.RenderMatchesAndMismatches(t => text.Substring(t.Begin, t.End - t.Begin)));
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DZ.Tools.Tests
{
    [TestFixture]
    public class QualityModelComparerTests
    {
        private static readonly List<Tag<TNER>> _Expected = new List<Tag<TNER>>
        {
            new Tag<TNER>(0, 4, TNER.P),
            new Tag<TNER>(15, 26, TNER.L),
            new Tag<TNER>(30, 74, TNER.P),
            new Tag<TNER>(64, 74, TNER.L)
        };

        [Test]
        public void StrictEqualResults_NoErrors()
        {
            var report = new List<Tag<TNER>>
            {
                new Tag<TNER>(0, 4, TNER.P),
                new Tag<TNER>(15, 26, TNER.L),
                new Tag<TNER>(30, 74, TNER.P),
                new Tag<TNER>(64, 74, TNER.L),
            }.CompareTo(_Expected, TagsMatchers<TNER>.Strict, NERWorker.I.Values, NERWorker.I.Undefined);
            //no errors found
            report.Mismatches.All(e => e.Value.Count == 0).AssertTrue();
            report.Matches[TNER.U].Select(m => m.Actual.Score).Assert(Is.All.EqualTo(1));
        }

        [Test]
        public void LenientEqualResults_NoErrors()
        {
            var report = new List<Tag<TNER>>
            {
                new Tag<TNER>(0,3,TNER.P),
                new Tag<TNER>(14,30,TNER.L),
                new Tag<TNER>(50,74,TNER.P),
                new Tag<TNER>(64,74,TNER.L),
            }.CompareTo(_Expected, TagsMatchers<TNER>.Lenient, NERWorker.I.Values, NERWorker.I.Undefined);
            //no errors found
            report.Mismatches.Values.Assert(Is.All.Matches<List<Mismatch<TNER>>>(e => e.Count == 0));
            report.Matches[TNER.U].Select(m => m.Actual.Score).Assert(Is.All.EqualTo(1));
        }

        [Test]
        public void SemiStrictEqualResults_NoErrors()
        {
            var report = new List<Tag<TNER>>
            {
                new Tag<TNER>(0,3,TNER.P),
                new Tag<TNER>(14,30,TNER.L),
                new Tag<TNER>(50,74,TNER.P),
                new Tag<TNER>(64,74,TNER.L),
            }.CompareTo(_Expected, TagsMatchers<TNER>.SemiStrict, NERWorker.I.Values, NERWorker.I.Undefined);
            //no errors found
            report.Mismatches.All(e => e.Value.Count == 0).AssertTrue();
            report.Matches[TNER.U][0].Actual.Score.AssertEqualTo(0.5);
            report.Matches[TNER.U][1].Actual.Score.AssertEqualTo(0.5);
        }

        [Test]
        public void StrictTypeMismatchAndMissingEntity()
        {
            var report = new List<Tag<TNER>>
            {
                //new Tag<TNER>(0, 4, TNER.P), -- missing
                new Tag<TNER>(15,26,TNER.P), // new Tag<TNER>(15, 26, TNER.L), -- incorrect type 
                new Tag<TNER>(30,74,TNER.P),//match
                new Tag<TNER>(64,74,TNER.L),//match
            }.CompareTo(_Expected, TagsMatchers<TNER>.Strict, NERWorker.I.Values, NERWorker.I.Undefined);
            report.Render().ToConsole();
            var allErrors = report.Mismatches.SelectMany(p => p.Value).ToList();
            allErrors.Count.AssertEqualTo(2);
            allErrors[1].ExpectedType.AssertEqualTo(TNER.L);
            allErrors[1].ActualType.AssertEqualTo(TNER.P);
        }

        [Test]
        public void LenientTypeMismatchAndMissingEntity()
        {
            var report = new List<Tag<TNER>>
            {
                new Tag<TNER>(10,30,TNER.P),
                new Tag<TNER>(30,74,TNER.P),
                new Tag<TNER>(64,74,TNER.L),
            }.CompareTo(_Expected, TagsMatchers<TNER>.Lenient, NERWorker.I.Values, NERWorker.I.Undefined);
            //one entity missing other has different type
            var allErrors = report.Mismatches.SelectMany(p => p.Value).ToList();
            allErrors.Count.AssertEqualTo(2);
            allErrors[1].ExpectedType.AssertEqualTo(TNER.L);
            allErrors[1].ActualType.AssertEqualTo(TNER.P);
        }

        [Test]
        public void StrictEdgesMismatchAndRedundantEntity()
        {
            var report = new List<Tag<TNER>>
            {
                new Tag<TNER>(0,4,TNER.P),
                new Tag<TNER>(15,20,TNER.L),
                new Tag<TNER>(30,74,TNER.P),
                new Tag<TNER>(64,74,TNER.L),
                new Tag<TNER>(75,80,TNER.L),
            }.CompareTo(_Expected, TagsMatchers<TNER>.Strict, NERWorker.I.Values, NERWorker.I.Undefined);
            //one error for 15-26 <> 15-20 another for 75-80 has no matches
            var allErrors = AllErrors(report);
            Console.WriteLine(report.Render());
            allErrors.Count.AssertEqualTo(3);
            allErrors[0].ExpectedType.AssertEqualTo(TNER.U);
            allErrors[1].ExpectedType.AssertEqualTo(TNER.U);
            allErrors[2].ExpectedType.AssertEqualTo(TNER.L);
        }

        private static List<Mismatch<TType>> AllErrors<TType>(ComparisonReport<TType> report)
        {
            var allErrors = report.Mismatches.SelectMany(p => p.Value).ToList();
            return allErrors;
        }

        [Test]
        public void ComplexStructures()
        {
            var etalon =
                "По словам <P>генерального директора <O>ОАО «ИнфоТеКС»</O> Андрея Чапчаева</P>";
            var actual =
                "По словам <P><E>генерального директора <O>ОАО «ИнфоТеКС»</O></E> Андрея Чапчаева</P>";
            ComparisonReport<TNER> report;
            var m = GetReport(etalon, actual, out report);
            var allErrors = AllErrors(report);
            Assert.That(allErrors.Count, Is.EqualTo(1));
            allErrors[0].ActualType.AssertEqualTo(TNER.E);
            allErrors[0].ExpectedType.AssertEqualTo(TNER.U);
        }

        [Test]
        public void IdenticalMarkup()
        {
            var etalon =
                "<P>Исполнительный директор <O>ENISA</O> Андреас Пиротти</P> (<P>Andreas Pirotti</P>) назвал";
            var actual =
                "<P>Исполнительный директор <O>ENISA</O> Андреас Пиротти</P> (<P>Andreas Pirotti</P>) назвал";
            ComparisonReport<TNER> report;
            var m = GetReport(etalon, actual, out report);
            var errors = AllErrors(report);
            Assert.That(errors, Is.Empty);
        }

        [Test]
        public void RedundantOrganization()
        {
            var etalon =
                "<O>Министерство обороны <L>Армении</L></O> и <O>Ереванский НИИ математических машин</O>";
            var actual =
                "Министерство обороны <L>Армении</L> и <O>Ереванский</O> <O>НИИ</O> математических машин";
            //expecting errors:
            //1) <O>Министерство обороны <G>Армении</G></O> is missing
            ComparisonReport<TNER> report;
            var m = GetReport(etalon, actual, out report);
            var errors = AllErrors(report);
            errors.Count.AssertEqualTo(1, "Mismatches");
            //<O>Ереванский НИИ математических машин</O> matches 2 tags <O>Ереванский</O> <O>НИИ</O>
            //handle it as a single match
            report.Matches[TNER.O].Count.AssertEqualTo(1);
        }

        [Test]
        public void DoubleMatchPrevented()
        {
            var etalon =
                "<O>законов «О телекоммуникациях» и «О корпорации NTT»</O>";
            var actual =
                "законов <O>«О телекоммуникациях»</O> и <O>«О корпорации NTT»</O>";
            //if some actual tag covers 2 etalon tags then treats these 2 matches as single
            ComparisonReport<TNER> report;
            var m = GetReport(etalon, actual, out report);
            AllErrors(report).Count.AssertEqualTo(0);
            report.Matches[TNER.O].Count.AssertEqualTo(1);
        }

        [Test]
        public void InnerTagsWithTheSameType()
        {
            var etalon =
                "<O>Government Public Key <O>Infrastructure</O></O>";
            var actual =
                "<O>Government Public Key <O>Infrastructure</O></O>";
            //if some actual tag covers 2 etalon tags then treats these 2 matches as single
            ComparisonReport<TNER> report;
            var m = GetReport(etalon, actual, out report);
            AllErrors(report).Count.AssertEqualTo(0);
            report.Matches[TNER.O].Count.AssertEqualTo(2);
        }

        private static TagsCorpus<TNER> GetReport(string etalon, string actual, out ComparisonReport<TNER> report)
        {
            var m = NERWorker.I.Parser.Parse(etalon);
            report = NERWorker.I.Parser.Parse(actual).Tags
                .CompareTo(
                    m.Tags,
                    TagsMatchers<TNER>.Lenient,
                    NERWorker.I.Values,
                    NERWorker.I.Undefined);
            return m;
        }
    }
}
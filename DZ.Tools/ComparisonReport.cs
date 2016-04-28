using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DZ.Tools
{
    /// <summary>
    /// Comaprison report that contains structured information about tags comparison
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    public class ComparisonReport<TType>
    {
        private const string _IntFormat = "0000000";
        private const string _DoubleFormat = "0.00000";
        private const int _FieldWidth = 10;
        private readonly List<TType> _valuesSet;
        /// <summary>
        /// Undefined tag type value
        /// </summary>
        public readonly TType Undefined;

        /// <summary>
        /// Creates new comparison report
        /// </summary>
        public ComparisonReport(
            List<Match<TType>> matches,
            List<Mismatch<TType>> mismatch,
            List<Tag<TType>> retrieved,
            List<Tag<TType>> relevant,
            List<TType> valuesSet,
            TType undefined)
        {
            _valuesSet = valuesSet;
            Undefined = undefined;
            Retrieved = Fill(() => new List<Tag<TType>>(), retrieved);
            Spread(Retrieved);
            Relevant = Fill(() => new List<Tag<TType>>(), relevant);
            Spread(Relevant);
            Matches = Fill(() => new List<Match<TType>>(), matches);
            Spread(Matches);
            Mismatches = Fill(() => new List<Mismatch<TType>>(), new List<Mismatch<TType>>());
            Spread(Mismatches, mismatch);
            Statistics = Fill(() => new PrecRecall(), new PrecRecall());
            Fill(Statistics);
        }

        /// <summary>
        /// Per type Prec/Recall/F1 collection
        /// </summary>
        public Dictionary<TType, PrecRecall> Statistics { get; private set; }
        /// <summary>
        /// Per type retrieved tags
        /// </summary>
        public Dictionary<TType, List<Tag<TType>>> Retrieved { get; private set; }
        /// <summary>
        /// Per type relevant tags
        /// </summary>
        public Dictionary<TType, List<Tag<TType>>> Relevant { get; private set; }
        /// <summary>
        /// Per type matches
        /// </summary>
        public Dictionary<TType, List<Match<TType>>> Matches { get; private set; }
        /// <summary>
        /// Per expected type Mismatches
        /// </summary>
        public Dictionary<TType, List<Mismatch<TType>>> Mismatches { get; private set; }

        private Dictionary<TType, T> Fill<T>(Func<T> creator, T undefinedValue)
        {
            var target = new Dictionary<TType, T>();
            for (var i = 0; i < _valuesSet.Count; i++)
            {
                var v = _valuesSet[i];
                if (!v.Equals(Undefined))
                {
                    target[_valuesSet[i]] = creator();
                }
                else
                {
                    target[v] = undefinedValue;
                }
            }
            return target;
        }

        /// <summary>
        /// Renders report to string
        /// </summary>
        /// <returns></returns>
        public string Render()
        {
            var res = new StringBuilder();
            AppendConfusionMatrix(res);
            res.Append("===================FMeasure:..............")
                .AppendLine(Statistics[Undefined].FMeasure.ToString(_DoubleFormat));
            Append(res, Statistics, s => s.FMeasure);
            res.Append("==================Precision:..............")
                .AppendLine(Statistics[Undefined].Precision.ToString(_DoubleFormat));
            Append(res, Statistics, s => s.Precision);
            res.Append("=====================Recall:..............")
                .AppendLine(Statistics[Undefined].Recall.ToString(_DoubleFormat));
            Append(res, Statistics, s => s.Recall);
            res.Append("==============Matches count:..............")
                .AppendLine(Matches[Undefined].Count.ToString(_IntFormat));
            Append(res, Matches);
            res.Append("============Retrieved count:..............")
                .AppendLine(Retrieved[Undefined].Count.ToString(_IntFormat));
            Append(res, Retrieved);
            res.Append("=============Relevant count:..............")
                .AppendLine(Relevant[Undefined].Count.ToString(_IntFormat));
            Append(res, Relevant);
            res.Append("===========Mismatches count:..............")
                .AppendLine(Mismatches.Sum(p => p.Value.Count).ToString(_IntFormat));
            Append(res, Mismatches, true);
            return res.ToString();
        }

        private void Append<T>(StringBuilder res, Dictionary<TType, T> source, Func<T, double> selector)
        {
            foreach (var pair in source)
            {
                if (!pair.Key.Equals(Undefined))
                {
                    res
                        .Append("*****************")
                        .Append(StringValue(pair.Key.ToString()))
                        .Append(":..............")
                        .AppendLine(selector(pair.Value).ToString(_DoubleFormat));
                }
            }
        }

        private void Append<T>(StringBuilder res, Dictionary<TType, List<T>> source, bool appendUndefined = false)
        {
            foreach (var pair in source)
            {
                if (appendUndefined || !pair.Key.Equals(Undefined))
                {
                    res
                        .Append("*****************")
                        .Append(StringValue(pair.Key.ToString()))
                        .Append(":..............")
                        .AppendLine(pair.Value.Count.ToString(_IntFormat));
                }
            }
        }

        private void Fill(Dictionary<TType, PrecRecall> statistics)
        {
            foreach (var pair in statistics)
            {
                statistics[pair.Key].Precision = Score(Matches[pair.Key]) /
                                                 (Convert.ToDouble(Retrieved[pair.Key].Count) + double.Epsilon);
                statistics[pair.Key].Recall = Score(Matches[pair.Key]) /
                                              (Convert.ToDouble(Relevant[pair.Key].Count) + double.Epsilon);
                statistics[pair.Key].FMeasure = FMeasure(pair.Value);
            }
        }

        private static double FMeasure(PrecRecall data)
        {
            return (2 * data.Recall * data.Precision + double.Epsilon) / (data.Recall + data.Precision + double.Epsilon);
        }

        private void Spread(Dictionary<TType, List<Mismatch<TType>>> dict, List<Mismatch<TType>> errors)
        {
            foreach (var entity in errors)
            {
                dict[entity.ExpectedType].Add(entity);
            }
        }

        private void Spread(Dictionary<TType, List<Tag<TType>>> dict)
        {
            foreach (var entity in dict[Undefined])
            {
                dict[entity.Type].Add(entity);
            }
        }

        private void Spread(Dictionary<TType, List<Match<TType>>> dict)
        {
            foreach (var entity in dict[Undefined])
            {
                dict[entity.Actual.Tag.Type].Add(entity);
            }
        }

        private double Score(List<Match<TType>> list) { return list.Sum(m => m.Actual.Score) + double.Epsilon; }

        private void AppendConfusionMatrix(StringBuilder table)
        {
            //preparing columns: First is undefined (fake column for names), then <v1> <v2> ...
            var colls = _valuesSet.Where(v => !v.Equals(Undefined)).ToList();
            colls.Insert(0, Undefined);
            colls.Add(Undefined);
            //peparing rows: last one is undefined for all non-categorized mismatches
            var rows = _valuesSet.Where(v => !v.Equals(Undefined)).ToList();
            rows.Add(Undefined);

            for (var i = 0; i < colls.Count; i++)
            {
                table.Append(i != 0 ? CellValue(colls[i].ToString()) : CellValue(@"Act\Exp"));
            }
            table.AppendLine();

            for (var i = 0; i < rows.Count; i++)
            {
                var type = rows[i];
                for (var j = 0; j < colls.Count; j++)
                {
                    var coll = colls[j];
                    if (j != 0)
                    {
                        table.Append(
                            coll.Equals(type)
                                ? CellValue(Matches[coll].Count.ToString())
                                : CellValue(Mismatches[coll].Count(e => e.ActualType.Equals(type)).ToString()));
                    }
                    else
                    {
                        table.Append(CellValue(type.ToString()));
                    }
                }
                table.AppendLine();
            }

            for (var j = 0; j < colls.Count; j++)
            {
                var coll = colls[j];
                table.Append(j != 0 ? CellValue(Mismatches[coll].Count.ToString()) : CellValue("All"));
            }
            table.AppendLine();
        }

        private static string CellValue(string content) { return Const.SpaceS + StringValue(content) + Const.SpaceS; }

        private static string StringValue(string content)
        {
            if (content.Length >= _FieldWidth)
            {
                return content.Substring(0, _FieldWidth);
            }
            var len = content.Length;
            for (var i = 0; i < _FieldWidth - len; i++)
            {
                content = Const.SpaceS + content;
            }
            return content;
        }
    }

    /// <summary>
    /// Prec/Recall/F1 container
    /// </summary>
    public class PrecRecall
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public PrecRecall()
        {
            Precision = double.Epsilon;
            Recall = double.Epsilon;
            FMeasure = double.Epsilon;
        }

        /// <summary>
        /// Precision
        /// </summary>
        public double Precision { get; set; }
        /// <summary>
        /// Recall
        /// </summary>
        public double Recall { get; set; }
        /// <summary>
        /// F1 measure
        /// </summary>
        public double FMeasure { get; set; }
    }
}
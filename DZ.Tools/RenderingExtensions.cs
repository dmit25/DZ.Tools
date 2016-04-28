using System;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace DZ.Tools
{
    /// <summary>
    /// Rendering methods
    /// </summary>
    [PublicAPI]
    public static class RenderingExtensions
    {
        /// <summary>
        /// Renders matches and mismatches to string
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="report"></param>
        /// <param name="tagRenderer"></param>
        /// <returns></returns>
        public static string RenderMatchesAndMismatches<TType>(
            this ComparisonReport<TType> report,
            Func<Tag<TType>, string> tagRenderer)
        {
            return "{0}\n{1}"
                .FormatWith(
                    report.RenderMatches(tagRenderer),
                    report.RenderMismatches(tagRenderer));
        }

        /// <summary>
        /// Renders <paramref name="report"/>'s matches to string
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="report"></param>
        /// <param name="tagRenderer"></param>
        /// <returns></returns>
        public static string RenderMatches<TType>(
            this ComparisonReport<TType> report,
            Func<Tag<TType>, string> tagRenderer)
        {
            return @"
#####################################################################################################
###################################MATCHES###########################################################
#####################################################################################################
{0}"
                .FormatWith(
                    report.Matches[report.Undefined].Aggregate(
                        new StringBuilder(),
                        (acc, t) => acc.AppendLine(t.Render(c => tagRenderer(c.Tag)))));
        }

        /// <summary>
        /// Renders <paramref name="report"/>'s mismatches to string
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="report"></param>
        /// <param name="tagRenderer"></param>
        /// <returns></returns>
        public static string RenderMismatches<TType>(
            this ComparisonReport<TType> report,
            Func<Tag<TType>, string> tagRenderer)
        {
            return @"
#####################################################################################################
##################################MISMATCHES#########################################################
#####################################################################################################
Expected  <<>>  Actual
{0}"
                .FormatWith(
                report.Mismatches.Aggregate(
                    new StringBuilder(),
                    (acc, t) =>
                    {
                        foreach (var error in t.Value)
                        {
                            acc.AppendLine(error.Render(tagRenderer));
                        }
                        return acc;
                    }));
        }

    }
}
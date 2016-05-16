using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace DZ.Tools
{
    /// <summary>
    /// Provides extensions to compare lists of tags
    /// </summary>
    [PublicAPI]
    public static class Comparer
    {
        /// <summary>
        /// Comapres two lists of tags and creates comparison report
        /// </summary>
        /// <typeparam name="TType">Type of tag</typeparam>
        /// <param name="actual">list of actual tags</param>
        /// <param name="expected">list of etalon tags</param>
        /// <param name="matcher">matcher that should be used during comparison. Default matchers could be found in <seealso cref="TagsMatchers{TType}"/></param>
        /// <param name="values">Values of type <typeparamref name="TType"/> that should be found in tags</param>
        /// <param name="undefined">So-called "NIL" or "UNDEFINED" type type for unmarked text or undefined tags</param>
        /// <returns></returns>
        [NotNull]
        public static ComparisonReport<TType> CompareTo<TType>(
            [NotNull] this List<Tag<TType>> actual,
            [NotNull] List<Tag<TType>> expected,
            [NotNull] ITagComparer<TType> matcher,
            [NotNull] List<TType> values,
            [NotNull] TType undefined)
        {
            var errors = new List<Mismatch<TType>>();
            var matches = new List<Match<TType>>();
            var matched = new HashSet<Tag<TType>>();
            var retrievedHash = new HashSet<Tag<TType>>();
            var errorsHash = new HashSet<Tag<TType>>();
            var comparer = TagsMatchers<TType>.Pos;
            actual.Sort(comparer);
            expected.Sort(comparer);
            var candidates = new List<Candidate<TType>>();
            //add all matches and errors comparing model to result
            foreach (var tag in expected)
            {
                GetCandidates(actual, matcher, tag, candidates);
                if (candidates.Count == 0)
                {
                    errors.Add(new Mismatch<TType>(undefined, tag));
                }
                else
                {
                    var match = SelectBestCandidate(candidates, tag).ToArray();
                    if (match.Length == 0)
                    {
                        errors.Add(new Mismatch<TType>(undefined, tag, candidates.First().Tag));
                        errorsHash.Add(candidates.First().Tag);
                    }
                    else
                    {
                        bool counted = false;
                        foreach (var candidate in match)
                        {
                            if (tag.Parent != null && tag.Parent.Type.Equals(tag.Type)
                                || !matched.Contains(candidate.Tag))
                            {
                                if (!counted)
                                {
                                    matches.Add(new Match<TType>(new Candidate<TType>(tag), candidate));
                                    counted = true;
                                }
                                else
                                {
                                    candidate.Score = 0;
                                    matches.Add(new Match<TType>(new Candidate<TType>(tag), candidate));
                                }
                            }
                            matched.Add(candidate.Tag);
                            retrievedHash.Add(candidate.Tag);
                        }
                    }
                }
            }
            //add all entities that has no intersection with model as mismatches
            foreach (var tag in actual.Where(t => !retrievedHash.Contains(t) && !errorsHash.Contains(t)))
            {
                GetCandidates(expected, matcher, tag, candidates);
                if (SelectBestCandidate(candidates, tag).FirstOrDefault() == null)
                {
                    errors.Add(new Mismatch<TType>(undefined, null, tag));
                }
            }

            return new ComparisonReport<TType>(matches, errors, actual, expected, values, undefined);
        }

        private static IOrderedEnumerable<Candidate<TType>> SelectBestCandidate<TType>(List<Candidate<TType>> candidates, Tag<TType> tag)
        {
            return candidates
                .Where(candidate => candidate.Tag.Type.Equals(tag.Type))
                .OrderByDescending(candidate => candidate.Score)
                .ThenBy(c => Math.Abs(c.Tag.Begin - tag.Begin) + Math.Abs(c.Tag.End - tag.End));
        }

        private static void GetCandidates<TType>(
            List<Tag<TType>> actual,
            ITagComparer<TType> matcher,
            Tag<TType> tag,
            List<Candidate<TType>> candidates)
        {
            candidates.Clear();
            var index = actual.StartSearchIndex(tag.Begin, tag.End);

            if (index >= 0)
            {
                for (var i = index; i < actual.Count; i++)
                {
                    var candidate = actual[i];
                    if (candidate.Begin > tag.End)
                    {
                        break;
                    }
                    var score = matcher.Compare(candidate, tag);
                    if (score > 0)
                    {
                        candidates.Add(new Candidate<TType>(candidate, score));
                    }
                }
            }
        }
    }
}
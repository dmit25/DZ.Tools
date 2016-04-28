using System;
using System.IO;
using System.Text;
using DZ.Tools.Interfaces;

namespace DZ.Tools
{
    /// <summary>
    /// parses csv corpus
    /// </summary>
    public class CsvCorpusParser<TTag> : ICorpusParser<TTag>
    {
        private readonly Func<string, TTag> _tagTypeParser;
        private readonly Func<TTag, bool> _tagTypeValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CsvCorpusParser(
            Func<string, TTag> tagTypeParser,
            Func<TTag, bool> tagTypeValidator)
        {
            _tagTypeParser = tagTypeParser;
            _tagTypeValidator = tagTypeValidator;
        }

        /// <summary>
        /// Parses model string representation into training model
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxTagLength">Maximum tag length</param>
        /// <returns></returns>
        public TagsCorpus<TTag> Parse(string input, int maxTagLength = -1)
        {
            var model = new TagsCorpus<TTag>();
            var textIndex = 0;
            StringBuilder clearedText = new StringBuilder();
            Tag<TTag> previousEntity = null;
            using (var reader = new StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        textIndex++;
                        clearedText.Append("\n");
                        previousEntity = null;
                        continue;
                    }
                    var components = line.Split(Const.SpaceC);
                    var word = components[0];
                    var type = _tagTypeParser(components[1]);
                    if (!_tagTypeValidator(type))
                    {
                        textIndex += word.Length + 1;
                        clearedText.Append(word).Append(Const.SpaceC);
                        previousEntity = null;
                    }
                    else
                    {
                        if (previousEntity == null || !previousEntity.Type.Equals(type))
                        {
                            previousEntity = new Tag<TTag>(textIndex, textIndex + word.Length, type);
                            model.Tags.Add(previousEntity);
                        }
                        else
                        {
                            previousEntity.End = textIndex + word.Length;
                        }
                        textIndex += word.Length + 1;
                        clearedText.Append(word).Append(Const.SpaceC);
                    }
                }
            }
            model.ClearedText = clearedText.ToString();
            return model;
        }


    }
}
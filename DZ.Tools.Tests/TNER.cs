using System;

namespace DZ.Tools.Tests
{
    /// <summary>
    /// Ner entity type
    /// </summary>
    public enum TNER
    {
        /// <summary>
        /// Undefined type
        /// </summary>
        U,
        /// <summary>
        /// Person type
        /// </summary>
        P,
        /// <summary>
        /// Organization type
        /// </summary>
        O,
        /// <summary>
        /// Product type
        /// </summary>
        R,
        /// <summary>
        /// Event type
        /// </summary>
        E,
        /// <summary>
        /// Location type
        /// </summary>
        L
    }

    /// <summary>
    /// Contains invariants for NER primitives
    /// </summary>
    public class NERWorker : EnumsTagWorker<TNER>
    {
        /// <summary>
        /// Creates new tags worker
        /// </summary>
        public NERWorker()
            : base(TNER.U, (t1, t2) => t1.CompareTo(t2))
        {
            Types.Add("NAME", TNER.P);
            Types.Add("ORG", TNER.O);
            Types.Add("GEO", TNER.L);
            Types.Add("ENTR", TNER.E);
            Types.Add("PROD", TNER.R);
        }
    }

    /// <summary>
    /// Parses entities from csv corpus
    /// </summary>
    public sealed class CsvEntitiesParser : CsvCorpusParser<TNER>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CsvEntitiesParser()
            : base(GetEntityType, t => t != TNER.U)
        {
        }

        private static TNER GetEntityType(string s)
        {
            switch (s)
            {
                case "I-LOC":
                case "B-LOC":
                case "S-LOC":
                case "E-LOC":
                    return TNER.L;
                case "I-PER":
                case "B-PER":
                case "S-PER":
                case "E-PER":
                    return TNER.P;
                case "I-ORG":
                case "B-ORG":
                case "S-ORG":
                case "E-ORG":
                    return TNER.O;
                case "O":
                case "I-MISC":
                case "B-MISC":
                case "S-MISC":
                case "E-MISC":
                    return TNER.U;
                default:
                    throw new ArgumentException("Unknown type " + s);
            }
        }
    }
}
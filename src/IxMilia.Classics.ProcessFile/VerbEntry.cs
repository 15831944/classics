using System.Text.RegularExpressions;

namespace IxMilia.Classics.ProcessFile
{
    public class VerbEntry : DictionaryEntry
    {
        private static readonly Regex _verbMatcher = new Regex(@"V \(([12345])..\)");

        public Conjugation Conjugation { get; }

        public override PartOfSpeech PartOfSpeech => PartOfSpeech.Verb;

        public VerbEntry(string entry, string definition, Conjugation conjugation, string flags)
            : base(entry, definition, flags)
        {
            Conjugation = conjugation;
        }

        internal static VerbEntry TryParse(string entry, string pos, string flags, string definition)
        {
            var matches = _verbMatcher.Match(pos);
            if (matches.Success)
            {
                var conjugation = (Conjugation)int.Parse(matches.Groups[1].Value);
                return new VerbEntry(entry, definition, conjugation, flags);
            }
            else
            {
                return new VerbEntry(entry, definition, Conjugation.Unknown, flags);
            }
        }
    }
}

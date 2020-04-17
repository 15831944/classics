using System;
using System.Text.RegularExpressions;

namespace IxMilia.Classics.ProcessFile
{
    public class NounEntry : DictionaryEntry
    {
        private static readonly Regex _nounMatcher = new Regex(@"N \(([12345])..\) ([MFN])");

        public Declension Declension { get; }
        public Gender Gender { get; }

        public override PartOfSpeech PartOfSpeech => PartOfSpeech.Noun;

        public NounEntry(string entry, string definition, Declension declension, Gender gender, string flags)
            : base(entry, definition, flags)
        {
            Declension = declension;
            Gender = gender;
        }

        internal static NounEntry TryParse(string entry, string pos, string flags, string definition)
        {
            var matches = _nounMatcher.Match(pos);
            if (matches.Success)
            {
                var declension = (Declension)int.Parse(matches.Groups[1].Value);
                var gender = ParseGender(matches.Groups[2].Value);
                return new NounEntry(entry, definition, declension, gender, flags);
            }
            else
            {
                return null;
            }
        }

        private static Gender ParseGender(string gender)
        {
            switch (gender)
            {
                case "M":
                    return Gender.Masculine;
                case "F":
                    return Gender.Feminine;
                case "N":
                    return Gender.Neuter;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

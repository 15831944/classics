using System.Linq;

namespace IxMilia.Classics.ProcessFile
{
    public abstract class DictionaryEntry
    {
        public string EntryKey { get; }
        public string Entry { get; }
        public string Definition { get; internal set; }
        public string Flags { get; }

        public abstract PartOfSpeech PartOfSpeech { get; }

        protected DictionaryEntry(string entry, string definition, string flags)
        {
            Entry = entry;
            Definition = definition;
            Flags = flags;
            EntryKey = GetEntryKey(entry);
        }

        public static DictionaryEntry ParseDictionaryEntry(string entry, string pos, string flags, string definition)
        {
            var part = pos.Split(" ".ToCharArray()).FirstOrDefault();
            switch (part)
            {
                case "ADJ":
                    return AdjectiveEntry.TryParse(entry, definition, flags);
                case "CONJ":
                    return new ConjuctionEntry(entry, definition, flags);
                case "N":
                    return NounEntry.TryParse(entry, pos, flags, definition);
                case "PREP":
                    return new PrepositionEntry(entry, definition, flags);
                case "V":
                    return VerbEntry.TryParse(entry, pos, flags, definition);
                default:
                    return new UnsupportedEntry(entry, pos, flags, definition);
            }
        }

        private static string GetEntryKey(string entry)
        {
            for (int i = 0; i < entry.Length; i++)
            {
                if (!char.IsLetter(entry[i]))
                {
                    return entry.Substring(0, i).ToLowerInvariant();
                }
            }

            return entry.ToLowerInvariant();
        }
    }
}

namespace IxMilia.Classics.ProcessFile
{
    public class UnsupportedEntry : DictionaryEntry
    {
        public UnsupportedEntry(string entry, string definition, string partOfSpeech, string flags)
            : base(entry, definition, flags)
        {
        }

        public override PartOfSpeech PartOfSpeech => PartOfSpeech.Unknown;
    }
}

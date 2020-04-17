namespace IxMilia.Classics.ProcessFile
{
    public class PrepositionEntry : DictionaryEntry
    {
        public PrepositionEntry(string entry, string definition, string flags)
            : base(entry, definition, flags)
        {
        }

        public override PartOfSpeech PartOfSpeech => PartOfSpeech.Preposition;
    }
}

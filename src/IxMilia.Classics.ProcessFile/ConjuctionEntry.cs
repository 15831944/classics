namespace IxMilia.Classics.ProcessFile
{
    public class ConjuctionEntry : DictionaryEntry
    {
        public ConjuctionEntry(string entry, string definition, string flags)
            : base(entry, definition, flags)
        {
        }

        public override PartOfSpeech PartOfSpeech => PartOfSpeech.Conjunction;
    }
}

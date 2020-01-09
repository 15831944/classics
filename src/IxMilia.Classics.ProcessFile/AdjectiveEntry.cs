// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Classics.ProcessFile
{
    public class AdjectiveEntry : DictionaryEntry
    {
        public override PartOfSpeech PartOfSpeech => PartOfSpeech.Adjective;

        public Declension Declension { get; }

        public AdjectiveEntry(string entry, string definition, Declension declension, string flags)
            : base(entry, definition, flags)
        {
            Declension = declension;
        }

        internal static AdjectiveEntry TryParse(string entry, string definition, string flags)
        {
            // entry looks like:
            //   magnus, magna -um, major -or -us, maximus -a -um
            //   ingens, ingentis(gen.), ingentior - or - us, ingentissimus - a - um
            var parts = entry.Split(",".ToCharArray());
            if (parts.Length < 2)
            {
                return null;
            }

            var secondPart = parts[1];
            var declension = secondPart.EndsWith("-um")
                ? Declension.Second
                : Declension.Third;
            return new AdjectiveEntry(entry, definition, declension, flags);
        }
    }
}

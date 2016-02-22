// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class ConjuctionEntry : DictionaryEntry
    {
        public ConjuctionEntry(string entry, string definition, string flags)
            : base(entry, definition, flags)
        {
        }

        public override PartOfSpeech PartOfSpeech => PartOfSpeech.Conjunction;

        public override IEnumerable<Stem> GetStems()
        {
            yield return new BasicStem(Entry, this);
        }
    }
}

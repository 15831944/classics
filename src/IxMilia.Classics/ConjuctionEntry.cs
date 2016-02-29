// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class ConjuctionEntry : DictionaryEntry
    {
        private Stem[] _stems;

        public ConjuctionEntry(string entry, string definition, string flags)
            : base(entry, definition, flags)
        {
            _stems = new[] { new BasicStem(Entry, this) };
        }

        public override PartOfSpeech PartOfSpeech => PartOfSpeech.Conjunction;

        public override IEnumerable<Stem> GetStems()
        {
            return _stems;
        }
    }
}

// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class UnsupportedEntry : DictionaryEntry
    {
        public UnsupportedEntry(string entry, string definition, string partOfSpeech, string flags)
            : base(entry, definition, flags)
        {
        }

        public override PartOfSpeech PartOfSpeech => PartOfSpeech.Unknown;

        public override IEnumerable<Stem> GetStems()
        {
            yield break;
        }
    }
}

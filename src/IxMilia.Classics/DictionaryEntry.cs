// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Classics
{
    public class DictionaryEntry
    {
        public string Entry { get; }
        public string Definition { get; }
        public string PartOfSpeech { get; }
        public string Flags { get; }

        public DictionaryEntry(string entry, string definition, string partOfSpeech, string flags)
        {
            Entry = entry;
            Definition = definition;
            PartOfSpeech = partOfSpeech;
            Flags = flags;
        }
    }
}

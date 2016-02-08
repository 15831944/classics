// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace IxMilia.Classics
{
    public abstract class Stem
    {
        public string StemPart { get; }
        public DictionaryEntry Entry { get; }

        internal Stem(string stemPart, DictionaryEntry entry)
        {
            StemPart = stemPart;
            Entry = entry;
        }

        public abstract IEnumerable<WordForm> GetForms();
    }
}

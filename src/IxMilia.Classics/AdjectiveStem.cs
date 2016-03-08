// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Classics
{
    public abstract class AdjectiveStem : Stem
    {
        public Declension Declension { get; }

        protected AdjectiveStem(Declension declension, string stemPart, DictionaryEntry entry)
            : base(stemPart, entry)
        {
            Declension = declension;
        }
    }
}

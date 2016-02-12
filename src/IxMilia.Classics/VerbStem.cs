// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Classics
{
    public abstract class VerbStem : Stem
    {
        public Conjugation Conjugation { get; }

        protected VerbStem(Conjugation conjugation, string stemPart, DictionaryEntry entry)
            : base(stemPart, entry)
        {
            Conjugation = conjugation;
        }
    }
}

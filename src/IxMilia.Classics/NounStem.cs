// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Classics
{
    public abstract class NounStem : Stem
    {
        public Declension Declension { get; }
        public Gender Gender { get; }

        protected NounStem(Declension declension, Gender gender, string stemPart, DictionaryEntry entry)
            : base(stemPart, entry)
        {
            Declension = declension;
            Gender = gender;
        }
    }
}

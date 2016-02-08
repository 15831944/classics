// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Classics
{
    public abstract class WordForm
    {
        public Stem Stem { get; }
        public string Suffix { get; }

        protected WordForm(Stem stem, string suffix)
        {
            Stem = stem;
            Suffix = suffix;
        }
    }
}

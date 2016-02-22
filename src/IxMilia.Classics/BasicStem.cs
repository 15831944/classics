// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class BasicStem : Stem
    {
        public BasicStem(string stemPart, DictionaryEntry entry)
            : base(stemPart, entry)
        {
        }

        public override IEnumerable<WordForm> GetForms()
        {
            yield return new BasicWordForm(this, string.Empty);
        }
    }
}

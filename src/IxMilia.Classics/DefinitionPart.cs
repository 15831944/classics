// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class DefinitionPart
    {
        public Stem Stem { get; }
        public Span Span { get; }
        public IEnumerable<WordForm> Forms { get; }

        public DefinitionPart(Stem stem, Span span, IEnumerable<WordForm> forms)
        {
            Stem = stem;
            Span = span;
            Forms = forms;
        }
    }
}

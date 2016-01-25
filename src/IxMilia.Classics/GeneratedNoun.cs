﻿// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Classics
{
    public class GeneratedNoun : GeneratedWord
    {
        public new NounStem Stem { get; }
        public Case Case { get; }
        public Number Number { get; }

        public GeneratedNoun(NounStem stem, Case @case, Number number, string suffix)
            : base(stem, suffix)
        {
            Stem = stem;
            Case = @case;
            Number = number;
        }
    }
}
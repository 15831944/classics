// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Classics
{
    public class NounForm : WordForm
    {
        public new NounStem Stem { get; }
        public Case Case { get; }
        public Number Number { get; }

        public NounForm(NounStem stem, Case @case, Number number, string form)
            : base(stem, form)
        {
            Stem = stem;
            Case = @case;
            Number = number;
        }
    }
}

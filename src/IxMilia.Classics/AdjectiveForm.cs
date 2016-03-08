// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Classics
{
    public class AdjectiveForm : WordForm
    {
        public Gender Gender { get; }
        public Case Case { get; }
        public Number Number { get; }

        internal AdjectiveForm(Stem stem, Gender gender, Case @case, Number number, string suffix)
            : base(stem, suffix)
        {
            Gender = gender;
            Case = @case;
            Number = number;
        }
    }
}

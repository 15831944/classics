// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class NounStemWithSpecificForm : NounStem, IEnumerable
    {
        private NounForm[] _forms;

        public NounStemWithSpecificForm(Declension declension, Gender gender, Case @case, Number number, string specificForm, DictionaryEntry entry)
            : base(declension, gender, specificForm, entry)
        {
            _forms = new[] { new NounForm(this, @case, number, string.Empty) };
        }

        public override IEnumerable<WordForm> GetForms() => _forms;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_forms).GetEnumerator();
        }
    }
}

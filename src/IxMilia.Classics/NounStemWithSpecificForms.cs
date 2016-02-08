// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class NounStemWithSpecificForms : NounStem, IEnumerable
    {
        private List<NounForm> _forms = new List<NounForm>();

        public NounStemWithSpecificForms(Declension declension, Gender gender, DictionaryEntry entry)
            : base(declension, gender, string.Empty, entry)
        {
        }

        internal void Add(Case @case, Number number, string fullForm)
        {
            _forms.Add(new NounForm(this, @case, number, fullForm));
        }

        public override IEnumerable<WordForm> GetForms() => _forms;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_forms).GetEnumerator();
        }
    }
}

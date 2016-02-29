// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class NounStemWithSpecificForm : NounStem, IEnumerable
    {
        private List<NounForm> _forms = new List<NounForm>();

        public NounStemWithSpecificForm(Declension declension, Gender gender, string specificForm, DictionaryEntry entry)
            : base(declension, gender, specificForm, entry)
        {
        }

        public void Add(Case @case, Number number)
        {
            _forms.Add(new NounForm(this, @case, number, string.Empty));
        }

        public override IEnumerable<WordForm> GetForms() => _forms;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_forms).GetEnumerator();
        }
    }
}

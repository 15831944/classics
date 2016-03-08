// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class AdjectiveStemWithSpecificForm : AdjectiveStem, IEnumerable
    {
        private List<AdjectiveForm> _forms = new List<AdjectiveForm>();

        public AdjectiveStemWithSpecificForm(Declension declension, string specificForm, DictionaryEntry entry)
            : base(declension, specificForm, entry)
        {
        }

        public void Add(Gender gender, Case @case, Number number)
        {
            _forms.Add(new AdjectiveForm(this, gender, @case, number, string.Empty));
        }

        public override IEnumerable<WordForm> GetForms() => _forms;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_forms).GetEnumerator();
        }
    }
}

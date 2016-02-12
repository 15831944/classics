// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class VerbStemWithSpecificForms : VerbStem, IEnumerable
    {
        private List<VerbForm> _forms = new List<VerbForm>();

        public VerbStemWithSpecificForms(Conjugation conjugation, DictionaryEntry entry)
            : base(conjugation, string.Empty, entry)
        {
        }

        internal void Add(Conjugation conjugation, Person person, Number number, Mood mood, Voice voice, Tense tense, string fullForm)
        {
            _forms.Add(new VerbForm(this, conjugation, person, number, mood, voice, tense, fullForm));
        }

        public override IEnumerable<WordForm> GetForms() => _forms;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_forms).GetEnumerator();
        }
    }
}

// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class VerbStemWithSpecificForm : VerbStem, IEnumerable
    {
        private VerbForm[] _forms;

        public VerbStemWithSpecificForm(Conjugation conjugation, Person person, Number number, Mood mood, Voice voice, Tense tense, string specificForm, DictionaryEntry entry)
            : base(conjugation, specificForm, entry)
        {
            _forms = new[] { new VerbForm(this, conjugation, person, number, mood, voice, tense, string.Empty) };
        }

        public override IEnumerable<WordForm> GetForms() => _forms;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_forms).GetEnumerator();
        }
    }
}

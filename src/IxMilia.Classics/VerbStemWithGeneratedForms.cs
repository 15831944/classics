// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class VerbStemWithGeneratedForms : VerbStem, IEnumerable
    {
        private List<Tuple<Person, Number, Mood, Voice, Tense>> _formsToGenerate = new List<Tuple<Person, Number, Mood, Voice, Tense>>();

        public VerbStemWithGeneratedForms(Conjugation conjugation, string stemPart, DictionaryEntry entry)
            : base(conjugation, stemPart, entry)
        {
        }

        public void Add(Person person, Number number, Mood mood, Voice voice, Tense tense)
        {
            _formsToGenerate.Add(Tuple.Create(person, number, mood, voice, tense));
        }

        public override IEnumerable<WordForm> GetForms()
        {
            foreach (var form in _formsToGenerate)
            {
                WordForm wf = null;
                switch (Conjugation)
                {
                    case Conjugation.First:
                        wf = GetFirstConjugationForm(form.Item1, form.Item2, form.Item3, form.Item4, form.Item5);
                        break;
                }

                if (wf != null)
                {
                    yield return wf;
                }
            }
        }

        private WordForm GetFirstConjugationForm(Person person, Number number, Mood mood, Voice voice, Tense tense)
        {
            var suffix = string.Empty;
            if (person == Person.First && number == Number.Singluar && mood == Mood.Indicative && voice == Voice.Active)
                suffix = "m";
            if (person == Person.Second && number == Number.Singluar && mood == Mood.Indicative && voice == Voice.Active)
                suffix = "s";
            if (person == Person.Third && number == Number.Singluar && mood == Mood.Indicative && voice == Voice.Active)
                suffix = "t";
            if (person == Person.First && number == Number.Plural && mood == Mood.Indicative && voice == Voice.Active)
                suffix = "mus";
            if (person == Person.Second && number == Number.Plural && mood == Mood.Indicative && voice == Voice.Active)
                suffix = "tis";
            if (person == Person.Third && number == Number.Plural && mood == Mood.Indicative && voice == Voice.Active)
                suffix = "nt";

            return new VerbForm(this, Conjugation, person, number, mood, voice, tense, suffix);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_formsToGenerate).GetEnumerator();
        }
    }
}

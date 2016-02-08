// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class NounStemWithGeneratedForms : NounStem, IEnumerable
    {
        private List<Tuple<Case, Number>> _formsToGenerate = new List<Tuple<Case, Number>>();

        public NounStemWithGeneratedForms(Declension declension, Gender gender, string stemPart, DictionaryEntry entry)
            : base(declension, gender, stemPart, entry)
        {
        }

        public void Add(Case @case, Number number)
        {
            _formsToGenerate.Add(Tuple.Create(@case, number));
        }

        public override IEnumerable<WordForm> GetForms()
        {
            foreach (var form in _formsToGenerate)
            {
                switch (Declension)
                {
                    case Declension.First:
                        yield return GetFirstDeclensionForm(form.Item1, form.Item2);
                        break;
                    case Declension.Second:
                        yield return GetSecondDeclensionForm(form.Item1, form.Item2);
                        break;
                }
            }
        }

        private WordForm GetFirstDeclensionForm(Case @case, Number number)
        {
            var suffix = string.Empty;
            if (number == Number.Singluar)
            {
                switch (@case)
                {
                    case Case.Nominative:
                    case Case.Ablative:
                    case Case.Vocative:
                        suffix = "a";
                        break;
                    case Case.Genitive:
                    case Case.Dative:
                        suffix = "ae";
                        break;
                    case Case.Accusative:
                        suffix = "am";
                        break;
                }
            }
            else
            {
                switch (@case)
                {
                    case Case.Nominative:
                    case Case.Vocative:
                        suffix = "ae";
                        break;
                    case Case.Genitive:
                        suffix = "arum";
                        break;
                    case Case.Dative:
                    case Case.Ablative:
                        suffix = "is";
                        break;
                    case Case.Accusative:
                        suffix = "as";
                        break;
                }
            }

            return new NounForm(this, @case, number, suffix);
        }

        private WordForm GetSecondDeclensionForm(Case @case, Number number)
        {
            var suffix = string.Empty;
            if (number == Number.Singluar)
            {
                switch (@case)
                {
                    case Case.Genitive:
                        suffix = "i";
                        break;
                    case Case.Dative:
                    case Case.Ablative:
                        suffix = "o";
                        break;
                    case Case.Accusative:
                        suffix = "um";
                        break;
                    case Case.Vocative:
                        // TODO: handle '-ius', e.g., the vocative of 'filius' is 'fili' or 'filii', not 'filie'
                        suffix = "e";
                        break;
                    default:
                        throw new InvalidOperationException("Should have been a specific form");
                }
            }
            else
            {
                switch (@case)
                {
                    case Case.Nominative:
                    case Case.Vocative:
                        suffix = "i";
                        break;
                    case Case.Genitive:
                        suffix = "orum";
                        break;
                    case Case.Dative:
                    case Case.Ablative:
                        suffix = "is";
                        break;
                    case Case.Accusative:
                        suffix = "os";
                        break;
                    default:
                        throw new InvalidOperationException("Should have been a specific form");
                }
            }

            return new NounForm(this, @case, number, suffix);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_formsToGenerate).GetEnumerator();
        }
    }
}

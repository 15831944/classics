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
                WordForm wf = null;
                switch (Declension)
                {
                    case Declension.First:
                        wf = GetFirstDeclensionForm(form.Item1, form.Item2);
                        break;
                    case Declension.Second:
                        wf = GetSecondDeclensionForm(form.Item1, form.Item2);
                        break;
                    case Declension.Third:
                        wf = GetThirdDeclensionForm(form.Item1, form.Item2);
                        break;
                    case Declension.Fourth:
                        wf = GetFourthDeclensionForm(form.Item1, form.Item2);
                        break;
                    case Declension.Fifth:
                        wf = GetFifthDeclensionForm(form.Item1, form.Item2);
                        break;
                }

                if (wf != null)
                {
                    yield return wf;
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

            if (StemPart == null && number != Number.Singluar && (@case != Case.Nominative || @case != Case.Vocative))
            {
                return null;
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
                        suffix = Gender == Gender.Masculine ? "i" : "a";
                        break;
                    case Case.Genitive:
                        suffix = "orum";
                        break;
                    case Case.Dative:
                    case Case.Ablative:
                        suffix = "is";
                        break;
                    case Case.Accusative:
                        suffix = Gender == Gender.Masculine ? "os" : "a";
                        break;
                    default:
                        throw new InvalidOperationException("Should have been a specific form");
                }
            }

            if (StemPart == null && number != Number.Singluar && (@case != Case.Nominative || @case != Case.Vocative))
            {
                return null;
            }

            return new NounForm(this, @case, number, suffix);
        }

        private WordForm GetThirdDeclensionForm(Case @case, Number number)
        {
            var suffix = string.Empty;
            if (number == Number.Singluar)
            {
                switch (@case)
                {
                    case Case.Genitive:
                        suffix = "is";
                        break;
                    case Case.Dative:
                        suffix = "i";
                        break;
                    case Case.Ablative:
                        suffix = "e";
                        break;
                    case Case.Accusative:
                        suffix = "es";
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
                    case Case.Accusative:
                        suffix = Gender == Gender.Neuter ? "a" : "es";
                        break;
                    case Case.Genitive:
                        suffix = "um";
                        break;
                    case Case.Dative:
                    case Case.Ablative:
                        suffix = "ibus";
                        break;
                    default:
                        throw new InvalidOperationException("Should have been a specific form");
                }
            }

            if (StemPart == null && number != Number.Singluar && (@case != Case.Nominative || @case != Case.Vocative))
            {
                return null;
            }

            return new NounForm(this, @case, number, suffix);
        }

        private WordForm GetFourthDeclensionForm(Case @case, Number number)
        {
            var suffix = string.Empty;
            if (number == Number.Singluar)
            {
                switch (@case)
                {
                    case Case.Genitive:
                        suffix = "us";
                        break;
                    case Case.Dative:
                        suffix = "ui";
                        break;
                    case Case.Ablative:
                        suffix = "u";
                        break;
                    case Case.Accusative:
                        suffix = "um";
                        break;
                    default:
                        throw new InvalidOperationException("Should have been a specific form");
                }

                if (Gender == Gender.Neuter && @case != Case.Genitive)
                {
                    suffix = "u";
                }
            }
            else
            {
                switch (@case)
                {
                    case Case.Nominative:
                    case Case.Vocative:
                    case Case.Accusative:
                        suffix = Gender == Gender.Neuter ? "ua" : "us";
                        break;
                    case Case.Genitive:
                        suffix = "uum";
                        break;
                    case Case.Dative:
                    case Case.Ablative:
                        suffix = "ibus";
                        break;
                    default:
                        throw new InvalidOperationException("Should have been a specific form");
                }
            }

            if (StemPart == null && number != Number.Singluar && (@case != Case.Nominative || @case != Case.Vocative))
            {
                return null;
            }

            return new NounForm(this, @case, number, suffix);
        }

        private WordForm GetFifthDeclensionForm(Case @case, Number number)
        {
            var suffix = string.Empty;
            if (number == Number.Singluar)
            {
                switch (@case)
                {
                    case Case.Genitive:
                    case Case.Dative:
                        suffix = "ei";
                        break;
                    case Case.Ablative:
                        suffix = "e";
                        break;
                    case Case.Accusative:
                        suffix = "em";
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
                    case Case.Accusative:
                        suffix = "es";
                        break;
                    case Case.Genitive:
                        suffix = "erum";
                        break;
                    case Case.Dative:
                    case Case.Ablative:
                        suffix = "ebus";
                        break;
                    default:
                        throw new InvalidOperationException("Should have been a specific form");
                }
            }

            if (StemPart == null && number != Number.Singluar && (@case != Case.Nominative || @case != Case.Vocative))
            {
                return null;
            }

            return new NounForm(this, @case, number, suffix);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_formsToGenerate).GetEnumerator();
        }
    }
}

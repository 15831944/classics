// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class AdjectiveStemWithGeneratedForms : AdjectiveStem, IEnumerable
    {
        private List<Tuple<Gender, Case, Number>> _formsToGenerate = new List<Tuple<Gender, Case, Number>>();

        public AdjectiveStemWithGeneratedForms(Declension declension, string stemPart, DictionaryEntry entry)
            : base(declension, stemPart, entry)
        {
        }

        public void Add(Gender gender, Case @case, Number number)
        {
            _formsToGenerate.Add(Tuple.Create(gender, @case, number));
        }

        public override IEnumerable<WordForm> GetForms()
        {
            // TODO: comparative and superlative
            foreach (var form in _formsToGenerate)
            {
                WordForm wf = null;
                switch (Declension)
                {
                    case Declension.Second:
                        wf = GetSecondDeclensionForm(form.Item1, form.Item2, form.Item3);
                        break;
                    case Declension.Third:
                        // TODO
                        break;
                    default:
                        throw new NotSupportedException();
                }

                if (wf != null)
                {
                    yield return wf;
                }
            }
        }

        private WordForm GetSecondDeclensionForm(Gender gender, Case @case, Number number)
        {
            var suffix = string.Empty;
            if (number == Number.Singluar)
            {
                switch (@case)
                {
                    case Case.Nominative:
                        suffix = GenderSwitch(gender, "us", "a", "um");
                        break;
                    case Case.Genitive:
                        suffix = GenderSwitch(gender, "i", "ae");
                        break;
                    case Case.Dative:
                        suffix = GenderSwitch(gender, "o", "ae");
                        break;
                    case Case.Ablative:
                        suffix = GenderSwitch(gender, "o", "a");
                        break;
                    case Case.Accusative:
                        suffix = GenderSwitch(gender, "um", "am");
                        break;
                    case Case.Vocative:
                        suffix = GenderSwitch(gender, "e", "a", "um");
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
                        suffix = GenderSwitch(gender, "i", "ae", "a");
                        break;
                    case Case.Genitive:
                        suffix = GenderSwitch(gender, "orum", "arum");
                        break;
                    case Case.Dative:
                        suffix = GenderSwitch(gender, "is");
                        break;
                    case Case.Ablative:
                        suffix = GenderSwitch(gender, "is");
                        break;
                    case Case.Accusative:
                        suffix = GenderSwitch(gender, "os", "as", "a");
                        break;
                    case Case.Vocative:
                        suffix = GenderSwitch(gender, "i", "ae", "a");
                        break;
                    default:
                        throw new InvalidOperationException("Should have been a specific form");
                }
            }

            if (StemPart == null && number != Number.Singluar && (@case != Case.Nominative || @case != Case.Vocative))
            {
                return null;
            }

            return new AdjectiveForm(this, gender, @case, number, suffix);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_formsToGenerate).GetEnumerator();
        }

        private static string GenderSwitch(Gender gender, string masculineForm, string feminineForm = null, string neuterForm = null)
        {
            feminineForm = feminineForm ?? masculineForm;
            neuterForm = neuterForm ?? masculineForm;
            switch (gender)
            {
                case Gender.Masculine:
                    return masculineForm;
                case Gender.Feminine:
                    return feminineForm;
                case Gender.Neuter:
                    return neuterForm;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}

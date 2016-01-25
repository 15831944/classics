// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class NounStem : Stem
    {
        public string NominativeSingular { get; }
        public Declension Declension { get; }
        public Gender Gender { get; }

        internal NounStem(string nominativeSingular, string stemPart, Declension declension, Gender gender, DictionaryEntry entry)
            : base(stemPart, entry)
        {
            NominativeSingular = nominativeSingular;
            Declension = declension;
            Gender = gender;
        }

        public override IEnumerable<WordForm> GetGeneratedForms()
        {
            switch (Declension)
            {
                case Declension.First:
                    return GetGeneratedFirstDeclensionForms();
                case Declension.Second:
                    return GetGeneratedSecondDeclensionForms();
                default:
                    return new WordForm[0];
            }
        }

        private IEnumerable<WordForm> GetGeneratedFirstDeclensionForms()
        {
            yield return new NounForm(this, Case.Nominative, Number.Singluar, NominativeSingular);
            yield return new NounForm(this, Case.Genitive, Number.Singluar, StemPart + "ae");
            yield return new NounForm(this, Case.Dative, Number.Singluar, StemPart + "ae");
            yield return new NounForm(this, Case.Accusative, Number.Singluar, StemPart + "am");
            yield return new NounForm(this, Case.Ablative, Number.Singluar, StemPart + "a");
            yield return new NounForm(this, Case.Vocative, Number.Singluar, StemPart + "a");

            yield return new NounForm(this, Case.Nominative, Number.Plural, StemPart + "ae");
            yield return new NounForm(this, Case.Genitive, Number.Plural, StemPart + "arum");
            yield return new NounForm(this, Case.Dative, Number.Plural, StemPart + "is");
            yield return new NounForm(this, Case.Accusative, Number.Plural, StemPart + "as");
            yield return new NounForm(this, Case.Ablative, Number.Plural, StemPart + "is");
            yield return new NounForm(this, Case.Vocative, Number.Plural, StemPart + "ae");
        }

        private IEnumerable<WordForm> GetGeneratedSecondDeclensionForms()
        {
            if (Gender == Gender.Neuter)
            {
                // TODO: handle words where the stem is empty, e.g., 'epos'
                yield return new NounForm(this, Case.Nominative, Number.Singluar, NominativeSingular);
                yield return new NounForm(this, Case.Genitive, Number.Singluar, StemPart + "i");
                yield return new NounForm(this, Case.Dative, Number.Singluar, StemPart + "o");
                yield return new NounForm(this, Case.Accusative, Number.Singluar, NominativeSingular);
                yield return new NounForm(this, Case.Ablative, Number.Singluar, StemPart + "o");
                yield return new NounForm(this, Case.Vocative, Number.Singluar, NominativeSingular);

                yield return new NounForm(this, Case.Nominative, Number.Plural, StemPart + "a");
                yield return new NounForm(this, Case.Genitive, Number.Plural, StemPart + "orum");
                yield return new NounForm(this, Case.Dative, Number.Plural, StemPart + "is");
                yield return new NounForm(this, Case.Accusative, Number.Plural, StemPart + "a");
                yield return new NounForm(this, Case.Ablative, Number.Plural, StemPart + "is");
                yield return new NounForm(this, Case.Vocative, Number.Plural, StemPart + "a");
            }
            else
            {
                yield return new NounForm(this, Case.Nominative, Number.Singluar, NominativeSingular);
                yield return new NounForm(this, Case.Genitive, Number.Singluar, StemPart + "i");
                yield return new NounForm(this, Case.Dative, Number.Singluar, StemPart + "o");
                yield return new NounForm(this, Case.Accusative, Number.Singluar, StemPart + "um");
                yield return new NounForm(this, Case.Ablative, Number.Singluar, StemPart + "o");
                yield return new NounForm(this, Case.Vocative, Number.Singluar, StemPart + "e");

                yield return new NounForm(this, Case.Nominative, Number.Plural, StemPart + "i");
                yield return new NounForm(this, Case.Genitive, Number.Plural, StemPart + "orum");
                yield return new NounForm(this, Case.Dative, Number.Plural, StemPart + "is");
                yield return new NounForm(this, Case.Accusative, Number.Plural, StemPart + "os");
                yield return new NounForm(this, Case.Ablative, Number.Plural, StemPart + "is");
                yield return new NounForm(this, Case.Vocative, Number.Plural, StemPart + "i");
            }
        }
    }
}

// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class NounStem : Stem
    {
        public Declension Declension { get; }
        public Gender Gender { get; }

        public NounStem(Declension declension, Gender gender, string stemPart, DictionaryEntry entry)
            : base(stemPart, entry)
        {
            Declension = declension;
            Gender = gender;
        }

        public IEnumerable<GeneratedNoun> GetGeneratedForms()
        {
            yield return new GeneratedNoun(this, Case.Nominative, Number.Singluar, "a");
            yield return new GeneratedNoun(this, Case.Genitive, Number.Singluar, "ae");
            yield return new GeneratedNoun(this, Case.Dative, Number.Singluar, "ae");
            yield return new GeneratedNoun(this, Case.Accusative, Number.Singluar, "am");
            yield return new GeneratedNoun(this, Case.Ablative, Number.Singluar, "a");
            yield return new GeneratedNoun(this, Case.Vocative, Number.Singluar, "a");

            yield return new GeneratedNoun(this, Case.Nominative, Number.Plural, "ae");
            yield return new GeneratedNoun(this, Case.Genitive, Number.Plural, "arum");
            yield return new GeneratedNoun(this, Case.Dative, Number.Plural, "is");
            yield return new GeneratedNoun(this, Case.Accusative, Number.Plural, "as");
            yield return new GeneratedNoun(this, Case.Ablative, Number.Plural, "is");
            yield return new GeneratedNoun(this, Case.Vocative, Number.Plural, "ae");
        }
    }
}

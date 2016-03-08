// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class AdjectiveEntry : DictionaryEntry
    {
        private IEnumerable<Stem> _stems;

        public override PartOfSpeech PartOfSpeech => PartOfSpeech.Adjective;

        public Declension Declension { get; }

        public AdjectiveEntry(string entry, string definition, Declension declension, string flags)
            : base(entry, definition, flags)
        {
            Declension = declension;
        }

        public override IEnumerable<Stem> GetStems()
        {
            if (_stems == null)
            {
                _stems = GenerateStems();
            }

            return _stems;
        }

        private IEnumerable<Stem> GenerateStems()
        {
            var nominative = GetNominativeSingularMasculineForm();
            var genitiveStem = Entry.Split(",".ToCharArray())[1].Trim();
            if (genitiveStem.EndsWith("a -um"))
            {
                // TODO: this is a hack and is way too restrictive
                genitiveStem = genitiveStem.Substring(0, genitiveStem.Length - 5);
            }

            var isDeclined = genitiveStem != "undeclined";

            // this form is always present
            yield return new AdjectiveStemWithSpecificForm(Declension, nominative, this)
            {
                {Gender.Masculine, Case.Nominative, Number.Singluar}
            };

            if (isDeclined)
            {
                yield return new AdjectiveStemWithGeneratedForms(Declension, genitiveStem, this)
                {
                    {Gender.Masculine, Case.Genitive, Number.Singluar},
                    {Gender.Masculine, Case.Dative, Number.Singluar},
                    {Gender.Masculine, Case.Accusative, Number.Singluar},
                    {Gender.Masculine, Case.Ablative, Number.Singluar},
                    {Gender.Masculine, Case.Vocative, Number.Singluar},
                    {Gender.Feminine, Case.Nominative, Number.Singluar},
                    {Gender.Feminine, Case.Genitive, Number.Singluar},
                    {Gender.Feminine, Case.Dative, Number.Singluar},
                    {Gender.Feminine, Case.Accusative, Number.Singluar},
                    {Gender.Feminine, Case.Ablative, Number.Singluar},
                    {Gender.Feminine, Case.Vocative, Number.Singluar},
                    {Gender.Neuter, Case.Nominative, Number.Singluar},
                    {Gender.Neuter, Case.Genitive, Number.Singluar},
                    {Gender.Neuter, Case.Dative, Number.Singluar},
                    {Gender.Neuter, Case.Accusative, Number.Singluar},
                    {Gender.Neuter, Case.Ablative, Number.Singluar},
                    {Gender.Neuter, Case.Vocative, Number.Singluar},
                    {Gender.Masculine, Case.Nominative, Number.Plural},
                    {Gender.Masculine, Case.Genitive, Number.Plural},
                    {Gender.Masculine, Case.Dative, Number.Plural},
                    {Gender.Masculine, Case.Accusative, Number.Plural},
                    {Gender.Masculine, Case.Ablative, Number.Plural},
                    {Gender.Masculine, Case.Vocative, Number.Plural},
                    {Gender.Feminine, Case.Nominative, Number.Plural},
                    {Gender.Feminine, Case.Genitive, Number.Plural},
                    {Gender.Feminine, Case.Dative, Number.Plural},
                    {Gender.Feminine, Case.Accusative, Number.Plural},
                    {Gender.Feminine, Case.Ablative, Number.Plural},
                    {Gender.Feminine, Case.Vocative, Number.Plural},
                    {Gender.Neuter, Case.Nominative, Number.Plural},
                    {Gender.Neuter, Case.Genitive, Number.Plural},
                    {Gender.Neuter, Case.Dative, Number.Plural},
                    {Gender.Neuter, Case.Accusative, Number.Plural},
                    {Gender.Neuter, Case.Ablative, Number.Plural},
                    {Gender.Neuter, Case.Vocative, Number.Plural},
                };
            }
        }

        private string GetNominativeSingularMasculineForm()
        {
            return Entry.Split(",".ToCharArray())[0];
        }

        internal static AdjectiveEntry TryParse(string entry, string definition, string flags)
        {
            // entry looks like:
            //   magnus, magna -um, major -or -us, maximus -a -um
            //   ingens, ingentis(gen.), ingentior - or - us, ingentissimus - a - um
            var parts = entry.Split(",".ToCharArray());
            if (parts.Length < 2)
            {
                return null;
            }

            var secondPart = parts[1];
            var declension = secondPart.EndsWith("-um")
                ? Declension.Second
                : Declension.Third;
            return new AdjectiveEntry(entry, definition, declension, flags);
        }
    }
}

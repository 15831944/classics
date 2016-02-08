// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IxMilia.Classics
{
    public class NounEntry : DictionaryEntry
    {
        private static readonly Regex _nounMatcher = new Regex(@"N \(([12345])..\) ([MFN])");

        public Declension Declension { get; }
        public Gender Gender { get; }

        public override PartOfSpeech PartOfSpeech => PartOfSpeech.Noun;

        public NounEntry(string entry, string definition, Declension declension, Gender gender, string flags)
            : base(entry, definition, flags)
        {
            Declension = declension;
            Gender = gender;
        }

        public override IEnumerable<Stem> GetStems()
        {
            var nominative = GetNominativeSingularForm();
            var genitiveStem = GetGenitiveSingularStem();
            switch (Declension)
            {
                case Declension.First:
                    yield return new NounStemWithGeneratedForms(Declension, Gender, genitiveStem, this)
                    {
                        {Case.Nominative, Number.Singluar},
                        {Case.Genitive, Number.Singluar},
                        {Case.Dative, Number.Singluar},
                        {Case.Accusative, Number.Singluar},
                        {Case.Ablative, Number.Singluar},
                        {Case.Vocative, Number.Singluar},
                        {Case.Nominative, Number.Plural},
                        {Case.Genitive, Number.Plural},
                        {Case.Dative, Number.Plural},
                        {Case.Accusative, Number.Plural},
                        {Case.Ablative, Number.Plural},
                        {Case.Vocative, Number.Plural},
                    };
                    break;
                case Declension.Second:
                    if (Gender == Gender.Masculine)
                    {
                        // nom. sg. which could be non-standard like 'vir'
                        yield return new NounStemWithSpecificForms(Declension, Gender, this)
                        {
                            {Case.Nominative, Number.Singluar, nominative}
                        };

                        // all the other forms
                        yield return new NounStemWithGeneratedForms(Declension, Gender, genitiveStem, this)
                        {
                            {Case.Genitive, Number.Singluar},
                            {Case.Dative, Number.Singluar},
                            {Case.Accusative, Number.Singluar},
                            {Case.Ablative, Number.Singluar},
                            {Case.Vocative, Number.Singluar},
                            {Case.Nominative, Number.Plural},
                            {Case.Genitive, Number.Plural},
                            {Case.Dative, Number.Plural},
                            {Case.Accusative, Number.Plural},
                            {Case.Ablative, Number.Plural},
                            {Case.Vocative, Number.Plural},
                        };
                    }
                    else
                    {
                        // nom., acc., and voc. are all the same
                        yield return new NounStemWithSpecificForms(Declension, Gender, this)
                        {
                            {Case.Nominative, Number.Singluar, nominative},
                            {Case.Accusative, Number.Singluar, nominative},
                            {Case.Vocative, Number.Singluar, nominative},
                        };

                        // all the other forms
                        yield return new NounStemWithGeneratedForms(Declension, Gender, genitiveStem, this)
                        {
                            {Case.Genitive, Number.Singluar},
                            {Case.Dative, Number.Singluar},
                            {Case.Ablative, Number.Singluar},
                            {Case.Nominative, Number.Plural},
                            {Case.Genitive, Number.Plural},
                            {Case.Dative, Number.Plural},
                            {Case.Accusative, Number.Plural},
                            {Case.Ablative, Number.Plural},
                            {Case.Vocative, Number.Plural},
                        };
                    }
                    break;
                case Declension.Third:
                case Declension.Fourth:
                case Declension.Fifth:
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private string GetNominativeSingularForm()
        {
            return Entry.Split(",".ToCharArray())[0];
        }

        private string GetGenitiveSingularStem()
        {
            var genitive = Entry.Split(",".ToCharArray())[1]
                .Trim()
                .Replace("(i)", "i"); // TODO:

            if (genitive == "-") return string.Empty;
            string suffix = string.Empty;
            switch (Declension)
            {
                case Declension.First:
                    suffix = "ae";
                    break;
                case Declension.Second:
                    suffix = "i";
                    break;
                case Declension.Third:
                    suffix = "is";
                    break;
                case Declension.Fourth:
                    suffix = "us";
                    if (genitive.EndsWith("u"))
                    {
                        suffix = "u";
                    }
                    break;
                case Declension.Fifth:
                    suffix = "ei";
                    break;
            }
            return genitive.RemoveSuffix(suffix);
        }

        internal static NounEntry TryParse(string entry, string pos, string flags, string definition)
        {
            var matches = _nounMatcher.Match(pos);
            if (matches.Success)
            {
                var declension = (Declension)int.Parse(matches.Groups[1].Value);
                var gender = ParseGender(matches.Groups[2].Value);
                return new NounEntry(entry, definition, declension, gender, flags);
            }
            else
            {
                return null;
            }
        }

        private static Gender ParseGender(string gender)
        {
            switch (gender)
            {
                case "M":
                    return Gender.Masculine;
                case "F":
                    return Gender.Feminine;
                case "N":
                    return Gender.Neuter;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

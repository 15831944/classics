// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IxMilia.Classics
{
    public class VerbEntry : DictionaryEntry
    {
        private static readonly Regex _verbMatcher = new Regex(@"V \(([12345])..\)");

        public Conjugation Conjugation { get; }

        public override PartOfSpeech PartOfSpeech => PartOfSpeech.Verb;

        public VerbEntry(string entry, string definition, Conjugation conjugation, string flags)
            : base(entry, definition, flags)
        {
            Conjugation = conjugation;
        }

        internal static VerbEntry TryParse(string entry, string pos, string flags, string definition)
        {
            var matches = _verbMatcher.Match(pos);
            if (matches.Success)
            {
                var conjugation = (Conjugation)int.Parse(matches.Groups[1].Value);
                return new VerbEntry(entry, definition, conjugation, flags);
            }
            else
            {
                return null;
            }
        }

        public override IEnumerable<Stem> GetStems()
        {
            var firstpp = GetFirstPrincipalPart();
            yield return new VerbStemWithSpecificForms(Conjugation, this)
            {
                {Conjugation, Person.First, Number.Singluar, Mood.Indicative, Voice.Active, Tense.Present, firstpp},
            };

            var presentStem = GetPresentStem();
            if (presentStem != null)
            {
                yield return new VerbStemWithGeneratedForms(Conjugation, presentStem, this)
                {
                    {Person.Second, Number.Singluar, Mood.Indicative, Voice.Active, Tense.Present},
                    {Person.Third, Number.Singluar, Mood.Indicative, Voice.Active, Tense.Present},
                    {Person.First, Number.Plural, Mood.Indicative, Voice.Active, Tense.Present},
                    {Person.Second, Number.Plural, Mood.Indicative, Voice.Active, Tense.Present},
                    {Person.Third, Number.Plural, Mood.Indicative, Voice.Active, Tense.Present},
                };
            }
        }

        private string GetFirstPrincipalPart()
        {
            return Entry.Split(",".ToCharArray())[0];
        }

        private string GetPresentStem()
        {
            var stem = Entry.Split(",".ToCharArray())[1].Trim();
            if (stem.EndsWith("re"))
            {
                return stem.RemoveSuffix("re");
            }

            // TODO: handle deponent and other verbs
            return null;
        }
    }
}

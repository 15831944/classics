// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace IxMilia.Classics
{
    public class DictionaryEntry
    {
        public string EntryKey { get; }
        public string Entry { get; }
        public string Definition { get; }
        public string Flags { get; }

        public WordType Type { get; private set; }
        public Declension Declension { get; private set; }
        public Gender Gender { get; private set; }

        private static readonly Regex _nounMatcher = new Regex(@"N \(([12345])..\) ([MFN])");

        internal DictionaryEntry(string entry, string definition, string partOfSpeech, string flags)
        {
            EntryKey = GetEntryKey(entry);
            Entry = entry;
            Definition = definition;
            Flags = flags;

            ParsePartOfSpeech(partOfSpeech);
        }

        public IEnumerable<Stem> GetStems()
        {
            switch (Type)
            {
                case WordType.Noun:
                    yield return new NounStem(EntryKey, GetNounGenitiveStem(), Declension, Gender, this);
                    break;
                default:
                    break;
            }
        }

        private string GetNounGenitiveStem()
        {
            var parts = Entry.Split(",".ToCharArray());
            var genitive = parts[1].Trim();

            // TODO: better handle some genitive forms having an extra optional (i)
            genitive = genitive.Replace("(i)", "i");
            if (genitive == "-")
            {
                // some words don't exist outside of the nominative
                return string.Empty;
            }

            switch (Declension)
            {
                case Declension.First:
                    return RemoveSuffix(genitive, "ae");
                case Declension.Second:
                    return RemoveSuffix(genitive, "i");
                case Declension.Third:
                    return RemoveSuffix(genitive, "is");
                case Declension.Fourth:
                    return genitive.EndsWith("us") ? RemoveSuffix(genitive, "us") : RemoveSuffix(genitive, "u");
                case Declension.Fifth:
                    return RemoveSuffix(genitive, "ei");
                default:
                    throw new InvalidOperationException();
            }
        }

        private static string RemoveSuffix(string text, string suffix)
        {
            Debug.Assert(text.EndsWith(suffix));
            return text.Substring(0, text.Length - suffix.Length);
        }

        private void ParsePartOfSpeech(string pos)
        {
            Type = WordType.Unknown;

            var match = _nounMatcher.Match(pos);
            if (match.Success)
            {
                Type = WordType.Noun;
                Declension = (Declension)int.Parse(match.Groups[1].Value);
                switch (match.Groups[2].Value)
                {
                    case "M":
                        Gender = Gender.Masculine;
                        break;
                    case "F":
                        Gender = Gender.Feminine;
                        break;
                    case "N":
                        Gender = Gender.Neuter;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown gender");
                }
            }
        }

        private static string GetEntryKey(string entry)
        {
            for (int i = 0; i < entry.Length; i++)
            {
                if (!char.IsLetter(entry[i]))
                {
                    return entry.Substring(0, i).ToLowerInvariant();
                }
            }

            return entry.ToLowerInvariant();
        }
    }
}

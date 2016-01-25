// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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
                    yield return new NounStem(Declension, Gender, EntryKey.Substring(0, EntryKey.Length - 1), this);
                    break;
                default:
                    break;
            }
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

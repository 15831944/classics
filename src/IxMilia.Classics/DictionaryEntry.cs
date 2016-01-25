// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Text.RegularExpressions;

namespace IxMilia.Classics
{
    public class DictionaryEntry
    {
        public string Entry { get; }
        public string Definition { get; }
        public string Flags { get; }

        public Declension Declension { get; private set; }
        public Gender Gender { get; private set; }

        private static readonly Regex _nounMatcher = new Regex(@"N \(([12345])..\) ([MFN])");

        public DictionaryEntry(string entry, string definition, string partOfSpeech, string flags)
        {
            Entry = entry;
            Definition = definition;
            Flags = flags;

            ParsePartOfSpeech(partOfSpeech);
        }

        private void ParsePartOfSpeech(string pos)
        {
            var match = _nounMatcher.Match(pos);
            if (match.Success)
            {
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
    }
}

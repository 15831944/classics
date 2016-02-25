// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace IxMilia.Classics
{
    public class LatinDictionary
    {
        private readonly Regex _definitionMatcher = new Regex(@"^#(.*)  ([A-Z].*)\[(.....)\] :: (.*)$");
        //                                                        ^^^^ word
        //                                                              ^^^^^^^^^ part of speech
        //                                                                       ^^^^^^^^^^^ flags
        //                                                                                      ^^^^ definition

        private Assembly CurrentAssembly => typeof(LatinDictionary).GetTypeInfo().Assembly;
        private Trie<char, Stem> _stemCache = new Trie<char, Stem>();

        public LatinDictionary()
        {
            LoadDictionary();
        }

        public IEnumerable<DefinitionGroup> GetDefinitions(string word)
        {
            word = word.ToLowerInvariant();

            var definitions = new List<DefinitionGroup>();

            var parts = GetDefinitionParts(word);
            definitions.AddRange(parts.Select(part => new DefinitionGroup(new[] { part })));

            // try `-ne` and `-que`
            foreach (var enc in Enclitics)
            {
                if (word.EndsWith(enc))
                {
                    var baseWord = word.Substring(0, word.Length - enc.Length);
                    var entry = GetEnclitic(enc);
                    var encliticStem = entry.GetStems().Single();
                    var encliticDefined = new DefinitionPart(encliticStem, new Span(baseWord.Length, enc.Length), entry.GetStems().SelectMany(s => s.GetForms()));
                    var baseMatches = GetDefinitionParts(baseWord);
                    foreach (var baseMatch in baseMatches)
                    {
                        definitions.Add(new DefinitionGroup(new[] { baseMatch, encliticDefined }));
                    }
                }
            }

            return definitions;
        }

        private DictionaryEntry GetEnclitic(string enc)
        {
            switch (enc)
            {
                case "ne":
                    return _neEnclitic;
                case "que":
                    return _queEnclitic;
                default:
                    throw new InvalidOperationException("Unexpected enclitic " + enc);
            }
        }

        private readonly string[] Enclitics = new string[] { "ne", "que" };

        private DictionaryEntry _neEnclitic;
        private DictionaryEntry _queEnclitic;

        private IEnumerable<DefinitionPart> GetDefinitionParts(string word)
        {
            var result = new List<DefinitionPart>();
            var matchingStems = _stemCache.GetValues(word.ToCharArray());
            foreach (var stem in matchingStems)
            {
                var forms = new List<WordForm>();
                if (word.StartsWith(stem.StemPart))
                {
                    foreach (var form in stem.GetForms())
                    {
                        if (word.Length == form.Stem.StemPart.Length + form.Suffix.Length && word.EndsWith(form.Suffix))
                        {
                            forms.Add(form);
                        }
                    }
                }

                if (forms.Count > 0)
                {
                    var part = new DefinitionPart(stem, new Span(0, word.Length), forms);
                    result.Add(part);
                }
            }

            return result;
        }

        private void LoadDictionary()
        {
            var dictionaryStream = CurrentAssembly.GetManifestResourceStream("IxMilia.Classics.Downloads.DICTPAGE.RAW");
            var streamReader = new StreamReader(dictionaryStream);
            for (var line = streamReader.ReadLine(); line != null; line = streamReader.ReadLine())
            {
                // each line in the dictionary looks like this:
                // #word  part of speech       ...           [XXXXX] :: definition;
                //      ^^ that's two spaces         char 102 ^
                // the five characters starting at position 102 give additional information about the word:
                // column 1 - age
                //   X = used throughout ages/unknown (default)
                //   A = archaic
                //   B = early
                //   C = classical
                //   D = late (3rd-5th C)
                //   E = later (6th-10th C)
                //   F = medieval (11th-15th C)
                //   G = scholar (15th+ C)
                //   H = modern (19th-21st C)
                // column 2 - area
                //   X = all or none
                //   A = agriculture, flora, fauna, land, equipment, rural
                //   B = biological, medical, body parts
                //   D = drama, music, theater, art, painting, sculpture
                //   E = ecclesiastic, biblical, religious
                //   G = grammar, rhetoric, logic, literature, schools
                //   L = legal, government, tax, financial, political, titles
                //   P = poetic
                //   S = science, philosophy, mathematics, units/measures
                //   T = technical, architecture, topography, surveying
                //   W = war, military, naval, ships, armor
                //   Y = mythology
                // column 3 - geography
                //   X = all or none
                //   A = Africa
                //   B = Britian
                //   C = China
                //   D = Scandinavia
                //   E = Egypt
                //   F = France, Gaul
                //   G = Germany
                //   H = Greece
                //   I = Italy, Rome
                //   J = India
                //   K = Balkans
                //   N = Netherlands
                //   P = Persia
                //   Q = Near East
                //   R = Russia
                //   S = Spain, Iberia
                //   U = Eastern Europe
                // column 4 - frequency
                //   X = unknown or unspecified
                //   A = very frequent - top 1000 words
                //   B = frequent - next 2000 words
                //   C = common - top 10000 words
                //   D = lesser - top 20000 words
                //   E = uncommon = 2-3 citations
                //   F = very rare = 1 citation
                //   I = inscription
                //   M = graffiti
                //   N = Pliny
                // column 5 - source
                //   X = General or unknown or too common to say
                //   A =
                //   B = C.H.Beeson, A Primer of Medieval Latin, 1925(Bee)
                //   C = Charles Beard, Cassell's Latin Dictionary 1892 (CAS)
                //   D = J.N.Adams, Latin Sexual Vocabulary, 1982(Sex)
                //   E = L.F.Stelten, Dictionary of Eccles.Latin, 1995(Ecc)
                //   F = Roy J.Deferrari, Dictionary of St.Thomas Aquinas, 1960(DeF)
                //   G = Gildersleeve + Lodge, Latin Grammar 1895(G + L)
                //   H = Collatinus Dictionary by Yves Ouvrard
                //   I = Leverett, F.P., Lexicon of the Latin Language, Boston 1845
                //   J =
                //   K = Calepinus Novus, modern Latin, by Guy Licoppe (Cal)
                //   L = Lewis, C.S., Elementary Latin Dictionary 1891
                //   M = Latham, Revised Medieval Word List, 1980
                //   N = Lynn Nelson, Wordlist
                //   O = Oxford Latin Dictionary, 1982(OLD)
                //   P = Souter, A Glossary of Later Latin to 600 A.D., Oxford 1949
                //   Q = Other, cited or unspecified dictionaries
                //   R = Plater & White, A Grammar of the Vulgate, Oxford 1926
                //   S = Lewis and Short, A Latin Dictionary, 1879(L + S)
                //   T = Found in a translation--  no dictionary reference
                //   U = Du Cange
                //   V = Vademecum in opus Saxonis - Franz Blatt(Saxo)
                //   W = My personal guess
                //   Y = Temp special code
                //   Z = Sent by user, no dictionary reference
                var match = _definitionMatcher.Match(line);
                var entry = match.Groups[1].Value.Trim();
                var pos = match.Groups[2].Value.Trim();
                var flags = match.Groups[3].Value.Trim();
                var definition = match.Groups[4].Value.Trim().TrimEnd(';');

                var dictEntry = DictionaryEntry.ParseDictionaryEntry(entry, pos, flags, definition);
                if (dictEntry != null)
                {
                    // cached `-ne` and `-que` enclitics for later use
                    if (dictEntry.PartOfSpeech == PartOfSpeech.Conjunction)
                    {
                        if (dictEntry.Entry == "ne" && _neEnclitic == null)
                        {
                            _neEnclitic = dictEntry;
                        }
                        else if (dictEntry.Entry == "que" && _queEnclitic == null)
                        {
                            _queEnclitic = dictEntry;
                        }
                    }

                    foreach (var stem in dictEntry.GetStems())
                    {
                        if (stem.StemPart?.Length > 0)
                        {
                            _stemCache.Add(stem.StemPart.ToCharArray(), stem);
                        }
                    }
                }
            }
        }
    }
}

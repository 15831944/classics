// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using Xunit;

namespace IxMilia.Classics.Test
{
    public class DictionaryTests
    {
        static readonly LatinDictionary Latin = new LatinDictionary();

        private DefinitionGroup GetSinglyDefinedWord(string word)
        {
            var definitions = Latin.GetDefinitions(word);
            var definitionGroup = definitions.Single();
            var uniqueStems = definitionGroup.Parts.Select(part => part.Stem).Distinct();
            Assert.Equal(1, uniqueStems.Count());
            return definitionGroup;
        }

        private DictionaryEntry GetSinglyDefinedEntry(string word)
        {
            var definitionGroup = GetSinglyDefinedWord(word);
            return definitionGroup.Parts.Single().Stem.Entry;
        }

        private void AssertNounType(string word, Declension declension, Gender gender)
        {
            var entry = (NounEntry)GetSinglyDefinedEntry(word);
            Assert.Equal(declension, entry.Declension);
            Assert.Equal(gender, entry.Gender);
        }

        private void AssertVerbType(string word, Conjugation conjugation)
        {
            var entry = (VerbEntry)GetSinglyDefinedEntry(word);
            Assert.Equal(conjugation, entry.Conjugation);
        }

        private void AssertConjuction(string word)
        {
            var entry = GetSinglyDefinedEntry(word);
            Assert.Equal(PartOfSpeech.Conjunction, entry.PartOfSpeech);
        }

        [Fact]
        public void NounPartOfSpeechParseTest1()
        {
            AssertNounType("porta", Declension.First, Gender.Feminine);
        }

        [Fact]
        public void NounPartOfSpeechParseTest2()
        {
            AssertNounType("annus", Declension.Second, Gender.Masculine);
        }

        [Fact]
        public void NounPartOfSpeechParseTest3()
        {
            AssertNounType("vir", Declension.Second, Gender.Masculine);
        }

        [Fact]
        public void NounPartOfSpeechParseTest4()
        {
            AssertNounType("epos", Declension.Third, Gender.Neuter);
        }

        [Fact]
        public void NounMatchesMultipleStemsButOnlyOneFormTest()
        {
            AssertNounType("arma", Declension.Second, Gender.Neuter);
        }

        [Fact]
        public void VerbConjugationTest1()
        {
            AssertVerbType("laudo", Conjugation.First);
        }

        [Fact]
        public void VerbConjugationTest2()
        {
            AssertVerbType("laudat", Conjugation.First);
        }

        [Fact]
        public void ConjuectionTest1()
        {
            AssertConjuction("que");
        }

        [Fact]
        public void EncliticTest()
        {
            var definition = Latin.GetDefinitions("armaque").Single();
            Assert.Equal(2, definition.Parts.Count());
            Assert.Equal("armum, armi", definition.Parts.First().Stem.Entry.Entry);
            Assert.Equal("que", definition.Parts.Last().Stem.Entry.Entry);
        }

        [Fact]
        public void TrieTest1()
        {
            var t = new StringTrie<string>();
            t.Add("arm", "armum");
            t.Add("armat", "armatus");
            t.Add("arment", "armenta");
            var three = t.GetValues("armenta").ToArray();
        }
    }
}

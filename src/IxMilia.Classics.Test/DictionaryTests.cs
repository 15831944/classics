// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using Xunit;

namespace IxMilia.Classics.Test
{
    public class DictionaryTests
    {
        static readonly LatinDictionary Latin = new LatinDictionary();

        private IGrouping<DictionaryEntry, WordForm> GetSingleEntry(string word)
        {
            var definitions = Latin.GetDefinitions(word);
            return definitions.Single();
        }

        private void AssertNounType(string word, Declension declension, Gender gender)
        {
            var entry = (NounEntry)GetSingleEntry(word).Key;
            Assert.Equal(declension, entry.Declension);
            Assert.Equal(gender, entry.Gender);
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
    }
}

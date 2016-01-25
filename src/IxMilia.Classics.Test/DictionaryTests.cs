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
            return Latin.GetDefinitions(word).Single();
        }

        [Fact]
        public void NounPartOfSpeechParseTest()
        {
            var entry = GetSingleEntry("porta").Key;
            Assert.Equal(Declension.First, entry.Declension);
            Assert.Equal(Gender.Feminine, entry.Gender);
        }
    }
}

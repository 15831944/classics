// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using Xunit;

namespace IxMilia.Classics.Test
{
    public class DictionaryTests
    {
        static readonly LatinDictionary Latin = new LatinDictionary();

        private DictionaryEntry GetSingleEntry(string word)
        {
            return Latin.GetDefinitions(word).Single();
        }

        [Fact]
        public void NounPartOfSpeechParseTest()
        {
            var word = GetSingleEntry("porta");
            Assert.Equal(Declension.First, word.Declension);
            Assert.Equal(Gender.Feminine, word.Gender);
        }
    }
}

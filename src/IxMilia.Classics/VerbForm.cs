// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Classics
{
    public class VerbForm : WordForm
    {
        public Conjugation Conjugation { get; }
        public Person Person { get; }
        public Number Number { get; }
        public Mood Mood { get; }
        public Voice Voice { get; }
        public Tense Tense { get; }

        internal VerbForm(Stem stem, Conjugation conjugation, Person person, Number number, Mood mood, Voice voice, Tense tense, string suffix)
            : base(stem, suffix)
        {
            Conjugation = conjugation;
            Person = person;
            Number = number;
            Mood = mood;
            Voice = voice;
            Tense = tense;
        }
    }
}

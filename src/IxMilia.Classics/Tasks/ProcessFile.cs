// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace IxMilia.Classics.Tasks
{
    public class ProcessFile : Task
    {
        [Required]
        public string File { get; set; }

        [Required]
        public string Dictionary { get; set; }

        [Required]
        public string Output { get; set; }

        private const string QueSuffix = "que";
        private readonly Regex _wordMatcher = new Regex("[a-zA-Z]+", RegexOptions.Compiled);

        private int _bookNumber;
        private IEnumerable<XElement> _elements;
        private int _currentLine;

        public override bool Execute()
        {
            LoadFile();
            LoadDictionary();

            _currentLine = 1;
            foreach (var element in _elements)
            {
                AssertCurrentLineNumber(element);
                switch (element.Name.LocalName)
                {
                    case "milestone":
                        break;
                    case "l":
                        var words = BreakLineIntoWords(element.Value);
                        //Console.WriteLine($"Found words: [{string.Join("|", words)}]");

                        _currentLine++;
                        break;
                }
            }

            return true;
        }

        private void LoadFile()
        {
            var file = XDocument.Load(File).Root;
            var content = file.Element("text").Element("body").Element("div1");
            _bookNumber = int.Parse(content.Attribute("n").Value);
            _elements = content.Elements();
        }

        private void LoadDictionary()
        {

        }

        private IEnumerable<string> BreakLineIntoWords(string line)
        {
            return _wordMatcher.Matches(line).Cast<Match>().SelectMany(m => BreakCompoundWords(m.Value));
        }

        private IEnumerable<string> BreakCompoundWords(string word)
        {
            if (word.EndsWith(QueSuffix))
            {
                var prefix = word.Substring(0, word.Length - QueSuffix.Length);
                var suffix = QueSuffix;
                yield return prefix;
                yield return suffix;
            }
            else
            {
                yield return word;
            }
        }

        private void AssertCurrentLineNumber(XElement element)
        {
            var natt = element.Attribute("n");
            if (natt != null)
            {
                var line = int.Parse(natt.Value);
                if (line != _currentLine)
                {
                    throw new Exception($"Expected current line to be {line} but was {_currentLine}");
                }
            }
        }
    }
}

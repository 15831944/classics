// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace IxMilia.Classics.BuildTasks
{
    public class ProcessFile : Task
    {
        [Required]
        public string File { get; set; }

        [Required]
        public string Output { get; set; }

        public int MaxLines { get; set; }

        private readonly Regex _wordMatcher = new Regex("[a-zA-Z]+", RegexOptions.Compiled);

        private int _bookNumber;
        private IEnumerable<XElement> _elements;
        private int _currentLine;
        private int _definedWords;
        private int _undefinedWords;

        public override bool Execute()
        {
            LoadFile();
            var latin = new LatinDictionary();

            _currentLine = 1;
            _definedWords = 0;
            _undefinedWords = 0;
            foreach (var element in _elements)
            {
                if (MaxLines > 0 && _currentLine > MaxLines)
                {
                    break;
                }

                AssertCurrentLineNumber(element);
                switch (element.Name.LocalName)
                {
                    case "milestone":
                        break;
                    case "l":
                        var words = BreakLineIntoWords(element.Value);
                        foreach (var word in words)
                        {
                            var matchedForms = latin.GetDefinitions(word).ToArray();
                            if (matchedForms.Length == 0)
                            {
                                _undefinedWords++;
                                Log.LogError($"Unable to define {word} on line {_currentLine}.");
                            }
                            else
                            {
                                _definedWords++;
                                foreach (var matchedForm in matchedForms)
                                {
                                    Console.WriteLine($"Found definition on line {_currentLine}: {word} = {matchedForm.Key.Entry} - {matchedForm.Key.Definition}");
                                }
                            }
                        }

                        _currentLine++;
                        break;
                }
            }

            if (MaxLines > 0)
            {
                Log.LogError($"Failing the build because {nameof(MaxLines)} was specified.");
            }

            Log.LogMessage($"Defined {_definedWords} words with {_undefinedWords} undefined.  {_definedWords * 100.0 / (_definedWords + _undefinedWords)}% success rate.");

            return true;
        }

        private void LoadFile()
        {
            var file = XDocument.Load(File).Root;
            var content = file.Element("text").Element("body").Element("div1");
            _bookNumber = int.Parse(content.Attribute("n").Value);
            _elements = content.Elements();
        }

        private IEnumerable<string> BreakLineIntoWords(string line)
        {
            return _wordMatcher.Matches(line).Cast<Match>().Select(m => m.Value);
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

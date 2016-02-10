// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public string OutputPath { get; set; }

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

            var content = new StringBuilder();
            var uncommonWords = new Dictionary<string, Tuple<string, string>>();

            content.AppendLine(@"\newchapter{Book 1}");

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
                        content.AppendLine(@"\indent");
                        break;
                    case "l":
                        content.Append(@"\lline{");
                        var wb = new StringBuilder();
                        foreach (var c in element.Value)
                        {
                            if (char.IsLetter(c))
                            {
                                wb.Append(c);
                            }
                            else
                            {
                                var word = wb.ToString();
                                wb.Clear();
                                DefineAndPrintWord(latin, content, uncommonWords, word);
                                content.Append(c);
                            }
                        }

                        if (wb.Length > 0)
                        {
                            DefineAndPrintWord(latin, content, uncommonWords, wb.ToString());
                            wb.Clear();
                        }

                        content.AppendLine("}");
                        _currentLine++;
                        break;
                }
            }

            System.IO.File.WriteAllText(Path.Combine(OutputPath, "content.tex"), content.ToString());
            System.IO.File.WriteAllText(Path.Combine(OutputPath, "commonwords.tex"), "");
            System.IO.File.WriteAllText(Path.Combine(OutputPath, "uncommonwords.tex"), string.Join("\r\n", uncommonWords.OrderBy(kvp => kvp.Key).Select(kvp => $@"\newuncommonterm{{{kvp.Key}}}{{{kvp.Value.Item1}}}{{{kvp.Value.Item2}}}")));

            if (MaxLines > 0)
            {
                Log.LogError($"Failing the build because {nameof(MaxLines)} was specified.");
            }

            Log.LogMessage($"Defined {_definedWords} words with {_undefinedWords} undefined.  {_definedWords * 100.0 / (_definedWords + _undefinedWords)}% success rate.");

            return true;
        }

        private void DefineAndPrintWord(LatinDictionary latin, StringBuilder content, Dictionary<string, Tuple<string, string>> uncommonWords, string word)
        {
            var matchedForms = latin.GetDefinitions(word).ToArray();
            bool defined = false;
            if (matchedForms.Length == 0)
            {
                _undefinedWords++;
                Log.LogError($"Unable to define {word} on line {_currentLine}.");
            }
            else
            {
                _definedWords++;
                if (matchedForms.Count() == 1)
                {
                    // assume all words are uncommon for now
                    var form = matchedForms.Single().Key;
                    content.Append($@"\uncom[{form.EntryKey}]{{{word}}}");
                    defined = true;
                }

                foreach (var matchedForm in matchedForms)
                {
                    uncommonWords[matchedForm.Key.EntryKey] = Tuple.Create(matchedForm.Key.Entry, matchedForm.Key.Definition);
                    Console.WriteLine($"Found definition on line {_currentLine}: {word} = {matchedForm.Key.Entry} - {matchedForm.Key.Definition}");
                }
            }

            if (!defined)
            {
                content.Append(word);
            }
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

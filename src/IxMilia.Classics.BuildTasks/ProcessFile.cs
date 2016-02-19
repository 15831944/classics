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

        public int CommonWordCount { get; set; } = 250;

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
            var definedWords = new Dictionary<string, Tuple<int, string, string>>();

            content.AppendLine(@"\newchapter{Book 1}");

            // define all words
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
                                if (wb.Length > 0)
                                {
                                    var word = wb.ToString();
                                    wb.Clear();
                                    DefineAndPrintWord(latin, content, definedWords, word);
                                }

                                AppendCharacter(content, c);
                            }
                        }

                        if (wb.Length > 0)
                        {
                            DefineAndPrintWord(latin, content, definedWords, wb.ToString());
                            wb.Clear();
                        }

                        content.AppendLine("}");
                        _currentLine++;
                        break;
                }
            }

            // sort common/uncommon words
            var ordered = definedWords.OrderByDescending(d => d.Value.Item1).ToList();
            var commonWords = ordered.Take(CommonWordCount).Select(d => new KeyValuePair<string, Tuple<string, string>>(d.Key, Tuple.Create(d.Value.Item2, d.Value.Item3)));
            var uncommonWords = ordered.Skip(CommonWordCount).Select(d => new KeyValuePair<string, Tuple<string, string>>(d.Key, Tuple.Create(d.Value.Item2, d.Value.Item3)));

            System.IO.File.WriteAllText(Path.Combine(OutputPath, "content.tex"), content.ToString());
            System.IO.File.WriteAllText(Path.Combine(OutputPath, "commonwords.tex"), string.Join("\r\n", commonWords.OrderBy(kvp => kvp.Key).Select(kvp => $@"\newcommonterm{{{kvp.Key}}}{{{kvp.Value.Item1}}}{{{kvp.Value.Item2}}}")));
            System.IO.File.WriteAllText(Path.Combine(OutputPath, "uncommonwords.tex"), string.Join("\r\n", uncommonWords.OrderBy(kvp => kvp.Key).Select(kvp => $@"\newuncommonterm{{{kvp.Key}}}{{{kvp.Value.Item1}}}{{{kvp.Value.Item2}}}")));

            if (MaxLines > 0)
            {
                Log.LogError($"Failing the build because {nameof(MaxLines)} was specified.");
            }

            Log.LogMessage($"Defined {_definedWords} words with {_undefinedWords} undefined.  {_definedWords * 100.0 / (_definedWords + _undefinedWords)}% success rate.");

            return true;
        }

        private static void AppendCharacter(StringBuilder sb, char c)
        {
            switch (c)
            {
                case '\u201C': // double left quote
                    sb.Append("``");
                    break;
                case '\u201D': // double right quote
                    sb.Append("''");
                    break;
                default:
                    sb.Append(c);
                    break;
            }
        }

        private void DefineAndPrintWord(LatinDictionary latin, StringBuilder content, Dictionary<string, Tuple<int, string, string>> definedWords, string word)
        {
            var matchedForms = latin.GetDefinitions(word).ToArray();
            bool defined = false;
            switch (matchedForms.Length)
            {
                case 0:
                    _undefinedWords++;
                    Log.LogError($"Unable to define {word} on line {_currentLine}.");
                    break;
                case 1:
                    _definedWords++;
                    defined = true;
                    var form = matchedForms.Single().Key;
                    content.Append($@"\agls{{{form.EntryKey}}}{{{word}}}");
                    if (definedWords.ContainsKey(form.EntryKey))
                    {
                        var tuple = definedWords[form.EntryKey];
                        definedWords[form.EntryKey] = Tuple.Create(tuple.Item1 + 1, tuple.Item2, tuple.Item3);
                    }
                    else
                    {
                        definedWords[form.EntryKey] = Tuple.Create(1, form.Entry, form.Definition);
                    }

                    Console.WriteLine($"Found definition on line {_currentLine}: {word} = {form.Entry} - {form.Definition}");
                    break;
                default:
                    _undefinedWords++;
                    Log.LogError($"Ambiguous definition for {word} on line {_currentLine}: [{string.Join("; ", matchedForms.Select(m => m.Key.Entry))}].");
                    break;
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

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
            var macro = "lline";
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
                        macro = "pline";
                        break;
                    case "l":
                        content.Append(@"\" + macro + "{");
                        macro = "lline";
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
            System.IO.File.WriteAllText(Path.Combine(OutputPath, "commonwords.tex"), string.Join("\r\n", commonWords.OrderBy(kvp => kvp.Key).Select(kvp => $@"\newcommonterm{{{EscapeString(kvp.Key)}}}{{{EscapeString(kvp.Value.Item1)}}}{{{EscapeString(kvp.Value.Item2)}}}")));
            System.IO.File.WriteAllText(Path.Combine(OutputPath, "uncommonwords.tex"), string.Join("\r\n", uncommonWords.OrderBy(kvp => kvp.Key).Select(kvp => $@"\newuncommonterm{{{EscapeString(kvp.Key)}}}{{{EscapeString(kvp.Value.Item1)}}}{{{EscapeString(kvp.Value.Item2)}}}")));

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
                case '_':
                    sb.Append(' ');
                    break;
                case '<':
                    sb.Append(@"\textless ");
                    break;
                case '>':
                    sb.Append(@"\textgreater ");
                    break;
                case '\\':
                    sb.Append(@"\textbackslash ");
                    break;
                case '%':
                case '{':
                case '}':
                    sb.Append('\\');
                    sb.Append(c);
                    break;
                case '\u201C': // left double quote
                    sb.Append("``");
                    break;
                case '\u201D': // right double quote
                    sb.Append("''");
                    break;
                default:
                    sb.Append(c);
                    break;
            }
        }

        private static string EscapeString(string str)
        {
            var sb = new StringBuilder();
            var seenStartQuote = false;
            foreach (var c in str)
            {
                switch (c)
                {
                    case '"':
                        sb.Append(seenStartQuote ? "''" : "``");
                        seenStartQuote = !seenStartQuote;
                        break;
                    default:
                        AppendCharacter(sb, c);
                        break;
                }
            }

            return sb.ToString();
        }

        private void DefineAndPrintWord(LatinDictionary latin, StringBuilder content, Dictionary<string, Tuple<int, string, string>> definedWords, string word)
        {
            var definitionGroups = latin.GetDefinitions(word).ToArray();
            bool defined = false;
            switch (definitionGroups.Length)
            {
                case 0:
                    _undefinedWords++;
                    Log.LogError($"Unable to define {word} on line {_currentLine}.");
                    break;
                case 1:
                    defined = true;
                    foreach (var part in definitionGroups.Single().Parts)
                    {
                        _definedWords++;
                        var entry = part.Stem.Entry;
                        content.Append($@"\agls{{{EscapeString(entry.EntryKey)}}}{{{EscapeString(word.Substring(part.Span.Offset, part.Span.Length))}}}");
                        if (definedWords.ContainsKey(entry.EntryKey))
                        {
                            var tuple = definedWords[entry.EntryKey];
                            definedWords[entry.EntryKey] = Tuple.Create(tuple.Item1 + 1, tuple.Item2, tuple.Item3);
                        }
                        else
                        {
                            definedWords[entry.EntryKey] = Tuple.Create(1, entry.Entry, entry.Definition);
                        }

                        Console.WriteLine($"Found definition on line {_currentLine}: {word} = {entry.Entry} - {entry.Definition}");
                    }

                    break;
                default:
                    _undefinedWords++;
                    Log.LogError($"Ambiguous definition for {word} on line {_currentLine}: [{string.Join("; ", definitionGroups.SelectMany(m => m.Parts.Select(fs => fs.Stem.Entry.Entry)))}].");
                    break;
            }

            if (!defined)
            {
                content.Append(EscapeString(word));
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

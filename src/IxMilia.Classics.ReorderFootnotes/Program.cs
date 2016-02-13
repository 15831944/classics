// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IxMilia.Classics.ReorderFootnotes
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Expected two arguments: glossary page order and glossary word order");
                Environment.Exit(1);
            }

            var pageOrderFile = args[0];
            var wordOrderFile = args[1];

            var wordKeys = File.ReadAllLines(wordOrderFile);

            int currentPage = -1;
            int wordsPerPage = 0;
            int keyOffset = 0;
            var sortedWords = new List<string>();
            foreach (var line in File.ReadAllLines(pageOrderFile))
            {
                if (line.StartsWith("PAGE="))
                {
                    Console.WriteLine($"Writing {wordsPerPage} for page {currentPage}");
                    var wordsOnThisPage = wordKeys.Skip(keyOffset)
                        .Take(wordsPerPage)
                        .OrderBy(w => w);
                    sortedWords.AddRange(wordsOnThisPage);
                    currentPage = int.Parse(line.Split('=')[1]);
                    keyOffset += wordsPerPage;
                    wordsPerPage = 0;
                }
                else if (line == "WORD")
                {
                    wordsPerPage++;
                }
                else
                {
                    throw new InvalidOperationException("Unexpected line value: " + line);
                }
            }

            File.WriteAllLines(wordOrderFile, sortedWords);
        }
    }
}

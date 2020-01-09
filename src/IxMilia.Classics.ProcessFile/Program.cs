using System;

namespace IxMilia.Classics.ProcessFile
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 5)
            {
                Console.WriteLine("Expected 5 arguments: <text-files> <gloss-files> <output-path> <common-word-count> <raw-dict-path>");
                Environment.Exit(1);
            }

            var textFiles = args[0].Split(',');
            var glossFiles = args[1].Split(',');
            var outputPath = args[2];
            var commonWordCount = int.Parse(args[3]);
            var rawDictionaryPath = args[4];

            var processor = new FileProcessor(textFiles, glossFiles, commonWordCount, rawDictionaryPath);
            processor.Process(outputPath);
        }
    }
}

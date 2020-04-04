using System;

namespace IxMilia.Classics.ProcessFile
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 6)
            {
                Console.WriteLine("Expected 6 arguments: <mode> <text-files> <gloss-files> <output-path> <common-word-count> <raw-dict-path>");
                Environment.Exit(1);
            }

            var mode = args[0];
            var textFiles = args[1].Split(',');
            var glossFiles = args[2].Split(',');
            var outputPath = args[3];
            var commonWordCount = int.Parse(args[4]);
            var rawDictionaryPath = args[5];

            var processor = new FileProcessor(mode, textFiles, glossFiles, commonWordCount, rawDictionaryPath);
            processor.Process(outputPath);
        }
    }
}

// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.IO;
using System.IO.Compression;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace IxMilia.Classics.Tasks
{
    public class Unzip : Task
    {
        [Required]
        public string Archive { get; set; }

        [Required]
        public string Destination { get; set; }

        public bool OverwriteExisting { get; set; }

        public override bool Execute()
        {
            using (var stream = new FileStream(Archive, FileMode.Open))
            using (var archive = new ZipArchive(stream))
            {
                foreach (var entry in archive.Entries)
                {
                    var destinationFile = Path.Combine(Destination, entry.FullName);
                    var directory = Path.GetDirectoryName(destinationFile);
                    Directory.CreateDirectory(directory);
                    if (OverwriteExisting || !File.Exists(destinationFile))
                    {
                        Log.LogMessage($"Unzipping to {destinationFile}");
                        using (var entryStream = entry.Open())
                        using (var outputFile = new FileStream(destinationFile, FileMode.Create))
                        {
                            entryStream.CopyTo(outputFile);
                        }
                    }
                }
            }

            return true;
        }
    }
}

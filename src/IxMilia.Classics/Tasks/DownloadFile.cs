// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.IO;
using System.Net;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace IxMilia.Classics.Tasks
{
    public class DownloadFile : Task
    {
        [Required]
        public string Url { get; set; }

        [Required]
        public string Destination { get; set; }

        public override bool Execute()
        {
            if (!File.Exists(Destination))
            {
                Log.LogMessage($"Downloading from {Url}");
                new WebClient().DownloadFile(Url, Destination);
            }

            return true;
        }
    }
}

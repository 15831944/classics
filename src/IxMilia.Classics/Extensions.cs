// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;

namespace IxMilia.Classics
{
    public static class Extensions
    {
        public static string RemoveSuffix(this string str, string suffix)
        {
            Debug.Assert(str.EndsWith(suffix));
            return str.Substring(0, str.Length - suffix.Length);
        }
    }
}

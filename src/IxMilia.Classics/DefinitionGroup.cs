// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace IxMilia.Classics
{
    public class DefinitionGroup
    {
        public IEnumerable<DefinitionPart> Parts { get; }

        public DefinitionGroup(IEnumerable<DefinitionPart> parts)
        {
            Parts = parts;
        }
    }
}

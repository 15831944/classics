// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace IxMilia.Classics
{
    public class StringTrie<TValue> : Trie<char, TValue>
    {
        public void Add(string key, TValue value)
        {
            Add(key.ToCharArray(), value);
        }

        public IEnumerable<TValue> GetValues(string key)
        {
            return GetValues(key.ToCharArray());
        }
    }

    public class Trie<TKeyType, TValue>
    {
        private Node _root = new Node();

        public Trie()
        {
        }

        public void Add(IEnumerable<TKeyType> key, TValue value)
        {
            var keyParts = key.ToArray();
            _root.Add(keyParts, value, 0);
        }

        public IEnumerable<TValue> GetValues(IEnumerable<TKeyType> key)
        {
            return _root.Get(key.ToArray(), 0);
        }

        private class Node
        {
            public Dictionary<TKeyType, Node> Children { get; } = new Dictionary<TKeyType, Node>();
            public List<TValue> Values = new List<TValue>();

            public void Add(TKeyType[] keys, TValue value, int keyOffset)
            {
                if (keyOffset >= keys.Length)
                {
                    Values.Add(value);
                }
                else
                {
                    var key = keys[keyOffset];
                    if (!Children.ContainsKey(key))
                    {
                        Children[key] = new Node();
                    }

                    Children[key].Add(keys, value, keyOffset + 1);
                }
            }

            public IEnumerable<TValue> Get(TKeyType[] keys, int keyOffset)
            {
                foreach (var value in Values)
                {
                    yield return value;
                }

                if (keyOffset < keys.Length)
                {
                    var key = keys[keyOffset];
                    if (Children.ContainsKey(key))
                    {
                        foreach (var childItem in Children[key].Get(keys, keyOffset + 1))
                        {
                            yield return childItem;
                        }
                    }
                }
            }
        }
    }
}

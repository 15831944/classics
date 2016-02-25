// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace IxMilia.Classics
{
    public class Trie<TKeyType, TValue>
    {
        private Node _root = new Node();

        public Trie()
        {
        }

        public void Add(IEnumerable<TKeyType> key, TValue value)
        {
            var keyParts = key.ToArray();
            _root.Add(keyParts, value);
        }

        public IEnumerable<TValue> GetValues(IEnumerable<TKeyType> key)
        {
            return _root.Get(key.ToArray());
        }

        private abstract class NodeOrValue
        {
        }

        private class NodeValue : NodeOrValue
        {
            public TValue Value { get; }

            public NodeValue(TValue value)
            {
                Value = value;
            }
        }

        private class Node : NodeOrValue
        {
            public Dictionary<TKeyType, List<NodeOrValue>> Children { get; } = new Dictionary<TKeyType, List<NodeOrValue>>();

            public void Add(TKeyType[] keys, TValue value)
            {
                if (keys.Length == 0)
                {
                    throw new InvalidOperationException("Keys cannot be empty");
                }

                var key = keys[0];
                if (!Children.ContainsKey(key))
                {
                    Children[key] = new List<NodeOrValue>();
                }

                NodeOrValue addedNode;
                if (keys.Length == 1)
                {
                    addedNode = new NodeValue(value);
                }
                else
                {
                    var node = new Node();
                    node.Add(keys.Skip(1).ToArray(), value);
                    addedNode = node;
                }

                Children[key].Add(addedNode);
            }

            public IEnumerable<TValue> Get(TKeyType[] keys)
            {
                if (keys.Length == 0 || !Children.ContainsKey(keys[0]))
                {
                    yield break;
                }
                else
                {
                    foreach (var collection in Children.Values)
                    {
                        var newKeys = keys.Skip(1).ToArray();
                        foreach (var childNode in collection)
                        {
                            if (childNode is NodeValue)
                            {
                                yield return ((NodeValue)childNode).Value;
                            }
                            else
                            {
                                foreach (var childValue in ((Node)childNode).Get(newKeys))
                                {
                                    yield return childValue;
                                }
                            }
                        }
                    }
                }
            }

            private IEnumerable<TValue> GetAllChildValues()
            {
                foreach (var item in Children.Values.SelectMany(x => x))
                {
                    if (item is NodeValue)
                    {
                        yield return ((NodeValue)item).Value;
                    }
                    else
                    {
                        foreach (var childValue in ((Node)item).GetAllChildValues())
                        {
                            yield return childValue;
                        }
                    }
                }
            }
        }
    }
}

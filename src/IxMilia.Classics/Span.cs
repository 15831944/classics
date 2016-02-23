// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Classics
{
    public class Span
    {
        public int Offset { get; }
        public int Length { get; }

        public Span(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Span);
        }

        public bool Equals(Span other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return Offset == other.Offset && Length == other.Length;
        }

        public override int GetHashCode()
        {
            return Offset.GetHashCode() ^ Length.GetHashCode();
        }

        public static bool operator ==(Span left, Span right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if ((object)left == null || (object)right == null)
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(Span left, Span right)
        {
            return !(right == left);
        }
    }
}

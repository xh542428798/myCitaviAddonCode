using System;
using System.Collections.Generic;

namespace AutoRef
{
    class StringEqualityComparer : IEqualityComparer<string>
    {
        // Fields

        readonly StringComparison _stringComparison;

        //  Constructors

        private StringEqualityComparer(StringComparison stringComparison) => _stringComparison = stringComparison;

        // Properties

        public static StringEqualityComparer OrdinalIgnoreCase => new StringEqualityComparer(StringComparison.OrdinalIgnoreCase);

        // Methods

        public bool Equals(string x, string y) => x.Equals(y, _stringComparison);

        public int GetHashCode(string obj) => obj.GetHashCode();
    }
}

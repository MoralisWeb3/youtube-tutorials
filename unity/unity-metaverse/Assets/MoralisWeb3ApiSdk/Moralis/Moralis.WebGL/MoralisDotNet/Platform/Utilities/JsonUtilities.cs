
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Moralis.WebGL.Platform.Utilities
{
    /// <summary>
    /// A simple recursive-descent JSON Parser based on the grammar defined at http://www.json.org
    /// and http://tools.ietf.org/html/rfc4627
    /// </summary>
    public class JsonUtilities
    {
        /// <summary>
        /// Place at the start of a regex to force the match to begin wherever the search starts (i.e.
        /// anchored at the index of the first character of the search, even when that search starts
        /// in the middle of the string).
        /// </summary>
        private static readonly string startOfString = "\\G";
        private static readonly char startObject = '{';
        private static readonly char endObject = '}';
        private static readonly char startArray = '[';
        private static readonly char endArray = ']';
        private static readonly char valueSeparator = ',';
        private static readonly char nameSeparator = ':';
        private static readonly char[] falseValue = "false".ToCharArray();
        private static readonly char[] trueValue = "true".ToCharArray();
        private static readonly char[] nullValue = "null".ToCharArray();
        private static readonly Regex numberValue = new Regex(startOfString + @"-?(?:0|[1-9]\d*)(?<frac>\.\d+)?(?<exp>(?:e|E)(?:-|\+)?\d+)?");
        private static readonly Regex stringValue = new Regex(startOfString + "\"(?<content>(?:[^\\\\\"]|(?<escape>\\\\(?:[\\\\\"/bfnrt]|u[0-9a-fA-F]{4})))*)\"", RegexOptions.Multiline);

        private static readonly Regex escapePattern = new Regex("\\\\|\"|[\u0000-\u001F]");

        private class JsonStringParser
        {
            public string Input { get; private set; }

            public char[] InputAsArray { get; private set; }
            public int CurrentIndex { get; private set; }

            public void Skip(int skip) => CurrentIndex += skip;

            public JsonStringParser(string input)
            {
                Input = input;
                InputAsArray = input.ToCharArray();
            }

            /// <summary>
            /// Parses JSON object syntax (e.g. '{}')
            /// </summary>
            internal bool ParseObject(out object output)
            {
                output = null;
                int initialCurrentIndex = CurrentIndex;
                if (!Accept(startObject))
                    return false;

                Dictionary<string, object> dict = new Dictionary<string, object> { };
                while (true)
                {
                    if (!ParseMember(out object pairValue))
                        break;

                    Tuple<string, object> pair = pairValue as Tuple<string, object>;
                    dict[pair.Item1] = pair.Item2;
                    if (!Accept(valueSeparator))
                        break;
                }
                if (!Accept(endObject))
                    return false;
                output = dict;
                return true;
            }

            /// <summary>
            /// Parses JSON member syntax (e.g. '"keyname" : null')
            /// </summary>
            private bool ParseMember(out object output)
            {
                output = null;
                if (!ParseString(out object key))
                    return false;
                if (!Accept(nameSeparator))
                    return false;
                if (!ParseValue(out object value))
                    return false;
                output = new Tuple<string, object>((string) key, value);
                return true;
            }

            /// <summary>
            /// Parses JSON array syntax (e.g. '[]')
            /// </summary>
            internal bool ParseArray(out object output)
            {
                output = null;
                if (!Accept(startArray))
                    return false;
                List<object> list = new List<object>();
                while (true)
                {
                    if (!ParseValue(out object value))
                        break;
                    list.Add(value);
                    if (!Accept(valueSeparator))
                        break;
                }
                if (!Accept(endArray))
                    return false;
                output = list;
                return true;
            }

            /// <summary>
            /// Parses a value (i.e. the right-hand side of an object member assignment or
            /// an element in an array)
            /// </summary>
            private bool ParseValue(out object output)
            {
                if (Accept(falseValue))
                {
                    output = false;
                    return true;
                }
                else if (Accept(nullValue))
                {
                    output = null;
                    return true;
                }
                else if (Accept(trueValue))
                {
                    output = true;
                    return true;
                }
                return ParseObject(out output) ||
                  ParseArray(out output) ||
                  ParseNumber(out output) ||
                  ParseString(out output);
            }

            /// <summary>
            /// Parses a JSON string (e.g. '"foo\u1234bar\n"')
            /// </summary>
            private bool ParseString(out object output)
            {
                output = null;
                if (!Accept(stringValue, out Match m))
                    return false;
                // handle escapes:
                int offset = 0;
                Group contentCapture = m.Groups["content"];
                StringBuilder builder = new StringBuilder(contentCapture.Value);
                foreach (Capture escape in m.Groups["escape"].Captures)
                {
                    int index = escape.Index - contentCapture.Index - offset;
                    offset += escape.Length - 1;
                    builder.Remove(index + 1, escape.Length - 1);
                    switch (escape.Value[1])
                    {
                        case '\"':
                            builder[index] = '\"';
                            break;
                        case '\\':
                            builder[index] = '\\';
                            break;
                        case '/':
                            builder[index] = '/';
                            break;
                        case 'b':
                            builder[index] = '\b';
                            break;
                        case 'f':
                            builder[index] = '\f';
                            break;
                        case 'n':
                            builder[index] = '\n';
                            break;
                        case 'r':
                            builder[index] = '\r';
                            break;
                        case 't':
                            builder[index] = '\t';
                            break;
                        case 'u':
                            builder[index] = (char) UInt16.Parse(escape.Value.Substring(2), NumberStyles.AllowHexSpecifier);
                            break;
                        default:
                            throw new ArgumentException("Unexpected escape character in string: " + escape.Value);
                    }
                }
                output = builder.ToString();
                return true;
            }

            /// <summary>
            /// Parses a number. Returns a long if the number is an integer or has an exponent,
            /// otherwise returns a double.
            /// </summary>
            private bool ParseNumber(out object output)
            {
                output = null;
                if (!Accept(numberValue, out Match m))
                    return false;
                if (m.Groups["frac"].Length > 0 || m.Groups["exp"].Length > 0)
                {
                    // It's a double.
                    output = Double.Parse(m.Value, CultureInfo.InvariantCulture);
                    return true;
                }
                else
                {
                    output = Int64.Parse(m.Value, CultureInfo.InvariantCulture);
                    return true;
                }
            }

            /// <summary>
            /// Matches the string to a regex, consuming part of the string and returning the match.
            /// </summary>
            private bool Accept(Regex matcher, out Match match)
            {
                match = matcher.Match(Input, CurrentIndex);
                if (match.Success)
                    Skip(match.Length);
                return match.Success;
            }

            /// <summary>
            /// Find the first occurrences of a character, consuming part of the string.
            /// </summary>
            private bool Accept(char condition)
            {
                int step = 0;
                int strLen = InputAsArray.Length;
                int currentStep = CurrentIndex;
                char currentChar;

                // Remove whitespace
                while (currentStep < strLen &&
                  ((currentChar = InputAsArray[currentStep]) == ' ' ||
                  currentChar == '\r' ||
                  currentChar == '\t' ||
                  currentChar == '\n'))
                {
                    ++step;
                    ++currentStep;
                }

                bool match = currentStep < strLen && InputAsArray[currentStep] == condition;
                if (match)
                {
                    ++step;
                    ++currentStep;

                    // Remove whitespace
                    while (currentStep < strLen &&
                      ((currentChar = InputAsArray[currentStep]) == ' ' ||
                      currentChar == '\r' ||
                      currentChar == '\t' ||
                      currentChar == '\n'))
                    {
                        ++step;
                        ++currentStep;
                    }

                    Skip(step);
                }
                return match;
            }

            /// <summary>
            /// Find the first occurrences of a string, consuming part of the string.
            /// </summary>
            private bool Accept(char[] condition)
            {
                int step = 0;
                int strLen = InputAsArray.Length;
                int currentStep = CurrentIndex;
                char currentChar;

                // Remove whitespace
                while (currentStep < strLen &&
                  ((currentChar = InputAsArray[currentStep]) == ' ' ||
                  currentChar == '\r' ||
                  currentChar == '\t' ||
                  currentChar == '\n'))
                {
                    ++step;
                    ++currentStep;
                }

                bool strMatch = true;
                for (int i = 0; currentStep < strLen && i < condition.Length; ++i, ++currentStep)
                    if (InputAsArray[currentStep] != condition[i])
                    {
                        strMatch = false;
                        break;
                    }

                bool match = currentStep < strLen && strMatch;
                if (match)
                    Skip(step + condition.Length);
                return match;
            }
        }

        /// <summary>
        /// Parses a JSON-text as defined in http://tools.ietf.org/html/rfc4627, returning an
        /// IDictionary&lt;string, object&gt; or an IList&lt;object&gt; depending on whether
        /// the value was an array or dictionary. Nested objects also match these types.
        /// </summary>
        public static object Parse(string input)
        {
            input = input.Trim();
            JsonStringParser parser = new JsonStringParser(input);

            if ((parser.ParseObject(out object output) ||
                parser.ParseArray(out output)) &&
                parser.CurrentIndex == input.Length)
                return output;
            throw new ArgumentException("Input JSON was invalid.");
        }

        /// <summary>
        /// Encodes a dictionary into a JSON string. Supports values that are
        /// IDictionary&lt;string, object&gt;, IList&lt;object&gt;, strings,
        /// nulls, and any of the primitive types.
        /// </summary>
        public static string Encode(IDictionary<string, object> dict)
        {
            if (dict == null)
                throw new ArgumentNullException();
            if (dict.Count == 0)
                return "{}";
            StringBuilder builder = new StringBuilder("{");
            foreach (KeyValuePair<string, object> pair in dict)
            {
                builder.Append(Encode(pair.Key));
                builder.Append(":");
                builder.Append(Encode(pair.Value));
                builder.Append(",");
            }
            builder[builder.Length - 1] = '}';
            return builder.ToString();
        }

        /// <summary>
        /// Encodes a list into a JSON string. Supports values that are
        /// IDictionary&lt;string, object&gt;, IList&lt;object&gt;, strings,
        /// nulls, and any of the primitive types.
        /// </summary>
        public static string Encode(IList<object> list)
        {
            if (list == null)
                throw new ArgumentNullException();
            if (list.Count == 0)
                return "[]";
            StringBuilder builder = new StringBuilder("[");
            foreach (object item in list)
            {
                builder.Append(Encode(item));
                builder.Append(",");
            }
            builder[builder.Length - 1] = ']';
            return builder.ToString();
        }

        /// <summary>
        /// Encodes an object into a JSON string.
        /// </summary>
        public static string Encode(object obj)
        {
            if (obj is IDictionary<string, object> dict)
                return Encode(dict);
            if (obj is IList<object> list)
                return Encode(list);
            if (obj is string str)
            {
                str = escapePattern.Replace(str, m =>
                {
                    switch (m.Value[0])
                    {
                        case '\\':
                            return "\\\\";
                        case '\"':
                            return "\\\"";
                        case '\b':
                            return "\\b";
                        case '\f':
                            return "\\f";
                        case '\n':
                            return "\\n";
                        case '\r':
                            return "\\r";
                        case '\t':
                            return "\\t";
                        default:
                            return "\\u" + ((ushort) m.Value[0]).ToString("x4");
                    }
                });
                return "\"" + str + "\"";
            }
            if (obj is null)
                return "null";
            if (obj is bool)
                return (bool) obj ? "true" : "false";
            if (!obj.GetType().GetTypeInfo().IsPrimitive)
                throw new ArgumentException("Unable to encode objects of type " + obj.GetType());
            return Convert.ToString(obj, CultureInfo.InvariantCulture);
        }
    }
}

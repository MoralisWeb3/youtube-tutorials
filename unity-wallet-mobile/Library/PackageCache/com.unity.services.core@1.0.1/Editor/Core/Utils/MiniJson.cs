/*
 * Copyright (c) 2013 Calvin Rien
 *
 * Based on the JSON parser by Patrick van Bergen
 * http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
 *
 * Simplified it so that it doesn't throw exceptions
 * and can be used in Unity iPhone with maximum code stripping.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

// Modified MiniJSON:
// * Aligned to Unity's standards.
// * Use C# 7 and 8 functionalities to simplify.
// * Fix float/double/decimal parsing/serialization to be culture invariant and use the advised format
//   (see: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#the-round-trip-r-format-specifier).
// * Use invariant culture for serialization when possible.
// * Limit unnecessary allocations.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    /// JSON uses Arrays and Objects. These correspond here to the data types IList and IDictionary.
    /// All numbers are parsed to doubles.
    /// </summary>
    /// <example>
    /// <code>
    /// using UnityEngine;
    /// using System.Collections;
    /// using System.Collections.Generic;
    /// using MiniJSON;
    ///
    /// public class MiniJSONTest : MonoBehaviour
    /// {
    ///     void Start ()
    ///     {
    ///         var jsonString = "{ \"array\": [1.44,2,3], " +
    ///                          "\"object\": {\"key1\":\"value1\", \"key2\":256}, " +
    ///                          "\"string\": \"The quick brown fox \\\"jumps\\\" over the lazy dog \", " +
    ///                          "\"unicode\": \"\\u3041 Men\u00fa sesi\u00f3n\", " +
    ///                          "\"int\": 65536, " +
    ///                          "\"float\": 3.1415926, " +
    ///                          "\"bool\": true, " +
    ///                          "\"null\": null }";
    ///
    ///         var dict = Json.Deserialize(jsonString) as Dictionary&lt;string,object&gt;;
    ///
    ///         Debug.Log("deserialized: " + dict.GetType());
    ///         Debug.Log("dict['array'][0]: " + ((List&lt;object&gt;) dict["array"])[0]);
    ///         Debug.Log("dict['string']: " + (string) dict["string"]);
    ///         Debug.Log("dict['float']: " + (double) dict["float"]); // floats come out as doubles
    ///         Debug.Log("dict['int']: " + (long) dict["int"]); // ints come out as longs
    ///         Debug.Log("dict['unicode']: " + (string) dict["unicode"]);
    ///
    ///         var str = Json.Serialize(dict);
    ///
    ///         Debug.Log("serialized: " + str);
    ///     }
    /// }
    /// </code>
    /// </example>
    static class MiniJson
    {
        /// <summary>
        /// Parse the string json into a value
        /// </summary>
        /// <param name="json">
        /// A JSON string.
        /// </param>
        /// <returns>
        /// Return a List&lt;object&gt;, a Dictionary&lt;string, object&gt;,
        /// a double, an integer, a string, null, true, or false.
        /// </returns>
        public static object Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return Parser.Parse(json);
        }

        static class Parser
        {
            enum Token
            {
                None,
                CurlyOpen,
                CurlyClose,
                SquaredOpen,
                SquaredClose,
                Colon,
                Comma,
                String,
                Number,
                True,
                False,
                Null
            }

            const string k_WordBreak = "{}[],:\"";

            public static object Parse(string jsonString)
            {
                var wordBuilder = new StringBuilder();
                var stringBuilder = new StringBuilder();
                StringReader reader;
                using (reader = new StringReader(jsonString))
                {
                    return ParseValue();
                }

                object ParseValue()
                {
                    var nextToken = GetNextToken();
                    return ParseByToken(nextToken);
                }

                Token GetNextToken()
                {
                    EatWhitespace();

                    if (IsReaderEmpty())
                        return Token.None;

                    switch (PeekNextChar())
                    {
                        case '{':
                        {
                            return Token.CurlyOpen;
                        }
                        case '}':
                        {
                            reader.Read();
                            return Token.CurlyClose;
                        }
                        case '[':
                        {
                            return Token.SquaredOpen;
                        }
                        case ']':
                        {
                            reader.Read();
                            return Token.SquaredClose;
                        }
                        case ',':
                        {
                            reader.Read();
                            return Token.Comma;
                        }
                        case '"':
                        {
                            return Token.String;
                        }
                        case ':':
                        {
                            return Token.Colon;
                        }
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case '-':
                        {
                            return Token.Number;
                        }
                    }

                    switch (ReadNextWord())
                    {
                        case "false":
                        {
                            return Token.False;
                        }
                        case "true":
                        {
                            return Token.True;
                        }
                        case "null":
                        {
                            return Token.Null;
                        }
                    }

                    return Token.None;
                }

                void EatWhitespace()
                {
                    while (!IsReaderEmpty()
                           && char.IsWhiteSpace(PeekNextChar()))
                    {
                        reader.Read();
                    }
                }

                char PeekNextChar() => Convert.ToChar(reader.Peek());

                string ReadNextWord()
                {
                    wordBuilder.Clear();

                    while (!IsWordBreak(PeekNextChar()))
                    {
                        wordBuilder.Append(ReadNextChar());

                        if (IsReaderEmpty())
                            break;
                    }

                    return wordBuilder.ToString();
                }

                bool IsWordBreak(char c) => char.IsWhiteSpace(c) || k_WordBreak.IndexOf(c) != -1;

                char ReadNextChar() => Convert.ToChar(reader.Read());

                bool IsReaderEmpty() => reader.Peek() == -1;

                object ParseByToken(Token token)
                {
                    switch (token)
                    {
                        case Token.String:
                        {
                            return ParseString();
                        }
                        case Token.Number:
                        {
                            return ParseNumber();
                        }
                        case Token.CurlyOpen:
                        {
                            return ParseObject();
                        }
                        case Token.SquaredOpen:
                        {
                            return ParseArray();
                        }
                        case Token.True:
                        {
                            return true;
                        }
                        case Token.False:
                        {
                            return false;
                        }
                        case Token.Null:
                        {
                            return null;
                        }
                        default:
                        {
                            return null;
                        }
                    }
                }

                string ParseString()
                {
                    stringBuilder.Clear();

                    // ditch opening quote
                    reader.Read();

                    var parsing = true;
                    while (!IsReaderEmpty()
                           && parsing)
                    {
                        var nextChar = ReadNextChar();
                        switch (nextChar)
                        {
                            case '"':
                            {
                                parsing = false;
                                break;
                            }
                            case '\\':
                            {
                                if (IsReaderEmpty())
                                {
                                    parsing = false;
                                }
                                else
                                {
                                    ParseEscapedChar();
                                }

                                break;
                            }

                            default:
                            {
                                stringBuilder.Append(nextChar);
                                break;
                            }
                        }
                    }

                    return stringBuilder.ToString();
                }

                void ParseEscapedChar()
                {
                    var escapedChar = ReadNextChar();
                    switch (escapedChar)
                    {
                        case '"':
                        case '\\':
                        case '/':
                        {
                            stringBuilder.Append(escapedChar);
                            break;
                        }
                        case 'b':
                        {
                            stringBuilder.Append('\b');
                            break;
                        }
                        case 'f':
                        {
                            stringBuilder.Append('\f');
                            break;
                        }
                        case 'n':
                        {
                            stringBuilder.Append('\n');
                            break;
                        }
                        case 'r':
                        {
                            stringBuilder.Append('\r');
                            break;
                        }
                        case 't':
                        {
                            stringBuilder.Append('\t');
                            break;
                        }
                        case 'u':
                        {
                            var hex = new char[4];
                            for (var i = 0; i < 4; i++)
                            {
                                hex[i] = ReadNextChar();
                            }

                            stringBuilder.Append((char)Convert.ToInt32(new string(hex), 16));
                            break;
                        }
                    }
                }

                object ParseNumber()
                {
                    var number = ReadNextWord();

                    if (number.IndexOf('.') == -1)
                    {
                        long.TryParse(number, out var parsedInt);
                        return parsedInt;
                    }

                    double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedDouble);
                    return parsedDouble;
                }

                Dictionary<string, object> ParseObject()
                {
                    var table = new Dictionary<string, object>();

                    // ditch opening brace
                    reader.Read();

                    // {
                    while (true)
                    {
                        var nextToken = GetNextToken();
                        switch (nextToken)
                        {
                            case Token.None:
                            {
                                return null;
                            }
                            case Token.Comma:
                            {
                                continue;
                            }
                            case Token.CurlyClose:
                            {
                                return table;
                            }
                            default:
                            {
                                // name
                                var name = ParseString();
                                if (name == null)
                                    return null;

                                // :
                                nextToken = GetNextToken();
                                if (nextToken != Token.Colon)
                                    return null;

                                // ditch the colon
                                reader.Read();

                                // value
                                table[name] = ParseValue();
                                break;
                            }
                        }
                    }
                }

                List<object> ParseArray()
                {
                    var array = new List<object>();

                    // ditch opening bracket
                    reader.Read();

                    // [
                    var parsing = true;
                    while (parsing)
                    {
                        var nextToken = GetNextToken();

                        switch (nextToken)
                        {
                            case Token.None:
                            {
                                return null;
                            }
                            case Token.Comma:
                            {
                                continue;
                            }
                            case Token.SquaredClose:
                            {
                                parsing = false;
                                break;
                            }
                            default:
                            {
                                var value = ParseByToken(nextToken);
                                array.Add(value);
                                break;
                            }
                        }
                    }

                    return array;
                }
            }
        }

        /// <summary>
        /// Convert a IDictionary / IList object or a simple type (string, int, etc.) into a JSON string.
        /// </summary>
        /// <param name="obj">
        /// A Dictionary&lt;string, object&gt; / List&lt;object&gt; to serialize.
        /// </param>
        /// <returns>
        /// Return a JSON encoded string, or null if object 'json' is not serializable.
        /// </returns>
        public static string Serialize(object obj)
        {
            var builder = new StringBuilder();
            SerializeValue(obj);
            return builder.ToString();

            void SerializeValue(object value)
            {
                switch (value)
                {
                    case null:
                    {
                        builder.Append("null");
                        break;
                    }
                    case string stringValue:
                    {
                        SerializeString(stringValue);
                        break;
                    }
                    case bool boolValue:
                    {
                        builder.Append(boolValue ? "true" : "false");
                        break;
                    }
                    case IList listValue:
                    {
                        SerializeArray(listValue);
                        break;
                    }
                    case IDictionary dictionaryValue:
                    {
                        SerializeObject(dictionaryValue);
                        break;
                    }
                    case char charValue:
                    {
                        SerializeString(new string(charValue, 1));
                        break;
                    }
                    default:
                    {
                        SerializeOther(value);
                        break;
                    }
                }
            }

            void SerializeObject(IDictionary value)
            {
                var first = true;

                builder.Append('{');

                foreach (var entry in value.Keys)
                {
                    if (!first)
                    {
                        builder.Append(',');
                    }

                    SerializeString(entry.ToString());
                    builder.Append(':');

                    SerializeValue(value[entry]);

                    first = false;
                }

                builder.Append('}');
            }

            void SerializeArray(IList value)
            {
                builder.Append('[');

                var first = true;

                foreach (var item in value)
                {
                    if (!first)
                    {
                        builder.Append(',');
                    }

                    SerializeValue(item);

                    first = false;
                }

                builder.Append(']');
            }

            void SerializeString(string value)
            {
                builder.Append('\"');

                var charArray = value.ToCharArray();
                foreach (var c in charArray)
                {
                    SerializeChar(c);
                }

                builder.Append('\"');
            }

            void SerializeChar(char c)
            {
                switch (c)
                {
                    case '"':
                    {
                        builder.Append("\\\"");
                        break;
                    }
                    case '\\':
                    {
                        builder.Append("\\\\");
                        break;
                    }
                    case '\b':
                    {
                        builder.Append("\\b");
                        break;
                    }
                    case '\f':
                    {
                        builder.Append("\\f");
                        break;
                    }
                    case '\n':
                    {
                        builder.Append("\\n");
                        break;
                    }
                    case '\r':
                    {
                        builder.Append("\\r");
                        break;
                    }
                    case '\t':
                    {
                        builder.Append("\\t");
                        break;
                    }
                    default:
                    {
                        var codepoint = Convert.ToInt32(c);
                        if (codepoint >= 32 && codepoint <= 126)
                        {
                            builder.Append(c);
                        }
                        else
                        {
                            builder.Append($"\\u{codepoint.ToString("x4")}");
                        }

                        break;
                    }
                }
            }

            void SerializeOther(object value)
            {
                switch (value)
                {
                    // NOTE: decimals lose precision during serialization.
                    // They always have, I'm just letting you know.
                    // Previously floats and doubles lost precision too.
                    case float floatValue:
                        builder.Append(floatValue.ToString("G9", CultureInfo.InvariantCulture));
                        break;

                    case int _:
                    case uint _:
                    case long _:
                    case sbyte _:
                    case byte _:
                    case short _:
                    case ushort _:
                    case ulong _:
                        builder.Append(value);
                        break;

                    case double _:
                    case decimal _:
                        builder.Append(Convert.ToDouble(value).ToString("G17", CultureInfo.InvariantCulture));
                        break;

                    case IConvertible convertibleValue:
                        SerializeString(convertibleValue.ToString(CultureInfo.InvariantCulture));
                        break;

                    default:
                        SerializeString(value.ToString());
                        break;
                }
            }
        }
    }
}

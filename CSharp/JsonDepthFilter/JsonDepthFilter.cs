using System;
using System.IO;
using System.Text;

namespace JsonDepthFilter;

public static class JsonDepthFilter
{
    public static string FilterJsonContent(string json, int maxDepth)
    {
        if (json is null) throw new ArgumentNullException(nameof(json));
        int index = 0;
        string? result = ParseValue(json, ref index, 0, maxDepth);
        return result ?? string.Empty;
    }

    public static string FilterJsonFile(string path, int maxDepth)
    {
        if (path is null) throw new ArgumentNullException(nameof(path));
        string json = File.ReadAllText(path);
        return FilterJsonContent(json, maxDepth);
    }

    private static string? ParseValue(string json, ref int index, int depth, int maxDepth)
    {
        SkipWhitespace(json, ref index);
        if (depth > maxDepth)
        {
            SkipToken(json, ref index);
            return null;
        }

        if (index >= json.Length) return null;
        char c = json[index];
        return c switch
        {
            '{' => ParseObject(json, ref index, depth, maxDepth),
            '[' => ParseArray(json, ref index, depth, maxDepth),
            '"' => ParseString(json, ref index),
            _ => ParsePrimitive(json, ref index)
        };
    }

    private static string ParseObject(string json, ref int index, int depth, int maxDepth)
    {
        var sb = new StringBuilder();
        sb.Append('{');
        index++; // skip '{'
        bool first = true;
        while (true)
        {
            SkipWhitespace(json, ref index);
            if (index >= json.Length) break;
            if (json[index] == '}')
            {
                index++;
                break;
            }

            string key = ParseString(json, ref index);
            SkipWhitespace(json, ref index);
            if (index >= json.Length || json[index] != ':') break;
            index++; // skip ':'
            string? value = ParseValue(json, ref index, depth + 1, maxDepth);
            if (value != null)
            {
                if (!first) sb.Append(',');
                sb.Append(key).Append(':').Append(value);
                first = false;
            }
            SkipWhitespace(json, ref index);
            if (index < json.Length && json[index] == ',')
            {
                index++; // skip comma and continue
                continue;
            }
        }
        sb.Append('}');
        return sb.ToString();
    }

    private static string ParseArray(string json, ref int index, int depth, int maxDepth)
    {
        var sb = new StringBuilder();
        sb.Append('[');
        index++; // skip '['
        bool first = true;
        while (true)
        {
            SkipWhitespace(json, ref index);
            if (index >= json.Length) break;
            if (json[index] == ']')
            {
                index++;
                break;
            }
            string? value = ParseValue(json, ref index, depth + 1, maxDepth);
            if (value != null)
            {
                if (!first) sb.Append(',');
                sb.Append(value);
                first = false;
            }
            SkipWhitespace(json, ref index);
            if (index < json.Length && json[index] == ',')
            {
                index++;
                continue;
            }
        }
        sb.Append(']');
        return sb.ToString();
    }

    private static string ParseString(string json, ref int index)
    {
        var sb = new StringBuilder();
        bool escape = false;
        sb.Append('"');
        index++; // skip opening quote
        while (index < json.Length)
        {
            char c = json[index++];
            sb.Append(c);
            if (escape)
            {
                escape = false;
            }
            else if (c == '\\')
            {
                escape = true;
            }
            else if (c == '"')
            {
                break;
            }
        }
        return sb.ToString();
    }

    private static string ParsePrimitive(string json, ref int index)
    {
        int start = index;
        while (index < json.Length && !IsStopChar(json[index]))
        {
            index++;
        }
        return json[start..index];
    }

    private static void SkipToken(string json, ref int index)
    {
        SkipWhitespace(json, ref index);
        if (index >= json.Length) return;
        char c = json[index];
        if (c == '{')
        {
            index++;
            while (true)
            {
                SkipWhitespace(json, ref index);
                if (index >= json.Length) return;
                if (json[index] == '}')
                {
                    index++;
                    return;
                }
                SkipString(json, ref index);
                SkipWhitespace(json, ref index);
                if (index < json.Length && json[index] == ':') index++;
                SkipToken(json, ref index);
                SkipWhitespace(json, ref index);
                if (index < json.Length && json[index] == ',')
                {
                    index++;
                    continue;
                }
            }
        }
        else if (c == '[')
        {
            index++;
            while (true)
            {
                SkipWhitespace(json, ref index);
                if (index >= json.Length) return;
                if (json[index] == ']')
                {
                    index++;
                    return;
                }
                SkipToken(json, ref index);
                SkipWhitespace(json, ref index);
                if (index < json.Length && json[index] == ',')
                {
                    index++;
                    continue;
                }
            }
        }
        else if (c == '"')
        {
            SkipString(json, ref index);
        }
        else
        {
            while (index < json.Length && !IsStopChar(json[index]))
            {
                index++;
            }
        }
    }

    private static void SkipString(string json, ref int index)
    {
        bool escape = false;
        index++; // skip opening quote
        while (index < json.Length)
        {
            char c = json[index++];
            if (escape)
            {
                escape = false;
            }
            else if (c == '\\')
            {
                escape = true;
            }
            else if (c == '"')
            {
                break;
            }
        }
    }

    private static void SkipWhitespace(string json, ref int index)
    {
        while (index < json.Length && char.IsWhiteSpace(json[index]))
        {
            index++;
        }
    }

    private static bool IsStopChar(char c) => c == ',' || c == '}' || c == ']' || char.IsWhiteSpace(c);
}


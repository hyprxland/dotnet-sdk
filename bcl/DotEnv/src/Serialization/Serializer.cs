using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Hyprx.DotEnv.Documents;
using Hyprx.DotEnv.Tokens;

namespace Hyprx.DotEnv.Serialization;

internal static class Serializer
{
    public static void SerializeDictionary(
        IEnumerable<KeyValuePair<string, string>> dictionary,
        TextWriter writer,
        DotEnvSerializerOptions? options = null)
    {
        var first = true;
        foreach (var item in dictionary)
        {
            if (!first)
                writer.WriteLine();
            else
                first = false;

            writer.Write(item.Key);
            writer.Write('=');
            writer.Write('"');
            writer.Write(item.Value);
            writer.Write('"');
        }
    }

    public static void SerializeDictionary(
        IEnumerable<KeyValuePair<string, object?>> dictionary,
        TextWriter writer,
        DotEnvSerializerOptions? options = null)
    {
        bool first = true;
        foreach (var item in dictionary)
        {
            if (!first)
            {
                writer.WriteLine();
            }
            else
            {
                first = false;
            }

            writer.Write(item.Key);
            writer.Write('=');
            writer.Write('"');
            writer.Write(item.Value);
            writer.Write('"');
        }
    }

    public static void SerializeDocument(
        DotEnvDocument document,
        TextWriter writer,
        DotEnvSerializerOptions? options = null)
    {
        var first = true;
        foreach (var item in document)
        {
            switch (item)
            {
                case DotEnvComment comment:
                    if (first)
                        first = false;
                    else
                        writer.WriteLine();
                    writer.Write("# ");
                    writer.Write(comment.RawValue);
                    break;

                case DotEnvEntry pair:
                    if (first)
                        first = false;
                    else
                        writer.WriteLine();
                    var quote = pair.Value.Contains("\"") ? '\'' : '"';
                    writer.Write(pair.Name);
                    writer.Write('=');
                    writer.Write(quote);
                    writer.Write(pair.Value);
                    writer.Write(quote);
                    break;

                case DotEnvEmptyLine _:
                    writer.WriteLine();
                    break;

                default:
                    throw new NotSupportedException($"The type {item.GetType()} is not supported.");
            }
        }
    }

    public static DotEnvDocument DeserializeDocument(ReadOnlySpan<char> value, DotEnvSerializerOptions? options = null)
    {
        using var sr = new StringReader(value.AsString());
        return DeserializeDocument(sr, options);
    }

    public static DotEnvDocument DeserializeDocument(string value, DotEnvSerializerOptions? options = null)
    {
        using var sr = new StringReader(value);
        return DeserializeDocument(sr, options);
    }

    public static DotEnvDocument DeserializeDocument(Stream value, DotEnvSerializerOptions? options = null)
    {
        using var sr = new StreamReader(value, Encoding.UTF8, true, -1, true);
        return DeserializeDocument(sr, options);
    }

    public static DotEnvDocument DeserializeDocument(TextReader reader, DotEnvSerializerOptions? options = null)
    {
        options ??= new DotEnvSerializerOptions();
        var r = new DotEnvReader(reader, options);
        var doc = new DotEnvDocument();
        string? key = null;

        while (r.Read())
        {
            switch (r.Current)
            {
                case EnvCommentToken commentToken:
                    doc.Add(new DotEnvComment(commentToken.RawValue));
                    continue;

                case EnvNameToken nameToken:
                    key = nameToken.Value;
                    continue;

                case EnvScalarToken scalarToken:
                    if (key is not null && key.Length > 0)
                    {
                        if (doc.TryGetNameValuePair(key, out var entry) && entry is not null)
                        {
                            entry.RawValue = scalarToken.RawValue;
                            key = null;
                            continue;
                        }

                        doc.Add(key, scalarToken.RawValue);
                        key = null;
                        continue;
                    }

                    throw new InvalidOperationException("Scalar token found without a name token before it.");
            }
        }

        bool expand = options.Expand;

        if (expand)
        {
            Func<string, string?> getVariable = (name) => Env.Get(name);
            if (options.ExpandVariables is not null)
            {
                var ev = options.ExpandVariables;
                getVariable = (name) =>
                {
                    if (doc.TryGetValue(name, out var value))
                        return value;

                    if (ev.TryGetValue(name, out value))
                        return value;

                    value = Env.Get(name);

                    return value;
                };
            }

            var eso = new EnvExpandOptions()
            {
                UnixAssignment = false,
                UnixCustomErrorMessage = false,
                GetVariable = getVariable,
                SetVariable = (name, value) => Env.Set(name, value),
            };
            foreach (var entry in doc)
            {
                if (entry is DotEnvEntry pair)
                {
                    var v = Env.Expand(pair.RawValue, eso);

                    // Only set the value if it has changed.
                    if (v.Length != pair.RawValue.Length || !v.SequenceEqual(pair.RawValue))
                        pair.SetRawValue(v);
                }
            }
        }

        return doc;
    }

    public static IDictionary DeserializeDictionary(DotEnvDocument document, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type)
    {
        if (!type.IsAssignableFrom(type))
            throw new ArgumentException("Type must be assignable from IDictionary<string, string>.", nameof(type));

        var obj = Activator.CreateInstance(type);
        if (obj is null)
            throw new ArgumentException("Type must be instantiable.", nameof(type));

        var dict = (IDictionary)obj;
        foreach (var (name, value) in document.AsNameValuePairEnumerator())
            dict.Add(name, value);

        return dict;
    }

#if NET6_0_OR_GREATER

    public static T DeserializeDictionary<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(DotEnvDocument document)
        where T : IDictionary<string, string?>
    {
        var type = typeof(T);
        var obj = Activator.CreateInstance<T>();
        if (obj is null)
            throw new InvalidOperationException($"Type {type.FullName} must be instantiable.");

        if (obj is not IDictionary<string, string?> dict)
            throw new InvalidOperationException($"Type {type.FullName} must be assignable from IDictionary<string, string>.");

        foreach (var (name, value) in document.AsNameValuePairEnumerator())
            dict.Add(name, value);

        return (T)dict;
    }

#else

    public static T DeserializeDictionary<T>(DotEnvDocument document)
        where T : IDictionary<string, string?>
    {
        var type = typeof(T);
        var obj = Activator.CreateInstance<T>();
        if (obj is null)
            throw new InvalidOperationException($"Type {type.FullName} must be instantiable.");

        if (obj is not IDictionary<string, string?> dict)
            throw new InvalidOperationException($"Type {type.FullName} must be assignable from IDictionary<string, string>.");

        foreach (var (name, value) in document.AsNameValuePairEnumerator())
            dict.Add(name, value);

        return (T)dict;
    }

#endif
}
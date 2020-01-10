using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace IxMilia.Classics.ProcessFile
{
    class FileProcessor
    {
        private string[] _textFiles;
        private string[] _glossFiles;
        private int _commonWordCount;
        private string _rawDictionaryPath;
        private int _currentLine;

        public FileProcessor(string[] textFiles, string[] glossFiles, int commonWordCount, string rawDictionaryPath)
        {
            _textFiles = textFiles;
            _glossFiles = glossFiles;
            _commonWordCount = commonWordCount;
            _rawDictionaryPath = rawDictionaryPath;
            _currentLine = 1;
        }

        public void Process(string outputPath)
        {
            var definedWords = new Dictionary<string, (int, string, string)>();
            var undefinedKeys = new HashSet<string>();
            var emptyGloss = new Dictionary<int, Gloss>();
            var latin = LatinDictionary.LoadDictionary(_rawDictionaryPath);

            for (int i = 0; i < _textFiles.Length; i++)
            {
                // prepare text content
                var textFile = _textFiles[i];
                var document = XDocument.Load(textFile).Root;
                var textContent = document.Element("text").Element("body").Element("div1");
                var bookNumber = int.Parse(textContent.Attribute("n").Value);
                var elements = textContent.Elements();

                // prepare glosses
                var glossFile = _glossFiles[i];
                var lineGlosses = new Dictionary<int, Dictionary<int, Gloss>>();
                if (File.Exists(glossFile))
                {
                    var gloss = XDocument.Load(glossFile).Root;
                    foreach (var l in gloss.Elements("l"))
                    {
                        var lineNumber = int.Parse(l.Attribute("n").Value);
                        var glossValues = l.Elements("g").Select(g => new Gloss(int.Parse(g.Attribute("o").Value), int.Parse(g.Attribute("l").Value), g.Attribute("k").Value)).ToDictionary(g => g.Offset, g => g);
                        lineGlosses.Add(lineNumber, glossValues);
                    }
                }

                // go
                var content = new StringBuilder();
                var jsObject = new StringBuilder();
                jsObject.AppendLine("var data = {");
                var macro = "lline";
                foreach (var element in elements)
                {
                    AssertCurrentLineNumber(element, bookNumber);
                    var glosses = lineGlosses.ContainsKey(_currentLine)
                        ? lineGlosses[_currentLine]
                        : emptyGloss;
                    switch (element.Name.LocalName)
                    {
                        case "milestone":
                            macro = "pline";
                            break;
                        case "l":
                            var lineText = element.Value;
                            
                            jsObject.AppendLine($"        \"{_currentLine}\": {{");
                            jsObject.AppendLine($"            \"text\": \"{EscapeStringToJs(lineText)}\",");
                            jsObject.AppendLine($"            \"glosses\": {{");
                            WriteGlossToJs(jsObject, latin, glosses.Values.OrderBy(g => g.Offset));
                            jsObject.AppendLine($"            }},");
                            jsObject.AppendLine($"        }},");

                            content.Append($@"\{macro}{{");
                            macro = "lline";
                            for (int j = 0; j < lineText.Length; j++)
                            {
                                if (glosses.TryGetValue(j, out var gloss))
                                {
                                    var glossed = lineText.Substring(gloss.Offset, gloss.Length);
                                    if (latin.TryGetValue(gloss.Key, out var  defined))
                                    {
                                        var (count, entry, definition) = definedWords.ContainsKey(gloss.Key)
                                            ? definedWords[gloss.Key]
                                            : (0, defined.Entry, defined.Definition);
                                        definedWords[gloss.Key] = (count + 1, entry, definition);
                                        content.Append($@"\agls{{{EscapeStringToLatex(gloss.Key)}}}{{{EscapeStringToLatex(glossed)}}}");
                                    }
                                    else
                                    {
                                        content.Append(EscapeStringToLatex(glossed));
                                        undefinedKeys.Add(gloss.Key);
                                    }

                                    j = gloss.Offset + gloss.Length - 1;
                                }
                                else
                                {
                                    AppendCharacterToLatex(content, lineText[j]);
                                }
                            }

                            content.AppendLine("}");
                            _currentLine++;
                            break;
                    }
                }

                jsObject.AppendLine("    };");

                var htmlContent = new StringBuilder();
                htmlContent.AppendLine("<html>");
                htmlContent.AppendLine($@"
<head>
  <script type='text/javascript'>
    {jsObject.ToString()}
    var colors = [
        '#FF7777',
        '#77FF77',
        '#7777FF',
    ];

    function loadLine() {{
        var lineNumber = document.getElementById('lineNo').value;
        var lineData = data[lineNumber];
        var content = '';
        var colorIndex = 0;
        var close = -1;
        for (var i = 0; i < lineData.text.length; i++) {{
            if (i == close) {{
                content += '</span>';
            }}

            var gloss = lineData.glosses[i];
            if (gloss) {{
                close = i + gloss.length;
                var color = colors[colorIndex++];
                if (colorIndex >= colors.length) {{
                    colorIndex = 0;
                }}
                content += ""<span style='background: "" + color + ""'>"";
            }}

            content += lineData.text.substr(i, 1);
        }}

        document.getElementById('output').innerHTML = content;
        setGlosses(lineData.glosses);
    }}

    function setGlosses(glosses) {{
        var content = '';
        var colorIndex = 0;
        for (var gloss in glosses) {{
            gloss = glosses[gloss];
            var color = colors[colorIndex++];
            if (colorIndex >= colors.length) {{
                colorIndex = 0;
            }}
            content += ""<p style='background: "" + color + ""'>"";
            content += gloss.key + ' / ' + gloss.definition;
            content += '</p>';
        }}

        document.getElementById('glosses').innerHTML = content;
    }}

  </script>
</head>
<body>
    Line number:
    <input id='lineNo' />
    <button onclick='loadLine()'>Load line</button>
    <div id='output'></div>
    <div id='glosses'></div>
</body>
");
                htmlContent.AppendLine("</html>");

                File.WriteAllText(Path.Combine(outputPath, $"content{bookNumber}.tex"), content.ToString());
                File.WriteAllText(Path.Combine(outputPath, $"content{bookNumber}.html"), htmlContent.ToString());
            }

            // sort common/uncommon words
            var ordered = definedWords.OrderByDescending(d => d.Value.Item1).ToList();
            var commonWords = ordered.Take(_commonWordCount).Select(d => new KeyValuePair<string, Tuple<string, string>>(d.Key, Tuple.Create(d.Value.Item2, d.Value.Item3)));
            var uncommonWords = ordered.Skip(_commonWordCount).Select(d => new KeyValuePair<string, Tuple<string, string>>(d.Key, Tuple.Create(d.Value.Item2, d.Value.Item3)));

            File.WriteAllText(Path.Combine(outputPath, "commonwords.tex"), string.Join("\r\n", commonWords.OrderBy(kvp => kvp.Key).Select(kvp => $@"\newcommonterm{{{EscapeStringToLatex(kvp.Key)}}}{{{EscapeStringToLatex(kvp.Value.Item1)}}}{{{EscapeStringToLatex(kvp.Value.Item2)}}}")));
            File.WriteAllText(Path.Combine(outputPath, "uncommonwords.tex"), string.Join("\r\n", uncommonWords.OrderBy(kvp => kvp.Key).Select(kvp => $@"\newuncommonterm{{{EscapeStringToLatex(kvp.Key)}}}{{{EscapeStringToLatex(kvp.Value.Item1)}}}{{{EscapeStringToLatex(kvp.Value.Item2)}}}")));
            File.WriteAllText(Path.Combine(outputPath, "undefinedKeys.txt"), string.Join("\r\n", undefinedKeys.OrderBy(x => x)));
        }

        private void AssertCurrentLineNumber(XElement element, int bookNumber)
        {
            var natt = element.Attribute("n");
            if (natt != null)
            {
                var line = int.Parse(natt.Value);
                if (line != _currentLine)
                {
                    Console.WriteLine($"Expected current line to be {line} but was {_currentLine} while processing book {bookNumber}.");
                }
            }
        }

        private static void AppendCharacterToLatex(StringBuilder sb, char c)
        {
            switch (c)
            {
                case '_':
                    sb.Append(' ');
                    break;
                case '<':
                    sb.Append(@"\textless{}");
                    break;
                case '>':
                    sb.Append(@"\textgreater{}");
                    break;
                case '\\':
                    sb.Append(@"\textbackslash{}");
                    break;
                case '%':
                case '{':
                case '}':
                    sb.Append('\\');
                    sb.Append(c);
                    break;
                case '\u201C': // left double quote
                    sb.Append("``");
                    break;
                case '\u201D': // right double quote
                    sb.Append("''");
                    break;
                default:
                    sb.Append(c);
                    break;
            }
        }

        private static string EscapeStringToLatex(string str)
        {
            var sb = new StringBuilder();
            var seenStartQuote = false;
            foreach (var c in str)
            {
                switch (c)
                {
                    case '"':
                        sb.Append(seenStartQuote ? "''" : "``");
                        seenStartQuote = !seenStartQuote;
                        break;
                    default:
                        AppendCharacterToLatex(sb, c);
                        break;
                }
            }

            return sb.ToString();
        }

        private static void WriteGlossToJs(StringBuilder sb, Dictionary<string, DictionaryEntry> dict, IEnumerable<Gloss> glosses)
        {
            foreach (var gloss in glosses)
            {
                var definition = dict.ContainsKey(gloss.Key)
                    ? dict[gloss.Key].Entry
                    : "UNDEFINED KEY: " + gloss.Key;
                sb.AppendLine($"                \"{gloss.Offset}\": {{");
                sb.AppendLine($"                    \"length\": {gloss.Length},");
                sb.AppendLine($"                    \"key\": \"{EscapeStringToJs(gloss.Key)}\",");
                sb.AppendLine($"                    \"definition\": \"{EscapeStringToJs(definition)}\",");
                sb.AppendLine("                },");
            }
        }

        private static string EscapeStringToJs(string str)
        {
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                switch (c)
                {
                    case '"':
                    case '\\':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        private struct Gloss
        {
            public int Offset { get; }
            public int Length { get; }
            public string Key { get; }

            public Gloss(int offset, int length, string key)
            {
                Offset = offset;
                Length = length;
                Key = key;
            }
        }
    }
}

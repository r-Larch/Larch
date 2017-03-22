using System;
using System.Collections.Generic;
using System.IO;


namespace Larch.Host.Parser {
    public class HostsFile {
        private static readonly string DefaultHostsFilePath = Environment.ExpandEnvironmentVariables(@"%WINDIR%\System32\drivers\etc\hosts");
        public readonly string FilePath;

        public HostsFile() {
            FilePath = DefaultHostsFilePath;
        }

        public HostsFile(string filePath) {
            FilePath = filePath;
        }


        public IEnumerable<HostsFileLine> GetHosts() {
            var lineNum = 0;
            foreach (var line in GetLines()) {
                lineNum++;
                if (line == null) continue;

                var fileLine = new HostsFileLine(lineNum);
                fileLine.Parse(line);
                if (!fileLine.IsCommentarLine) {
                    yield return fileLine;
                }
            }
        }

        private IEnumerable<string> GetLines() {
            using (var fs = File.Open(FilePath, FileMode.Open, FileAccess.Read)) {
                using (var sr = new StreamReader(fs)) {
                    while (!sr.EndOfStream) {
                        var line = sr.ReadLine()?.Trim();
                        yield return line;
                    }
                }
            }
        }

        public static string CreateTextLine(IFileLine line) {
            if (line.IsCommentarLine) {
                line.IsDisabled = false;
            }

            var sb = new StringBuilder();
            if (line.IsDisabled) {
                sb.Append("# ");
            }
            if (!line.IsCommentarLine) {
                sb.Append(line.Ip?.Trim());
                sb.Append(" ");
                sb.Append(line.Domain?.Trim());
            }
            if (!string.IsNullOrEmpty(line.Commentar)) {
                if (!line.IsCommentarLine) {
                    sb.Append(" ");
                }
                sb.Append("#");
                sb.Append(" ");
                sb.Append(line.Commentar.Trim());
            }
            return sb.ToString();
        }

        public string Append(FileLine fileLine) {
            var line = CreateTextLine(fileLine);
            using (var file = File.Open(FilePath, FileMode.Append, FileAccess.Write)) {
                using (var fw = new StreamWriter(file)) {
                    fw.WriteLine(line);
                }
            }

            return line;
        }
    }
}
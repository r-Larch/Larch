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

        public void AppendLine(string line) {
            using (var file = File.Open(FilePath, FileMode.Append, FileAccess.Write)) {
                using (var fw = new StreamWriter(file)) {
                    fw.WriteLine(line);
                }
            }
        }
    }
}
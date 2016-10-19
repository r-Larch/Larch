using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Host {
    internal class Buffer {
        private readonly List<List<char>> _buffer;
        public LineEndig LineEndig { get; set; }

        public bool HasChanged { get; private set; }
        public int LongestLineLenght { get; private set; }


        #region ctor

        public Buffer(List<List<char>> buffer, int longestLineLenght) {
            _buffer = buffer;
            LongestLineLenght = longestLineLenght;
            HasChanged = true;
        }

        #endregion ctor


        public static Buffer FromString(string str) {
            var buffer = new List<List<char>>();
            var longestLineLenght = 0;
            foreach (var lineStr in str.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None)) {
                var line = new List<char>();

                if (lineStr != null) {
                    line = lineStr.ToCharArray().ToList();

                    if (line.Count > longestLineLenght) {
                        longestLineLenght = line.Count;
                    }
                }

                buffer.Add(line);
            }

            return new Buffer(buffer, longestLineLenght);
        }

        public static Buffer FromStream(FileStream fs) {
            string str;
            using (var sr = new StreamReader(fs)) {
                str = sr.ReadToEnd();
            }
            return FromString(str);
        }


        #region public

        public int Size() {
            var lineEndigLen = GetLineEndig().Length;
            return _buffer.Sum(line => line.Count + lineEndigLen);
        }


        public IEnumerable<char> GetBuffer() {
            return GetBuffer(0, _buffer.Count);
        }

        public IEnumerable<char> GetBufferForRect(Rect rect) {
            var index = rect.X;
            var count = rect.Height;

            if (index < 0) {
                index = 0;
            } else if (index > _buffer.Count) {
                index = _buffer.Count;
            }

            if (count < 0) {
                count = 0;
            } else if (count > _buffer.Count) {
                count = _buffer.Count;
            }

            return GetBuffer(index, count);
        }

        public IEnumerable<char> GetBuffer(int index, int count) {
            var lineEndig = GetLineEndig();
            foreach (var line in _buffer.GetRange(index, count)) {
                foreach (var c in line) {
                    yield return c;
                }
                foreach (var c in lineEndig) {
                    yield return c;
                }
            }
        }

        #endregion public


        #region private

        private string GetLineEndig() {
            switch (LineEndig) {
                default:
                case LineEndig.Crlf:
                    return "\r\n";
                case LineEndig.Cr:
                    return "\r";
                case LineEndig.Lf:
                    return "\n";
            }
        }

        #endregion private


        public void ResetChangedFlag() {
            HasChanged = false;
        }
    }


    public enum LineEndig {
        Crlf = 0,
        Cr = 1,
        Lf = 2
    }
}
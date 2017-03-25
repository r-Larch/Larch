using System;
using System.Collections.Generic;
using System.Linq;


namespace Larch.Host.Contoller {
    public class ConsoleWriter {
        public int LinesCount;
        private readonly List<IEnumerable<char>> _buffer;
        private AvailableColors _colors;

        public ConsoleWriter() {
            _buffer = new List<IEnumerable<char>>();
        }

        public ConsoleWriter Write(string str) {
            _buffer.Add(str);
            return this;
        }

        public ConsoleWriter Write<T>(Match<T> match, int formatLeng) {
            var leng = formatLeng - match.Value.Length;
            Write(match);
            if (leng > 0) {
                _buffer.Add(Enumerable.Repeat(' ', leng));
            }
            return this;
        }

        public ConsoleWriter Write<T>(Match<T> match) {
            _colors = new AvailableColors();
            _buffer.Add(RenderFilter(match));
            return this;
        }

        private IEnumerable<char> RenderFilter<T>(Match<T> filter) {
            var chars = filter.Value.ToCharArray();
            var c = Console.ForegroundColor;
            for (var i = 0; i < chars.Length; i++) {
                var part = filter.Matches.LastOrDefault(x => x.IsMatch && i >= x.Start && i < x.Stop);
                Console.ForegroundColor = part != null ? _colors.GetColor(part.Num) : c;
                yield return chars[i];
            }

            Console.ForegroundColor = c;
        }

        public static ConsoleWriter Create(string str) {
            return new ConsoleWriter().Write(str);
        }

        public void Flush(bool newLine = true) {
            foreach (var sign in _buffer.SelectMany(x => x)) {
                Console.Write(sign);
            }

            if (newLine) {
                Console.WriteLine();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;


namespace Host {
    internal class ConsoleEx {
        private readonly Rect _baseViewRect;

        public ConsoleEx(Rect viewRect) {
            _baseViewRect = viewRect;
        }

        public void Curser(int x, int y) {
            Console.SetCursorPosition(x, y);
        }

        public void Clear() {
            Console.Clear();
        }

        public Rect GetVisibleRect() {
            var curserX = Console.CursorLeft;
            var curserY = Console.CursorTop;

            return new Rect() {
                X = _baseViewRect.X + curserX,
                Y = _baseViewRect.X + curserY,
                Width = _baseViewRect.Width,
                Height = _baseViewRect.Height
            };
        }

        public void Write(Buffer buffer) {
            var rect = GetVisibleRect();
            var charBuffer = buffer.GetBufferForRect(rect).ToArray();
            Clear();
            Console.Out.Write(charBuffer, 0, charBuffer.Length);
        }

    }
}
using System;
using System.IO;
using System.Threading;


namespace Host {
    public class Editor : IDisposable{
        private const int Fps = 30;

        private readonly string _file;
        private readonly Rect _positionRect;
        private Buffer _buffer;
        private Painter _painter;
        private ConsoleEx _console;

        public Editor(string file, int x, int y, int width, int height) {
            _file = file;
            _positionRect = new Rect() {
                X = x,
                Y = y,
                Width = width,
                Height = height
            };

            _console = new ConsoleEx(_positionRect);
        }

        public void Init() {
            using (var fs = File.Open(_file, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                _buffer = Buffer.FromStream(fs);
            }

            _painter = new Painter(_buffer, Fps, _console);
            _painter.StartPainting();

            while (true) {
                Thread.Sleep(10000);
            }
        }

        public void Dispose() {
            _painter.Dispose();
            _buffer = null;
        }
    }
}
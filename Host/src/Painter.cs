using System;
using System.Threading;


namespace Host {
    public class Painter : IDisposable {
        private readonly Buffer _buffer;
        private readonly int _fps;
        private readonly ConsoleEx _console;
        private Timer _timer;

        internal Painter(Buffer buffer, int fps, ConsoleEx console) {
            _buffer = buffer;
            _fps = fps;
            _console = console;
        }

        public void StartPainting() {
            long ticks = 1000/_fps;
            _timer = new Timer(Paint, null, 0, ticks);
        }

        private void Paint(object state) {
            if (_buffer.HasChanged) {
                _console.Write(_buffer);
                _buffer.ResetChangedFlag();
            }
        }


        public void Dispose() {
            _timer?.Dispose();
        }
    }
}
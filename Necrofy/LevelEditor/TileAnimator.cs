using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Necrofy
{
    class TileAnimator {
        private const double MillisecondsPerFrame = 1000.0 / 60.0;

        public event EventHandler Animated;

        private readonly Dictionary<int, Entry> entries;
        private CancellationTokenSource animationCancel = new CancellationTokenSource();
        public bool Running { get; private set; }

        public TileAnimator() {
            entries = new Dictionary<int, Entry>();
        }

        public TileAnimator(LoadedLevel level, TileAnimLevelMonster levelMonster) {
            entries = new Dictionary<int, Entry>();
            foreach (TileAnimLevelMonster.Entry e in levelMonster.entries) {
                if (e.frames.Count > 0) {
                    entries[e.initialTile] = new Entry(level, e);
                }
            }
        }

        public void ProcessTile(int bgTile, int x, int y, int gfxTile) {
            if (entries.TryGetValue(gfxTile, out Entry entry)) {
                entry.AddLocation(bgTile, x, y);
            }
        }

        public void Run() {
            if (entries.Count == 0) {
                return;
            }

            animationCancel.Cancel();
            animationCancel = new CancellationTokenSource();
            RunAsync(animationCancel.Token);
            Running = true;
        }

        public void Pause() {
            animationCancel.Cancel();
            animationCancel = new CancellationTokenSource();
            Running = false;
        }
        
        public void Advance() {
            if (entries.Values.All(e => e.Done)) {
                return;
            }
            while (!RunFrame()) { }
            Animated?.Invoke(this, EventArgs.Empty);
        }

        public void Restart() {
            foreach (Entry e in entries.Values) {
                e.Restart();
            }
            Animated?.Invoke(this, EventArgs.Empty);
        }

        private async void RunAsync(CancellationToken cancellationToken) {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            double prevFrameMS = 0;
            
            while (!cancellationToken.IsCancellationRequested) {
                while (timer.Elapsed.TotalMilliseconds >= prevFrameMS + MillisecondsPerFrame) {
                    if (RunFrame()) {
                        Animated?.Invoke(this, EventArgs.Empty);
                    }
                    prevFrameMS = Math.Max(timer.Elapsed.TotalMilliseconds - 1000, prevFrameMS + MillisecondsPerFrame);
                }
                await Task.Delay(1);
            }
        }

        private bool RunFrame() {
            bool changed = false;
            foreach (Entry e in entries.Values) {
                changed |= e.RunFrame();
            }
            return changed;
        }

        private class Entry
        {
            private readonly LoadedLevel level;
            private readonly TileAnimLevelMonster.Entry entry;
            private readonly List<Location> locations = new List<Location>();

            private int curAnimationFrame = 0;
            private int curFrame = 0;

            public bool Done => curAnimationFrame < 0;

            public Entry(LoadedLevel level, TileAnimLevelMonster.Entry entry) {
                this.level = level;
                this.entry = entry;
            }

            public void AddLocation(int bgTile, int x, int y) {
                locations.Add(new Location(bgTile, x, y));
            }

            public bool RunFrame() {
                if (Done) {
                    return false;
                }

                curFrame++;
                if (curFrame >= entry.frames[curAnimationFrame].delay) {
                    RenderFrame();
                    curAnimationFrame++;
                    if (curAnimationFrame >= entry.frames.Count) {
                        curAnimationFrame = entry.loop ? 0 : -1;
                    }
                    curFrame = 0;
                    return true;
                }
                return false;
            }

            public void Restart() {
                curAnimationFrame = 0;
                curFrame = 0;
                RenderFrame();
            }

            private void RenderFrame() {
                ushort tile = curAnimationFrame == 0 && curFrame == 0 ? entry.initialTile : entry.frames[curAnimationFrame].tile;
                foreach (Location location in locations) {
                    Bitmap image = level.tiles[location.bgTile];
                    BitmapData data = image.LockBits(new Rectangle(location.x * 8, location.y * 8, 8, 8), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                    SNESGraphics.DrawTile(data, 0, 0, new LoadedTilemap.Tile(level.tilemap.tiles[location.bgTile][location.x, location.y], tile), level.graphics.linearGraphics);
                    image.UnlockBits(data);
                }
            }

            private class Location
            {
                public readonly int bgTile;
                public readonly int x;
                public readonly int y;

                public Location(int bgTile, int x, int y) {
                    this.bgTile = bgTile;
                    this.x = x;
                    this.y = y;
                }
            }
        }
    }
}

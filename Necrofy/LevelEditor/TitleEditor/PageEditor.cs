using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Necrofy
{
    partial class PageEditor : UserControl, ObjectSelector<WrappedTitleWord>.IHost
    {
        private static readonly Brush textSelectionBrush = new SolidBrush(Color.FromArgb(128, SystemColors.Highlight));

        private TitleEditor titleEditor;
        public TitlePage page;
        private LoadedLevelTitleCharacters loadedCharacters;
        private List<WrappedTitleWord> wrappedWords = new List<WrappedTitleWord>();

        private MouseMode mouseMode = MouseMode.None;
        private bool mouseDown = false;

        private readonly ObjectSelector<WrappedTitleWord> objectSelector;

        private WrappedTitleWord hoveredWord = null;
        private WrappedTitleWord textEditWord = null;

        public event EventHandler SelectedWordsChanged;
        private HashSet<WrappedTitleWord> _selectedWords = new HashSet<WrappedTitleWord>();
        public HashSet<WrappedTitleWord> SelectedWords {
            get {
                return _selectedWords;
            }
            private set {
                _selectedWords = value;
                SelectedWordsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public IEnumerable<WrappedTitleWord> AllWords => wrappedWords;

        private CancellationTokenSource caretBlinkCancel = new CancellationTokenSource();
        private bool caretBlinkOn = false;

        private int TextSelectionStart { get; set; } = -1;
        private int _textSelectionEnd = -1;
        private int TextSelectionEnd {
            get {
                return _textSelectionEnd;
            }
            set {
                caretBlinkCancel.Cancel();
                caretBlinkCancel = new CancellationTokenSource();
                _textSelectionEnd = value;
                if (value >= 0) {
                    DoCaretBlink(caretBlinkCancel.Token);
                } else {
                    Invalidate();
                }
            }
        }

        private enum MouseMode
        {
            None,
            MoveWord,
            Text,
        }

        private static readonly Dictionary<MouseMode, Cursor> mouseModeToCursor = new Dictionary<MouseMode, Cursor>() {
            { MouseMode.None, Cursors.Default },
            { MouseMode.MoveWord, Cursors.SizeAll },
            { MouseMode.Text, Cursors.IBeam }
        };

        public PageEditor() {
            InitializeComponent();
            LostFocus += PageEditor_LostFocus;
            objectSelector = new ObjectSelector<WrappedTitleWord>(this, positionStep: 8, width: Width - 8, height: Height - 8);
        }

        public void LoadPage(TitleEditor titleEditor, TitlePage originalPage, LoadedLevelTitleCharacters loadedCharacters) {
            this.titleEditor = titleEditor;
            page = originalPage.JsonClone();
            this.loadedCharacters = loadedCharacters;
            wrappedWords = page.words.Select(w => new WrappedTitleWord(w, loadedCharacters)).ToList();
            Invalidate();
        }

        public void SelectAll() {
            objectSelector.SelectAll();
        }

        public void SelectNone() {
            objectSelector.SelectNone();
            TextSelectionStart = -1;
            TextSelectionEnd = -1;
        }

        private async void DoCaretBlink(CancellationToken cancellationToken) {
            caretBlinkOn = false;
            while (!cancellationToken.IsCancellationRequested) {
                caretBlinkOn = !caretBlinkOn;
                Invalidate();
                await Task.Delay(SystemInformation.CaretBlinkTime);
            }
        }

        private int GetCaretPosition(int x) {
            if (x < textEditWord.CharXPositions[0]) {
                return 0;
            }
            for (int i = 0; i < textEditWord.CharXPositions.Count - 1; i++) {
                if (x >= textEditWord.CharXPositions[i] && x < textEditWord.CharXPositions[i + 1]) {
                    if (x < (textEditWord.CharXPositions[i] + textEditWord.CharXPositions[i + 1]) / 2) {
                        return i;
                    } else {
                        return i + 1;
                    }
                }
            }
            return textEditWord.CharXPositions.Count - 1;
        }

        private void PageEditor_Paint(object sender, PaintEventArgs e) {
            foreach (WrappedTitleWord word in wrappedWords) {
                word.Render(e.Graphics);
                if (SelectedWords.Contains(word) && word != textEditWord) {
                    e.Graphics.DrawRectangleProper(Pens.White, word.VisibleBounds);
                } else if (word == hoveredWord) {
                    e.Graphics.DrawRectangleProper(Pens.Gray, word.VisibleBounds);
                }
                if (word == textEditWord) {
                    if (TextSelectionEnd != TextSelectionStart) {
                        int startX = word.CharXPositions[Math.Min(TextSelectionStart, TextSelectionEnd)];
                        int endX = word.CharXPositions[Math.Max(TextSelectionStart, TextSelectionEnd)];
                        e.Graphics.FillRectangle(textSelectionBrush, startX, word.VisibleBounds.Y, endX - startX, word.VisibleBounds.Height);
                    }
                    if (caretBlinkOn) {
                        int width = Math.Max(1, SystemInformation.CaretWidth);
                        e.Graphics.FillRectangle(Brushes.White, word.CharXPositions[TextSelectionEnd] - width / 2, word.VisibleBounds.Y, width, word.VisibleBounds.Height);
                    }
                }
            }

            objectSelector.DrawSelectionRectangle(e.Graphics);
        }

        private void PageEditor_MouseMove(object sender, MouseEventArgs e) {
            if (mouseDown) {
                if (mouseMode == MouseMode.Text) {
                    TextSelectionEnd = GetCaretPosition(e.X);
                } else {
                    objectSelector.MouseMove(e.X, e.Y);
                }
            } else {
                MouseMode prevMouseMode = mouseMode;
                WrappedTitleWord prevHoveredWord = hoveredWord;

                hoveredWord = wrappedWords.Where(w => w.MoveBounds.Contains(e.Location)).Reverse().OrderBy(w => e.Location.DistanceFrom(w.VisibleBounds)).FirstOrDefault();

                mouseMode = MouseMode.None;
                if (hoveredWord != null) {
                    if (hoveredWord.TextBounds.Contains(e.Location)) {
                        mouseMode = MouseMode.Text;
                    } else {
                        mouseMode = MouseMode.MoveWord;
                    }
                }

                if (mouseMode != prevMouseMode || hoveredWord != prevHoveredWord) {
                    Cursor = mouseModeToCursor[mouseMode];
                    Invalidate();
                }
            }
        }

        private void PageEditor_MouseDown(object sender, MouseEventArgs e) {
            mouseDown = true;

            textEditWord = null;
            if (mouseMode == MouseMode.Text) {
                objectSelector.SelectNone();
                SelectedWords = new HashSet<WrappedTitleWord>() { hoveredWord };
                textEditWord = hoveredWord;
                TextSelectionStart = GetCaretPosition(e.X);
            } else {
                TextSelectionStart = -1;
                foreach (WrappedTitleWord word in wrappedWords) {
                    word.UseExtendedBounds(word == hoveredWord);
                }
                objectSelector.MouseDown(e.X, e.Y);
            }
            TextSelectionEnd = TextSelectionStart;

            Invalidate();
        }

        private void PageEditor_MouseUp(object sender, MouseEventArgs e) {
            mouseDown = false;
            if (mouseMode != MouseMode.Text) {
                objectSelector.MouseUp();
            }
            PageEditor_MouseMove(sender, e);
            titleEditor.undoManager.ForceNoMerge();
            Invalidate();
        }

        private void PageEditor_MouseLeave(object sender, EventArgs e) {
            hoveredWord = null;
            Invalidate();
        }

        private void PageEditor_LostFocus(object sender, EventArgs e) {
            textEditWord = null;
            Invalidate();
        }

        public IEnumerable<WrappedTitleWord> GetObjects() {
            return wrappedWords;
        }

        public void SelectionChanged() {
            SelectedWords = objectSelector.GetSelectedObjects();
            Invalidate();
        }

        public void MoveSelectedObjects(int dx, int dy, int snap) {
            titleEditor.undoManager.Do(new MoveWordAction(SelectedWords, dx / 8, dy / 8));
        }

        public WrappedTitleWord CreateObject(int x, int y) {
            throw new NotImplementedException();
        }

        public IEnumerable<WrappedTitleWord> CloneSelection() {
            throw new NotImplementedException();
        }
    }
}

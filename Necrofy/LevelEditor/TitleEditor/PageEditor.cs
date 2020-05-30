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
                if (!_selectedWords.SetEquals(value)) {
                    _selectedWords = value;
                    SelectedWordsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public IEnumerable<WrappedTitleWord> AllWords => wrappedWords;

        private CancellationTokenSource caretBlinkCancel = new CancellationTokenSource();
        private bool caretBlinkOn = false;

        private int _textSelectionStart = -1;
        private int TextSelectionStart {
            get {
                return _textSelectionStart;
            }
            set {
                _textSelectionStart = value;
                Invalidate();
            }
        }
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

        public int CaretPosition {
            get {
                return TextSelectionEnd;
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
            if (textEditWord == null) {
                objectSelector.SelectAll();
            } else {
                TextSelectionStart = 0;
                TextSelectionEnd = textEditWord.Chars.Count;
            }
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

        public void SetCaretPosition(WrappedTitleWord word, int position) {
            if (word == textEditWord) {
                TextSelectionStart = position;
                TextSelectionEnd = position;
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

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (IsDisposed) return;

            foreach (WrappedTitleWord word in wrappedWords) {
                word.Render(e.Graphics);
                if (SelectedWords.Contains(word) && word != textEditWord) {
                    e.Graphics.DrawRectangleProper(Pens.White, word.VisibleBounds);
                } else if (word == hoveredWord) {
                    e.Graphics.DrawRectangleProper(Pens.Gray, word.VisibleBounds);
                }
                if (word == textEditWord && Focused) {
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

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
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

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            mouseDown = true;

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
        
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            mouseDown = false;
            if (mouseMode != MouseMode.Text) {
                objectSelector.MouseUp();
            }
            OnMouseMove(e);
            titleEditor.undoManager.ForceNoMerge();
            Invalidate();
        }
        
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            hoveredWord = null;
            Invalidate();
        }
        
        protected override void OnDoubleClick(EventArgs e) {
            base.OnDoubleClick(e);
            if (textEditWord != null) {
                int mouseX = PointToClient(MousePosition).X;

                int i;
                for (i = 0; i < textEditWord.CharXPositions.Count - 1; i++) {
                    if (mouseX >= textEditWord.CharXPositions[i] && mouseX < textEditWord.CharXPositions[i + 1]) {
                        break;
                    }
                }

                TextSelectionStart = FindWordBoundary(i + 1, false);
                TextSelectionEnd = FindWordBoundary(i, true);
            }
        }

        protected override void OnEnter(EventArgs e) {
            base.OnEnter(e);
            Invalidate();
        }

        protected override void OnLeave(EventArgs e) {
            base.OnLeave(e);
            Invalidate();
        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            DoTextEdit(newChars => {
                char c = char.ToLowerInvariant(e.KeyChar);
                if (TitlePage.ValidChars.Contains(c)) {
                    DeleteSelectedChars(newChars);

                    int bestIndex = TextSelectionEnd;
                    string bestString = "";
                    for (int i = TextSelectionEnd; i >= 0; i--) {
                        if (TitlePage.BytesToString(newChars, i, TextSelectionEnd, false, out string s) && TitlePage.StringToBytesMap.ContainsKey(s + c)) {
                            bestIndex = i;
                            bestString = s + c;
                        }
                    }
                    TextSelectionEnd += UpdateSubstring(newChars, bestIndex, TextSelectionEnd, bestString);
                }
            });
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            Keys code = keyData & Keys.KeyCode;
            if (code == Keys.Left || code == Keys.Right) {
                if (textEditWord != null) {
                    if (TextSelectionStart != TextSelectionEnd && !keyData.HasFlag(Keys.Shift)) {
                        if (code == Keys.Right) {
                            TextSelectionStart = Math.Max(TextSelectionStart, TextSelectionEnd);
                        } else {
                            TextSelectionStart = Math.Min(TextSelectionStart, TextSelectionEnd);
                        }
                        TextSelectionEnd = TextSelectionStart;
                    } else {
                        if (keyData.HasFlag(Keys.Control)) {
                            TextSelectionEnd = FindWordBoundary(TextSelectionEnd, code == Keys.Right);
                        } else {
                            TextSelectionEnd = Math.Max(0, Math.Min(textEditWord.Chars.Count, TextSelectionEnd + (code == Keys.Right ? 1 : -1)));
                        }
                        if (!keyData.HasFlag(Keys.Shift)) {
                            TextSelectionStart = TextSelectionEnd;
                        }
                    }
                    titleEditor.undoManager.ForceNoMerge();
                }
                return true;
            } else if (code == Keys.Up || code == Keys.Down) {
                return true;
            } else if (code == Keys.Delete) {
                DoTextEdit(newChars => {
                    if (TextSelectionStart == TextSelectionEnd) {
                        if (keyData.HasFlag(Keys.Control)) {
                            UpdateSubstring(newChars, TextSelectionEnd, FindWordBoundary(TextSelectionEnd, true), "");
                        } else {
                            int i;
                            string s = "";
                            for (i = TextSelectionEnd + 1; i <= textEditWord.Chars.Count; i++) {
                                if (TitlePage.BytesToString(newChars, TextSelectionEnd, i, false, out s)) {
                                    break;
                                }
                            }

                            if (i <= textEditWord.Chars.Count) {
                                UpdateSubstring(newChars, TextSelectionEnd, i, s.Substring(1));
                            } else if (TextSelectionEnd < textEditWord.Chars.Count) {
                                newChars.RemoveAt(TextSelectionEnd);
                            }
                        }
                    } else {
                        DeleteSelectedChars(newChars);
                    }
                });
                return true;
            } else if (code == Keys.Back) {
                DoTextEdit(newChars => {
                    if (TextSelectionStart == TextSelectionEnd) {
                        if (keyData.HasFlag(Keys.Control)) {
                            TextSelectionEnd += UpdateSubstring(newChars, FindWordBoundary(TextSelectionEnd, false), TextSelectionEnd, "");
                        } else {
                            int i;
                            string s = "";
                            for (i = TextSelectionEnd - 1; i >= 0; i--) {
                                if (TitlePage.BytesToString(newChars, i, TextSelectionEnd, false, out s)) {
                                    break;
                                }
                            }

                            if (i >= 0) {
                                TextSelectionEnd += UpdateSubstring(newChars, i, TextSelectionEnd, s.Substring(0, s.Length - 1));
                            } else if (TextSelectionEnd > 0) {
                                newChars.RemoveAt(TextSelectionEnd - 1);
                                TextSelectionEnd--;
                            }
                        }
                    } else {
                        DeleteSelectedChars(newChars);
                    }
                });
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void CharPressed(byte c) {
            DoTextEdit(newChars => {
                DeleteSelectedChars(newChars);
                newChars.Insert(TextSelectionEnd, c);
                TextSelectionEnd++;
                TextSelectionStart = TextSelectionEnd;
                SelectAfterDelay(); // Trying to select right away doesn't work, so add some delay
            });
        }

        private async void SelectAfterDelay() {
            await Task.Delay(1);
            Select();
        }

        private void DoTextEdit(Action<List<byte>> action) {
            if (textEditWord != null) {
                List<byte> newChars = new List<byte>(textEditWord.Chars);
                int oldCaretPosition = CaretPosition;

                action(newChars);

                if (!newChars.SequenceEqual(textEditWord.Chars)) {
                    titleEditor.undoManager.Do(new ChangeWordTextAction(this, textEditWord, newChars, oldCaretPosition));
                }
            }
        }

        private static int UpdateSubstring(List<byte> newChars, int start, int end, string newString) {
            List<byte> newBytes = TitlePage.StringToBytes(newString);
            newChars.RemoveRange(start, end - start);
            newChars.InsertRange(start, newBytes);

            return newBytes.Count - (end - start);
        }

        private void DeleteSelectedChars(List<byte> newChars) {
            int start = Math.Min(TextSelectionStart, TextSelectionEnd);
            newChars.RemoveRange(start, Math.Abs(TextSelectionEnd - TextSelectionStart));
            TextSelectionEnd = start;
        }

        private int FindWordBoundary(int position, bool forward) {
            int direction = forward ? 1 : -1;
            int i;
            for (i = position + direction - 1; i >= 0 && i < textEditWord.Chars.Count; i += direction) {
                if (textEditWord.Chars[i] == TitlePage.SpaceCharValue) {
                    break;
                }
            }
            return Math.Max(0, Math.Min(textEditWord.Chars.Count, i + 1));
        }

        public IEnumerable<WrappedTitleWord> GetObjects() {
            return wrappedWords;
        }

        public void SelectionChanged() {
            textEditWord = null;
            SelectedWords = new HashSet<WrappedTitleWord>(objectSelector.GetSelectedObjects());
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

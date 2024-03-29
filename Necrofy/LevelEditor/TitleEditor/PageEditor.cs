﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Necrofy
{
    partial class PageEditor : UserControl, ObjectSelector<WrappedTitleWord>.IHost {
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
        public HashSet<WrappedTitleWord> SelectedWords {
            get {
                return objectSelector.GetSelectedObjects();
            }
        }

        public IEnumerable<WrappedTitleWord> SelectableWords => wrappedWords.Where(w => w.Selectable);

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
                SelectedWordsChanged?.Invoke(this, EventArgs.Empty);
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
                SelectedWordsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int CaretPosition {
            get {
                return TextSelectionEnd;
            }
        }

        private enum MouseMode {
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
            objectSelector = new ObjectSelector<WrappedTitleWord>(this, positionStep: 8, maxX: SNESGraphics.ScreenWidth - 8, maxY: SNESGraphics.ScreenHeight - 8);
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

        private int GetCharacterPosition(int x) {
            if (x < textEditWord.CharXPositions[0]) {
                return 0;
            }
            for (int i = 0; i < textEditWord.CharXPositions.Count - 1; i++) {
                if (x >= textEditWord.CharXPositions[i] && x < textEditWord.CharXPositions[i + 1]) {
                    return i;
                }
            }
            return textEditWord.CharXPositions.Count - 1;
        }

        private void GetWordStartAndEnd(int x, out int start, out int end) {
            int charPosition = GetCharacterPosition(x);
            start = FindWordBoundary(charPosition + 1, false);
            end = FindWordBoundary(charPosition, true);
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
                        e.Graphics.FillRectangle(Brushes.White, word.CharXPositions[TextSelectionEnd], word.VisibleBounds.Y, width, word.VisibleBounds.Height);
                    }
                }
            }

            objectSelector.DrawSelectionRectangle(e.Graphics);
        }

        // Used to ignore mouse move events that are generated by setting the property grid objects
        private Point prevMousePosition = new Point(int.MinValue, int.MinValue);

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (e.Location == prevMousePosition) {
                return;
            }
            prevMousePosition = e.Location;
            DoMouseMove(e);
        }

        private bool wordSelect = false;
        private int wordSelectStartWordStart;
        private int wordSelectStartWordEnd;

        private void DoMouseMove(MouseEventArgs e) {
            if (mouseDown) {
                if (mouseMode == MouseMode.Text) {
                    if (wordSelect) {
                        GetWordStartAndEnd(e.X, out int wordStart, out int wordEnd);
                        if (wordStart < wordSelectStartWordStart) {
                            TextSelectionStart = wordSelectStartWordEnd;
                            TextSelectionEnd = wordStart;
                        } else {
                            TextSelectionStart = wordSelectStartWordStart;
                            TextSelectionEnd = wordEnd;
                        }
                    } else {
                        TextSelectionEnd = GetCaretPosition(e.X);
                    }
                } else {
                    objectSelector.MouseMove(e.X, e.Y);
                }
            } else {
                MouseMode prevMouseMode = mouseMode;
                WrappedTitleWord prevHoveredWord = hoveredWord;

                hoveredWord = SelectableWords.Where(w => w.MoveBounds.Contains(e.Location)).Reverse().OrderBy(w => e.Location.DistanceFrom(w.VisibleBounds)).FirstOrDefault();

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

        private Stopwatch doubleClickTimer = new Stopwatch();
        private Point prevClickLocation = Point.Empty;

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            mouseDown = true;

            if (mouseMode == MouseMode.Text) {
                bool shiftSelect = textEditWord == hoveredWord && ModifierKeys.HasFlag(Keys.Shift);
                textEditWord = hoveredWord;
                keepTextEditWord = true;
                objectSelector.SelectObjects(new[] { hoveredWord });

                wordSelect = doubleClickTimer.IsRunning
                    && doubleClickTimer.ElapsedMilliseconds <= SystemInformation.DoubleClickTime
                    && Math.Abs(e.X - prevClickLocation.X) <= SystemInformation.DoubleClickSize.Width / 2
                    && Math.Abs(e.Y - prevClickLocation.Y) <= SystemInformation.DoubleClickSize.Height / 2;
                if (wordSelect) {
                    GetWordStartAndEnd(e.X, out wordSelectStartWordStart, out wordSelectStartWordEnd);
                    TextSelectionStart = wordSelectStartWordStart;
                    TextSelectionEnd = wordSelectStartWordEnd;
                    doubleClickTimer.Stop();
                } else {
                    TextSelectionEnd = GetCaretPosition(e.X);
                    if (!shiftSelect) {
                        TextSelectionStart = TextSelectionEnd;
                    }
                    doubleClickTimer.Restart();
                }
            } else {
                TextSelectionStart = -1;
                TextSelectionEnd = -1;
                hoveredWord?.UseExtendedBounds(true);
                objectSelector.MouseDown(e.X, e.Y);
                hoveredWord?.UseExtendedBounds(false);
            }
            prevClickLocation = e.Location;
        }

        private bool allowNextMerge = false;

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            mouseDown = false;
            if (mouseMode != MouseMode.Text) {
                objectSelector.MouseUp();
            }
            DoMouseMove(e);
            if (!allowNextMerge) {
                titleEditor.undoManager.ForceNoMerge();
            }
            allowNextMerge = false;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            hoveredWord = null;
            Invalidate();
        }
        
        protected override void OnEnter(EventArgs e) {
            base.OnEnter(e);
            Invalidate();
        }

        protected override void OnLeave(EventArgs e) {
            base.OnLeave(e);
            prevMousePosition = new Point(int.MinValue, int.MinValue);
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
                if (textEditWord == null) {
                    objectSelector.KeyDown(keyData);
                } else {
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
                if (textEditWord == null) {
                    objectSelector.KeyDown(keyData);
                }
                return true;
            } else if (code == Keys.Home || code == Keys.End) {
                if (textEditWord != null) {
                    TextSelectionEnd = code == Keys.Home ? 0 : textEditWord.CharXPositions.Count - 1;
                    if (!keyData.HasFlag(Keys.Shift)) {
                        TextSelectionStart = TextSelectionEnd;
                    }
                }
                return true;
            } else if (code == Keys.Delete) {
                bool processed = DoTextEdit(newChars => {
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
                if (processed) {
                    return true;
                }
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

        private bool DoTextEdit(Action<List<byte>> action) {
            if (textEditWord != null) {
                List<byte> newChars = new List<byte>(textEditWord.Chars);
                int oldCaretPosition = CaretPosition;

                action(newChars);

                if (!newChars.SequenceEqual(textEditWord.Chars)) {
                    titleEditor.undoManager.Do(new ChangeWordTextAction(this, textEditWord, newChars, oldCaretPosition));
                }
                return true;
            }
            return false;
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

        public bool CanCopy {
            get {
                if (textEditWord == null) {
                    return SelectedWords.Count > 0;
                } else {
                    return TextSelectionStart != TextSelectionEnd;
                }
            }
        }

        public void Copy() {
            if (textEditWord == null) {
                if (SelectedWords.Count > 0) {
                    Clipboard.SetText(JsonConvert.SerializeObject(SelectedWords.Select(w => w.word)));
                }
            } else {
                if (TextSelectionStart != TextSelectionEnd) {
                    List<byte> chars = new List<byte>();
                    int end = Math.Max(TextSelectionStart, TextSelectionEnd);
                    for (int i = Math.Min(TextSelectionStart, TextSelectionEnd); i < end; i++) {
                        chars.Add(textEditWord.Chars[i]);
                    }
                    Clipboard.SetText(JsonConvert.SerializeObject(chars));
                }
            }
        }

        public void Paste() {
            try {
                List<TitlePage.Word> words = JsonConvert.DeserializeObject<List<TitlePage.Word>>(Clipboard.GetText());
                byte minX = words.Min(w => w.x);
                byte minY = words.Min(w => w.y);
                foreach (TitlePage.Word word in words) {
                    word.x -= minX;
                    word.y -= minY;
                }

                List<WrappedTitleWord> wrappedWords = words.Select(w => new WrappedTitleWord(w, loadedCharacters)).ToList();
                titleEditor.undoManager.Do(new AddWordAdtion(this, wrappedWords));
                objectSelector.SelectObjects(wrappedWords);
                return;
            } catch (Exception) { }

            try {
                List<byte> chars = JsonConvert.DeserializeObject<List<byte>>(Clipboard.GetText());
                bool textEdit = DoTextEdit(newChars => {
                    DeleteSelectedChars(newChars);
                    newChars.InsertRange(TextSelectionEnd, chars);
                    TextSelectionStart += chars.Count;
                    TextSelectionEnd = TextSelectionStart;
                });
                if (!textEdit) {
                    TitlePage.Word word = new TitlePage.Word(0, 0, titleEditor.SelectedPalette);
                    word.chars = chars;
                    WrappedTitleWord[] wrappedWords = new[] { new WrappedTitleWord(word, loadedCharacters) };
                    titleEditor.undoManager.Do(new AddWordAdtion(this, wrappedWords));
                    objectSelector.SelectObjects(wrappedWords);
                }
            } catch (Exception) { }
        }

        public void Delete() {
            List<WrappedTitleWord> words = new List<WrappedTitleWord>(SelectedWords);
            List<int> zIndexes = objectSelector.SortAndGetZIndexes(words);
            titleEditor.undoManager.Do(new RemoveWordAction(this, words, zIndexes));
        }

        public void AddWords(List<WrappedTitleWord> words, List<int> zIndexes = null) {
            for (int i = 0; i < words.Count; i++) {
                if (zIndexes == null) {
                    page.words.Add(words[i].word);
                    wrappedWords.Add(words[i]);
                } else {
                    page.words.Insert(zIndexes[i], words[i].word);
                    wrappedWords.Insert(zIndexes[i], words[i]);
                }
            }
        }

        public void RemoveWords(List<WrappedTitleWord> words, bool updateSelection = true) {
            foreach (WrappedTitleWord word in words) {
                page.words.Remove(word.word);
                wrappedWords.Remove(word);
            }
            if (updateSelection) {
                if (!SelectableWords.Contains(textEditWord)) {
                    textEditWord = null;
                }
                keepTextEditWord = true;
                objectSelector.UpdateSelection();
            }
        }

        public void CenterHorizontally() {
            objectSelector.CenterHorizontally();
            titleEditor.undoManager.ForceNoMerge();
        }

        public void CenterVertically() {
            objectSelector.CenterVertically();
            titleEditor.undoManager.ForceNoMerge();
        }

        public void MoveUp() {
            objectSelector.MoveUp(out List<WrappedTitleWord> words, out List<int> oldZIndexes, out List<int> newZIndexes);
            titleEditor.undoManager.Do(new ChangeWordZIndexAction(this, words, oldZIndexes, newZIndexes));
        }

        public void MoveDown() {
            objectSelector.MoveDown(out List<WrappedTitleWord> words, out List<int> oldZIndexes, out List<int> newZIndexes);
            titleEditor.undoManager.Do(new ChangeWordZIndexAction(this, words, oldZIndexes, newZIndexes));
        }

        public void MoveToFront() {
            objectSelector.MoveToFront(out List<WrappedTitleWord> words, out List<int> oldZIndexes, out List<int> newZIndexes);
            titleEditor.undoManager.Do(new ChangeWordZIndexAction(this, words, oldZIndexes, newZIndexes));
        }

        public void MoveToBack() {
            objectSelector.MoveToBack(out List<WrappedTitleWord> words, out List<int> oldZIndexes, out List<int> newZIndexes);
            titleEditor.undoManager.Do(new ChangeWordZIndexAction(this, words, oldZIndexes, newZIndexes));
        }

        public void ParsePositionChange(string position, bool isX) {
            objectSelector.ParsePositionChange(position, isX);
        }

        public void CharactersUpdated() {
            foreach (WrappedTitleWord word in wrappedWords) {
                word.CalculateBounds();
            }
            Invalidate();
        }

        public IEnumerable<WrappedTitleWord> GetObjects() {
            return wrappedWords;
        }

        private bool keepTextEditWord = false;
        private HashSet<WrappedTitleWord> prevSelectedWords = new HashSet<WrappedTitleWord>();

        public void SelectionChanged() {
            bool textEditWordChanged = false;
            if (textEditWord != null && !keepTextEditWord) {
                textEditWordChanged = true;
                textEditWord = null;
            }
            keepTextEditWord = false;

            Invalidate();
            if (textEditWordChanged || !prevSelectedWords.SetEquals(objectSelector.GetSelectedObjects())) {
                SelectedWordsChanged?.Invoke(this, EventArgs.Empty);
                prevSelectedWords = new HashSet<WrappedTitleWord>(SelectedWords);
            }
        }

        public void MoveSelectedObjects(int dx, int dy, int snap) {
            titleEditor.undoManager.Do(new MoveWordAction(SelectedWords, dx / 8, dy / 8));
        }

        public void SetSelectedObjectsPosition(int? x, int? y) {
            titleEditor.undoManager.Do(new MoveWordAction(SelectedWords, (byte?)x, (byte?)y));
        }

        public WrappedTitleWord CreateObject(int x, int y) {
            TitlePage.Word word = new TitlePage.Word((byte)(x / 8), (byte)Math.Max(0, y / 8 - LoadedLevelTitleCharacters.height / 2), titleEditor.SelectedPalette);
            WrappedTitleWord wrappedWord = new WrappedTitleWord(word, loadedCharacters);
            titleEditor.undoManager.Do(new AddWordAdtion(this, new[] { wrappedWord }));

            mouseMode = MouseMode.Text;
            textEditWord = wrappedWord;
            TextSelectionStart = 0;
            TextSelectionEnd = 0;

            keepTextEditWord = true;
            allowNextMerge = true;
            return wrappedWord;
        }

        public IEnumerable<WrappedTitleWord> CloneSelection() {
            List<WrappedTitleWord> clone = SelectedWords.Select(w => w.word).JsonClone().Select(w => new WrappedTitleWord(w, loadedCharacters)).ToList();
            titleEditor.undoManager.Do(new AddWordAdtion(this, clone));
            return clone;
        }
    }
}

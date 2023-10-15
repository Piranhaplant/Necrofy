using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    partial class PaletteEditor : EditorWindow
    {
        private readonly LoadedPalette palette;
        private readonly Color[] colors;
        private new UndoManager<PaletteEditor> undoManager;

        private int uiUpdate;
        private bool colorSelectorFocused = false;
        
        private Color SelectedColor => colorSelector.SelectionExists ? colorSelector.Colors[colorSelector.SelectionStartIndex] : Color.Black;

        public PaletteEditor(LoadedPalette palette) {
            InitializeComponent();

            Title = palette.paletteName;
            this.palette = palette;
            colors = (Color[])palette.colors.Clone();
            colorSelector.Colors = colors;

            colorSelector.SelectionChanged += ColorSelector_SelectionChanged;
            colorEditor.ColorChanged += ColorEditor_ColorChanged;
            SelectNone();

            Status = "Tip: Colors can be copied into an image editor, then pasted back into Necrofy after making changes.";
        }
        
        public void UpdateColorSelectorSize() {
            colorSelector.Width = (int)(16 * 16 * Zoom);
            colorSelector.Height = colorSelector.Width / 2;
        }

        protected override UndoManager Setup() {
            Zoom = 2.0f;
            undoManager = new UndoManager<PaletteEditor>(mainWindow.UndoButton, mainWindow.RedoButton, this);
            return undoManager;
        }

        public override void Displayed() {
            base.Displayed();
            colorSelector.Focus();
        }

        public override bool HasSelection => true;
        public override bool CanCopy => colorSelectorFocused && colorSelector.SelectionExists;
        public override bool CanPaste => colorSelectorFocused && colorSelector.SelectionExists;
        public override bool CanDelete => colorSelectorFocused && colorSelector.SelectionExists;

        public override void SelectAll() {
            colorSelector.SelectAll();
        }

        public override void SelectNone() {
            colorSelector.SelectionStart = Point.Empty;
            colorSelector.SelectionEnd = Point.Empty;
        }

        public override bool CanZoom => true;

        protected override void ZoomChanged() {
            base.ZoomChanged();
            UpdateColorSelectorSize();
        }

        protected override void DoSave(Project project) {
            palette.colors = (Color[])colors.Clone();
            palette.Save(project);
        }

        private const int ClipboardImageSquareSize = 32;

        public override void Copy() {
            DataObject clipboardData = new DataObject();
            
            Point selectionMin = colorSelector.SelectionMin;
            Point selectionMax = colorSelector.SelectionMax;
            ushort[,] colors = new ushort[selectionMax.X - selectionMin.X + 1, selectionMax.Y - selectionMin.Y + 1];
            for (int y = selectionMin.Y; y <= selectionMax.Y; y++) {
                for (int x = selectionMin.X; x <= selectionMax.X; x++) {
                    colors[x - selectionMin.X, y - selectionMin.Y] = SNESGraphics.RGBToSNES(colorSelector.Colors[colorSelector.PointToIndex(new Point(x, y))]);
                }
            }
            clipboardData.SetText(JsonConvert.SerializeObject(new ClipboardContents(colors)));

            using (Bitmap image = new Bitmap(colors.GetWidth() * ClipboardImageSquareSize, colors.GetHeight() * ClipboardImageSquareSize))
            using (Graphics g = Graphics.FromImage(image)) {
                for (int y = 0; y < colors.GetHeight(); y++) {
                    for (int x = 0; x < colors.GetWidth(); x++) {
                        using (Brush b = new SolidBrush(SNESGraphics.SNESToRGB(colors[x, y]))) {
                            g.FillRectangle(b, x * ClipboardImageSquareSize, y * ClipboardImageSquareSize, ClipboardImageSquareSize, ClipboardImageSquareSize);
                        }
                    }
                }
                clipboardData.SetImage(image);
                Clipboard.SetDataObject(clipboardData, true);
            }
        }

        public override void Paste() {
            try {
                if (Clipboard.ContainsText()) {
                    ushort[,] snesColors = JsonConvert.DeserializeObject<ClipboardContents>(Clipboard.GetText()).colors;
                    if (snesColors == null) {
                        return;
                    }
                    Color[,] colors = new Color[snesColors.GetWidth(), snesColors.GetHeight()];
                    for (int y = 0; y < colors.GetHeight(); y++) {
                        for (int x = 0; x < colors.GetWidth(); x++) {
                            colors[x, y] = SNESGraphics.SNESToRGB(snesColors[x, y]);
                        }
                    }
                    undoManager.Do(new PasteColorsAction(colors, colorSelector.SelectionMin));
                } else if (Clipboard.ContainsImage()) {
                    Image image = Clipboard.GetImage();
                    Color[,] colors = new Color[(int)Math.Ceiling(image.Width / (double)ClipboardImageSquareSize),
                        (int)Math.Ceiling(image.Height / (double)ClipboardImageSquareSize)];
                    using (Bitmap bitmap = new Bitmap(image)) {
                        for (int y = 0; y < colors.GetHeight(); y++) {
                            for (int x = 0; x < colors.GetWidth(); x++) {
                                colors[x, y] = NormalizeColor(bitmap.GetPixel(x * ClipboardImageSquareSize, y * ClipboardImageSquareSize));
                            }
                        }
                    }
                    undoManager.Do(new PasteColorsAction(colors, colorSelector.SelectionMin));
                }
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }
        }

        public override void Delete() {
            undoManager.Do(new ChangeColorAction(Color.Black, colorSelector.SelectionMin, colorSelector.SelectionMax));
        }

        private void UpdateSelectedColor() {
            if (colorSelector.SelectionExists) {
                uiUpdate++;
                colorEditor.SelectRGB(SelectedColor);
                uiUpdate--;
            }
        }

        private Color NormalizeColor(Color c) {
            return SNESGraphics.SNESToRGB(SNESGraphics.RGBToSNES(c));
        }

        private void ColorSelector_SelectionChanged(object sender, EventArgs e) {
            UpdateSelectedColor();
            undoManager?.ForceNoMerge();
            RaiseSelectionChanged();

            Point start = colorSelector.SelectionStart;
            Point end = colorSelector.SelectionEnd;
            string info = $"Selected: ({Math.Min(start.X, end.X)}, {Math.Min(start.Y, end.Y)})";
            if (end != start) {
                info += $" - ({Math.Max(start.X, end.X)}, {Math.Max(start.Y, end.Y)})";
            }
            Info1 = info;
        }

        private void ColorEditor_ColorChanged(object sender, EventArgs e) {
            if (colorSelector.SelectionExists && uiUpdate == 0) {
                Color c = NormalizeColor(colorEditor.SelectedColor);
                undoManager.Do(new ChangeColorAction(c, colorSelector.SelectionMin, colorSelector.SelectionMax));
            }
        }
        
        public Color[,] GetColors(Point start, Point end) {
            Color[,] colors = new Color[end.X - start.X + 1, end.Y - start.Y + 1];
            for (int y = start.Y; y <= end.Y;  y++) {
                for (int x = start.X; x <= end.X; x++) {
                    int index = colorSelector.PointToIndex(new Point(x, y));
                    if (index < colorSelector.Colors.Length) {
                        colors[x - start.X, y - start.Y] = colorSelector.Colors[index];
                    }
                }
            }
            return colors;
        }
        
        public void SetColor(Color color, Point start, Point end) {
            for (int y = start.Y; y <= end.Y; y++) {
                for (int x = start.X; x <= end.X; x++) {
                    int index = colorSelector.PointToIndex(new Point(x, y));
                    if (index < colorSelector.Colors.Length) {
                        colorSelector.Colors[index] = color;
                    }
                }
            }
            UpdateColors();
        }
        
        public void SetColors(Color[,] colors, Point start) {
            for (int y = 0; y < colors.GetHeight(); y++) {
                for (int x = 0; x < colors.GetWidth(); x++) {
                    int index = colorSelector.PointToIndex(new Point(x + start.X, y + start.Y));
                    if (index < colorSelector.Colors.Length) {
                        colorSelector.Colors[index] = colors[x, y];
                    }
                }
            }
            UpdateColors();
        }

        private void UpdateColors() {
            colorSelector.Repaint();
            if (SelectedColor != NormalizeColor(colorEditor.SelectedColor)) {
                UpdateSelectedColor();
            }
        }

        private void colorSelector_Enter(object sender, EventArgs e) {
            colorSelectorFocused = true;
            RaiseSelectionChanged();
        }

        private void colorSelector_Leave(object sender, EventArgs e) {
            colorSelectorFocused = false;
            RaiseSelectionChanged();
        }

        private class ClipboardContents
        {
            public readonly ushort[,] colors;

            public ClipboardContents(ushort[,] colors) {
                this.colors = colors;
            }
        }
    }
}

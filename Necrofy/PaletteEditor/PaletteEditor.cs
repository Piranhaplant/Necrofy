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
        private UndoManager<PaletteEditor> undoManager;

        private int uiUpdate;
        private bool colorSelectorFocused = false;

        private ColorSelector currentColorSelector;
        private bool useScratchPadForActions = false;

        private Color SelectedColor => currentColorSelector.SelectionExists ? currentColorSelector.Colors[currentColorSelector.SelectionStartIndex] : Color.Black;

        public PaletteEditor(LoadedPalette palette) {
            InitializeComponent();

            Title = palette.paletteName;
            this.palette = palette;
            colorSelector.Colors = palette.colors;
            currentColorSelector = colorSelector;

            scratchPad.Colors = new Color[128];
            for (int i = 0; i < scratchPad.Colors.Length; i++) {
                scratchPad.Colors[i] = Color.Black;
            }

            SelectNone();
            colorSelector.Select();

            Status = "Tip: Colors can be copied into an image editor, then pasted back into Necrofy after making changes.";
        }
        
        public void UpdateColorSelectorSize() {
            colorSelector.Width = (int)(16 * 16 * Zoom);
            colorSelector.Height = colorSelector.Width / 2;
            scratchPad.Size = colorSelector.Size;
        }

        protected override UndoManager Setup() {
            Zoom = 2.0f;
            undoManager = new UndoManager<PaletteEditor>(mainWindow.UndoButton, mainWindow.RedoButton, this);
            return undoManager;
        }

        public override bool HasSelection => true;
        public override bool CanCopy => colorSelectorFocused && currentColorSelector.SelectionExists;
        public override bool CanPaste => colorSelectorFocused && currentColorSelector.SelectionExists;
        public override bool CanDelete => colorSelectorFocused && currentColorSelector.SelectionExists;

        public override void SelectAll() {
            currentColorSelector.SelectAll();
        }

        public override void SelectNone() {
            currentColorSelector.SelectionStart = Point.Empty;
            currentColorSelector.SelectionEnd = Point.Empty;
        }

        public override bool CanZoom => true;

        protected override void ZoomChanged() {
            base.ZoomChanged();
            UpdateColorSelectorSize();
        }

        protected override void DoSave(Project project) {
            palette.Save(project);
        }

        private const int ClipboardImageSquareSize = 32;

        public override void Copy() {
            DataObject clipboardData = new DataObject();

            Point selectionMin = currentColorSelector.SelectionMin;
            Point selectionMax = currentColorSelector.SelectionMax;
            ushort[,] colors = new ushort[selectionMax.X - selectionMin.X + 1, selectionMax.Y - selectionMin.Y + 1];
            for (int y = selectionMin.Y; y <= selectionMax.Y; y++) {
                for (int x = selectionMin.X; x <= selectionMax.X; x++) {
                    colors[x - selectionMin.X, y - selectionMin.Y] = SNESGraphics.RGBToSNES(currentColorSelector.Colors[currentColorSelector.PointToIndex(new Point(x, y))]);
                }
            }
            clipboardData.SetText(JsonConvert.SerializeObject(colors));

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
                    ushort[,] snesColors = JsonConvert.DeserializeObject<ushort[,]>(Clipboard.GetText());
                    Color[,] colors = new Color[snesColors.GetWidth(), snesColors.GetHeight()];
                    for (int y = 0; y < colors.GetHeight(); y++) {
                        for (int x = 0; x < colors.GetWidth(); x++) {
                            colors[x, y] = SNESGraphics.SNESToRGB(snesColors[x, y]);
                        }
                    }
                    DoAction(new PasteColorsAction(colors, currentColorSelector.SelectionMin));
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
                    DoAction(new PasteColorsAction(colors, currentColorSelector.SelectionMin));
                }
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }
        }

        public override void Delete() {
            DoAction(new ChangeColorAction(Color.Black, currentColorSelector.SelectionMin, currentColorSelector.SelectionMax));
        }

        private void DoAction(UndoAction<PaletteEditor> action) {
            if (currentColorSelector == colorSelector) {
                undoManager.Do(action);
            } else {
                useScratchPadForActions = true;
                undoManager.Perform(action);
                useScratchPadForActions = false;
            }
        }

        private void UpdateSelectedColor() {
            if (currentColorSelector.SelectionExists) {
                uiUpdate++;
                colorEditor.SelectRGB(SelectedColor);
                uiUpdate--;
            }
        }

        private Color NormalizeColor(Color c) {
            return SNESGraphics.SNESToRGB(SNESGraphics.RGBToSNES(c));
        }

        private void ColorSelector_SelectionChanged(object sender, EventArgs e) {
            if (sender != currentColorSelector) {
                currentColorSelector.SelectNone();
                currentColorSelector = (ColorSelector)sender;
            }
            UpdateSelectedColor();
            undoManager?.ForceNoMerge();
            RaiseSelectionChanged();
        }

        private void ColorEditor_ColorChanged(object sender, EventArgs e) {
            if (currentColorSelector.SelectionExists && uiUpdate == 0) {
                Color c = NormalizeColor(colorEditor.SelectedColor);
                DoAction(new ChangeColorAction(c, currentColorSelector.SelectionMin, currentColorSelector.SelectionMax));
            }
        }

        private ColorSelector GetActionColorSelector() {
            return useScratchPadForActions ? scratchPad : colorSelector;
        }

        public Color[,] GetColors(Point start, Point end) {
            ColorSelector cs = GetActionColorSelector();
            Color[,] colors = new Color[end.X - start.X + 1, end.Y - start.Y + 1];
            for (int y = start.Y; y <= end.Y;  y++) {
                for (int x = start.X; x <= end.X; x++) {
                    int index = cs.PointToIndex(new Point(x, y));
                    if (index < cs.Colors.Length) {
                        colors[x - start.X, y - start.Y] = cs.Colors[index];
                    }
                }
            }
            return colors;
        }

        public void SetColor(Color color, Point start, Point end) {
            ColorSelector cs = GetActionColorSelector();
            for (int y = start.Y; y <= end.Y; y++) {
                for (int x = start.X; x <= end.X; x++) {
                    cs.Colors[cs.PointToIndex(new Point(x, y))] = color;
                }
            }
            UpdateColors();
        }

        public void SetColors(Color[,] colors, Point start) {
            ColorSelector cs = GetActionColorSelector();
            for (int y = 0; y < colors.GetHeight(); y++) {
                for (int x = 0; x < colors.GetWidth(); x++) {
                    int index = cs.PointToIndex(new Point(x + start.X, y + start.Y));
                    if (index < cs.Colors.Length) {
                        cs.Colors[index] = colors[x, y];
                    }
                }
            }
            UpdateColors();
        }

        private void UpdateColors() {
            colorSelector.Repaint();
            scratchPad.Repaint();
            if (SelectedColor != NormalizeColor(colorEditor.SelectedColor)) {
                UpdateSelectedColor();
            }
        }

        private void colorSelector_Enter(object sender, EventArgs e) {
            colorSelectorFocused = true;
            RaiseSelectionChanged();
        }

        private void colorSelector_Leave(object sender, EventArgs e) {
            colorSelectorFocused = ActiveControl is ColorSelector;
            RaiseSelectionChanged();
        }

        private void lblScratchPad_Click(object sender, EventArgs e) {
            if (scratchPad.Visible) {
                scratchPad.Visible = false;
                lblScratchPad.Text = "▼ Scratch Pad";
            } else {
                scratchPad.Visible = true;
                lblScratchPad.Text = "▲ Scratch Pad";
            }
        }
    }
}

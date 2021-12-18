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

        private Color SelectedColor => colorSelector.SelectionStart >= 0 ? colorSelector.Colors[colorSelector.SelectionStart] : Color.Black;

        public PaletteEditor(LoadedPalette palette) {
            InitializeComponent();

            Title = palette.paletteName;
            this.palette = palette;
            colorSelector.Colors = palette.colors;

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

        public override bool HasSelection => true;
        public override bool CanCopy => colorSelectorFocused && colorSelector.SelectionStart >= 0;
        public override bool CanPaste => colorSelectorFocused && colorSelector.SelectionStart >= 0;
        public override bool CanDelete => colorSelectorFocused && colorSelector.SelectionStart >= 0;

        public override void SelectAll() {
            colorSelector.SelectionStart = 0;
            colorSelector.SelectionEnd = colorSelector.Colors.Length - 1;
        }

        public override void SelectNone() {
            colorSelector.SelectionStart = 0;
            colorSelector.SelectionEnd = 0;
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

            ushort[] colors = new ushort[colorSelector.SelectionMax - colorSelector.SelectionMin + 1];
            for (int i = 0; i < colors.Length; i++) {
                colors[i] = SNESGraphics.RGBToSNES(colorSelector.Colors[colorSelector.SelectionMin + i]);
            }
            clipboardData.SetText(JsonConvert.SerializeObject(colors));

            using (Bitmap image = new Bitmap(colors.Length * ClipboardImageSquareSize, ClipboardImageSquareSize))
            using (Graphics g = Graphics.FromImage(image)) {
                for (int i = 0; i < colors.Length; i++) {
                    using (Brush b = new SolidBrush(SNESGraphics.SNESToRGB(colors[i]))) {
                        g.FillRectangle(b, ClipboardImageSquareSize * i, 0, ClipboardImageSquareSize, ClipboardImageSquareSize);
                    }
                }
                clipboardData.SetImage(image);
                Clipboard.SetDataObject(clipboardData, true);
            }
        }

        public override void Paste() {
            try {
                if (Clipboard.ContainsText()) {
                    ushort[] snesColors = JsonConvert.DeserializeObject<ushort[]>(Clipboard.GetText());
                    Color[] colors = new Color[snesColors.Length];
                    for (int i = 0; i < colors.Length; i++) {
                        colors[i] = SNESGraphics.SNESToRGB(snesColors[i]);
                    }
                    undoManager.Do(new PasteColorsAction(colors, colorSelector.SelectionMin));
                } else if (Clipboard.ContainsImage()) {
                    Image image = Clipboard.GetImage();
                    Color[] colors = new Color[(int)Math.Ceiling(image.Width / (double)ClipboardImageSquareSize)];
                    using (Bitmap bitmap = new Bitmap(image)) {
                        for (int i = 0; i < colors.Length; i++) {
                            colors[i] = bitmap.GetPixel(i * ClipboardImageSquareSize, 0);
                        }
                    }
                    undoManager.Do(new PasteColorsAction(colors, colorSelector.SelectionMin));
                }
            } catch (Exception e) { }
        }

        public override void Delete() {
            undoManager.Do(new ChangeColorAction(Color.Black, colorSelector.SelectionMin, colorSelector.SelectionMax));
        }

        private void UpdateSelectedColor() {
            if (colorSelector.SelectionStart >= 0) {
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
        }

        private void ColorEditor_ColorChanged(object sender, EventArgs e) {
            if (colorSelector.SelectionStart >= 0 && uiUpdate == 0) {
                Color c = NormalizeColor(colorEditor.SelectedColor);
                undoManager.Do(new ChangeColorAction(c, colorSelector.SelectionMin, colorSelector.SelectionMax));
            }
        }

        public Color[] GetColors(int start, int end) {
            Color[] colors = new Color[end - start + 1];
            for (int i = start; i < start + colors.Length && i < colorSelector.Colors.Length; i++) {
                colors[i - start] = colorSelector.Colors[i];
            }
            return colors;
        }

        public void SetColor(Color color, int start, int end) {
            for (int i = start; i <= end; i++) {
                colorSelector.Colors[i] = color;
            }
            UpdateColors();
        }

        public void SetColors(Color[] colors, int start) {
            for (int i = start; i < start + colors.Length && i < colorSelector.Colors.Length; i++) {
                colorSelector.Colors[i] = colors[i - start];
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
    }
}

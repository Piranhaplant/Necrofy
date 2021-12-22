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

        private Color SelectedColor => currentColorSelector.SelectionStart >= 0 ? currentColorSelector.Colors[currentColorSelector.SelectionStart] : Color.Black;

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
        public override bool CanCopy => colorSelectorFocused && currentColorSelector.SelectionStart >= 0;
        public override bool CanPaste => colorSelectorFocused && currentColorSelector.SelectionStart >= 0;
        public override bool CanDelete => colorSelectorFocused && currentColorSelector.SelectionStart >= 0;

        public override void SelectAll() {
            currentColorSelector.SelectionStart = 0;
            currentColorSelector.SelectionEnd = currentColorSelector.Colors.Length - 1;
        }

        public override void SelectNone() {
            currentColorSelector.SelectionStart = 0;
            currentColorSelector.SelectionEnd = 0;
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

            ushort[] colors = new ushort[currentColorSelector.SelectionMax - currentColorSelector.SelectionMin + 1];
            for (int i = 0; i < colors.Length; i++) {
                colors[i] = SNESGraphics.RGBToSNES(currentColorSelector.Colors[currentColorSelector.SelectionMin + i]);
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
                    DoAction(new PasteColorsAction(colors, currentColorSelector.SelectionMin));
                } else if (Clipboard.ContainsImage()) {
                    Image image = Clipboard.GetImage();
                    Color[] colors = new Color[(int)Math.Ceiling(image.Width / (double)ClipboardImageSquareSize)];
                    using (Bitmap bitmap = new Bitmap(image)) {
                        for (int i = 0; i < colors.Length; i++) {
                            colors[i] = bitmap.GetPixel(i * ClipboardImageSquareSize, 0);
                        }
                    }
                    DoAction(new PasteColorsAction(colors, currentColorSelector.SelectionMin));
                }
            } catch (Exception e) { }
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
            if (currentColorSelector.SelectionStart >= 0) {
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
                currentColorSelector.SelectionStart = -1;
                currentColorSelector.SelectionEnd = -1;
                currentColorSelector = (ColorSelector)sender;
            }
            UpdateSelectedColor();
            undoManager?.ForceNoMerge();
        }

        private void ColorEditor_ColorChanged(object sender, EventArgs e) {
            if (currentColorSelector.SelectionStart >= 0 && uiUpdate == 0) {
                Color c = NormalizeColor(colorEditor.SelectedColor);
                DoAction(new ChangeColorAction(c, currentColorSelector.SelectionMin, currentColorSelector.SelectionMax));
            }
        }

        private ColorSelector GetActionColorSelector() {
            return useScratchPadForActions ? scratchPad : colorSelector;
        }

        public Color[] GetColors(int start, int end) {
            ColorSelector cs = GetActionColorSelector();
            Color[] colors = new Color[end - start + 1];
            for (int i = start; i < start + colors.Length && i < cs.Colors.Length; i++) {
                colors[i - start] = cs.Colors[i];
            }
            return colors;
        }

        public void SetColor(Color color, int start, int end) {
            ColorSelector cs = GetActionColorSelector();
            for (int i = start; i <= end; i++) {
                cs.Colors[i] = color;
            }
            UpdateColors();
        }

        public void SetColors(Color[] colors, int start) {
            ColorSelector cs = GetActionColorSelector();
            for (int i = start; i < start + colors.Length && i < cs.Colors.Length; i++) {
                cs.Colors[i] = colors[i - start];
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

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

        private Color SelectedColor => colorSelector.SelectionStart >= 0 ? colorSelector.Colors[colorSelector.SelectionStart] : Color.Black;

        public PaletteEditor(LoadedPalette palette) {
            InitializeComponent();
            Title = palette.paletteName;
            this.palette = palette;
            colorSelector.Colors = palette.colors;

            colorSelector.SelectionChanged += ColorSelector_SelectionChanged;
            colorEditor.ColorChanged += ColorEditor_ColorChanged;
            SelectNone();
        }

        protected override UndoManager Setup() {
            undoManager = new UndoManager<PaletteEditor>(mainWindow.UndoButton, mainWindow.RedoButton, this);
            return undoManager;
        }

        public override bool HasSelection => true;
        public override bool CanCopy => colorSelector.SelectionStart >= 0;
        public override bool CanPaste => colorSelector.SelectionStart >= 0;
        public override bool CanDelete => false;

        public override void SelectAll() {
            colorSelector.SelectionStart = 0;
            colorSelector.SelectionEnd = colorSelector.Colors.Length - 1;
        }

        public override void SelectNone() {
            colorSelector.SelectionStart = 0;
            colorSelector.SelectionEnd = 0;
        }

        protected override void DoSave(Project project) {
            palette.Save(project);
        }

        public override void Copy() {
            ushort[] colors = new ushort[colorSelector.SelectionMax - colorSelector.SelectionMin + 1];
            for (int i = 0; i < colors.Length; i++) {
                colors[i] = SNESGraphics.RGBToSNES(colorSelector.Colors[colorSelector.SelectionMin + i]);
            }
            Clipboard.SetText(JsonConvert.SerializeObject(colors));
        }

        public override void Paste() {
            try {
                ushort[] snesColors = JsonConvert.DeserializeObject<ushort[]>(Clipboard.GetText());
                Color[] colors = new Color[snesColors.Length];
                for (int i = 0; i < colors.Length; i++) {
                    colors[i] = SNESGraphics.SNESToRGB(snesColors[i]);
                }
                undoManager.Do(new PasteColorsAction(colors, colorSelector.SelectionMin));
            } catch (Exception) { }
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
            for (int i = 0; i < colors.Length; i++) {
                colors[i] = colorSelector.Colors[i + start];
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class ToolManager<T> where T : ITool
    {
        private readonly MainWindow mainWindow;

        private readonly Dictionary<Keys, T> toolShortcutKeys = new Dictionary<Keys, T>();
        private readonly Dictionary<ToolStripGrouper.ItemType, T> toolForItemType = new Dictionary<ToolStripGrouper.ItemType, T>();
        private readonly Dictionary<T, ToolStripGrouper.ItemType> itemTypeForTool = new Dictionary<T, ToolStripGrouper.ItemType>();

        public T currentTool { get; private set; }

        public event EventHandler ToolChanged;

        public ToolManager(MainWindow mainWindow) {
            this.mainWindow = mainWindow;
        }

        public void SetupTool(T tool, ToolStripGrouper.ItemType itemType, Keys shortcutKeys) {
            toolShortcutKeys[shortcutKeys] = tool;
            toolForItemType[itemType] = tool;
            itemTypeForTool[tool] = itemType;
        }

        public void ChangeToSelectedTool() {
            foreach (ToolStripGrouper.ItemType type in toolForItemType.Keys) {
                if (mainWindow.GetToolStripItem(type).Checked) {
                    ChangeTool(toolForItemType[type]);
                    return;
                }
            }
        }

        public void ChangeTool(T tool) {
            if (!ReferenceEquals(tool, currentTool)) {
                T previousTool = currentTool;
                previousTool?.DoneBeingUsed();
                currentTool = tool;

                foreach (ToolStripGrouper.ItemType type in toolForItemType.Keys) {
                    mainWindow.GetToolStripItem(type).Checked = false;
                }
                mainWindow.GetToolStripItem(itemTypeForTool[tool]).Checked = true;
                
                ToolChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool KeyDown(Keys keyData) {
            if (toolShortcutKeys.TryGetValue(keyData, out T t)) {
                ChangeTool(t);
                return true;
            }
            return false;
        }

        public bool ToolStripItemClicked(ToolStripGrouper.ItemType item) {
            if (toolForItemType.TryGetValue(item, out T t)) {
                ChangeTool(t);
                return true;
            }
            return false;
        }
    }

    interface ITool
    {
        void DoneBeingUsed();
    }
}

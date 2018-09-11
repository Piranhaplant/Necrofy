using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Necrofy.Properties;

namespace Necrofy
{
    partial class ProjectBrowser : DockContent
    {
        private readonly MainWindow mainWindow;
        private readonly Project project;

        private readonly Dictionary<Bitmap, int> imageIndexMap = new Dictionary<Bitmap, int>();

        public ProjectBrowser(MainWindow mainWindow, Project project) {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.project = project;

            treeImages.Images.Add(Resources.document); // Used as default image
            populateTree(tree.Nodes, project.path);
        }

        private void populateTree(TreeNodeCollection parent, string path) {
            string [] dirs = Directory.GetDirectories(path);
            Array.Sort(dirs, new NumericStringComparer());
            foreach (string dir in dirs) {
                TreeNode child = parent.Add(Path.GetFileName(dir));
                setImage(child, Resources.folder);

                populateTree(child.Nodes, dir);
                if (child.Nodes.Count == 0) {
                    child.Remove();
                }
            }

            string[] files = Directory.GetFiles(path);
            Array.Sort(files, new NumericStringComparer());
            foreach (string file in files) {
                Asset.NameInfo info = Asset.GetInfo(project, project.GetRelativePath(file));
                if (info != null) {
                    TreeNode child = parent.Add(info.DisplayName);
                    setImage(child, info.DisplayImage);
                    child.Tag = info;
                }
            }
        }

        private void tree_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
            setImage(e.Node, Resources.folder_open);
        }

        private void tree_BeforeCollapse(object sender, TreeViewCancelEventArgs e) {
            setImage(e.Node, Resources.folder);
        }

        private void tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
            Asset.NameInfo info = (Asset.NameInfo)e.Node.Tag;
            EditorWindow editor = info.GetEditor(project);
            if (editor != null) {
                mainWindow.ShowEditor(editor);
            }
        }

        private void setImage(TreeNode node, Bitmap image) {
            if (!imageIndexMap.ContainsKey(image)) {
                imageIndexMap[image] = treeImages.Images.Count;
                treeImages.Images.Add(image);
            }
            node.ImageIndex = imageIndexMap[image];
            node.SelectedImageIndex = node.ImageIndex;
        }
    }
}

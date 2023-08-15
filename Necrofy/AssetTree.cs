using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class AssetTree
    {
        private const string ClipboardDropEffect = "Preferred DropEffect";

        private readonly Project project;
        private readonly FileSystemWatcher watcher;
        
        public Folder Root { get; private set; }

        public event EventHandler<AssetEventArgs> AssetChanged;
        public event EventHandler<AssetEventArgs> AssetAdded;
        public event EventHandler<AssetEventArgs> AssetRemoved;
        public event EventHandler<NodeEventArgs> NodeRenamed;
        public event EventHandler<RenameCompletedEventArgs> RenameCompleted;

        public AssetTree(Project project, ISynchronizeInvoke synchronizingObject) {
            this.project = project;

            Root = ReadAssets(project.path, null);

            watcher = new FileSystemWatcher(project.path);
            watcher.SynchronizingObject = synchronizingObject;
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Changed += File_Changed;
            watcher.Created += File_Created;
            watcher.Deleted += File_Deleted;
            watcher.Renamed += File_Renamed;
            watcher.EnableRaisingEvents = true;
        }

        private Folder ReadAssets(string folder, Folder parent) {
            Folder f = new Folder(Path.GetFileName(folder), parent);

            foreach (string dir in Directory.GetDirectories(folder)) {
                f.Folders.Add(ReadAssets(dir, f));
            }

            foreach (string file in Directory.GetFiles(folder)) {
                ReadAsset(file, f);
            }

            return f;
        }

        private AssetEntry ReadAsset(string file, Folder folder) {
            Asset.NameInfo info = Asset.GetInfo(project, project.GetRelativePath(file));
            if (info != null) {
                AssetEntry entry = new AssetEntry(info, Path.GetFileName(file), folder);
                folder.Assets.Add(entry);
                return entry;
            }
            return null;
        }

        public void Rename(Node node, string newName) {
            string oldFilename = node.GetFilename(project.path);
            if (node is Folder) {
                string newFilename = Path.Combine(Path.GetDirectoryName(oldFilename), newName);
                Directory.Move(oldFilename, newFilename);
            } else if (node is AssetEntry asset) {
                if (!asset.Asset.CanBeRenamedTo(ref newName)) {
                    throw new Exception($"Name '{newName}' is not valid.");
                }
                string newFilename = asset.Asset.GetFilename(project.path, replacementName: newName);
                File.Move(oldFilename, newFilename);
            }
        }

        public void Delete(Node node) {
            if (node is Folder folder) {
                Directory.Delete(folder.GetFilename(project.path), true);
            } else if (node is AssetEntry asset) {
                File.Delete(asset.GetFilename(project.path));
            }
        }

        public void Cut(Node node) {
            DataObject clipboardData = new DataObject();
            clipboardData.SetFileDropList(new System.Collections.Specialized.StringCollection() { node.GetFilename(project.path) });
            clipboardData.SetData(ClipboardDropEffect, new MemoryStream(BitConverter.GetBytes((int)DragDropEffects.Move)));
            Clipboard.SetDataObject(clipboardData);
        }

        public void Copy(Node node) {
            DataObject clipboardData = new DataObject();
            clipboardData.SetFileDropList(new System.Collections.Specialized.StringCollection() { node.GetFilename(project.path) });
            Clipboard.SetDataObject(clipboardData);
        }

        public void Paste(Node target) {
            if (target is AssetEntry asset) {
                target = target.Parent;
            }
            string destinationFolder = target.GetFilename(project.path);

            bool doMove = false;
            MemoryStream dropEffect = Clipboard.GetData(ClipboardDropEffect) as MemoryStream;
            if (dropEffect != null) {
                DragDropEffects effectType = (DragDropEffects)dropEffect.ReadByte();
                doMove = effectType.HasFlag(DragDropEffects.Move);
            }

            foreach (string file in Clipboard.GetFileDropList()) {
                string destinationFile = Path.Combine(destinationFolder, Path.GetFileName(file));
                if (Directory.Exists(file)) {
                    destinationFile = GetUniqueFilename(destinationFile, true);
                    if (doMove) {
                        Directory.Move(file, destinationFile);
                    } else {
                        CopyDirectory(file, destinationFile);
                    }
                } else {
                    destinationFile = GetUniqueFilename(destinationFile, false);
                    if (doMove) {
                        File.Move(file, destinationFile);
                    } else {
                        File.Copy(file, destinationFile);
                    }
                }
            }
        }

        private static string GetUniqueFilename(string baseFilename, bool isDirectory) {
            string newFilename = baseFilename;
            int num = 1;
            if (isDirectory) {
                while (Directory.Exists(newFilename)) {
                    num += 1;
                    newFilename = baseFilename + num.ToString();
                }
            } else {
                Asset.PathParts pathParts = Asset.PathParts.Parse(baseFilename);
                string baseName = pathParts.name;
                while (File.Exists(newFilename)) {
                    num += 1;
                    pathParts.name = baseName + num.ToString();
                    newFilename = pathParts.GetFilename("");
                }
            }
            return newFilename;
        }

        private static void CopyDirectory(string sourceDirectory, string destDirectory) {
            Directory.CreateDirectory(destDirectory);
            foreach (string subDir in Directory.GetDirectories(sourceDirectory)) {
                CopyDirectory(subDir, Path.Combine(destDirectory, Path.GetFileName(subDir)));
            }
            foreach (string file in Directory.GetFiles(sourceDirectory)) {
                File.Copy(file, Path.Combine(destDirectory, Path.GetFileName(file)));
            }
        }

        private void File_Changed(object sender, FileSystemEventArgs e) {
            Console.WriteLine($"File changed: {e.Name}");
            if (Root.FindAsset(e.Name, out AssetEntry asset)) {
                asset.Asset.Refresh();
                AssetChanged?.Invoke(this, new AssetEventArgs(asset));
            }
        }

        private void File_Created(object sender, FileSystemEventArgs e) {
            Console.WriteLine($"File created: {e.Name}");
            File_Created(e);
        }
        private void File_Created(FileSystemEventArgs e) {
            if (Root.FindFolder(Path.GetDirectoryName(e.Name), out Folder parentFolder)) {
                if (parentFolder.Folders.Any(f => f.Name == Path.GetFileName(e.Name)) || parentFolder.Assets.Any(a => a.Name == Path.GetFileName(e.Name))) {
                    return;
                }
                if (Directory.Exists(e.FullPath)) {
                    Folder folder = ReadAssets(e.FullPath, parentFolder);
                    parentFolder.Folders.Add(folder);
                    FolderAdded(folder);
                } else {
                    AssetEntry asset = ReadAsset(e.FullPath, parentFolder);
                    if (asset != null) {
                        AssetAdded?.Invoke(this, new AssetEventArgs(asset));
                    }
                }
            }
        }

        private void FolderAdded(Folder folder) {
            foreach (Folder subFolder in folder.Folders) {
                FolderAdded(subFolder);
            }
            foreach (AssetEntry asset in folder.Assets) {
                AssetAdded?.Invoke(this, new AssetEventArgs(asset));
            }
        }

        private void File_Deleted(object sender, FileSystemEventArgs e) {
            Console.WriteLine($"File deleted: {e.Name}");
            File_Deleted(e);
        }
        private void File_Deleted(FileSystemEventArgs e) {
            if (Root.FindFolder(e.Name, out Folder folder)) {
                folder.Parent.Folders.Remove(folder);
                FolderRemoved(folder);
            } else if (Root.FindAsset(e.Name, out AssetEntry asset)) {
                asset.Parent.Assets.Remove(asset);
                AssetRemoved?.Invoke(this, new AssetEventArgs(asset));
            }
        }

        private void FolderRemoved(Folder folder) {
            foreach (Folder subFolder in folder.Folders) {
                FolderRemoved(subFolder);
            }
            foreach (AssetEntry asset in folder.Assets) {
                AssetRemoved?.Invoke(this, new AssetEventArgs(asset));
            }
        }

        private void File_Renamed(object sender, RenamedEventArgs e) {
            Console.WriteLine($"File renamed: {e.OldName} -> {e.Name}");
            Node renamedNode = null;

            if (Root.FindFolder(Path.GetDirectoryName(e.Name), out Folder newParent)) {
                Asset.RenameResults results = new Asset.RenameResults();
                if (Root.FindAsset(e.OldName, out AssetEntry asset)) {
                    if (asset.Asset.Rename(project, e.Name, results)) {
                        renamedNode = asset;
                    }
                } else if (Root.FindFolder(e.OldName, out Folder folder)) {
                    RenameFolder(folder, e.Name, results);
                    renamedNode = folder;
                }
                if (renamedNode != null) {
                    foreach (KeyValuePair<string, string> failure in results.failedRenames) {
                        File_Deleted(new FileSystemEventArgs(WatcherChangeTypes.Deleted, project.path, failure.Key));
                    }
                    renamedNode.Move(newParent, Path.GetFileName(e.Name));
                    foreach (KeyValuePair<string, string> failure in results.failedRenames) {
                        File_Created(new FileSystemEventArgs(WatcherChangeTypes.Created, project.path, failure.Value));
                    }
                    NodeRenamed?.Invoke(this, new NodeEventArgs(renamedNode));
                    Asset.RenameAssetReferences(project, results);
                    RenameCompleted?.Invoke(this, new RenameCompletedEventArgs(results));
                    return;
                }
            }
            File_Deleted(new FileSystemEventArgs(e.ChangeType, project.path, e.OldName));
            File_Created(e);
        }

        private void RenameFolder(Folder folder, string newName, Asset.RenameResults results) {
            results.isFolder = true;
            foreach (Folder child in folder.Folders) {
                RenameFolder(child, Path.Combine(newName, child.Name), results);
            }
            foreach (AssetEntry asset in folder.Assets) {
                string newAssetName = Path.Combine(newName, asset.Name);
                asset.Asset.Rename(project, newAssetName, results);
            }
        }

        public abstract class Node
        {
            public string Name { get; private set; }
            public Folder Parent { get; private set; }
            public abstract string DisplayName { get; }

            public Node(string name, Folder parent) {
                Name = name;
                Parent = parent;
            }

            public virtual void Move(Folder newParent, string newName) {
                Parent = newParent;
                Name = newName;
            }

            public abstract string GetFilename(string rootPath);
        }

        public class Folder : Node
        {
            public List<Folder> Folders { get; private set; } = new List<Folder>();
            public List<AssetEntry> Assets { get; private set; } = new List<AssetEntry>();

            public override string DisplayName => Name;

            public Folder(string name, Folder parent) : base(name, parent) { }

            public override void Move(Folder newParent, string newName) {
                Parent.Folders.Remove(this);
                newParent.Folders.Add(this);
                base.Move(newParent, newName);
            }

            public override string GetFilename(string rootPath) {
                if (Parent == null) {
                    return rootPath;
                } else {
                    return Path.Combine(Parent.GetFilename(rootPath), Name);
                }
            }

            public bool FindFolder(string path, out Folder folder) {
                folder = this;
                if (path.Length > 0) {
                    foreach (string part in path.Split(Path.DirectorySeparatorChar)) {
                        folder = folder.Folders.Find(f => f.Name == part);
                        if (folder == null) {
                            return false;
                        }
                    }
                }
                return true;
            }

            public bool FindAsset(string path, out AssetEntry asset) {
                asset = null;
                if (FindFolder(Path.GetDirectoryName(path), out Folder folder)) {
                    asset = folder.Assets.Find(a => a.Name == Path.GetFileName(path));
                } else {
                    asset = Assets.Find(a => a.Name == path);
                }
                return asset != null;
            }

            public IEnumerable<Asset.NameInfo> Enumerate() {
                foreach (Folder f in Folders) {
                    foreach (Asset.NameInfo asset in f.Enumerate()) {
                        yield return asset;
                    }
                }
                foreach (AssetEntry asset in Assets) {
                    yield return asset.Asset;
                }
            }
        }

        public class AssetEntry : Node
        {
            public Asset.NameInfo Asset { get; private set; }

            public override string DisplayName => Asset.DisplayName;

            public AssetEntry(Asset.NameInfo asset, string name, Folder parent) : base(name, parent) {
                Asset = asset;
            }

            public override void Move(Folder newParent, string newName) {
                Parent.Assets.Remove(this);
                newParent.Assets.Add(this);
                base.Move(newParent, newName);
            }

            public override string GetFilename(string rootPath) {
                return Path.Combine(Parent.GetFilename(rootPath), Name);
            }
        }

        public class NodeComparer : Comparer<Node>
        {
            public static readonly NodeComparer instance = new NodeComparer();

            public override int Compare(Node x, Node y) {
                if (x is Folder && y is AssetEntry) {
                    return -1;
                } else if (x is AssetEntry && y is Folder) {
                    return 1;
                } else if (x is Folder) {
                    return NumericStringComparer.instance.Compare(x.Name, y.Name);
                } else {
                    AssetEntry xAsset = x as AssetEntry;
                    AssetEntry yAsset = y as AssetEntry;
                    if (xAsset.Asset.DisplayName == yAsset.Asset.DisplayName) {
                        return xAsset.Asset.Category.CompareTo(yAsset.Asset.Category);
                    } else {
                        return NumericStringComparer.instance.Compare(xAsset.DisplayName, yAsset.DisplayName);
                    }
                }
            }
        }
    }

    class NodeEventArgs : EventArgs
    {
        public AssetTree.Node Node { get; private set; }

        public NodeEventArgs(AssetTree.Node node) {
            Node = node;
        }
    }

    class AssetEventArgs : EventArgs
    {
        public AssetTree.AssetEntry Asset { get; private set; }

        public AssetEventArgs(AssetTree.AssetEntry asset) {
            Asset = asset;
        }
    }

    class RenameCompletedEventArgs : EventArgs
    {
        public Asset.RenameResults Results { get; private set; }

        public RenameCompletedEventArgs(Asset.RenameResults results) {
            Results = results;
        }
    }
}

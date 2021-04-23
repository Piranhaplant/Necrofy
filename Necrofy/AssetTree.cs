using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Necrofy
{
    class AssetTree
    {
        private readonly Project project;
        private readonly FileSystemWatcher watcher;
        
        public Folder Root { get; private set; }

        public event EventHandler<AssetEventArgs> AssetChanged;
        public event EventHandler<AssetEventArgs> AssetAdded;
        public event EventHandler<AssetEventArgs> AssetRemoved;

        public AssetTree(Project project) {
            this.project = project;

            Root = ReadAssets(project.path, null);

            watcher = new FileSystemWatcher(project.path);
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

            string[] dirs = Directory.GetDirectories(folder);
            Array.Sort(dirs, NumericStringComparer.instance);
            foreach (string dir in dirs) {
                f.Folders.Add(ReadAssets(dir, f));
            }

            string[] files = Directory.GetFiles(folder);
            Array.Sort(files, NumericStringComparer.instance);
            foreach (string file in files) {
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

        private void File_Changed(object sender, FileSystemEventArgs e) {
            if (Root.FindAsset(e.Name, out AssetEntry asset)) {
                asset.Asset.Refresh();
                AssetChanged?.Invoke(this, new AssetEventArgs(asset));
            }
        }

        // TODO: Preserve sorting when adding files/folders
        private void File_Created(object sender, FileSystemEventArgs e) {
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
            File_Deleted(sender, new FileSystemEventArgs(e.ChangeType, project.path, e.OldName));
            File_Created(sender, e);
        }

        public class Node
        {
            public string Name { get; private set; }
            public Folder Parent { get; private set; }

            public Node(string name, Folder parent) {
                Name = name;
                Parent = parent;
            }
        }

        public class Folder : Node
        {
            public List<Folder> Folders { get; private set; } = new List<Folder>();
            public List<AssetEntry> Assets { get; private set; } = new List<AssetEntry>();

            public Folder(string name, Folder parent) : base(name, parent) { }

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

            public AssetEntry(Asset.NameInfo asset, string name, Folder parent) : base(name, parent) {
                Asset = asset;
            }
        }
    }

    class AssetEventArgs : EventArgs
    {
        public AssetTree.AssetEntry Asset { get; private set; }

        public AssetEventArgs(AssetTree.AssetEntry asset) {
            Asset = asset;
        }
    }
}

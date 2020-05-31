using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class AssetTree
    {
        private readonly Project project;
        private readonly FileSystemWatcher watcher;

        private readonly Dictionary<string, Folder> foldersByName = new Dictionary<string, Folder>();
        private readonly Dictionary<string, Asset.NameInfo> assetsByName = new Dictionary<string, Asset.NameInfo>();
        public Folder Root { get; private set; }

        public event EventHandler<AssetEventArgs> AssetChanged;

        public AssetTree(Project project) {
            this.project = project;

            watcher = new FileSystemWatcher(project.path);
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Changed += File_Changed;
            watcher.Created += File_Created;
            watcher.Deleted += File_Deleted;
            watcher.Renamed += File_Renamed;
            watcher.EnableRaisingEvents = true;

            Root = ReadAssets(project.path) ?? new Folder("", new List<Folder>(), new List<Asset.NameInfo>());
        }

        private Folder ReadAssets(string folder) {
            List<Folder> subFolders = new List<Folder>();
            List<Asset.NameInfo> assets = new List<Asset.NameInfo>();

            string[] dirs = Directory.GetDirectories(folder);
            Array.Sort(dirs, NumericStringComparer.instance);
            foreach (string dir in dirs) {
                Folder subFolder = ReadAssets(dir);
                if (subFolder != null) {
                    subFolders.Add(subFolder);
                    foldersByName[dir] = subFolder;
                }
            }

            string[] files = Directory.GetFiles(folder);
            Array.Sort(files, NumericStringComparer.instance);
            foreach (string file in files) {
                Asset.NameInfo info = Asset.GetInfo(project, project.GetRelativePath(file));
                if (info != null) {
                    assets.Add(info);
                    assetsByName[file] = info;
                }
            }

            if (subFolders.Count > 0 || assets.Count > 0) {
                return new Folder(Path.GetFileName(folder), subFolders, assets);
            }
            return null;
        }

        private void File_Changed(object sender, FileSystemEventArgs e) {
            if (!Directory.Exists(e.FullPath) && assetsByName.TryGetValue(e.FullPath, out Asset.NameInfo asset)) {
                asset.Refresh();
                AssetChanged?.Invoke(this, new AssetEventArgs(asset));
            }
        }

        private void File_Created(object sender, FileSystemEventArgs e) {
            // TODO
        }

        private void File_Deleted(object sender, FileSystemEventArgs e) {
            // TODO
        }

        private void File_Renamed(object sender, RenamedEventArgs e) {
            // TODO
        }

        public class Folder
        {
            public string Name { get; private set; }
            public List<Folder> Folders { get; private set; }
            public List<Asset.NameInfo> Assets { get; private set; }

            public Folder(string name, List<Folder> folders, List<Asset.NameInfo> assets) {
                Name = name;
                Folders = folders;
                Assets = assets;
            }

            public IEnumerable<Asset.NameInfo> Enumerate() {
                foreach (Folder f in Folders) {
                    foreach (Asset.NameInfo asset in f.Enumerate()) {
                        yield return asset;
                    }
                }
                foreach (Asset.NameInfo asset in Assets) {
                    yield return asset;
                }
            }
        }
    }

    class AssetEventArgs : EventArgs
    {
        public Asset.NameInfo Asset { get; private set; }

        public AssetEventArgs(Asset.NameInfo asset) {
            this.Asset = asset;
        }
    }
}

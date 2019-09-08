using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class TilesetSuggestions
    {
        private readonly Dictionary<Direction, List<Category>> directions = new Dictionary<Direction, List<Category>>();

        public TilesetSuggestions(string json) {
            JsonData data = JsonConvert.DeserializeObject<JsonData>(json);
            Dictionary<string, Category> nameToCategory = new Dictionary<string, Category>();

            ReadDirection(data.left, Direction.Left, nameToCategory);
            ReadDirection(data.top, Direction.Top, nameToCategory);
            ReadDirection(data.right, Direction.Right, nameToCategory);
            ReadDirection(data.bottom, Direction.Bottom, nameToCategory);

            ReadConnections(data.left, Direction.Left, nameToCategory);
            ReadConnections(data.top, Direction.Top, nameToCategory);
            ReadConnections(data.right, Direction.Right, nameToCategory);
            ReadConnections(data.bottom, Direction.Bottom, nameToCategory);
        }

        public List<ushort> GetTiles(ushort startTile, Direction direction) {
            foreach (Category category in directions[direction]) {
                if (category.tiles.Contains(startTile)) {
                    List<ushort> tiles = new List<ushort>();
                    foreach (Category connection in category.connections) {
                        tiles.AddRange(connection.tiles);
                    }
                    return tiles;
                }
            }
            return new List<ushort>();
        }

        private void ReadDirection(List<JsonData.Category> jsonCategories, Direction direction, Dictionary<string, Category> nameToCategory) {
            List<Category> categories = new List<Category>();
            directions.Add(direction, categories);

            foreach (JsonData.Category jsonCategory in jsonCategories) {
                Category category = new Category(jsonCategory.tiles);
                categories.Add(category);
                nameToCategory.Add(jsonCategory.name, category);
            }
        }

        private void ReadConnections(List<JsonData.Category> jsonCategories, Direction direction, Dictionary<string, Category> nameToCategory) {
            List<Category> categories = directions[direction];
            for (int i = 0; i < categories.Count; i++) {
                categories[i].connections.AddRange(jsonCategories[i].connections.Select(name => nameToCategory[name]));
            }
        }

        public enum Direction
        {
            Left,
            Top,
            Right,
            Bottom,
        }

        private class Category
        {
            public readonly List<ushort> tiles;
            public readonly List<Category> connections = new List<Category>();

            public Category(List<ushort> tiles) {
                this.tiles = tiles;
            }
        }

        private class JsonData
        {
            public List<Category> left;
            public List<Category> top;
            public List<Category> right;
            public List<Category> bottom;

            public class Category
            {
                public string name;
                public List<ushort> tiles;
                public List<string> connections;
            }
        }
    }
}

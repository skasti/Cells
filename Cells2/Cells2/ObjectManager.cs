using System;
using Cells.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Cells.QuadTree;
using Cells.Geometry;
using Rectangle = Cells.Geometry.Rectangle;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Cells
{
    public class ObjectManager
    {
        public Stopwatch UpdateStopwatch { get; } = new Stopwatch();
        public Stopwatch CollisionStopwatch { get; } = new Stopwatch();
        public Stopwatch DrawStopwatch { get; } = new Stopwatch();
        static Dictionary<Type,int> ObjectLimit = new Dictionary<Type, int>
        {
            {typeof(Organism), 1000}
        };

        static readonly ObjectManager _instance = new ObjectManager();
        public static ObjectManager Instance { get { return _instance; } }

        private readonly List<GameObject> _gameObjects = new List<GameObject>();
        private readonly List<GameObject> _addQueue = new List<GameObject>();
        private readonly List<GameObject> _removeQueue = new List<GameObject>();

        private readonly Node SearchTree = new Node(new Rectangle(Vector2.Zero, Game1.WorldBounds));

        public bool Add(GameObject gameObject)
        {
            if (ObjectLimit.ContainsKey(gameObject.GetType()))
            {
                if (_gameObjects.Count(go => gameObject.GetType() == go.GetType()) + _addQueue.Count(go => gameObject.GetType() == go.GetType()) >= ObjectLimit[gameObject.GetType()])
                    return false;
            }

            if (!_gameObjects.Contains(gameObject) && !_addQueue.Contains(gameObject))
            {
                _addQueue.Add(gameObject);
                return true;
            }

            return false;
        }

        public void Remove(GameObject gameObject)
        {
            if (_gameObjects.Contains(gameObject) && !_removeQueue.Contains(gameObject))
                _removeQueue.Add(gameObject);
        }

        public void Update(float deltaTime)
        {
            UpdateStopwatch.Start();
            foreach (var obj in _gameObjects)
                obj.Update(deltaTime);
            UpdateStopwatch.Stop();

            CollisionStopwatch.Start();
            CheckCollisions(deltaTime);
            CollisionStopwatch.Stop();

            _gameObjects.AddRange(_addQueue);
            SearchTree.AddRange(_addQueue);
            _addQueue.Clear();

            foreach (var oldObject in _removeQueue)
            {
                _gameObjects.Remove(oldObject);
                SearchTree.Remove(oldObject);
            }

            _removeQueue.Clear();
        }

        public void CheckCollisions(float deltaTime)
        {
            foreach (var gameObject in _gameObjects)
            {
                var node = gameObject.CurrentNode ?? SearchTree;
                node.FindObjects(gameObject.Bounds.Surround(4f, new Vector2(500,500)), o => o != gameObject && o.Bounds.Intersects(gameObject.Bounds))
                    .ForEach(c => gameObject.HandleCollision(c, deltaTime));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            foreach (var obj in _gameObjects.OrderBy(o => o.DrawPriority))
                obj.Draw(spriteBatch);
            spriteBatch.End();
        }

        public IEnumerable<T> GetObjectsWithinRange<T>(GameObject self, float range) where T : GameObject
        {
            var searchBounds = new Rectangle(
                self.Position - (Vector2.One * range * 1.5f),
                Vector2.One * range * 3f
            );
            var node = self.CurrentNode ?? SearchTree;
            return node.FindObjects(searchBounds, o => (o is T) && (o != self) && !_removeQueue.Contains(o) && ((self.Position - o.Position).Length() < range)).Cast<T>();
        }

        public int Count<T>() where T : GameObject
        {
            return _gameObjects.Count(go => go is T);
        }

        public IEnumerable<T> GetObjects<T>()
        {
            return _gameObjects.Where(go => go is T).Cast<T>();
        }

        public IEnumerable<T> GetObjects<T>(Vector2 point)
        {
            return _gameObjects.Where(go => go is T && go.Bounds.Contains(point)).Cast<T>();
        }
    }
}

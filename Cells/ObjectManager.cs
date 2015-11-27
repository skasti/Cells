using Cells.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Cells
{
    public class ObjectManager
    {
        static readonly ObjectManager _instance = new ObjectManager();
        public static ObjectManager Instance { get { return _instance; } }

        private readonly List<GameObject> _gameObjects = new List<GameObject>();
        private readonly List<GameObject> _addQueue = new List<GameObject>();
        private readonly List<GameObject> _removeQueue = new List<GameObject>();

        public void Add(GameObject gameObject)
        {
            if (!_gameObjects.Contains(gameObject) && !_addQueue.Contains(gameObject))
                _addQueue.Add(gameObject);
        }

        public void Remove(GameObject gameObject)
        {
            if (_gameObjects.Contains(gameObject) && !_removeQueue.Contains(gameObject))
                _removeQueue.Add(gameObject);
        }

        public void Update(float deltaTime)
        {
            foreach (var obj in _gameObjects)
                obj.Update(deltaTime);

            CheckCollisions(deltaTime);

            _gameObjects.AddRange(_addQueue);
            _addQueue.Clear();

            foreach (var oldObject in _removeQueue)
                _gameObjects.Remove(oldObject);

            _removeQueue.Clear();
        }

        public void CheckCollisions(float deltaTime)
        {
            foreach (var gameObject in _gameObjects)
            {
                _gameObjects
                    .Where(o => o != gameObject && o.Bounds.Intersects(gameObject.Bounds))
                    .ToList().ForEach(c => gameObject.HandleCollision(c, deltaTime));
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
            return _gameObjects.Where(o => (o is T) && (o != self) && !_removeQueue.Contains(o) && ((self.Position - o.Position).Length() < range)).Cast<T>();
        }

        public int Count<T>() where T : GameObject
        {
            return _gameObjects.Count(go => go is T);
        }
    }
}

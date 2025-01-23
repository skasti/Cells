using System;
using System.Collections.Generic;
using System.Linq;
using Cells.GameObjects;
using Microsoft.Xna.Framework;
using Rectangle = Cells.Geometry.Rectangle;

namespace Cells.QuadTree
{
    public class Node
    {
        const int SplitThreshold = 20;
        const int MinWidth = 250;
        const int MinHeight = 250;
        public Node Parent { get; private set; }
        public List<Node> Children { get; private set; } = new List<Node>();
        public Rectangle Bounds { get; private set; }
        public List<GameObject> Objects { get; private set; } = new List<GameObject>();

        public Node(Rectangle bounds)
        {
            Bounds = bounds;
        }

        public Node(Node parent, Rectangle bounds) : this(bounds)
        {
            Parent = parent;
        }

        public bool Contains(Rectangle bounds)
        {
            return Bounds.Contains(bounds);
        }

        public bool Contains(Vector2 position)
        {
            return Bounds.Contains(position);
        }

        public bool Intersects(Rectangle bounds)
        {
            return Bounds.Intersects(bounds);
        }

        private bool defaultPredicate(GameObject o)
        {
            return true;
        }

/*
public List<GameObject> FindObjects(Vector2 position, float range, Func<GameObject, bool> predicate = null, bool initialCall = true)
        {
            var result = new List<GameObject>();
            if (!Contains(position) && !Intersects(position, range) && !ContainedBy(position, range))
            {
                if (initialCall && Parent != null)
                    return Parent.FindObjects(position, range, predicate);

                return result;
            }

            if (Children == null || Children.Count == 0)
            {
                if (searchBox.Contains(Bounds))
                {
                    if (predicate != null)
                        result.AddRange(Objects.Where(o => predicate(o)));
                    else
                        result.AddRange(Objects.Where(o => predicate(o)));

                    return result;
                }

                foreach (var gameObject in Objects)
                {
                    if (searchBox.Contains(gameObject.Position.ToPoint()))
                    {
                        if (predicate == null || predicate(gameObject))
                            result.Add(gameObject);
                    }
                }
                return result;
            }

            foreach (var childNode in Children)
            {
                result.AddRange(childNode.FindObjects(searchBox, predicate, false));
            }

            return result;
        }

        private bool Intersects(Vector2 position, float range)
        {
            Bounds.Contains()
            throw new NotImplementedException();
        }
        */

        public List<GameObject> FindObjects(Rectangle searchBox, Func<GameObject, bool> predicate = null, bool initialCall = true)
        {
            var result = new List<GameObject>();
            if (!Contains(searchBox) && !Intersects(searchBox) && !searchBox.Contains(Bounds))
            {
                if (initialCall && Parent != null)
                    return Parent.FindObjects(searchBox, predicate);

                return result;
            }

            if (Children == null || Children.Count == 0)
            {
                if (searchBox.Contains(Bounds))
                {
                    if (predicate != null)
                        result.AddRange(Objects.Where(o => predicate(o)));
                    else
                        result.AddRange(Objects.Where(o => predicate(o)));

                    return result;
                }

                foreach (var gameObject in Objects)
                {
                    if (searchBox.Contains(gameObject.Position))
                    {
                        if (predicate == null || predicate(gameObject))
                            result.Add(gameObject);
                    }
                }
                return result;
            }

            foreach (var childNode in Children)
            {
                result.AddRange(childNode.FindObjects(searchBox, predicate, false));
            }

            return result;
        }

        public bool Add(GameObject gameObject)
        {
            if (!Contains(gameObject.Position))
                return false;

            foreach (var childNode in Children)
            {
                if (childNode.Add(gameObject))
                    return true;
            }

            if (Children.Count == 0)
            {
                Objects.Add(gameObject);
                gameObject.CurrentNode = this;

                if (ShouldSplit())
                    Split();

                return true;
            }

            return false;
        }

        public Node UpdateObjectNode(GameObject gameObject, bool depthSearch = false)
        {
            if (Contains(gameObject.Position))
            {
                foreach (var childNode in Children) {
                    var node = childNode.UpdateObjectNode(gameObject, true);
                    if (node != null) {
                        return node;
                    }
                }

                if (!Objects.Contains(gameObject))
                    Objects.Add(gameObject);

                return this;
            } else {
                if (Objects.Contains(gameObject))
                    Objects.Remove(gameObject);

                if (!depthSearch && Parent != null)
                    return Parent.UpdateObjectNode(gameObject, false);
            }

            return null;
        }

        private bool ShouldSplit()
        {
            if ((Children?.Count ?? 0) > 0)
                return false;

            if (Bounds.Width < MinWidth * 2)
                return false;

            if (Bounds.Height < MinHeight * 2)
                return false;

            return Objects.Count >= SplitThreshold;
        }

        private void Split()
        {
            var ChildWidth = Bounds.Width / 2;
            var ChildHeight = Bounds.Height / 2;

            for (var x = 0; x < 2; x++)
            {
                for (var y = 0; y < 2; y++)
                {
                    var node = new Node(new Rectangle(
                        Bounds.X + ChildWidth * x,
                        Bounds.Y + ChildHeight * y,
                        ChildWidth,
                        ChildHeight));

                    Children.Add(node);
                }
            }

            var tempObjects = new List<GameObject>();
            tempObjects.AddRange(Objects);
            Objects.Clear();

            foreach (var gameObject in tempObjects)
            {
                Add(gameObject);
            }
        }

        internal void AddRange(List<GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
                Add(gameObject);
        }

        internal void Remove(GameObject oldObject)
        {
            if (this == oldObject.CurrentNode)
                Objects.Remove(oldObject);
            else
                oldObject.CurrentNode?.Remove(oldObject);
        }
    }
}
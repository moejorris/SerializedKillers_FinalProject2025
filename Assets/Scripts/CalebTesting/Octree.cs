using UnityEngine;
using System.Collections.Generic;


namespace Octrees
{
    public class Octree
    {
        public OctreeNode root;
        public Bounds bounds;

        public Octree(GameObject[] worldObjects, float minNodeSize)
        {
            CalculateBounds(worldObjects);
            CreateTree(worldObjects, minNodeSize);
        }

        void CreateTree(GameObject[] worldObjects, float minNodeSize)
        {
            root = new OctreeNode(bounds, minNodeSize);

            foreach (var obj in worldObjects)
            {
                root.Divide(obj);
            }
        }

        void CalculateBounds(GameObject[] worldObjects)
        {
            foreach (var obj in worldObjects)
            {
                bounds.Encapsulate(obj.GetComponent<Collider>().bounds);
            }

            Vector3 size = Vector3.one * Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) * 0.5f;
            bounds.SetMinMax(bounds.center - size, bounds.center + size);
        }
    }
    public class OctreeNode
    {
        public List<OctreeObject> objects = new();

        static int nextId;
        public readonly int id;

        public Bounds bounds;
        Bounds[] childBounds = new Bounds[8];
        public OctreeNode[] children;
        public bool IsLeaf => children == null;

        float minNodeSize;

        public OctreeNode(Bounds bounds, float minNodeSize)
        {
            id = nextId++;

            this.bounds = bounds;
            this.minNodeSize = minNodeSize;
            Vector3 newSize = bounds.size * 0.5f; // halved size
            Vector3 centerOffset = bounds.size * 0.25f; // quarter offset
            Vector3 parentCenter = bounds.center;

            for (int i = 0; i < 8; i++)
            {
                Vector3 childCenter = parentCenter;
                childCenter.x += centerOffset.x * ((i & 1) == 0 ? -1 : 1);
                childCenter.y += centerOffset.y * ((i & 2) == 0 ? -1 : 1);
                childCenter.z += centerOffset.z * ((i & 4) == 0 ? -1 : 1);
                childBounds[i] = new Bounds(childCenter, newSize);
            }
        }

        public void Divide(GameObject obj) => Divide(new OctreeObject(obj));

        void Divide(OctreeObject octreeObject)
        {
            if (bounds.size.x <= minNodeSize)
            {
                AddObject(octreeObject);
                return;
            }

            children ??= new OctreeNode[8];

            bool intersectChild = false;

            for (int i = 0; i < 8; i++)
            {
                children[i] ??= new OctreeNode(childBounds[i], minNodeSize);

                if (octreeObject.Intersects(childBounds[i]))
                {
                    children[i].Divide(octreeObject);
                    intersectChild = true;
                }
            }

            if (!intersectChild)
            {
                AddObject(octreeObject);
            }
        }

        void AddObject(OctreeObject octreeObject) => objects.Add(octreeObject);

        public void DrawNode()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            foreach (OctreeObject obj in objects)
            {
                if (obj.Intersects(bounds))
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(bounds.center, bounds.size);
                }
            }

            if (children != null)
            {
                foreach (OctreeNode child in children)
                {
                    if (child != null) child.DrawNode();
                }
            }
        }
    }

    public class OctreeObject
    {
        Bounds bounds;

        public OctreeObject(GameObject obj)
        {
            bounds = obj.GetComponent<Collider>().bounds;
        }

        public bool Intersects(Bounds boundsToCheck) => bounds.Intersects(boundsToCheck);
    }
}


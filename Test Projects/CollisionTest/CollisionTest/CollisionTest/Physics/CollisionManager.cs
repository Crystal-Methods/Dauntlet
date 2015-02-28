using CollisionTest.SpriteSystem;
using Microsoft.Xna.Framework;

namespace CollisionTest.Physics
{
    public static class CollisionManager
    {

        public static bool DetectCollision(Entity collider1, Entity collider2)
        {
            var vertices1 = GetVertices(collider1.Bounds); // get verticies of first bounding box
            var axes1 = new Vector2[vertices1.Length]; // obtain separating axes
            for (var i = 0; i < vertices1.Length; i++)
            {
                Vector2 p1 = vertices1[i];
                Vector2 p2 = vertices1[i + 1 == vertices1.Length ? 0 : i + 1];
                Vector2 normal = Perpendicular(p1 - p2);
                normal.Normalize();
                axes1[i] = normal;
            }

            var vertices2 = GetVertices(collider2.Bounds); // get verticies of second bounding box
            var axes2 = new Vector2[vertices2.Length]; // obtain separating axes
            for (var i = 0; i < vertices2.Length; i++)
            {
                Vector2 p1 = vertices2[i];
                Vector2 p2 = vertices2[i + 1 == vertices2.Length ? 0 : i + 1];
                Vector2 normal = Perpendicular(p1 - p2);
                normal.Normalize();
                axes2[i] = normal;
            }

            // loop over the axes1
            for (int i = 0; i < axes1.Length; i++)
            {
                Projection p1 = Project(axes1[i], vertices1);
                Projection p2 = Project(axes1[i], vertices2);
                if (!p1.DoesOverlap(p2)) // if any projection does not overlap, no intersect
                    return false;
            }

            // loop over the axes2
            for (int i = 0; i < axes2.Length; i++)
            {
                Projection p1 = Project(axes2[i], vertices1);
                Projection p2 = Project(axes2[i], vertices2);
                if (!p1.DoesOverlap(p2)) // if any projection does not overlap, no intersect
                    return false;
            }

            // if we get here then we know that every axis had overlap on it
            // so we can guarantee an intersection
            return true;
        }



        private class Projection
        {
            public float Min { private get; set; }
            public float Max { private get; set; }

            public bool DoesOverlap(Projection other)
            {
                if (Max > other.Min)
                    return true;
                if (Min > other.Max)
                    return true;
                return false;
            }
        }

        private static Vector2 Perpendicular(Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        private static Vector2[] GetVertices(Rectangle sourceRect)
        {
            var vertices = new Vector2[4];
            vertices[0] = new Vector2(sourceRect.X, sourceRect.Y);
            vertices[1] = new Vector2(sourceRect.X + sourceRect.Width, sourceRect.Y);
            vertices[2] = new Vector2(sourceRect.X + sourceRect.Width, sourceRect.Y + sourceRect.Height);
            vertices[3] = new Vector2(sourceRect.X, sourceRect.Y + sourceRect.Height);
            return vertices;
        }

        private static Projection Project(Vector2 axis, Vector2[] vertices)
        {
            var min = Vector2.Dot(axis, vertices[0]);
            var max = min;
            for (var i = 1; i < vertices.Length; i++)
            {
                // NOTE: the axis must be normalized to get accurate projections
                var p = Vector2.Dot(axis, vertices[i]);
                if (p < min)
                    min = p;
                else if (p > max)
                    max = p;
            }
            var proj = new Projection {Max = max, Min = min};
            return proj;
        }

        




    }
}

using System;
using CollisionTest.SpriteSystem;
using Microsoft.Xna.Framework;

namespace CollisionTest.Physics
{
    public static class CollisionManager
    {

        public static bool DetectCollision(Entity collider1, Entity collider2)
        {
            // Exit with no intersection if found separated along an axis
            if (collider1.FutureBounds.Bottom < collider2.FutureBounds.Top || collider1.FutureBounds.Top > collider2.FutureBounds.Bottom)
                return false;
            if (collider1.FutureBounds.Right < collider2.FutureBounds.Left || collider1.FutureBounds.Left > collider2.FutureBounds.Right)
                return false;

            // No separating axis found, therefor there is at least one overlapping axis
            return true;
        }

        public static void ResolveCollision(Entity e1, Entity e2)
        {
            // Calculate relative velocity
            Vector2 relVel = e2.Velocity - e1.Velocity;

            var pX1 = new Projection(e1.FutureBounds.Left, e1.FutureBounds.Right);
            var pX2 = new Projection(e2.FutureBounds.Left, e2.FutureBounds.Right);
            var pY1 = new Projection(e1.FutureBounds.Top, e1.FutureBounds.Bottom);
            var pY2 = new Projection(e2.FutureBounds.Top, e2.FutureBounds.Bottom);

            float pen1 = GetPenetration(pX1, pX2);
            float pen2 = GetPenetration(pY1, pY2);

            float penetration = Math.Min(pen1, pen2);
            Vector2 normal = Math.Abs(penetration - pen1) < 0.000001 ? new Vector2(1, 0) : new Vector2(0, 1);

            // Calculate relative velocity in terms of the normal direction
            float velAlongNormal = Vector2.Dot(relVel, normal);

            // Do not resolve if velocities are separating
            //if (velAlongNormal > 0)
            //    return;

            // Calculate restitution
            //float e = Math.Min(A.restitution, B.restitution);

            // Calculate impulse scalar
            //float j = -(1 + e)*velAlongNormal;
            float j = -velAlongNormal;
            j /= e1.InvMass + e2.InvMass;

            // Apply impulse
            Vector2 impulse = j*normal;
            e1.Velocity -= e1.InvMass*impulse;
            e2.Velocity += e2.InvMass*impulse;
        }

        internal struct Projection
        {
            private float _min;
            private float _max;

            public float Min
            {
                get { return _min; }
                set { _min = value; }
            }

            public float Max
            {
                get { return _max; }
                set { _max = value; }
            }

            public Projection(float min, float max)
            {
                _min = min;
                _max = max;
            }
        }

        internal static float GetPenetration(Projection p1, Projection p2)
        {
            if (p1.Max > p2.Max && p1.Min < p2.Min) // if p1 contains p2
            {
                float a = Math.Abs(p1.Min - p2.Min);
                float b = Math.Abs(p1.Max - p2.Max);
                return (p2.Max - p2.Min) + Math.Min(a, b);
            }
            if (p2.Max > p1.Max && p2.Min < p1.Min) // if p2 contains p1
            {
                float a = Math.Abs(p1.Min - p2.Min);
                float b = Math.Abs(p1.Max - p2.Max);
                return (p1.Max - p1.Min) + Math.Min(a, b);
            }
            return Math.Abs(Math.Max(p1.Min, p2.Min) - Math.Min(p1.Max, p2.Max));
        }

    }
}

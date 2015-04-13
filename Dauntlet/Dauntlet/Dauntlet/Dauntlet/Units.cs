using System;
using FarseerPhysics;
using Microsoft.Xna.Framework;

namespace Dauntlet
{
    public static class Units
    {
        /// <summary>
        /// Returns the integer converted from display to sim units
        /// </summary>
        public static float Sim(this int x)
        {
            return ConvertUnits.ToSimUnits(x);
        }

        /// <summary>
        /// Returns the integer converted from sim to display units
        /// </summary>
        public static float Dis(this int x)
        {
            return ConvertUnits.ToDisplayUnits(x);
        }

        /// <summary>
        /// Returns the float converted from display to sim units
        /// </summary>
        public static float Sim(this float x)
        {
            return ConvertUnits.ToSimUnits(x);
        }

        /// <summary>
        /// Returns the float converted from sim to display units
        /// </summary>
        public static float Dis(this float x)
        {
            return ConvertUnits.ToDisplayUnits(x);
        }

        /// <summary>
        /// Returns the Vector converted from display to sim units
        /// </summary>
        public static Vector2 Sim(this Vector2 v)
        {
            return ConvertUnits.ToSimUnits(v);
        }

        /// <summary>
        /// Returns the Vector converted from sim to display units
        /// </summary>
        public static Vector2 Dis(this Vector2 v)
        {
            return ConvertUnits.ToDisplayUnits(v);
        }

        /// <summary>
        /// Creates a new Vector2 in sim units from inputs in display units
        /// </summary>
        public static Vector2 SimVector(float x, float y)
        {
            return new Vector2(ConvertUnits.ToSimUnits(x), ConvertUnits.ToSimUnits(y));
        }

        /// <summary>
        /// Creates a new Vector2 in display units from inputs in sim units
        /// </summary>
        public static Vector2 DisVector(float x, float y)
        {
            return new Vector2(ConvertUnits.ToDisplayUnits(x), ConvertUnits.ToDisplayUnits(y));
        }

    }
}

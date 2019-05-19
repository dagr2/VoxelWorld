using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuesSpielc
{
    class WorldGenerator
    {
        private static Godot.OpenSimplexNoise noise = new Godot.OpenSimplexNoise() {
            Octaves=4,
            Period=4,
            Seed=0,
            Lacunarity=1,
            Persistence=1
        };
        private static int HEIGHT = 128;
        public static int Height { get{ return HEIGHT; } set { HEIGHT = value; } }
        public static float Stretch { get; set; } = 0.5f;

        public static float HeightAt(int x, int y)
        {
            float r = (1 + noise.GetNoise2d(x * Stretch, y * Stretch)) / 2 * Height;
            return r;
        }

        public static float HeightAt(float x, float y)
        {
            float r = (1 + noise.GetNoise2d(x * Stretch, y * Stretch)) / 2 * Height;
            return r;
        }

        public static int GetBlock(int x, int y, int z)
        {
            int res = 0;
            float r = HeightAt(x, z);
            if (y < r || y <= 0) res = 1;
            return res;
        }
    }
}

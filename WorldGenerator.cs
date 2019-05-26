using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuesSpielc
{
    class WorldGenerator
    {
        public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static Godot.OpenSimplexNoise noise = new Godot.OpenSimplexNoise() {
            Octaves=4,
            Period=4,
            Seed=0,
            Lacunarity=1,
            Persistence=1
        };
        private static Godot.OpenSimplexNoise noise2 = new Godot.OpenSimplexNoise() {
            Octaves=4,
            Period=2,
            Seed=0,
            Lacunarity=1,
            Persistence=1
        };
        private static int HEIGHT = 128;
        public static int Height { get{ return HEIGHT; } set { HEIGHT = value; } }
        public static float Stretch { get; set; } = 0.1f;

        public static float HeightAt(int x, int y)
        {
            float r = (1 + noise.GetNoise2d(x * Stretch, y * Stretch)) / 2 * (Height-1);
            return 5;
        }

        public static float HeightAt(float x, float y)
        {
            float r = (1 + noise.GetNoise2d(x * Stretch, y * Stretch)) / 2 * Height;
            return r;
        }

        public static int GetBlock(int x, int y, int z)
        {			
            int res = 0;
            
            float r = (float)Math.Round(HeightAt(x, z));
            if (y < r || y <= 0)
            {                
                if (y==(int)r) {
                    float n=noise2.GetNoise2d(x,z);
                    res = (int)Math.Round((n+1)/2 + 1 );
                }
                else res=1;
            }

/*
            float r = (noise.GetNoise3d(x * Stretch, y * Stretch, z * Stretch) + 1) / 2;
            int res = (int)Math.Round(r);
            if (y < 2 || y > 2 * Height - 2) res = 1;
*/
            return res;
        }
    }
}

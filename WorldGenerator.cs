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

        public static int GetBlock(int x, int y, int z)
        {
            int res = 0;
            //float r = Height / 2;
            float r = (1 + noise.GetNoise2d(x*Stretch, z* Stretch)) / 2 * Height;
    

            if (y < r || y <= 0) res = 1;
            //if (x+y+z==0) Godot.GD.Print(x,", ", y,", ", z,", ", r.ToString().Replace(",","."));
            return res;
        }
    }
}

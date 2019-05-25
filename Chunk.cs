using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuesSpielc
{
    public class Chunk:StaticBody
    {
        public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private int _x = 0;
        private int _y = 0;
        private MeshInstance mi;// = new MeshInstance();
        private CollisionShape cs;
        private ConcavePolygonShape ccs;
        public bool mesh_ready = false;
        private Octet oct;
        private bool dirty = true;

        Dictionary<string, int> blocks = new Dictionary<string, int>();

        public int GetBlock(int x, int y, int z)
        {
            //return WorldGenerator.GetBlock(x ,y,z);
            return oct.GetValue(new Vector3(x, y, z));
        }

        public void SetBlock(int x, int y, int z, int v)
        {
            mesh_ready = false;
            oct.SetValue(new Vector3(x, y, z), v);
            dirty = true;
            //blocks[x + "," + y + "," + z] = v;
            ////mesh_ready = false;
            //GD.Print(String.Format("Setting block at {0},{1},{2} to {3}", x, y, z, v));
            //CalcMesh();
        }

        public Chunk(int x, int y)
        {
            _x = x;_y = y;
            oct = new Octet(new Vector3(x, 0, y), new Vector3(16, 16, 16));
            mi = new MeshInstance();
            AddChild(mi);
            CollisionLayer = 3;
            cs = new CollisionShape();
            ccs = new ConcavePolygonShape();
            cs.SetShape(ccs);
            AddChild(cs);

            //for (int i=0;i<16;i++) oct.SetValue(new Vector3(x+i, 0, y+i), 1);
            //oct.SetValue(new Vector3(15, 0, 15), 1);
            //CalcMesh();
        }

        public override void _Ready() {

        }

        public void SetMaterial(Material mat)
        {
            mi.MaterialOverride = mat;
        }

        private Vector2 GetTexCoords(int bt, int side, int vertX, int vertY)
        {
            float ts = 16.0f;
            float t = 1.0f / ts;
            float tx = 1.0f / 6.0f;
            float ty = 1.0f / 20.0f;
            float o = 0.0f;
            Vector2 res = new Vector2(0, 0);
            res.x = tx * (side + vertX);
            res.y = ty * (bt-1+(1-vertY));
            return res;
        }

        int f = 16;
        
        public void CalcMesh() {
            long start = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            dirty = false;
            mesh_ready = true;
            Block[] blocks = oct.GetBlocks();
            SurfaceTool st = new SurfaceTool();
            float s = 0.5f;
            st.Clear();
            st.Begin(Mesh.PrimitiveType.Triangles);
            List<Vector3> faces = new List<Vector3>();
            World w=World.world;

            foreach (Block b in blocks)
            {
                int x = (int)Math.Truncate(b.Pos.x); int y = (int)Math.Truncate(b.Pos.y); int z = (int)Math.Truncate(b.Pos.z);
                int bv = b.Val;// oct.GetBlock(x + f * _x, y, z + f * _y);
                //bv = GetBlock(x + f * _x, y, z + f * _y);
                int bTop = w.GetBlock(x + f * _x, y + 1, z + f * _y);
                int bBottom = w.GetBlock(x + f * _x, y - 1, z + f * _y);
                int bLeft = w.GetBlock(x + f * _x - 1, y, z + f * _y);
                int bRight = w.GetBlock(x + f * _x + 1, y, z + f * _y);
                int bFront = w.GetBlock(x + f * _x, y, z + f * _y - 1);
                int bBack = w.GetBlock(x + f * _x, y, z + f * _y + 1);
                float tex = 0; float t = 1.0f / 8.0f;
                float x1, y2 = t * (bv - 1.0f), x2, y1 = t * bv;
                
                // TOP
                if (bv > 0 && bTop < 1)
                {
                    //GD.Print("top face at ", b.Pos);
                    //if (bv > 0) GD.Print("1 at ", b.Pos);
                    x1 = 0 * t; x2 = 1 * t;

                    st.AddUv(GetTexCoords(bv, 0, 0, 0));
                    st.AddVertex(new Vector3(x - s, y + s, z - s));
                    faces.Add(new Vector3(x - s, y + s, z - s));

                    st.AddUv(GetTexCoords(bv, 0, 1, 0));
                    st.AddVertex(new Vector3(x + s, y + s, z - s));
                    faces.Add(new Vector3(x + s, y + s, z - s));

                    st.AddUv(GetTexCoords(bv, 0, 0, 1));
                    st.AddVertex(new Vector3(x - s, y + s, z + s));
                    faces.Add(new Vector3(x - s, y + s, z + s));

                    st.AddUv(GetTexCoords(bv, 0, 1, 1));
                    st.AddVertex(new Vector3(x + s, y + s, z + s));
                    faces.Add(new Vector3(x + s, y + s, z + s));

                    st.AddUv(GetTexCoords(bv, 0, 0, 1));
                    st.AddVertex(new Vector3(x - s, y + s, z + s));
                    faces.Add(new Vector3(x - s, y + s, z + s));

                    st.AddUv(GetTexCoords(bv, 0, 1, 0));
                    st.AddVertex(new Vector3(x + s, y + s, z - s));
                    faces.Add(new Vector3(x + s, y + s, z - s));
                }

                // Bottom
                if (bv > 0 && bBottom < 1)
                {
                    //GD.Print("Bottom face at ", b.Pos);
                    st.AddUv(GetTexCoords(bv, 1, 0, 0));
                    st.AddVertex(new Vector3(x - s, y - s, z - s));
                    faces.Add(new Vector3(x - s, y - s, z - s));

                    st.AddUv(GetTexCoords(bv, 1, 1, 0));
                    st.AddVertex(new Vector3(x - s, y - s, z + s));
                    faces.Add(new Vector3(x - s, y - s, z + s));

                    st.AddUv(GetTexCoords(bv, 1, 0, 1));
                    st.AddVertex(new Vector3(x + s, y - s, z - s));
                    faces.Add(new Vector3(x + s, y - s, z - s));

                    st.AddUv(GetTexCoords(bv, 1, 1, 1));
                    st.AddVertex(new Vector3(x + s, y - s, z + s));
                    faces.Add(new Vector3(x + s, y - s, z + s));

                    st.AddUv(GetTexCoords(bv, 1, 0, 1));
                    st.AddVertex(new Vector3(x + s, y - s, z - s));
                    faces.Add(new Vector3(x + s, y - s, z - s));

                    st.AddUv(GetTexCoords(bv, 1, 1, 0));
                    st.AddVertex(new Vector3(x - s, y - s, z + s));
                    faces.Add(new Vector3(x - s, y - s, z + s));
                }

                // Front
                if (bv > 0 && bFront < 1)
                {
                    x1 = 2 * t; x2 = 3 * t;

                    st.AddUv(GetTexCoords(bv, 2, 0, 0));
                    st.AddVertex(new Vector3(x - s, y - s, z - s));
                    faces.Add(new Vector3(x - s, y - s, z - s));

                    st.AddUv(GetTexCoords(bv, 2, 1, 0));
                    st.AddVertex(new Vector3(x + s, y - s, z - s));
                    faces.Add(new Vector3(x + s, y - s, z - s));

                    st.AddUv(GetTexCoords(bv, 2, 0, 1));
                    st.AddVertex(new Vector3(x - s, y + s, z - s));
                    faces.Add(new Vector3(x - s, y + s, z - s));

                    st.AddUv(GetTexCoords(bv, 2, 1, 1));
                    st.AddVertex(new Vector3(x + s, y + s, z - s));
                    faces.Add(new Vector3(x + s, y + s, z - s));

                    st.AddUv(GetTexCoords(bv, 2, 0, 1));
                    st.AddVertex(new Vector3(x - s, y + s, z - s));
                    faces.Add(new Vector3(x - s, y + s, z - s));

                    st.AddUv(GetTexCoords(bv, 2, 1, 0));
                    st.AddVertex(new Vector3(x + s, y - s, z - s));
                    faces.Add(new Vector3(x + s, y - s, z - s));
                }

                // Back
                if (bv > 0 && bBack < 1)
                {
                    x1 = 3 * t; x2 = 4 * t;

                    st.AddUv(GetTexCoords(bv, 3, 1, 0));
                    st.AddVertex(new Vector3(x - s, y - s, z + s));
                    faces.Add(new Vector3(x - s, y - s, z + s));

                    st.AddUv(GetTexCoords(bv, 3, 1, 1));
                    st.AddVertex(new Vector3(x - s, y + s, z + s));
                    faces.Add(new Vector3(x - s, y + s, z + s));

                    st.AddUv(GetTexCoords(bv, 3, 0, 0));
                    st.AddVertex(new Vector3(x + s, y - s, z + s));
                    faces.Add(new Vector3(x + s, y - s, z + s));

                    st.AddUv(GetTexCoords(bv, 3, 0, 1));
                    st.AddVertex(new Vector3(x + s, y + s, z + s));
                    faces.Add(new Vector3(x + s, y + s, z + s));

                    st.AddUv(GetTexCoords(bv, 3, 0, 0));
                    st.AddVertex(new Vector3(x + s, y - s, z + s));
                    faces.Add(new Vector3(x + s, y - s, z + s));

                    st.AddUv(GetTexCoords(bv, 3, 1, 1));
                    st.AddVertex(new Vector3(x - s, y + s, z + s));
                    faces.Add(new Vector3(x - s, y + s, z + s));
                }


                // Left
                if (bv > 0 && bLeft < 1)
                {
                    x1 = 4 * t; x2 = 5 * t;

                    st.AddUv(GetTexCoords(bv, 4, 1, 0));
                    st.AddVertex(new Vector3(x - s, y - s, z - s));
                    faces.Add(new Vector3(x - s, y - s, z - s));

                    st.AddUv(GetTexCoords(bv, 4, 1, 1));
                    st.AddVertex(new Vector3(x - s, y + s, z - s));
                    faces.Add(new Vector3(x - s, y + s, z - s));

                    st.AddUv(GetTexCoords(bv, 4, 0, 0));
                    st.AddVertex(new Vector3(x - s, y - s, z + s));
                    faces.Add(new Vector3(x - s, y - s, z + s));

                    st.AddUv(GetTexCoords(bv, 4, 0, 1));
                    st.AddVertex(new Vector3(x - s, y + s, z + s));
                    faces.Add(new Vector3(x - s, y + s, z + s));

                    st.AddUv(GetTexCoords(bv, 4, 0, 0));
                    st.AddVertex(new Vector3(x - s, y - s, z + s));
                    faces.Add(new Vector3(x - s, y - s, z + s));

                    st.AddUv(GetTexCoords(bv, 4, 1, 1));
                    st.AddVertex(new Vector3(x - s, y + s, z - s));
                    faces.Add(new Vector3(x - s, y + s, z - s));
                }


                // Right
                if (bv > 0 && bRight < 1)
                {
                    st.AddUv(GetTexCoords(bv, 5, 0, 0));
                    st.AddVertex(new Vector3(x + s, y - s, z - s));
                    faces.Add(new Vector3(x + s, y - s, z - s));

                    st.AddUv(GetTexCoords(bv, 5, 1, 0));
                    st.AddVertex(new Vector3(x + s, y - s, z + s));
                    faces.Add(new Vector3(x + s, y - s, z + s));

                    st.AddUv(GetTexCoords(bv, 5, 0, 1));
                    st.AddVertex(new Vector3(x + s, y + s, z - s));
                    faces.Add(new Vector3(x + s, y + s, z - s));

                    st.AddUv(GetTexCoords(bv, 5, 1, 1));
                    st.AddVertex(new Vector3(x + s, y + s, z + s));
                    faces.Add(new Vector3(x + s, y + s, z + s));

                    st.AddUv(GetTexCoords(bv, 5, 0, 1));
                    st.AddVertex(new Vector3(x + s, y + s, z - s));
                    faces.Add(new Vector3(x + s, y + s, z - s));

                    st.AddUv(GetTexCoords(bv, 5, 1, 0));
                    st.AddVertex(new Vector3(x + s, y - s, z + s));
                    faces.Add(new Vector3(x + s, y - s, z + s));
                }
            }
            st.GenerateNormals();
            mi.Mesh = st.Commit();
            try
            {
                //ccs.SetFaces(mi.Mesh.GetFaces());
                ccs.SetFaces(faces.ToArray());
            }
            catch (Exception ex)
            {
                World.log(ex.Message);
            }
            mesh_ready = true;
            long stop = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            long dur = stop - start;
            World.log("Drawed " + blocks.Length + " blocks with "+faces.Count+" faces in ",dur,"ms");
        }

        public void _CalcMesh()
        {
            SurfaceTool st = new SurfaceTool();
            float s = 0.5f;
            st.Clear();
            st.Begin(Mesh.PrimitiveType.Triangles);
            List<Vector3> faces = new List<Vector3>();
          
            for (int y=0;y<WorldGenerator.Height;y++)
                for (int z=0;z<16;z++)
                    for (int x = 0; x < 16; x++)
                    {
                        int bv = WorldGenerator.GetBlock(x + f * _x, y,  z + f * _y);
                        bv = GetBlock(x + f * _x, y, z + f * _y);
                        int bTop = WorldGenerator.GetBlock(x + f * _x, y+1, z + f * _y);
                        int bBottom = WorldGenerator.GetBlock(x + f * _x, y-1, z + f * _y);
                        int bLeft = WorldGenerator.GetBlock(x + f * _x -1, y, z + f * _y);
                        int bRight = WorldGenerator.GetBlock(x + f * _x +1, y, z + f * _y);
                        int bFront = WorldGenerator.GetBlock(x + f * _x, y, z + f * _y -1);
                        int bBack = WorldGenerator.GetBlock(x + f * _x, y, z + f * _y +1);
                        float tex = 0;float t = 1.0f / 8.0f;
                        float x1, y2=t*(bv-1.0f), x2, y1=t*bv;

                        // TOP
                        if (bv > 0 && bTop < 1)
                        {
                            x1 = 0 * t; x2 = 1 * t;

                            st.AddUv(GetTexCoords(bv,0,0,0));
                            st.AddVertex(new Vector3(x - s, y + s, z - s));
                            faces.Add(new Vector3(x - s, y + s, z - s));

                            st.AddUv(GetTexCoords(bv, 0, 1, 0));
                            st.AddVertex(new Vector3(x + s, y + s, z - s));
                            faces.Add(new Vector3(x + s, y + s, z - s));

                            st.AddUv(GetTexCoords(bv, 0, 0, 1));
                            st.AddVertex(new Vector3(x - s, y + s, z + s));
                            faces.Add(new Vector3(x - s, y + s, z + s));

                            st.AddUv(GetTexCoords(bv, 0, 1, 1));
                            st.AddVertex(new Vector3(x + s, y + s, z + s));
                            faces.Add(new Vector3(x + s, y + s, z + s));

                            st.AddUv(GetTexCoords(bv, 0, 0, 1));
                            st.AddVertex(new Vector3(x - s, y + s, z + s));
                            faces.Add(new Vector3(x - s, y + s, z + s));

                            st.AddUv(GetTexCoords(bv, 0, 1, 0));
                            st.AddVertex(new Vector3(x + s, y + s, z - s));
                            faces.Add(new Vector3(x + s, y + s, z - s));
                        }

                        // Bottom
                        if (bv > 0 && bBottom < 1)
                        {
                            st.AddUv(GetTexCoords(bv, 1, 0, 0));
                            st.AddVertex(new Vector3(x - s, y - s, z - s));
                            faces.Add(new Vector3(x - s, y - s, z - s));

                            st.AddUv(GetTexCoords(bv, 1, 1, 0));
                            st.AddVertex(new Vector3(x - s, y - s, z + s));
                            faces.Add(new Vector3(x - s, y - s, z + s));

                            st.AddUv(GetTexCoords(bv, 1, 0, 1));
                            st.AddVertex(new Vector3(x + s, y - s, z - s));
                            faces.Add(new Vector3(x + s, y - s, z - s));

                            st.AddUv(GetTexCoords(bv, 1, 1, 1));
                            st.AddVertex(new Vector3(x + s, y - s, z + s));
                            faces.Add(new Vector3(x + s, y - s, z + s));

                            st.AddUv(GetTexCoords(bv, 1, 0, 1));
                            st.AddVertex(new Vector3(x + s, y - s, z - s));
                            faces.Add(new Vector3(x + s, y - s, z - s));

                            st.AddUv(GetTexCoords(bv, 1, 1, 0));
                            st.AddVertex(new Vector3(x - s, y - s, z + s));
                            faces.Add(new Vector3(x - s, y - s, z + s));
                        }

                        // Front
                        if (bv > 0 && bFront < 1)
                        {
                            x1 = 2 * t; x2 = 3 * t;

                            st.AddUv(GetTexCoords(bv, 2, 0, 0));
                            st.AddVertex(new Vector3(x - s, y - s, z - s));
                            faces.Add(new Vector3(x - s, y - s, z - s));

                            st.AddUv(GetTexCoords(bv, 2, 1, 0));
                            st.AddVertex(new Vector3(x + s, y - s, z - s));
                            faces.Add(new Vector3(x + s, y - s, z - s));

                            st.AddUv(GetTexCoords(bv, 2, 0, 1));
                            st.AddVertex(new Vector3(x - s, y + s, z - s));
                            faces.Add(new Vector3(x - s, y + s, z - s));

                            st.AddUv(GetTexCoords(bv, 2, 1, 1));
                            st.AddVertex(new Vector3(x + s, y + s, z - s));
                            faces.Add(new Vector3(x + s, y + s, z - s));

                            st.AddUv(GetTexCoords(bv, 2, 0, 1));
                            st.AddVertex(new Vector3(x - s, y + s, z - s));
                            faces.Add(new Vector3(x - s, y + s, z - s));

                            st.AddUv(GetTexCoords(bv, 2, 1, 0));
                            st.AddVertex(new Vector3(x + s, y - s, z - s));
                            faces.Add(new Vector3(x + s, y - s, z - s));
                        }

                        // Back
                        if (bv > 0 && bBack < 1)
                        {
                            x1 = 3 * t; x2 = 4 * t;

                            st.AddUv(GetTexCoords(bv, 3, 1, 0));
                            st.AddVertex(new Vector3(x - s, y - s, z + s));
                            faces.Add(new Vector3(x - s, y - s, z + s));

                            st.AddUv(GetTexCoords(bv, 3, 1, 1));
                            st.AddVertex(new Vector3(x - s, y + s, z + s));
                            faces.Add(new Vector3(x - s, y + s, z + s));

                            st.AddUv(GetTexCoords(bv, 3, 0, 0));
                            st.AddVertex(new Vector3(x + s, y - s, z + s));
                            faces.Add(new Vector3(x + s, y - s, z + s));

                            st.AddUv(GetTexCoords(bv, 3, 0, 1));
                            st.AddVertex(new Vector3(x + s, y + s, z + s));
                            faces.Add(new Vector3(x + s, y + s, z + s));

                            st.AddUv(GetTexCoords(bv, 3, 0, 0));
                            st.AddVertex(new Vector3(x + s, y - s, z + s));
                            faces.Add(new Vector3(x + s, y - s, z + s));

                            st.AddUv(GetTexCoords(bv, 3, 1, 1));
                            st.AddVertex(new Vector3(x - s, y + s, z + s));
                            faces.Add(new Vector3(x - s, y + s, z + s));
                        }


                        // Left
                        if (bv > 0 && bLeft < 1)
                        {
                            x1 = 4 * t; x2 = 5 * t;

                            st.AddUv(GetTexCoords(bv, 4, 1, 0));
                            st.AddVertex(new Vector3(x - s, y - s, z - s));
                            faces.Add(new Vector3(x - s, y - s, z - s));

                            st.AddUv(GetTexCoords(bv, 4, 1, 1));
                            st.AddVertex(new Vector3(x - s, y + s, z - s));
                            faces.Add(new Vector3(x - s, y + s, z - s));

                            st.AddUv(GetTexCoords(bv, 4, 0, 0));
                            st.AddVertex(new Vector3(x - s, y - s, z + s));
                            faces.Add(new Vector3(x - s, y - s, z + s));

                            st.AddUv(GetTexCoords(bv, 4, 0, 1));
                            st.AddVertex(new Vector3(x - s, y + s, z + s));
                            faces.Add(new Vector3(x - s, y + s, z + s));

                            st.AddUv(GetTexCoords(bv, 4, 0, 0));
                            st.AddVertex(new Vector3(x - s, y - s, z + s));
                            faces.Add(new Vector3(x - s, y - s, z + s));

                            st.AddUv(GetTexCoords(bv, 4, 1, 1));
                            st.AddVertex(new Vector3(x - s, y + s, z - s));
                            faces.Add(new Vector3(x - s, y + s, z - s));
                        }


                        // Right
                        if (bv > 0 && bRight < 1)
                        {
                            st.AddUv(GetTexCoords(bv, 5, 0, 0));
                            st.AddVertex(new Vector3(x + s, y - s, z - s));
                            faces.Add(new Vector3(x + s, y - s, z - s));

                            st.AddUv(GetTexCoords(bv, 5, 1, 0));
                            st.AddVertex(new Vector3(x + s, y - s, z + s));
                            faces.Add(new Vector3(x + s, y - s, z + s));

                            st.AddUv(GetTexCoords(bv, 5, 0, 1));
                            st.AddVertex(new Vector3(x + s, y + s, z - s));
                            faces.Add(new Vector3(x + s, y + s, z - s));

                            st.AddUv(GetTexCoords(bv, 5, 1, 1));
                            st.AddVertex(new Vector3(x + s, y + s, z + s));
                            faces.Add(new Vector3(x + s, y + s, z + s));

                            st.AddUv(GetTexCoords(bv, 5, 0, 1));
                            st.AddVertex(new Vector3(x + s, y + s, z - s));
                            faces.Add(new Vector3(x + s, y + s, z - s));

                            st.AddUv(GetTexCoords(bv, 5, 1, 0));
                            st.AddVertex(new Vector3(x + s, y - s, z + s));
                            faces.Add(new Vector3(x + s, y - s, z + s));
                        }
                    }

            st.GenerateNormals();
            mi.Mesh = st.Commit();
            try
            {
                //ccs.SetFaces(mi.Mesh.GetFaces());
                ccs.SetFaces(faces.ToArray());
            } catch(Exception ex)
            {
                World.log(ex.Message);
            }
            mesh_ready = true;
            if (World.log_on) World.log("Drawed ", blocks.Count, " block with ", faces.Count, " verticies (", faces.Count / 3, " quads) in");
        }
        
    }
}

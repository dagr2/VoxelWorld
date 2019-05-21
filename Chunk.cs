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
        private int _x = 0;
        private int _y = 0;
        private MeshInstance mi;// = new MeshInstance();
        private CollisionShape cs;
        private ConcavePolygonShape ccs;
        public bool mesh_ready = false;

        Dictionary<string, int> blocks = new Dictionary<string, int>();

        public int GetBlock(int x, int y, int z)
        {
            if (blocks.Keys.Contains(x + "," + y + "," + z))
            {
                GD.Print("Have block at "+ x + "," + y + "," + z);
                return blocks[x + "," + y + "," + z];
            }
           // if (mesh_ready) GD.Print("Getting block from gd");
            return WorldGenerator.GetBlock(x ,y,z);
        }

        public void SetBlock(int x, int y, int z, int v)
        {
            blocks[x + "," + y + "," + z] = v;
            //mesh_ready = false;
            GD.Print(String.Format("Setting block at {0},{1},{2} to {3}", x, y, z, v));
            CalcMesh();
            
        }

        public Chunk(int x, int y)
        {
            _x = x;_y = y;

            mi = new MeshInstance();
            AddChild(mi);
            cs = new CollisionShape();
            ccs = new ConcavePolygonShape();
            cs.SetShape(ccs);
            AddChild(cs);
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
            
            //if (vertY==0) vertY=1; else vertY=0;

            Vector2 res = new Vector2(0, 0);
            res.x = tx * (side + vertX);
            res.y = ty * (bt-1+(1-vertY));

            //if (side == 3) res = res.Rotated((float)Math.PI / 2.0f);
            
            //if (side==3){float tmp=res.x;res.x=res.y;res.y=tmp;}

            //return new Vector2(vertX, vertY);
            return res;
        }
        int f = 16;
        
        public void CalcMesh()
        {SurfaceTool st = new SurfaceTool();
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
                Spatial.log(ex.Message);
            }
            mesh_ready = true;
        }
        
    }
}

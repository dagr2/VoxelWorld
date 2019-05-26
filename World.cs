using Godot;
using NeuesSpielc;
using System;
using System.Collections.Generic;

public class World : Godot.Spatial
{
    public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    Dictionary<string,Chunk> chunks=new Dictionary<string,Chunk>();
    SpatialMaterial mat = (SpatialMaterial)ResourceLoader.Load("res://mat1.tres");
    public int vischunks = 1;
    public int Height = 64;
    public static bool log_on = true;

    public World()
    {
        world = this;
    }

    public float HeightAt(float x, float y)
    {
        return 8;
        return WorldGenerator.HeightAt(x, y);
    }

    public override void _Ready()
    {
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

        WorldGenerator.Height = Height;
        WorldGenerator.Stretch = 0.09f;
        KinematicBody player = (KinematicBody)FindNode("Player");
        
        //IsoSurface iso=new IsoSurface();
        //AddChild(iso);

        //for (int y = -vischunks; y < vischunks+1; y++) for (int x = -vischunks; x < vischunks+1; x++) AddChunk(x, y);
        try
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(CalcMeshes));
            t.Priority = System.Threading.ThreadPriority.Highest;
            t.Start();
            
        } catch (Exception ex)
        {
            GD.PrintErr(ex.Message);
        }
        /*
        Thread t = new Thread();
        GD.Print(t.Start(this, "CalcMeshes"));
        */
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        logger.Error("Exception! {0}", e);
    }

    public static void log(string m)
    {
        // Console.WriteLine(m);
        //if (log_on) GD.Print(m);
    }
    public static void log(params object[] o)
    {
        // Console.WriteLine(m);
        //if (log_on) GD.Print(o);
        logger.Debug(o);
    }
    public void Debug(string s) { logger.Debug(s); }
    public static World world;
    private void TestAndCreateChunk(int x, int y)
    {
        logger.Trace("TestAndCreateChunk at {0},{1} ", x, y);
        if (!chunks.ContainsKey(x + "," + y))
        {
            //log("Adding chunk " + x + "," + y);
            Chunk c=AddChunk(x, y);
            WorldGenerator.Height = 10;
            bool l = log_on;
            log_on = false;
            for (int px = 0; px < 16; px++) for (int pz = 0; pz < 16; pz++)
                {
                    float h = WorldGenerator.HeightAt(x*16 + px, y*16 + pz);
                    for (int i = 0; i <= h; i++)
                    {
                        SetBlock(x*16 + px, i, y*16 + pz, 1);
                    }
                    //oct.SetValue(new Vector3(x + px, 14, y + pz), 1);
                }
            log_on = l;
        }
    }
    object lck=new object();

    public void CalcMeshes()
    {
        //GD.Print("CalcMeshes");
        KinematicBody player = (KinematicBody)FindNode("Player");
        int ocx = 0;int ocy = 0;
        while (true)
        try {
                System.Threading.Thread.Sleep(100);
            Vector3 ppos = player.Translation;
            int cx = (int)Math.Truncate((ppos.x) / 16.0);
            int cy = (int)Math.Truncate((ppos.z) / 16.0);

            if (ocx != cx || ocy != cy)
            {
// GD.Print("player at chunk " + cx + "," + cy);
                }
            Label lbl = (Label)FindNode("Label");
            if (lbl != null) lbl.Text = "player at chunk " + cx + "," + cy;

            ocx = cx;ocy = cy;
            for (int r = 0; r < vischunks;r++) {
                for (int r2 = -r; r2 < r+1 ; r2++)
                {
                    TestAndCreateChunk(cx - r2, cy - r);
                    TestAndCreateChunk(cx - r2, cy + r);
                    TestAndCreateChunk(cx - r, cy - r2);
                    TestAndCreateChunk(cx + r, cy - r2);
                }
            }
            lock(lck) foreach (Chunk c in chunks.Values)
            {
                try
                {
                    if (!c.mesh_ready) c.CalcMesh();
                }
                catch (Exception ex)
                {
                    GD.PrintErr("Exception: " + ex.Message);
                }
            }
                AddChilds();
        } catch(Exception e) { GD.PrintErr(e); }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        //((Light)FindNode("Sun")).RotateX(delta / 2.0f);
    }

    public void AddChilds()
    {
        foreach (Chunk c in chunks.Values)
        {
            if (c.mesh_ready)
                if (!GetChildren().Contains(c)) AddChild(c);
        }
    }

    public int GetBlock(int x,int y,int z)
    {
        int res = 0;
        if (y <= WorldGenerator.HeightAt(x, z)) res = 1;

        int cx = (int)Math.Truncate(x / 16.0);
        int cy = (int)Math.Truncate(z / 16.0);

        if (chunks.ContainsKey(cx + "," + cy)) res= chunks[cx + "," + cy].GetBlock(x,y,z);
        return res;
    }

    public void SetBlock(int x,int y,int z, int v)
    {
        int cx = (int)Math.Truncate(x / 16.0f);
        int cy = (int)Math.Truncate(z / 16.0f);
        if (z<0 && z%16!=0) cy-=1;
        if (x<0 && x%16!=0) cx-=1;
        //if (cx<0)x+=1; if (cy<0)z+=1;
        logger.Info("Set block at {0},{1},{2} with cx={3} and cy={4}", x, y, z,cx,cy);

        if (!chunks.ContainsKey(cx + "," + cy)) AddChunk(cx, cy);
        if (chunks.ContainsKey(cx + "," + cy))
        {
            Chunk c = chunks[cx + "," + cy];
            c.SetBlock(x, y, z, v);
        }
        
    }
    public Chunk AddChunk(int x, int y)
    {
        float cx = x * 16.0f;float cy = y * 16.0f;
        int icx = (int)Math.Truncate(cx); int icy = (int)Math.Truncate(cy);
        //if (x < 0) icx++; if (y < 0) icy++;
        logger.Debug("Adding Chunk {x},{y} at {icx},{icy} (cx={cx}, cy={cy})", x, y, icx,icy,cx,cy);
        bool l = log_on;
        log_on = true;
        Chunk c = new Chunk(icx,icy);
        c.SetMaterial(mat);
        chunks[x + "," + y] = c;
        c.Translate(new Vector3(0 , 0, 0 ));
        //logger.Debug("Added Chunk at {0},{1}", x * 16, y * 16);
        return c;
        //AddChild(c);
    }
}

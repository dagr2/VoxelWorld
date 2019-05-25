using Godot;
using NeuesSpielc;
using System;
using System.Collections.Generic;

public class World : Godot.Spatial
{
    Dictionary<string,Chunk> chunks=new Dictionary<string,Chunk>();
    SpatialMaterial mat = (SpatialMaterial)ResourceLoader.Load("res://mat1.tres");
    public int vischunks = 1;
    public int Height = 64;

    public float HeightAt(float x, float y)
    {
        return 1;
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

    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        GD.PrintErr(e.ExceptionObject.ToString());        
    }

    public static void log(string m)
    {
       // Console.WriteLine(m);
        GD.Print(m);
    }
    private void TestAndCreateChunk(int x, int y)
    {
        if (!chunks.ContainsKey(x + "," + y))
        {
            //log("Adding chunk " + x + "," + y);
            AddChunk(x, y);
        }
    }
    object lck=new object();

    public void CalcMeshes()
    {
        KinematicBody player = (KinematicBody)FindNode("Player");
        int ocx = 0;int ocy = 0;
        while (true)
        try {
            Vector3 ppos = player.Translation;
            int cx = (int)Math.Round((ppos.x) / 16);
            int cy = (int)Math.Round((ppos.z) / 16);

            if (ocx != cx || ocy != cy)
            {
                GD.Print("player at chunk " + cx + "," + cy);
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

    public void GetBlock(int x,int y,int z, int v)
    {

    }

    public int GetBlock(int x,int y,int z)
    {
        return 0;

    }

    public void AddChunk(int x, int y)
    {
        Chunk c = new Chunk(x, y);
        c.SetMaterial(mat);
        chunks[x + "," + y] = c;
        c.Translate(new Vector3(x * 16 - 8, 0, y * 16 - 8));
        
        //AddChild(c);
    }
}

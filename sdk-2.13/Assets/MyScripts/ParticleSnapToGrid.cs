using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSnapToGrid : MonoBehaviour {
    public float particleSize = 1f;
    public Color particleColor = Color.white;
    public bool randomColorAlpha = true; // For MetallicSmoothness random offset
    public float xDistance = 0.25f;
    public float yDistance = 0.25f;
    //public float zDistance = 0.25f;
    public int xSize = 10;
    public int ySize = 10;
    //public int zSize = 10;
    //public float OffsetEven = 0.125f;
    //public bool updateEveryFrame = false;

    private float even;
    private Vector3[] positions;
    public ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    private List<Vector4> customData = new List<Vector4>();
    private List<Vector4> customData2 = new List<Vector4>();

    public float gap = 0.005f;

    Gradient gradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;
    // Use this for initialization
    void Start()
    {
        //particles = new ParticleSystem.Particle[(xSize) * (ySize)];
      
        //ps = GetComponent<ParticleSystem>();
        UpdateGrid();
    }

    public void UpdateGrid()
    {
        GenerateGrid();
        EmitParticles();
        // CreateOffsetVector();
        //SetParticles();
        //ParticleSystemRenderer psrend = GetComponent<ParticleSystemRenderer>();
       // psrend.SetActiveVertexStreams(new List<ParticleSystemVertexStream>(new ParticleSystemVertexStream[] { ParticleSystemVertexStream.Position, ParticleSystemVertexStream.Normal, ParticleSystemVertexStream.Color, ParticleSystemVertexStream.UV, ParticleSystemVertexStream.Center, ParticleSystemVertexStream.Tangent, ParticleSystemVertexStream.Custom1XYZ }));
       // psrend.alignment = ParticleSystemRenderSpace.Local;
    }

    private void GenerateGrid()
    {
        //int count = Mathf.FloorToInt((xSize / gap + 1) * (ySize / gap + 1));
        positions = new Vector3[(xSize)*(ySize)];
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                var i = x + (y * (xSize));
                positions[i] = new Vector3(x * xDistance-2.2f, y * yDistance-1.22f, 0);
                //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //cube.transform.position = positions[i];
                //cube.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            }
        }
    }

    void EmitParticles()
    {


        //ps.Emit(xSize * ySize);
        //print(ps.particleCount);

        //collison
        var collision = ps.collision;
        collision.enabled = true;
        collision.bounce = 0f;
        collision.type = ParticleSystemCollisionType.World;
        collision.mode = ParticleSystemCollisionMode.Collision3D;
        collision.colliderForce = 0f;
        collision.multiplyColliderForceByParticleSpeed = true;
        
        collision.collidesWith = LayerMask.GetMask("Environment") ;
        #region color over life time
        /*color over life time
        
        gradient = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[1];
        colorKey[0].color = Color.white;
        colorKey[0].time = 0.0f;
     

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[3];
        alphaKey[0].alpha = 0.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 0.5f;
        alphaKey[2].alpha = 0.0f;
        alphaKey[2].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);

        var colorOverlifeTime = ps.colorOverLifetime;
        colorOverlifeTime.enabled = true;
        colorOverlifeTime.color = gradient;
        */
        #endregion


        //trails
        var trails = ps.trails;
        //trails.enabled = true;
        
        //force
        var force = ps.forceOverLifetime;
        //force.enabled = true;
        force.y = -1;
        force.x = 0;
        int particleCount = xSize * ySize;
        particles = new ParticleSystem.Particle[particleCount];
        for (int i = 0; i < particleCount; i++)
        {
            var emitParams = new ParticleSystem.EmitParams();
            emitParams.position = positions[i];
            //emitParams.velocity = new Vector3(-10, 0, 0);
            //print(emitParams.position);
            ps.Emit(emitParams,1);
            

        }
        ps.GetParticles(particles);
        //print(ps.collision.enabled);
        print(particles[0].velocity);

        
    }

    private void SetParticles()
    {

        print("enterGenerateParticles");

        
        ps.GetParticles(particles);
        
        //particles
        //for (int i = 0; i < particles.Length; i++)
        //{
        //    ParticleSystem.Particle p = particles[i];
        //    p.position = positions[i];
        //    if (randomColorAlpha == true)
        //        particleColor.a = Random.Range(0f, 1f);
        //    p.startColor = particleColor;
        //    p.startSize = particleSize;
        //    particles[i] = p;
        //    //print(particles[i].GetCurrentColor(ps));
        //}

        

        var collisionMod = ps.collision;
        collisionMod.enabled = true;
        var lightMod = ps.lights;
        lightMod.enabled = true;
        var forceMod = ps.forceOverLifetime;
        forceMod.enabled = true;
        forceMod.x = -10;
        var sizeMod = ps.sizeOverLifetime;
        sizeMod.enabled = true;


        ps.SetParticles(particles, particles.Length);

     
    }

    private void FixedUpdate()
    {

    }
  


}

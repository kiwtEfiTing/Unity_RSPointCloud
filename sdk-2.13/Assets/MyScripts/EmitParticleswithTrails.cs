﻿using System;
using UnityEngine;
using Intel.RealSense;
using System.Linq;
using System.Collections.Generic;

public class EmitParticleswithTrails : MonoBehaviour
{
    public bool mirrored;
    public float pointsSize = 10;
    public int skipParticles = 60;
    public ParticleSystem pointCloudParticles;

    private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[0];
    private PointCloud pc = new PointCloud();
    private Points.Vertex[] vertices;
    private byte[] lastColorImage;
    private Align aligner;
    GameObject cube;
    // Use this for initialization
    void Start()
    {
        aligner = new Align(Intel.RealSense.Stream.Color);
        if (RealSenseDevice.Instance.ActiveProfile.Streams.FirstOrDefault(x => x.Stream == Stream.Depth) == null)
        {
            Debug.Log("Can't create point cloud, depthstream must be enabled");
            return;
        }
        if (RealSenseDevice.Instance.ActiveProfile.Streams.FirstOrDefault(x => x.Stream == Stream.Color) != null)
        {
            RealSenseDevice.Instance.onNewSampleSet += OnFrames;
        }
        else
        {
            RealSenseDevice.Instance.onNewSample += OnFrame;
        }

        //cube = GameObject.Find("Cube");
    }
    int w;
    int h;
    private void OnFrame(Frame frame)
    {
        //Debug.Log("onframe");
        if (frame.Profile.Stream != Stream.Depth)
            return;
        var depthFrame = frame as DepthFrame;
        w = depthFrame.Width;
        h = depthFrame.Height;
        if (!UpdateParticleParams(depthFrame.Width, depthFrame.Height))
        {

            Debug.Log("Unable to craete point cloud");
            return;
        }

        using (var points = pc.Calculate(depthFrame))
        {
            setParticals(points, null);
        }
    }

    //object l = new object();
    private void OnFrames(FrameSet frames)
    {

        using (var aligned = aligner.Process(frames))
        {
            using (var colorFrame = aligned.ColorFrame)
            using (var depthFrame = aligned.DepthFrame)
            {
                if (depthFrame == null)
                {
                    Debug.Log("No depth frame in frameset, can't create point cloud");
                    return;
                }

                if (!UpdateParticleParams(depthFrame.Width, depthFrame.Height))
                {
                    Debug.Log("Unable to craete point cloud");
                    return;
                }

                using (var points = pc.Calculate(depthFrame))
                {
                    //setParticals(points, colorFrame);
                }
            }
        }
    }


    public List<Vector3> particlesInRangePos = new List<Vector3>();
    Vector2[] currentPos;
    Vector2[] prePos;
    private void setParticals(Points points, VideoFrame colorFrame)
    {
        if (points == null)
            throw new Exception("Frame in queue is not a points frame");

        if (colorFrame != null)
        {
            if (lastColorImage == null)
            {
                int colorFrameSize = colorFrame.Height * colorFrame.Stride;
                lastColorImage = new byte[colorFrameSize];
            }
            colorFrame.CopyTo(lastColorImage);
        }

        vertices = vertices ?? new Points.Vertex[points.Count];
        points.CopyTo(vertices);

        Debug.Assert(vertices.Length == particles.Length);
        int mirror = mirrored ? -1 : 1;
        //print("test");
        //print(vertices.Length + "   " + w + "    " + h);

        particlesInRangePos = new List<Vector3>() { };
        for (int hor = 0; hor < w; hor += skipParticles)
        {
            for (int ver = 0; ver < h; ver += skipParticles)
            {
                //print("inside test");
                //var index = skipParticles * (hor + ver * (w / skipParticles + 1));
                var index = hor + w * ver;
                //print(index + "   " + hor +"   " + ver);
                var v = vertices[index];
                if (v.z > 0 && v.z < 0.5f)
                {
                    //particles[index].position = new Vector3(v.x * mirror, v.y, v.z);
                    particlesInRangePos.Add(new Vector3(v.x * mirror, v.y, v.z));
                    //particles[index].startSize = v.z * pointsSize * 0.02f;
                    //if (lastColorImage != null)
                    //    particles[index].startColor = new Color32(lastColorImage[index * 3], lastColorImage[index * 3 + 1], lastColorImage[index * 3 + 2], 255);
                    //}
                   // else
                   // {
                   //     byte z = (byte)(v.z / 2f * 255);
                  //      particles[index].startColor = new Color32(z, z, z, 255);
                  //  }
                }
                else //Required since we reuse the array
                {
                  //  particles[index].position = Vector3.zero;
                  //  particles[index].startSize = 0;
                  //  particles[index].startColor = Color.black;
                }


            }
        }




        //for (int index = 0; index < vertices.Length; index += skipParticles)
        //{
        //    var v = vertices[index];
        //    print(index);
        //    if (v.z > 0 && v.z < 1f)
        //    {
        //        particlesInRangePos.Add(new Vector3(v.x * mirror, v.y, v.z));

        //        //if (Vector2.Distance(particles[index].position, new Vector2(v.x * mirror, v.y)) > 0.2)
        //        //{

        //        //currentPos[index] = new Vector2(v.x * mirror, v.y);
        //        //if (Vector2.Distance(prePos[index], currentPos[index]) > 0.1f)
        //        // {
        //        particles[index].position = new Vector3(v.x * mirror, v.y, v.z);
        //        //particlesInRangePos.Add(particles[index].position);
        //        particles[index].startSize = v.z * pointsSize * 0.02f;

        //        //prePos[index] = new Vector2(v.x * mirror, v.y);
        //        //print(prePos[index]);

        //        if (lastColorImage != null)
        //            particles[index].startColor = new Color32(lastColorImage[index * 3], lastColorImage[index * 3 + 1], lastColorImage[index * 3 + 2], 255);
        //        //}
        //        else
        //        {
        //            byte z = (byte)(v.z / 2f * 255);
        //            particles[index].startColor = new Color32(z, z, z, 255);
        //        }

        //        //  }



        //    }
        //    else //Required since we reuse the array
        //    {
        //        particles[index].position = Vector3.zero;
        //        particles[index].startSize = 0;
        //        particles[index].startColor = Color.black;
        //    }


        //}
    }


    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    for (float x = 0; x < 400; x += size)
    //    {
    //        for (float z = 0; z < 400; z += size)
    //        {
    //            var point = GetNearestPointOnGrid(new Vector3(-x, z, 0f));
    //            Gizmos.DrawSphere(point, 1f);
    //        }

    //    }
    //}

    private bool UpdateParticleParams(int width, int height)
    {
        var numParticles = (width * height);
        if (particles.Length != numParticles)
        {
            particles = new ParticleSystem.Particle[numParticles];
            prePos = new Vector2[numParticles];
            currentPos = new Vector2[numParticles];
        }

        return true;
    }
    float sumx;
    float sumy;
    public Vector3 average;
    void FixedUpdate()
    {
        //Either way, update particles
        //print(particlesInRangePos.Count);
        //pointCloudParticles.SetParticles(particles, particles.Length);

        // print(particlesInRangePos.Count + "..." + 1 / Time.deltaTime);
        //sumx = 0;
        // sumy = 0;

        // for (int i = 0; i < particlesInRangePos.Count; i++)
        // {
        //    sumx += particlesInRangePos[i].x;
        //     sumy += particlesInRangePos[i].y;




        //  }
        //  average = new Vector3(-1 * sumx / particlesInRangePos.Count, (-1) * sumy / particlesInRangePos.Count, 0);
        //print(average);
        //cube.transform.position = average;
        //cube.transform.localScale = new Vector3(1f, 1f, 1f);
        //s}
        var trails = pointCloudParticles.trails;
        //trails.enabled = true;
        var collision = pointCloudParticles.collision;
        collision.enabled = true;
        collision.bounce = 1f;
        collision.type = ParticleSystemCollisionType.World;
        collision.mode = ParticleSystemCollisionMode.Collision2D;
        collision.colliderForce = 1f;
        collision.multiplyColliderForceByParticleSpeed = true;

        collision.collidesWith = LayerMask.GetMask("Environment");
        var force = pointCloudParticles.forceOverLifetime;
        force.enabled = true;
        force.y = -1;
        force.x = 0;

        for (int k = 0; k < particlesInRangePos.Count; k++)
        {
            var emitParams = new ParticleSystem.EmitParams();
            emitParams.position = particlesInRangePos[k];
            //emitParams.velocity = new Vector3(-10, 0, 0);
            //print(emitParams.position);
            pointCloudParticles.Emit(emitParams, 1);
        }





    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleTest : MonoBehaviour {

    private ParticleSystem ps;
    private ParticleSystem.Particle[] pars;
    private void Awake()
    {
        
        ps = GetComponent<ParticleSystem>();
    }
    private void Start()
    {
        pars = new ParticleSystem.Particle[100];
        for (int i = 0; i < pars.Length; i++)
        {
            pars[i].startColor = Color.white;
            pars[i].startSize = 1;
            pars[i].position = Vector3.zero;

        }


        ps.SetParticles(pars, pars.Length);
        var forceMod = ps.forceOverLifetime;
        forceMod.enabled = true;
        forceMod.x = -10;
    }

    private void Update()
    {
        
    }
}

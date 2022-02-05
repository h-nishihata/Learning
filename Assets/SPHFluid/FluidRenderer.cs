using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Fluid2D))]
public class FluidRenderer : MonoBehaviour
{
    public Fluid2D solver;
    public Material RenderParticleMat;
    public Color WaterColor;

    private void OnRenderObject()
    {
        DrawParticle();
    }

    private void DrawParticle()
    {
        RenderParticleMat.SetPass(0);
        RenderParticleMat.SetColor("_WaterColor", WaterColor);
        RenderParticleMat.SetBuffer("_ParticlesBuffer", solver.ParticlesBufferRead);
        Graphics.DrawProceduralNow(MeshTopology.Points, solver.NumParticles);
    }
}

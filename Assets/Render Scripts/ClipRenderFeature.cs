using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ClipRenderFeature : ScriptableRendererFeature
{
    public LayerMask targetLayer;
    public Material desaturationMaterial;
    public Vector4 dist;
    private ClipRenderPass desaturationRenderPass;

    public override void Create()
    {
        desaturationRenderPass = new ClipRenderPass(targetLayer, desaturationMaterial, dist);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(desaturationRenderPass);
    }
}

public class ClipRenderPass : ScriptableRenderPass
{
    private LayerMask targetLayer;
    private Material desaturationMaterial;
    private Vector4 dis;
    private RenderTargetIdentifier sourceIdentifier;
    private RenderTargetHandle destinationHandle;
    private List<ShaderTagId> shaderTagsList = new List<ShaderTagId>();
    private FilteringSettings filteringSettings;
    public ClipRenderPass(LayerMask targetLayer, Material desaturationMaterial,Vector4 dis)
    {
        this.targetLayer = targetLayer;
        this.desaturationMaterial = desaturationMaterial;
        this.dis=dis;
        destinationHandle.Init("_DesaturationTempTexture");
        filteringSettings = new FilteringSettings(RenderQueueRange.opaque, targetLayer);

        shaderTagsList.Add(new ShaderTagId("SRPDefaultUnlit"));
        shaderTagsList.Add(new ShaderTagId("UniversalForward"));
        shaderTagsList.Add(new ShaderTagId("UniversalForwardOnly"));
    }
    private RTHandle rtTemp;
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (desaturationMaterial == null)
            return;

   

        CommandBuffer cmd = CommandBufferPool.Get("DesaturationRenderPass");
  
  
        using (new ProfilingScope(cmd, new ProfilingSampler("DesaturationRenderPass")))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
          
       

            SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
            DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagsList, ref renderingData, sortingCriteria);
            var layerMask = targetLayer;
            if (desaturationMaterial != null)
            {
                
                drawingSettings.overrideMaterial = desaturationMaterial;
              
            }

          
         
        
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            
    
            cmd.SetGlobalTexture("_CameraColorTexture", sourceIdentifier);
            
            cmd.Blit(sourceIdentifier, destinationHandle.Identifier(),desaturationMaterial);
         
            cmd.Blit(destinationHandle.Identifier(), sourceIdentifier);
       
        }
    
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        base.OnCameraSetup(cmd, ref renderingData);
  
    }
    public void SetSource(RenderTargetIdentifier sourceIdentifier)
    {
        this.sourceIdentifier = sourceIdentifier;
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        if (cmd == null)
            throw new System.Exception("cmd");

        cmd.ReleaseTemporaryRT(destinationHandle.id);
    }
}
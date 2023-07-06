using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderPass : ScriptableRendererFeature
{
    class LensFlarePass : ScriptableRenderPass
    {
        private Material _material;
        RenderTargetIdentifier colorBuffer, temporaryBuffer;
        const string ProfilerTag = "Template Pass";
        private LayerMask targetLayer;
        public LensFlarePass(Material material, LayerMask targetLayer)
        {
            _material = material;
            targetLayer = targetLayer;
        }

        public override void Execute(ScriptableRenderContext context,
            ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque, targetLayer);
         

        

            RenderStateBlock stateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            DrawingSettings drawing = new DrawingSettings();
           
            context.DrawRenderers(renderingData.cullResults,ref drawing, ref filteringSettings);
            //context.draw
            // Execute the command buffer and release it.
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    private LensFlarePass _lensFlarePass;
    public Material material;
    public LayerMask targetLayer;

    public override void Create()
    {
        _lensFlarePass = new LensFlarePass(material, targetLayer);
        // Draw the lens flare effect after the skybox.
        _lensFlarePass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
        
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material != null )
        {
            renderer.EnqueuePass(_lensFlarePass);
        }
    }
}
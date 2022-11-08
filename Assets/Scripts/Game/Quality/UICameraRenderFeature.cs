using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Quality
{
    public class UICameraRenderFeature : ScriptableRendererFeature
    {
        class CustomRenderPass : ScriptableRenderPass
        {
            public const string PassName = "MainGameCameraBlit";
            private ProfilingSampler customProfilingSampler = new ProfilingSampler(PassName);

            private RenderTargetIdentifier sourceRenderTargetIdentifier;

            // This method is called before executing the render pass.
            // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
            // When empty this render pass will render to the active camera render target.
            // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
            // The render pipeline will ensure target setup and clearing happens in a performant manner.
            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
            }

            public void Setup(RenderTargetIdentifier source)
            {
                sourceRenderTargetIdentifier = source;
            }

            // Here you can implement the rendering logic.
            // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
            // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
            // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var gameMainCamera = GameMainCamera.Current;
                if (gameMainCamera == null || gameMainCamera.TargetRenderTexture == null)
                {
                    return;
                }

                var cmd = CommandBufferPool.Get(PassName);
                using (new ProfilingScope(cmd, customProfilingSampler))
                {
                    cmd.Blit(gameMainCamera.TargetRenderTexture, sourceRenderTargetIdentifier);
                }

                context.ExecuteCommandBuffer(cmd);
            }

            // Cleanup any allocated resources that were created during the execution of this render pass.
            public override void OnCameraCleanup(CommandBuffer cmd)
            {
            }
        }

        CustomRenderPass customPass;

        /// <inheritdoc/>
        public override void Create()
        {
            customPass = new CustomRenderPass();

            // Configures where the render pass should be injected.
            customPass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            customPass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(customPass);
        }
    }
}
using MitarashiDango.AvatarUtils;
using nadena.dev.ndmf;

[assembly: ExportsPlugin(typeof(AvatarUtilsNDMFPlugin))]

namespace MitarashiDango.AvatarUtils
{
    public class AvatarUtilsNDMFPlugin : Plugin<AvatarUtilsNDMFPlugin>
    {
        protected override void Configure()
        {
            InPhase(BuildPhase.Generating)
                .BeforePlugin("nadena.dev.modular-avatar")
                .Run("Run MitarashiDango's Avatar Utils Processes", ctx => Processing(ctx));
        }

        private void Processing(BuildContext ctx)
        {
            ModifyAnimatorControllerProcess(ctx);
            FaceEmoteControlProcess(ctx);
            PhysBonesSwitcherProcess(ctx);
        }

        private void ModifyAnimatorControllerProcess(BuildContext ctx)
        {
            var processor = new AnimatorControllerModifierProcessor();
            processor.Run(ctx);
        }

        private void FaceEmoteControlProcess(BuildContext ctx)
        {
            var processor = new FaceEmoteControlProcessor();
            processor.Run(ctx);
        }

        private void PhysBonesSwitcherProcess(BuildContext ctx)
        {
            var processor = new PhysBonesSwitcherProcessor();
            processor.Run(ctx);
        }
    }
}
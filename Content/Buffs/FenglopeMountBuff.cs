
using PolWorldMounts.Content.Mounts;
using Terraria;
using Terraria.ModLoader;

namespace PolWorldMounts.Content.Buffs {
    public class FenglopeMountBuff : ModBuff {
        public override void SetStaticDefaults() {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<FenglopeMount>(), player);
            player.buffTime[buffIndex] = 10;
            base.Update(player, ref buffIndex);
        }
    }
}
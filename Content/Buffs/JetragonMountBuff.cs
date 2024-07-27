using PolWorldMounts.Content.Mounts;
using Terraria;
using Terraria.ModLoader;

namespace PolWorldMounts.Content.Buffs {
    public class JetragonMountBuff : ModBuff {
        public override void SetStaticDefaults() {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<JetragonMount>(), player);
            player.buffTime[buffIndex] = 10;
            base.Update(player, ref buffIndex);
        }
    }
}
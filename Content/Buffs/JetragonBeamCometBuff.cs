using Terraria;
using Terraria.ModLoader;

namespace PolWorldMounts.Content.Buffs
{
    public class JetragonBeamCometBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true; // This is a debuff
            Main.buffNoSave[Type] = true; // This buff won't persist when exiting the world
            Main.buffNoTimeDisplay[Type] = false; // This buff will have a time display
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] % 60 == 0)
            {
                player.buffTime[buffIndex] -= 1;
            }
            base.Update(player, ref buffIndex);
        }
    }
}

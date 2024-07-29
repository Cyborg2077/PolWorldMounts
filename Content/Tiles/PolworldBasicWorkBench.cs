using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ObjectData;
using Terraria.Localization;
using PolWorldMounts.Content.Items.WorkBench;

namespace PolWorldMounts.Content.Tiles
{
    public class PolworldBasicWorkBench : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileTable[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Width = 4;
            
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 200, 200), ModContent.GetInstance<PolworldBasicWorkBenchItem>().DisplayName);

            // 设置贴图
            Main.tileMergeDirt[Type] = true;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
        }
    }
}

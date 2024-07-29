using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace PolWorldMounts.Content.Items.WorkBench
{
    public class PolworldBasicWorkBenchItem : ModItem
    {
    
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 14;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.PolworldBasicWorkBench>(); // 设置对应的Tile
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {          
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 30);
            recipe.AddIngredient(ItemID.Sapphire, 10);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}

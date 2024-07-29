using PolWorldMounts.Content.Mounts;
using PolWorldMounts.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PolWorldMounts.Content.Items.Mounts
{
	public class JetragonMountItem : ModItem
	{
		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 30;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(gold: 3);
			Item.rare = ItemRarityID.Master;
			Item.UseSound = SoundID.Item79;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<JetragonMount>();
		}

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LunarBar, 10);      
            recipe.AddIngredient(ItemID.Silk, 20);
            recipe.AddIngredient(ItemID.Sapphire, 5);
			recipe.AddTile(Mod, "PolworldBasicWorkBench");
            recipe.Register();
        }
	}
}

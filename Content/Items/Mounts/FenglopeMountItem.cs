using PolWorldMounts.Content.Mounts;
using PolWorldMounts.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PolWorldMounts.Content.Items.Mounts
{
	public class FenglopeMountItem : ModItem
	{
		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 30;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(gold: 3);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item79;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<FenglopeMount>();
		}

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.HallowedBar, 20);
			recipe.AddIngredient(ItemID.SoulofFlight, 5);
			recipe.AddIngredient(ItemID.SoulofLight, 10);
            recipe.AddIngredient(ItemID.Sapphire, 5);
			recipe.AddTile(Mod, "PolworldBasicWorkBench");
            recipe.Register();
        }
	}
}

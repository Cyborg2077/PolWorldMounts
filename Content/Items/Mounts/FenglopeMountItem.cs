using PolWorldMounts.Content.Mounts;
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
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item79;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<FenglopeMount>();
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 1);
            recipe.Register();
        }
	}
}

using System.ComponentModel;
using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader.Config;

namespace PolWorldMounts.Content
{
    public class PolworldModConfig : ModConfig
    {
        [DefaultValue(Keys.Z)]
        [Label("Mount Dash Key")]
        [CustomModConfigItem(typeof(KeyBindElement))]
        public Keys MountDashKey;

        public override ConfigScope Mode => ConfigScope.ClientSide;
    }
}

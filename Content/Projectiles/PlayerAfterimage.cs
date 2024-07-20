using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PolWorldMounts.Content.Projectiles
{
    public class PlayerAfterimage : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 20; // 设置投射物的宽度
            Projectile.height = 20; // 设置投射物的高度
            Projectile.aiStyle = 0; // 自定义AI
            Projectile.friendly = false; // 投射物不会对敌人造成伤害
            Projectile.hostile = false; // 投射物不会对玩家造成伤害
            Projectile.penetrate = -1; // 无限穿透
            Projectile.timeLeft = 30; // 持续时间
            Projectile.alpha = 255; // 初始透明度
            Projectile.light = 0.5f; // 发光强度
            Projectile.ignoreWater = true; // 忽略水
            Projectile.tileCollide = false; // 不与瓦片碰撞
        }

        public override void AI()
        {
            Projectile.alpha += 8; // 逐渐变透明
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill(); // 完全透明时销毁
            }

            Projectile.rotation += 0.1f * Projectile.direction; // 旋转效果，可根据需要调整
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 在这里自定义绘制逻辑
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = sourceRectangle.Size() / 2f;

            spriteBatch.Draw(texture, position, sourceRectangle, lightColor * (1f - (Projectile.alpha / 255f)), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}

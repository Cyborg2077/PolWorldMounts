using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PolWorldMounts.Content.Projectiles
{
    public class DragonMeteorProjectile : ModProjectile
    {
        private bool hasAccelerated = false; // 是否已经开始加速
        private int hoverTime = 60; // 悬停时间（帧）

        public override void SetDefaults()
        {
            Projectile.width = 32; // 弹幕宽度
            Projectile.height = 32; // 弹幕高度
            Projectile.friendly = true; // 是否对玩家友好
            Projectile.hostile = false; // 是否对敌人友好
            Projectile.tileCollide = true; // 是否与瓷砖碰撞
            Projectile.penetrate = -1; // 穿透数量，设置为-1表示不会消失
            Projectile.timeLeft = 300; // 弹幕的存活时间（帧）
            Projectile.light = 0.8f; // 弹幕的光亮度
            Projectile.ignoreWater = true; // 是否忽略水
            Projectile.extraUpdates = 1; // 每帧更新次数
            Main.projFrames[Projectile.type] = 4; // 设置弹幕的帧数
        }

        public override void AI()
        {
            if (hoverTime > 0)
            {
                hoverTime--;
                Projectile.velocity = Vector2.Zero; // 悬停时速度为零

                // 不产生粒子特效
                if (hoverTime % 10 == 0) // 每10帧产生一次粒子特效（可以调整频率）
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Pink, 0f, 0f, 100, default(Color), 1.0f);
                    Main.dust[dustIndex].velocity *= 0.5f;
                    Main.dust[dustIndex].scale *= 1.2f;
                    Main.dust[dustIndex].noGravity = true;
                }
            }
            else if (!hasAccelerated)
            {
                Vector2 targetPosition = Main.MouseWorld;
                Vector2 direction = targetPosition - Projectile.Center;
                direction.Normalize();
                Projectile.velocity = direction * 2f; // 设置初始速度
                hasAccelerated = true;
            }
            else
            {
                Projectile.velocity *= 1.05f; // 缓慢加速
            }

            // 添加粒子效果或其他视觉效果
            if (hoverTime <= 0) // 只有在加速阶段才产生粒子特效
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Red, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 100, default(Color), 1.0f);
                Main.dust[dustIndex].velocity *= 0.5f;
                Main.dust[dustIndex].scale *= 1.2f;
                Main.dust[dustIndex].noGravity = true;
            }
        }
        public override void Kill(int timeLeft)
        {
            // 创建爆炸效果的范围伤害
            float explosionRadius = 150f; // 爆炸范围
            int damage = 300; // 爆炸伤害

            // 查找爆炸范围内的敌人并造成伤害
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this) && Vector2.Distance(npc.Center, Projectile.Center) < explosionRadius)
                {
                    // 计算伤害并应用
                    npc.SimpleStrikeNPC(damage, 0, false, 0, null, false, 0, false); // hitDirection 设置为 -1 表示无特定方向
                }
            }

            // 播放音效
            for (int i = 0; i < 20; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color), 2f);
                Main.dust[dustIndex].velocity *= 1.4f;
            }
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Blue, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 5f;
                dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Red, 0f, 0f, 100, default(Color), 2f);
                Main.dust[dustIndex].velocity *= 3f;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            // 获取当前帧的源矩形
            Rectangle sourceRectangle = new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height);

            // 绘制弹幕
            Main.spriteBatch.Draw(
                Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                lightColor,
                Projectile.rotation,
                new Vector2(Projectile.width / 2, Projectile.height / 2),
                Projectile.scale,
                SpriteEffects.None,
                0f
            );

            return false; // 返回false，表示我们自己绘制了弹幕
        }
    }
}

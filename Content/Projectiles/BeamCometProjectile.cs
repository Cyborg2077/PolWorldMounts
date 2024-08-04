using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PolWorldMounts.Content.Projectiles
{
    public class BeamCometProjectile : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 10; // 弹幕宽度
            Projectile.height = 10; // 弹幕高度
            Projectile.friendly = true; // 是否对玩家友好
            Projectile.hostile = false; // 是否对敌人友好
            Projectile.tileCollide = true; // 是否与瓷砖碰撞
            Projectile.penetrate = 5; // 穿透数量
            Projectile.timeLeft = 600; // 弹幕的存活时间（帧）
            Projectile.light = 0.5f; // 弹幕的光亮度
            Projectile.ignoreWater = true; // 是否忽略水
            Projectile.extraUpdates = 1; // 每帧更新次数
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 mousePosition = Main.MouseWorld;
            NPC target = FindClosestNPC(800f); // 设定追踪范围为800像素

            if (target != null)
            {
                // 如果找到目标，计算弹幕朝向目标的方向
                Vector2 directionToTarget = target.Center - Projectile.Center;
                directionToTarget.Normalize();

                // 平滑过渡到目标方向
                float smoothFactor = 0.05f; // 调整平滑的因子，0到1之间，数值越小转向越平滑
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, directionToTarget * 4f, smoothFactor);
            }

            // 粒子效果
            for (int i = 0; i < 1; i++) // 每帧生成1个粒子
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Pink, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, new Color(205, 71, 208), 0.7f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.velocity *= 0.3f;
                dust.scale *= 0.95f;
            }

            // 添加光效
            Lighting.AddLight(Projectile.Center, 0.84f, 0.48f, 0.73f);
        }


        // 辅助方法，查找最近的敌人
        private NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this))
                {
                    float sqrDistanceToNPC = Vector2.DistanceSquared(npc.Center, Projectile.Center);
                    if (sqrDistanceToNPC < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToNPC;
                        closestNPC = npc;
                    }
                }
            }
            return closestNPC;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 获取当前帧的源矩形
            Rectangle sourceRectangle = new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height);

            // 获取弹幕的贴图
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            // 拖影效果参数
            float alpha = 0.5f; // 拖影的透明度
            float scale = Projectile.scale; // 拓影的缩放

            // 获取弹幕的速度方向
            Vector2 velocity = Projectile.velocity;
            velocity.Normalize(); // 归一化速度向量，用于确定拖影的方向

            // 计算拖影偏移量
            float offsetDistance = 10f; // 拖影偏移量的距离（可以根据需要调整）

            // 绘制拖影
            for (int i = 0; i < 20; i++) // 绘制5个拖影（可以调整数量）
            {
                // 计算每个拖影的位置
                Vector2 offset = velocity * offsetDistance * i; // 沿着速度方向的偏移量
                Color color = Color.Lerp(lightColor, Color.Transparent, alpha / 5f * i); // 逐渐变透明的颜色

                Main.spriteBatch.Draw(
                    texture,
                    Projectile.Center - Main.screenPosition - offset,
                    sourceRectangle,
                    color,
                    Projectile.rotation,
                    new Vector2(Projectile.width / 2, Projectile.height / 2),
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }

            // 绘制正常的弹幕
            Main.spriteBatch.Draw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                lightColor,
                Projectile.rotation,
                new Vector2(Projectile.width / 2, Projectile.height / 2),
                scale,
                SpriteEffects.None,
                0f
            );

            return false; // 返回false，表示我们自己绘制了弹幕
        }


    }
}

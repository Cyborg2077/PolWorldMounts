using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PolWorldMounts.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PolWorldMounts.Content.Projectiles
{
    public class FlareStormProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 168; // 弹幕宽度
            Projectile.height = 164; // 弹幕高度
            Projectile.friendly = true; // 是否对玩家友好
            Projectile.hostile = false; // 是否对敌人友好
            Projectile.tileCollide = true; // 是否与瓷砖碰撞
            Projectile.penetrate = 9999; // 穿透数量
            Projectile.timeLeft = 300; // 弹幕的存活时间（帧）
            Projectile.light = 0.8f; // 弹幕的光亮度
            Projectile.ignoreWater = true; // 是否忽略水
            Projectile.extraUpdates = 1; // 每帧更新次数
            Projectile.aiStyle = -1; // 自定义AI
            Main.projFrames[Projectile.type] = 4; // 设置弹幕的帧数
        }

        public override void AI()
        {
            // 创建火焰粒子特效
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Yellow);
                dust.noGravity = true; // 粒子不受重力影响
                dust.velocity *= 1.2f; // 增加粒子速度
                dust.scale *= 1.5f; // 增加粒子规模
            }

            // 设置弹幕沿地面前进
            if (Projectile.velocity.Y == 0f) // 检查弹幕是否在地面上
            {
                if (Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X = Math.Max(Projectile.velocity.X - 0.1f, 0); // 左移减速
                }
                else if (Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X = Math.Min(Projectile.velocity.X + 0.1f, 0); // 右移减速
                }

                if (Math.Abs(Projectile.velocity.X) < 0.2f)
                {
                    Projectile.velocity.X = 0f; // 停止移动
                }
            }
            else
            {
                Projectile.velocity.Y += 0.2f; // 重力作用
            }

            // 添加光效
            Lighting.AddLight(Projectile.Center, 0.8f, 0.4f, 0.1f); // 添加橙色光效

            // 更新动画帧
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5) // 每5帧切换一次图片
            {
                Projectile.frame++;
                Projectile.frame %= 4; // 循环帧动画
                Projectile.frameCounter = 0;
            }

            // 吸附敌怪
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && Vector2.Distance(Projectile.Center, npc.Center) < 300f)
                {
                    Vector2 direction = Projectile.Center - npc.Center;
                    direction.Normalize();
                    npc.velocity += direction * 0.1f; // 吸附效果
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300); // 造成燃烧效果，持续5秒
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero; // 停止移动，但不消失
            return false; // 不摧毁弹幕
        }

        public override void Kill(int timeLeft)
        {
            // 可以在弹幕摧毁时添加特效
        }
    }
}
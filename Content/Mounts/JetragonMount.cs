using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PolWorldMounts.Content.Mounts
{
    public class JetragonMount : ModMount
    {
        private const float FlightAcceleration = 0.8f; // 飞行加速度
        private const float MaxVerticalSpeed = 25f; // 最大垂直速度
        private const float HorizontalAcceleration = 0.1f; // 水平加速度
        private const float DecelerationFactor = 0.95f; // 减速因子
        private bool isShooting = false;
        private bool isShootingMeteor = false;
        private bool isShootingFlareStorm = false;

        public override void SetStaticDefaults()
        {
            MountData.jumpHeight = 15; // How high the mount can jump.
            MountData.acceleration = HorizontalAcceleration; // The rate at which the mount speeds up.
            MountData.jumpSpeed = 15f; // The rate at which the player and mount ascend towards (negative y velocity) the jump height when the jump button is pressed.
            MountData.blockExtraJumps = true; // 阻止饰品增加跳跃次数
            MountData.constantJump = true; // Allows you to hold the jump button down.
            MountData.heightBoost = 20; // Height between the mount and the ground
            MountData.fallDamage = 0f; // Fall damage multiplier.
            MountData.runSpeed = 25f; // The speed of the mount
            MountData.dashSpeed = 15f; // The speed the mount moves when in the state of dashing.
            MountData.flightTimeMax = 999999; // 实现持续飞行

            MountData.fatigueMax = 0;
            MountData.buff = ModContent.BuffType<Buffs.JetragonMountBuff>(); // The ID number of the buff assigned to the mount.

            // Effects
            // MountData.spawnDust = ModContent.DustType<Dusts.Sparkle>(); // The ID of the dust spawned when mounted or dismounted.

            // Frame data and player offsets
            MountData.totalFrames = 3; // Amount of animation frames for the mount
            MountData.playerYOffsets = Enumerable.Repeat(20, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code
            MountData.xOffset = -10;
            MountData.yOffset = -12;
            MountData.playerHeadOffset = 30; // 确保玩家头部不被遮挡
            MountData.bodyFrame = 4; // 调整这个参数，以确保与玩家的动画帧兼容

            // Standing
            MountData.standingFrameCount = 3;
            MountData.standingFrameDelay = 8;
            MountData.standingFrameStart = 0;
            // Running
            MountData.runningFrameCount = 3;
            MountData.runningFrameDelay = 8;
            MountData.runningFrameStart = 0;
            // Flying
            MountData.flyingFrameCount = 3;
            MountData.flyingFrameDelay = 8;
            MountData.flyingFrameStart = 0;
            // In-air
            MountData.inAirFrameCount = 3;
            MountData.inAirFrameDelay = 8;
            MountData.inAirFrameStart = 0;
            // Idle
            MountData.idleFrameCount = 0;
            MountData.idleFrameDelay = 8;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;
            // Swim
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;

            if (!Main.dedServ)
            {
                MountData.textureWidth = MountData.backTexture.Width() + 20;
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }

        public override void UpdateEffects(Player player)
        {
            // 不受重力影响
            player.gravity = 0f;

            // 控制上升和下降
            if (player.controlJump)
            {
                player.velocity.Y = MathHelper.Clamp(
                    player.velocity.Y - FlightAcceleration,
                    -MaxVerticalSpeed,
                    MaxVerticalSpeed
                ); // 上升
            }
            else if (player.controlDown)
            {
                player.velocity.Y = MathHelper.Clamp(
                    player.velocity.Y + FlightAcceleration,
                    -MaxVerticalSpeed,
                    MaxVerticalSpeed
                ); // 下降
            }
            else
            {
                player.velocity.Y *= DecelerationFactor; // 缓慢减速至静止
            }

            // 控制左右移动
            if (player.controlLeft)
            {
                player.velocity.X = MathHelper.Clamp(
                    player.velocity.X - MountData.acceleration,
                    -MountData.runSpeed,
                    MountData.runSpeed
                );
            }
            else if (player.controlRight)
            {
                player.velocity.X = MathHelper.Clamp(
                    player.velocity.X + MountData.acceleration,
                    -MountData.runSpeed,
                    MountData.runSpeed
                );
            }
            else
            {
                player.velocity.X *= DecelerationFactor; // 缓慢减速至静止
            }

            // 常驻粉色粒子特效
            Vector2 pinkDustOffset = new Vector2(-40 * player.direction, -30); // 设定偏移量，这里乘以玩家方向是为了当玩家镜像翻转时，贴图也会镜像翻转
            for (int i = 0; i < 3; i++) // 每帧生成3个粒子
            {
                Vector2 pinkDustPosition =
                    player.Center
                    + pinkDustOffset
                    + new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10));
                int pinkDustIndex = Dust.NewDust(
                    pinkDustPosition,
                    0,
                    0,
                    DustID.PinkTorch,
                    0f,
                    0f,
                    100,
                    default,
                    1.5f
                );
                Main.dust[pinkDustIndex].noGravity = true; // 粉色粒子不受重力影响
            }

            // 添加技能检测和冷却逻辑
            if (!player.buffType.Contains(ModContent.BuffType<Buffs.JetragonBeamCometBuff>()))
            {
                // 检查是否按下X键并且当前没有在发射弹幕
                if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.X) && !isShooting)
                {
                    // 启动异步发射弹幕
                    isShooting = true;
                    FireProjectiles(player);
                }
            }

            if (!player.buffType.Contains(ModContent.BuffType<Buffs.JetragonMeteorBuff>()))
            {
                if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.C) && !isShootingMeteor)
                {
                    isShootingMeteor = true;
                    ShootMeteors(player);
                }
            }

            if (!player.buffType.Contains(ModContent.BuffType<Buffs.JetragonFlareStormBuff>()))
            {
                if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Z) && !isShootingFlareStorm)
                {
                    isShootingFlareStorm = true;
                    FireFlareStorm(player);
                }
            }
        }

        public override void Load()
        {
            base.Load();
        }

        private async void FireProjectiles(Player player)
        {
            Vector2 mousePosition = Main.MouseWorld; // 获取鼠标位置
            Vector2 playerPosition = player.Center; // 获取玩家位置

            for (int i = 0; i < 6; i++)
            {
                Vector2 offset = new Vector2(player.direction * -20, -20); // 在X轴上偏移20个单位，可以根据需要调整偏移量

                // 应用偏移量到player.Center
                Vector2 spawnPosition = playerPosition + offset;

                // 计算方向向量，从玩家到鼠标
                Vector2 directionToMouse = mousePosition - spawnPosition;

                // 仅保留水平分量，忽略垂直方向
                directionToMouse.Normalize(); // 归一化方向向量

                // 设置速度，调整速度大小以适应你的需求
                Vector2 projectileVelocity = directionToMouse * 10f; // 10f 为弹幕速度的大小

                try
                {
                    // 创建弹幕
                    Projectile.NewProjectile(player.GetSource_FromThis(), spawnPosition, projectileVelocity, ModContent.ProjectileType<Projectiles.BeamCometProjectile>(), 200, 2, player.whoAmI);
                }
                catch (Exception ex)
                {
                    // 处理异常，记录错误或采取其他措施
                    Main.NewText($"Error creating projectile: {ex.Message}");
                    continue;
                }

                // 等待0.2秒（200毫秒）
                await Task.Delay(200);
            }

            // 添加冷却Buff
            player.AddBuff(ModContent.BuffType<Buffs.JetragonBeamCometBuff>(), 300); // 10秒

            // 完成发射后重置状态
            isShooting = false;
        }

        private async void ShootMeteors(Player player)
        {
            Vector2 mousePosition = Main.MouseWorld;
            for (int k = 0; k < 4; k++)
            {
                for (int i = 0; i < 4; i++)
                {
                    // 随机偏移以模拟不同的落点
                    Vector2 offset = new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100));
                    Vector2 targetPosition = mousePosition + offset;

                    // 从玩家位置到目标位置的方向
                    Vector2 direction = targetPosition - player.Center;
                    direction.Normalize();
                    float speed = 10f; // 陨石速度

                    // 随机化初始速度方向（沿弧形路径）
                    float angle = Main.rand.NextFloat(-MathHelper.Pi / 4, MathHelper.Pi / 4); // 随机弧形角度
                    Vector2 initialVelocity = new Vector2(
                        direction.X * (float)Math.Cos(angle) - direction.Y * (float)Math.Sin(angle),
                        direction.X * (float)Math.Sin(angle) + direction.Y * (float)Math.Cos(angle)
                    ) * speed;

                    // 应用偏移量到player.Center
                    Vector2 offsetProj = new Vector2(
                        (player.direction * -30) + Main.rand.NextFloat(-10f, 10f), // 随机化X偏移量
                        -60 + Main.rand.NextFloat(-10f, 10f) // 随机化Y偏移量
                    );

                    Vector2 spawnPosition = player.Center + offsetProj;
                    try
                    {
                        // 创建弹幕
                        int projIndex = Projectile.NewProjectile(player.GetSource_FromThis(), spawnPosition, initialVelocity, ModContent.ProjectileType<Projectiles.DragonMeteorProjectile>(), 200, 2, player.whoAmI);
                        // 获取创建的弹幕
                        Projectile projectile = Main.projectile[projIndex];

                        // 设置弹幕的初始状态，确保它会在开始时沿弧形路径运动
                        projectile.ai[0] = 1; // 使用 ai[0] 标记弹幕已开始弧形运动
                    }
                    catch (NullReferenceException ex)
                    {
                        continue;
                    }
                    // 等待0.1秒（100毫秒）
                    await Task.Delay(50);
                }
                isShootingMeteor = false;
                // 等待0.3秒（300毫秒）
                await Task.Delay(500);
            }
            player.AddBuff(ModContent.BuffType<Buffs.JetragonMeteorBuff>(), 1200);
        }

        private async void FireFlareStorm(Player player)
        {
            Vector2 mousePosition = Main.MouseWorld; // 获取鼠标位置
            Vector2 playerPosition = player.Center; // 获取玩家位置

            for (int i = 0; i < 5; i++) // 生成5个FlareStorm弹幕
            {
                Vector2 spawnPosition = playerPosition + new Vector2(0, 10); // 在玩家下方生成弹幕

                // 计算方向向量，从玩家到鼠标
                Vector2 directionToMouse = mousePosition - spawnPosition;
                directionToMouse.Y = 0;
                directionToMouse.Normalize(); // 归一化方向向量

                // 设置速度，调整速度大小以适应你的需求
                Vector2 velocity = directionToMouse * 10f;

                try
                {
                    // 创建FlareStorm弹幕
                    Projectile.NewProjectile(player.GetSource_FromThis(), spawnPosition, velocity, ModContent.ProjectileType<Projectiles.FlareStormProjectile>(), 300, 2, player.whoAmI);
                }
                catch (NullReferenceException ex)
                {
                    // 处理异常，记录错误或采取其他措施
                    continue;
                }

                // 等待0.1秒（100毫秒）
                await Task.Delay(100);
            }

            // 添加冷却时间

            player.AddBuff(ModContent.BuffType<Buffs.JetragonFlareStormBuff>(), 1800); // 10秒

            // 完成发射后重置状态
            isShootingFlareStorm = false;
        }

    }
}

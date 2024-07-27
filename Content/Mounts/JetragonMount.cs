using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Linq;

namespace PolWorldMounts.Content.Mounts
{
    public class JetragonMount : ModMount
    {
        private const float FlightAcceleration = 0.8f; // 飞行加速度
        private const float MaxVerticalSpeed = 25f; // 最大垂直速度
        private const float HorizontalAcceleration = 0.1f; // 水平加速度
        private const float DecelerationFactor = 0.95f; // 减速因子

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
            MountData.idleFrameCount = 3;
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
                player.velocity.Y = MathHelper.Clamp(player.velocity.Y - FlightAcceleration, -MaxVerticalSpeed, MaxVerticalSpeed); // 上升
            }
            else if (player.controlDown)
            {
                player.velocity.Y = MathHelper.Clamp(player.velocity.Y + FlightAcceleration, -MaxVerticalSpeed, MaxVerticalSpeed); // 下降
            }
            else
            {
                player.velocity.Y *= DecelerationFactor; // 缓慢减速至静止
            }

            // 控制左右移动
            if (player.controlLeft)
            {
                player.velocity.X = MathHelper.Clamp(player.velocity.X - MountData.acceleration, -MountData.runSpeed, MountData.runSpeed);
            }
            else if (player.controlRight)
            {
                player.velocity.X = MathHelper.Clamp(player.velocity.X + MountData.acceleration, -MountData.runSpeed, MountData.runSpeed);
            }
            else
            {
                player.velocity.X *= DecelerationFactor; // 缓慢减速至静止
            }

            // 常驻粉色粒子特效
            Vector2 pinkDustOffset = new Vector2(-40 * player.direction, -30); // 设定偏移量
            for (int i = 0; i < 3; i++) // 每帧生成3个粒子
            {
                Vector2 pinkDustPosition = player.Center + pinkDustOffset + new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10));
                int pinkDustIndex = Dust.NewDust(pinkDustPosition, 0, 0, DustID.PinkTorch, 0f, 0f, 100, default, 1.5f);
                Main.dust[pinkDustIndex].noGravity = true; // 粉色粒子不受重力影响
            }
        }
    }
}

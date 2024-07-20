using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameInput;
using System.Linq;
using Microsoft.Build.Evaluation;

namespace PolWorldMounts.Content.Mounts
{
    public class FenglopeMount : ModMount
    {
        private const int DashCooldown = 60; // 冲刺冷却时间，单位：帧
        private const int DashDuration = 120; // 冲刺持续时间，单位：帧
        private const float DashSpeed = 30f; // 冲刺速度
        private const int DashDamage = 50; // 冲刺造成的伤害
        private const float DashKnockBack = 10f; // 冲刺造成的击退力



        private int dashTimeLeft = 0;
        private int dashCooldown = 0;

        public override void SetStaticDefaults()
        {
            MountData.jumpHeight = 20; // How high the mount can jump.
            MountData.acceleration = 0.19f; // The rate at which the mount speeds up.
            MountData.jumpSpeed = 20f; // The rate at which the player and mount ascend towards (negative y velocity) the jump height when the jump button is pressed.
            MountData.blockExtraJumps = false; // Determines whether or not you can use a double jump (like cloud in a bottle) while in the mount.
            MountData.constantJump = true; // Allows you to hold the jump button down.
            MountData.heightBoost = 20; // Height between the mount and the ground
            MountData.fallDamage = 0f; // Fall damage multiplier.
            MountData.runSpeed = 11f; // The speed of the mount
            MountData.dashSpeed = 8f; // The speed the mount moves when in the state of dashing.
            MountData.flightTimeMax = 0; // The amount of time in frames a mount can be in the state of flying.

            MountData.fatigueMax = 0;
            MountData.buff = ModContent.BuffType<Buffs.FenglopeMountBuff>(); // The ID number of the buff assigned to the mount.

            // Effects
            // MountData.spawnDust = ModContent.DustType<Dusts.Sparkle>(); // The ID of the dust spawned when mounted or dismounted.

            // Frame data and player offsets
            MountData.totalFrames = 5; // Amount of animation frames for the mount
            MountData.playerYOffsets = Enumerable.Repeat(20, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code
            MountData.xOffset = 20;
            MountData.yOffset = -12;
            MountData.playerHeadOffset = 22;
            MountData.bodyFrame = 0;
            // Standing
            MountData.standingFrameCount = 0;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;
            // Running
            MountData.runningFrameCount = 4;
            MountData.runningFrameDelay = 120;
            MountData.runningFrameStart = 1;
            // Flying
            MountData.flyingFrameCount = 0;
            MountData.flyingFrameDelay = 0;
            MountData.flyingFrameStart = 2;
            // In-air
            MountData.inAirFrameCount = 1;
            MountData.inAirFrameDelay = 12;
            MountData.inAirFrameStart = 2;
            // Idle
            MountData.idleFrameCount = 4;
            MountData.idleFrameDelay = 12;
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
            if (dashCooldown > 0)
            {
                dashCooldown--;
            }

            if (dashTimeLeft > 0)
            {
                dashTimeLeft--;
                player.velocity.X = player.direction * DashSpeed;

                Rectangle hitbox = new Rectangle((int)(player.position.X + player.velocity.X), (int)(player.position.Y + player.velocity.Y), player.width, player.height);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (target.active && !target.friendly && target.Hitbox.Intersects(hitbox))
                    {
                        NPC.HitInfo hitInfo = new NPC.HitInfo{
                            Damage = DashDamage,
                            Knockback = DashKnockBack,
                            HitDirection = player.direction
                        };
                        target.StrikeNPC(hitInfo);
                    }
                }

                // 生成粒子特效
                for (int i = 0; i < 10; i++) // 每帧生成10个粒子
                {
                    Vector2 dustPosition = player.Center + new Vector2(Main.rand.Next(-20, 5), Main.rand.Next(-20, 5));
                    Dust.NewDust(dustPosition, 0, 0, DustID.Snow, player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 100, default, 1.5f);
                }
            }
            else if (dashCooldown == 0)
            {
                // 检测冲刺按键 (例如 X 键)
                if (PlayerInput.Triggers.JustPressed.MouseRight)
                {
                    dashTimeLeft = DashDuration;
                    dashCooldown = DashCooldown;
                }
            }
        }
    }
}

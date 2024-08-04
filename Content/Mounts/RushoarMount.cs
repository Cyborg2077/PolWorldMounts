using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameInput;
using System.Linq;
using Terraria.Audio;
using Microsoft.Xna.Framework.Input;
using PolWorldMounts.Content;

namespace PolWorldMounts.Content.Mounts
{
    public class RushoarMount : ModMount
    {
        private Keys mountDashKey;
        private const int DashCooldown = 60; // 冲刺冷却时间，单位：帧
        private const int DashDuration = 90; // 冲刺持续时间，单位：帧
        private const float DashSpeed = 12f; // 冲刺速度
        private const int DashDamage = 50; // 冲刺造成的伤害
        private const float DashKnockBack = 10f; // 冲刺造成的击退力

        private int dashTimeLeft = 0;
        private int dashCooldown = 0;

        public override void SetStaticDefaults()
        {
            MountData.jumpHeight = 5; // How high the mount can jump.
            MountData.acceleration = 0.19f; // The rate at which the mount speeds up.
            MountData.jumpSpeed = 5f; // The rate at which the player and mount ascend towards (negative y velocity) the jump height when the jump button is pressed.
            MountData.blockExtraJumps = true; // 阻止饰品增加跳跃次数
            MountData.constantJump = true; // Allows you to hold the jump button down.
            MountData.heightBoost = -20; // Height between the mount and the ground
            MountData.fallDamage = 0f; // Fall damage multiplier.
            MountData.runSpeed = 8f; // The speed of the mount
            MountData.dashSpeed = 8f; // The speed the mount moves when in the state of dashing.
            MountData.flightTimeMax = 0; // The amount of time in frames a mount can be in the state of flying.

            MountData.fatigueMax = 0;
            MountData.buff = ModContent.BuffType<Buffs.RushoarMountBuff>(); // The ID number of the buff assigned to the mount.

            // Effects
            // MountData.spawnDust = ModContent.DustType<Dusts.Sparkle>(); // The ID of the dust spawned when mounted or dismounted.

            // Frame data and player offsets
            MountData.totalFrames = 5; // Amount of animation frames for the mount
            MountData.playerYOffsets = Enumerable.Repeat(20, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code
            MountData.xOffset = 20;
            MountData.yOffset = -12;
            MountData.playerHeadOffset = 22;
            MountData.bodyFrame = 3;
            // Standing
            MountData.standingFrameCount = 0;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;
            // Running
            MountData.runningFrameCount = 4;
            MountData.runningFrameDelay = 50;
            MountData.runningFrameStart = 0;
            // Flying
            MountData.flyingFrameCount = 0;
            MountData.flyingFrameDelay = 0;
            MountData.flyingFrameStart = 2;
            // In-air
            MountData.inAirFrameCount = 1;
            MountData.inAirFrameDelay = 12;
            MountData.inAirFrameStart = 1;
            // Idle
            MountData.idleFrameCount = 0;
            MountData.idleFrameDelay = 12;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;
            // Swim
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;

            mountDashKey = ModContent.GetInstance<PolworldModConfig>().MountDashKey;

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
                BreakTreesAlongPath(player, hitbox);

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (target.active && !target.friendly && target.Hitbox.Intersects(hitbox))
                    {
                        NPC.HitInfo hitInfo = new NPC.HitInfo
                        {
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
                    Dust.NewDust(dustPosition, 0, 0, DustID.Dirt, player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 100, default, 1.5f);
                }
            }
            else if (dashCooldown == 0 && !player.HasBuff(ModContent.BuffType<Buffs.RushoarExhaustedBuff>()))
            {
                mountDashKey = ModContent.GetInstance<PolworldModConfig>().MountDashKey;
                if (Main.keyState.IsKeyDown(mountDashKey))
                {
                    dashTimeLeft = DashDuration;
                    dashCooldown = DashCooldown;
                    player.AddBuff(ModContent.BuffType<Buffs.RushoarExhaustedBuff>(), 600);
                }
            }
        }

        private void BreakTreesAlongPath(Player player, Rectangle hitbox)
        {
            int tileStartX = hitbox.Left / 16;
            int tileEndX = hitbox.Right / 16;
            int tileStartY = hitbox.Top / 16;
            int tileEndY = hitbox.Bottom / 16;

            for (int x = tileStartX; x <= tileEndX; x++)
            {
                for (int y = tileStartY; y <= tileEndY; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if ((tile.TileType == TileID.Trees
                    //tile.TileType == TileID.PalmTree ||
                    //tile.TileType == TileID.VanityTreeSakura
                    )
                    && tile.HasTile)
                    {
                        // 如果是树顶
                        if (tile.TileFrameY == 0)
                        {
                            WorldGen.KillTile(x, y, false, false, true);
                            SoundEngine.PlaySound(SoundID.Item14);
                        }
                        // 如果是树身
                        else if (tile.TileFrameY % 22 == 0)
                        {
                            WorldGen.KillTile(x, y, false, false, true);
                            SoundEngine.PlaySound(SoundID.Item14);
                        }
                    }
                }
            }
        }
    }
}

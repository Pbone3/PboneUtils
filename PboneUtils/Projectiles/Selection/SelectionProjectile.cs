﻿using System;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using PboneUtils.UI.States;
using PboneUtils.UI;
using Terraria.GameContent;

namespace PboneUtils.Projectiles.Selection
{
    public abstract class SelectionProjectile : PboneUtilsProjectile
    {
        public virtual Action<int, int> TileAction { get; }
        public virtual Func<Rectangle, bool> PreAction { get; }

        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.ShadowBeamFriendly; // Aka invisible

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        // AI Properties
        public bool Initialized { get => Projectile.localAI[0] == 1; set => Projectile.localAI[0] = value ? 1 : 0; }
        public float StartX { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public float StartY { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }

        public Point StartPosition => new Vector2(StartX, StartY).ToPoint();
        public Point CurrentPosition => Projectile.Center.ToTileCoordinates();

        public override bool? CanCutTiles() => false;
        public override void AI()
        {
            base.AI();

            if (Owner.whoAmI == Main.myPlayer) // Only run on the owners client
            {
                // Kill if owner can't use items, is CCed (frozen, webbed, stoned), dead
                if (Owner.noItems || Owner.CCed || Owner.dead)
                {
                    Projectile.Kill();
                }

                // If they aren't channeling, then do stuff before killing yourself
                if (!Owner.channel)
                {
                    Rectangle rect = GetRectangle();
                    if (PreAction == null || PreAction(rect))
                    {
                        for (int i = 0; i < rect.Width / 16; i++)
                        {
                            for (int j = 0; j < rect.Height / 16; j++)
                            {
                                TileAction(rect.X / 16 + i, rect.Y / 16 + j);
                            }
                        }
                    }

                    Projectile.Kill();
                }

                Projectile.timeLeft = 2;
                Projectile.localAI[1]++; // Timer for draw fading

                Vector2 mouseWorld = Main.MouseWorld;
                if (Owner.gravDir == -1f)
                    mouseWorld.Y = (Main.screenHeight - Main.mouseY) + Main.screenPosition.Y;

                // Move to MouseWorld
                if (Projectile.Center != mouseWorld)
                {
                    Projectile.netUpdate = true;
                    Projectile.Center = mouseWorld;
                    Projectile.velocity = Vector2.Zero;
                }

                // Only run once
                if (!Initialized)
                {
                    // Save the tile coords of the start position to ai0 and ai1
                    StartX = (int)Projectile.Center.X / 16;
                    StartY = (int)Projectile.Center.Y / 16;

                    Initialized = true;
                }

                Projectile.velocity = Vector2.Zero;

                int ownerDir = Math.Sign(Owner.velocity.X);
                if (ownerDir != 0)
                    Owner.ChangeDir(ownerDir);

                Owner.heldProj = Projectile.whoAmI;
                Owner.itemRotation = 0f;
                Owner.itemAnimation = 2;
                Owner.itemTime = 2;
                Owner.itemAnimationMax = 3;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            overWiresUI.Add(Projectile.whoAmI);
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);

            RadialMenu menu = PboneUtils.UI.GetUIState<RadialMenuContainer>().Internal;
            if (Owner.whoAmI == Main.myPlayer && menu != null && !menu.IsHovered()) // Only draw on owner's client
            {
                float fade = MathHelper.Lerp(0.1f, 0.3f, (float)(Math.Sin(Main.GameUpdateCount / 10f) + 1f) / 2f);
                Color color = Color.Lerp(Color.Transparent, Color.White, fade);

                Rectangle destination = GetRectangle();
                destination.X -= (int)Main.screenPosition.X;
                destination.Y -= (int)Main.screenPosition.Y;

                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, destination, null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
        }

        public Rectangle GetRectangle()
        {
            Rectangle rect = new Rectangle(
                (int)(StartX * 16f),
                (int)(StartY * 16f),
                (int)((CurrentPosition.X - (int)StartX) * 16f),
                (int)((CurrentPosition.Y - (int)StartY) * 16f)
            );

            if (rect.Width >= 0 && rect.Width < 16)
            {
                rect.Width = 16;
            }
            if (rect.Height >= 0 && rect.Height < 16)
            {
                rect.Height = 16;
            }

            // Don't be negative, please
            if (rect.Width < 0)
            {
                rect.Width *= -1;
                if (rect.Width < 16)
                    rect.Width = 16;

                rect.X -= rect.Width;
            }
            if (rect.Height < 0)
            {
                rect.Height *= -1;
                if (rect.Height < 16)
                    rect.Height = 16;

                rect.Y -= rect.Height;
            }

            return rect;
        }
    }
}

﻿using Terraria;
using Terraria.ModLoader;
using PboneUtils.Projectiles.Storage;
using PboneUtils.ID;
using Terraria.ID;
using System.Collections.Generic;
using PboneUtils.DataStructures;
using Terraria.ModLoader.IO;

namespace PboneUtils
{
    public class PbonePlayer : ModPlayer
    {
        #region Fields
        // Storage
        public bool VoidPig;
        public int SafeGargoyleChest;
        public bool SafeGargoyleOpen = false;

        // Tools
        public bool DeluxeTreasureMagnet;

        // Item Config
        public Dictionary<string, ItemConfig> ItemConfigs;
        #endregion

        public override void Initialize()
        {
            base.Initialize();
            VoidPig = false;
            SafeGargoyleChest = -1;
            SafeGargoyleOpen = false;
            DeluxeTreasureMagnet = false;

            ItemConfigs = ItemConfig.DefaultConfigs();
        }

        public override void OnEnterWorld(Player player)
        {
            base.OnEnterWorld(player);
            PboneUtils.UI.OpenRadialMenu("Liquid");
        }

        public override TagCompound Save()
        {
            base.Save();
            TagCompound tag = new TagCompound();
            foreach (KeyValuePair<string, ItemConfig> kvp in ItemConfigs)
            {
                tag.Add(kvp.Key, kvp.Value.Save());
            }

            return tag;
        }

        public override void Load(TagCompound tag)
        {
            base.Load(tag);
            foreach (string s in ItemConfigs.Keys)
            {
                ItemConfigs[s].Load(tag, s);
            }
        }

        public override void ResetEffects()
        {
            base.ResetEffects();
            VoidPig = false;
            DeluxeTreasureMagnet = false;
        }

        public override void PreUpdateBuffs()
        {
            base.PreUpdateBuffs();
            if (SafeGargoyleChest >= 0)
            {
                if (Main.projectile[SafeGargoyleChest].active && Main.projectile[SafeGargoyleChest].type == ModContent.ProjectileType<PetrifiedSafeProjectile>())
                {
                    int oldChest = player.chest;
                    player.chest = ChestID.Safe;
                    SafeGargoyleOpen = true;

                    int num17 = (int)((player.position.X + player.width * 0.5) / 16.0);
                    int num18 = (int)((player.position.Y + player.height * 0.5) / 16.0);
                    player.chestX = (int)Main.projectile[SafeGargoyleChest].Center.X / 16;
                    player.chestY = (int)Main.projectile[SafeGargoyleChest].Center.Y / 16;
                    if ((oldChest != -3 && oldChest != -1) || num17 < player.chestX - Player.tileRangeX || num17 > player.chestX + Player.tileRangeX + 1 || num18 < player.chestY - Player.tileRangeY || num18 > player.chestY + Player.tileRangeY + 1)
                    {
                        SafeGargoyleChest = -1;
                        if (player.chest != -1)
                        {
                            Main.PlaySound(SoundID.Item37);
                        }

                        if (oldChest != -3)
                            player.chest = oldChest;
                        else
                            player.chest = -1;

                        Recipe.FindRecipes();
                    }
                }
                else
                {
                    Main.PlaySound(SoundID.Item37);
                    SafeGargoyleChest = -1;
                    player.chest = ChestID.None;
                    Recipe.FindRecipes();
                }
            }
        }
    }
}
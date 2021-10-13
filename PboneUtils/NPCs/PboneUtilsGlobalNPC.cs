﻿using PboneUtils.Items.Storage;
using PboneUtils.Items.Misc;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using PboneUtils.MiscModsPlayers;
using PboneLib.CustomLoading.Content.Implementations;
using Terraria.GameContent.Bestiary;
using PboneUtils.Items.Clovers;

namespace PboneUtils.NPCs
{
    public class PboneUtilsGlobalNPC : PGlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            switch (npc.type)
            {
                case NPCID.DD2Betsy:
                    if (PboneUtilsConfig.Instance.StorageItemsToggle)
                        npcLoot.Add(new DropPerPlayerOnThePlayer(ModContent.ItemType<DefendersCrystal>(), 1, 1, 1, null));
                    break;
            }
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            switch (type)
            {
                case NPCID.Wizard:
                    if (PboneUtilsConfig.Instance.PhilosophersStoneToggle)
                    {
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<PhilosophersStone>());
                        nextSlot++;
                    }
                    break;

                case NPCID.BestiaryGirl:
                    if (PboneUtilsConfig.Instance.CloversToggle)
                    {
                        BestiaryUnlockProgressReport bestiaryProgressReport = Main.GetBestiaryProgressReport();

                        if (bestiaryProgressReport.CompletionPercent >= 0.07f)
                        {
                            shop.item[nextSlot].SetDefaults(ModContent.ItemType<FourLeafClover>());
                            nextSlot++;
                        }
                    }
                    break;
            }
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
            PbonePlayer modPlayer = player.GetModPlayer<PbonePlayer>();
            spawnRate = (int)(spawnRate * modPlayer.SpawnRateMultiplier);
            maxSpawns = (int)(maxSpawns * modPlayer.MaxSpawnsMultiplier);
        }
    }
}

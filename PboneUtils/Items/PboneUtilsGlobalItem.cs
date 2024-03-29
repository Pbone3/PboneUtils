﻿using PboneLib.CustomLoading.Content.Implementations.Globals;
using PboneLib.Utils;
using PboneUtils.Helpers;
using PboneUtils.Items.Misc;
using PboneUtils.MiscModsPlayers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace PboneUtils.Items
{
    public class PboneUtilsGlobalItem : PGlobalItem
    {
        public override void SetDefaults(Item item)
        {
            base.SetDefaults(item);
            TreasureBagValueCalculator calc = ModContent.GetInstance<TreasureBagValueCalculator>();
            if (TreasureBagValueCalculator.Loaded && PboneUtilsConfig.Instance.AverageBossBags && item.value == 0 && calc.AveragedValues.ContainsKey(item.type))
            {
                item.value = calc.AveragedValues[item.type];
            }

            if (PboneUtilsConfig.Instance.MaxStackIncrease)
            {
                // Any non-coin with a max stack greater than twenty
                if (!CoinHelper.CoinTypes.Contains(item.type) && item.maxStack >= 20)
                {
                    // Increase to 9999 if it's below
                    item.TryIncreaseMaxStack(9999);
                }
            }
        }

        public override void UpdateInventory(Item item, Player player)
        {
            base.UpdateInventory(item, player);
            PbonePlayer mPlayer = player.GetModPlayer<PbonePlayer>();

            if (mPlayer.VoidPig && CoinHelper.CoinTypes.Contains(item.type))
            {
                CoinHelper.VoidPig(player.inventory, player.bank.item);
            }
        }

        public override bool OnPickup(Item item, Player player)
        {
            PbonePlayer mPlayer = player.GetModPlayer<PbonePlayer>();

            if (mPlayer.PhilosophersStone && player.HeldItem.type == ModContent.ItemType<PhilosophersStone>() && !CoinHelper.CoinTypes.Contains(item.type))
            {
                SoundEngine.PlaySound(SoundID.CoinPickup);
                player.SellItem(item, item.stack);

                return false;
            }

            return base.OnPickup(item, player);
        }

        public override bool CanPickup(Item item, Player player)
        {
            PbonePlayer mPlayer = player.GetModPlayer<PbonePlayer>();
            if (mPlayer.PhilosophersStone && player.HeldItem.type == ModContent.ItemType<PhilosophersStone>())
            {
                return true;
            }

            return base.CanPickup(item, player);
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            if ((item.healLife > 0 || item.healMana > 0) && PboneUtilsConfig.Instance.EndlessPotions && item.stack >= PboneUtilsConfig.Instance.EndlessPotionsSlider)
                return false;

            return base.ConsumeItem(item, player);
        }
    }
}

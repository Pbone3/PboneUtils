﻿using PboneLib.CustomLoading.Content.Implementations.Globals;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PboneUtils.Items.CellPhoneApps
{
    public partial class AppGlobalItem : PGlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => PhoneItems.Contains(entity.type);

        public List<(int item, string appId)> Apps = new List<(int, string)>();

        // Shellphone already has right click functionality which changes its destination.
        public static List<int> PhoneItems = new() { ItemID.CellPhone }; //, ItemID.Shellphone, ItemID.ShellphoneSpawn, ItemID.ShellphoneHell, ItemID.ShellphoneOcean }; // No ShellphoneDummy

		public override GlobalItem Clone(Item item, Item itemClone)
        {
            AppGlobalItem gItem = base.Clone(item, itemClone) as AppGlobalItem;
            gItem.Apps = Apps;

            return gItem;
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            if (PhoneItems.Contains(item.type))
            {
                tag.Add("AppCount", Apps.Count);

                for (int i = 0; i < Apps.Count; i++)
                {
                    tag.Add("AppId" + i, Apps[i].appId);
                    tag.Add("AppItem" + i, Apps[i].item);
                }
            }

            base.SaveData(item, tag);
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            if (PhoneItems.Contains(item.type))
            {
                Apps.Clear();

                int count = tag.GetAsInt("AppCount");

                for (int i = 0; i < count; i++)
                {
                    Apps.Add(
                        (tag.Get<int>("AppItem" + i),
                        tag.Get<string>("AppId" + i))
                        );
                }
            }

            base.LoadData(item, tag);
        }

        public override bool CanRightClick(Item item)
        {
            if (PhoneItems.Contains(item.type))
                return Main.mouseItem.ModItem is AppItem app && !Apps.Contains((app.BaseID, app.AppName));

            return base.CanRightClick(item);
        }

        public override void RightClick(Item item, Player player)
        {
            base.RightClick(item, player);

            if (PhoneItems.Contains(item.type))
            {
                if (Main.mouseItem.ModItem is AppItem app)
                {
                    if (Apps.Contains((app.BaseID, app.AppName)))
                        return;

                    Apps.Add((app.BaseID, app.AppName));
                    Main.mouseItem.TurnToAir();
                }
            }
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            if (PhoneItems.Contains(item.type))
                return false;

            return base.ConsumeItem(item, player);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            if (PhoneItems.Contains(item.type))
            {
                tooltips.Add(new TooltipLine(Mod, "PboneUtils:CellPhoneInfo", Language.GetTextValue("Mods.PboneUtils.Common.CellPhoneInfo")));

                foreach ((int item, string appId) tuple in Apps)
                {
                    tooltips.Add(
                        new TooltipLine(Mod, "PboneUtils:CellPhoneAppDescription-" + tuple.appId.GetHashCode(),
                        Language.GetTextValue("Mods.PboneUtils.Common.CellPhone." + tuple.appId)));
                }
            }
        }

        public override bool AltFunctionUse(Item item, Player player)
        {
            if (PhoneItems.Contains(item.type))
                return Apps.Contains((ItemID.TeleportationPotion, "Teleportation"));

            return base.AltFunctionUse(item, player);
        }

        public override bool? UseItem(Item item, Player player)
        {
            if (PhoneItems.Contains(item.type) && player.altFunctionUse == 2 && Apps.Contains((ItemID.TeleportationPotion, "Teleportation")))
            {
                player.TeleportationPotion();
                return true;
            }

            return base.UseItem(item, player);
        }
    }
}

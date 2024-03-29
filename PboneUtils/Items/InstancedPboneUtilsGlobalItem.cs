﻿using PboneLib.CustomLoading.Content.Implementations.Globals;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace PboneUtils.Items
{
    public class InstancedPboneUtilsGlobalItem : PGlobalItem
    {
        public override bool InstancePerEntity => true;
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            // TODO FIXME! Implement Clone properly
            //itemClone.GetGlobalItem<InstancedPboneUtilsGlobalItem>().AutoswingOrig = item.GetGlobalItem<InstancedPboneUtilsGlobalItem>().AutoswingOrig;
            //itemClone.GetGlobalItem<InstancedPboneUtilsGlobalItem>().ChangedAutoswing = item.GetGlobalItem<InstancedPboneUtilsGlobalItem>().ChangedAutoswing;
            return base.Clone(item, itemClone);
        }

        public bool AutoswingOrig;
        public bool ChangedAutoswing = false;

        public override void SetDefaults(Item item)
        {
            base.SetDefaults(item);
            AutoswingOrig = item.autoReuse;
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (PboneUtilsConfig.Instance.AutoswingOnEverything)
            {
                // Don't autoswing blacklisted items
                if (PboneUtilsConfig.Instance.AutoswingOnEverythingBlacklist != null)
                {
                    foreach (ItemDefinition itemDefinition in PboneUtilsConfig.Instance.AutoswingOnEverythingBlacklist)
                    {
                        if (itemDefinition.Type == item.type)
                        {
                            item.autoReuse = AutoswingOrig;
                            return true;
                        }
                    }
                }

                // Check for spears (at least vanilla ones)
                Projectile projectile = new Projectile();
                projectile.SetDefaults(item.shoot);
                if (projectile.aiStyle == 19)
                {
                    item.autoReuse = AutoswingOrig;
                    return true;
                }

                item.autoReuse = true;
                ChangedAutoswing = true;

                return true;
            }
            else if (!PboneUtilsConfig.Instance.AutoswingOnEverything && ChangedAutoswing)
            {
                item.autoReuse = AutoswingOrig;
                ChangedAutoswing = false;

                return true;
            }

            return true;
        }
    }
}

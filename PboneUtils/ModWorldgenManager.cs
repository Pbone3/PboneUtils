﻿using Microsoft.Xna.Framework;
using PboneLib.Utils;
using PboneUtils.Tiles;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace PboneUtils
{
    public class ModWorldgenManager
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public void GenPetrifiedSafes(GenerationProgress progress, GameConfiguration config)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            progress.Message = Language.GetTextValue("Mods.PboneUtils.WorldGen.PetrifiedSafes");
            progress.Value = 0;

            const int amount = 15;
            const int maxTries = 7500;

            int underworldHeight = Main.maxTilesY - 200;
            Vector2[] existingPositions = new Vector2[amount];

            for (int attempts = 0; attempts < amount; attempts++)
            {
                bool success = false;
                int tries = 0;

                while (!success)
                {
                    tries++;
                    if (tries > maxTries)
                        break;

                    int i = WorldGen.genRand.Next(2, Main.maxTilesX);
                    int j = WorldGen.genRand.Next(underworldHeight, Main.maxTilesY);
                    Vector2 pos = new Vector2(i, j);

                    bool tooClose = false;
                    int minimumDistance = 10;

                    foreach (Vector2 point in existingPositions)
                    {
                        if (Vector2.Distance(point, pos) < minimumDistance)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (tooClose)
                        continue;

                    success = MiscUtils.BetterPlaceObject(i, j, ModContent.TileType<PetrifiedSafeTile>());

                    if (success)
                        existingPositions[attempts] = new Vector2(i, j);
                }

                progress.Set(attempts / maxTries);
            }
        }
    }
}

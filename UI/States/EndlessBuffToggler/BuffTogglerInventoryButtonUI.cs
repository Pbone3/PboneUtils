﻿using Microsoft.Xna.Framework.Graphics;
using PboneUtils.UI.Elements;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace PboneUtils.UI.States.EndlessBuffToggler
{
    public class BuffTogglerInventoryButtonUI : UIState
    {
        public UIImage Icon;
        public UIHoverTextImageButton IconHighlight;

        public override void OnInitialize()
        {
            base.OnInitialize();

            int left = 26;
            // Uncomment this next fargo's update, so I don't overlap with my fargo's ui
            //if (PboneUtils.FargowiltasLoaded)
            //{
            //    left += 60; // 58 (width of inventory slot) + 2 (distance between inventory slots)
            //}

            Icon = new UIImage(PboneUtils.Textures.UI.BuffTogglerInventoryButton);
            Icon.Left.Set(left, 0f);
            Icon.Top.Set(262, 0f);
            Append(Icon);

            IconHighlight = new UIHoverTextImageButton(PboneUtils.Textures.UI.BuffTogglerInventoryButton_MouseOver, "dummy");
            IconHighlight.Left.Set(-2, 0f);
            IconHighlight.Top.Set(-2, 0f);
            IconHighlight.SetVisibility(1f, 0f);
            IconHighlight.OnClick += IconHighlight_OnClick;
            Icon.Append(IconHighlight);

            base.OnActivate();
        }

        private void IconHighlight_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            PboneUtils.UI.BuffToggler.ToggleBuffTogglerMenu();
            PboneUtils.UI.BuffToggler.BuffTogglerMenu.RebuildGrid();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            IconHighlight.Text = Language.GetTextValue("Mods.PboneUtils.UI.EndlessBuffTogglerInventoryButton.MouseOver");

            if (Main.playerInventory)
                base.Draw(spriteBatch);
        }
    }
}
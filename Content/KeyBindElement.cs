using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using System;
using Terraria;
using Terraria.ModLoader;

namespace PolWorldMounts.Content
{
    public class KeyBindElement : ConfigElement
    {
        private Keys currentKey;
        private bool waitingForKey;

        public override void OnBind()
        {
            base.OnBind();
            currentKey = (Keys)MemberInfo.GetValue(Item);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            waitingForKey = true;
            Main.blockInput = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (waitingForKey)
            {
                foreach (Keys key in Enum.GetValues(typeof(Keys)))
                {
                    if (Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key))
                    {
                        currentKey = key;
                        waitingForKey = false;
                        Main.blockInput = false;
                        MemberInfo.SetValue(Item, currentKey);
                        break;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            CalculatedStyle dimensions = GetDimensions();
            string textToDraw = waitingForKey ? "请输入按键..." : currentKey.ToString();
            Vector2 textPosition = new Vector2(dimensions.X + 200, dimensions.Y); // 调整 +200 以右移文本显示位置
            Utils.DrawBorderString(spriteBatch, textToDraw, textPosition, Color.White);
        }
    }
}

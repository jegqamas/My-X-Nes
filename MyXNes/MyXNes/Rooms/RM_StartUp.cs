/*********************************************************************\
*This file is part of My X Nes                                        *
*A Nintendo Entertainment System Emulator.                            *
*                                                                     *
*Copyright (C) 2010 - 2011 Ala Hadid                                  *
*E-mail: mailto:ahdsoftwares@hotmail.com                              *
*                                                                     *
*My X Nes is free software: you can redistribute it and/or modify     *
*it under the terms of the GNU General Public License as published by *
*the Free Software Foundation, either version 3 of the License, or    *
*(at your option) any later version.                                  *
*                                                                     *
*My X Nes is distributed in the hope that it will be useful,          *
*but WITHOUT ANY WARRANTY; without even the implied warranty of       *
*MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
*GNU General Public License for more details.                         *
*                                                                     *
*You should have received a copy of the GNU General Public License    *
*along with My X Nes.  If not, see <http://www.gnu.org/licenses/>.    *
\*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace MyXNes
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class RM_StartUp : Microsoft.Xna.Framework.GameComponent
    {
        SpriteFont Font_small;
        SpriteFont Font_normal;
        int CountDown = 240;
        int ColorValue = 0;
        bool FadeIn = true;

        public RM_StartUp(Game game)
            : base(game)
        {
            Font_normal = Program.CORE.Content.Load<SpriteFont>("Font_normal");
            Font_small = Program.CORE.Content.Load<SpriteFont>("Font_small");
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (ColorValue < 0xFF & FadeIn)
                ColorValue += 3;
            else if (CountDown > 0)
            {
                FadeIn = false;
                CountDown--;
            }
            else
            {
                if (ColorValue > 0)
                    ColorValue -= 3;
                else
                {
                    Program.CORE.ROOM = CurrentRoom.Loading;
                }
            }

            base.Update(gameTime);
        }
        public void Draw()
        {
            Color color = new Color(ColorValue, ColorValue, ColorValue, ColorValue);
            //Draw the background
            Program.CORE.DrawBackground();
            //Draw about
            int Y = 185;
            Program.CORE.SpriteBatch.DrawString(Font_normal, "Nintendo Entertainment System Emulator (port of My Nes emulator).", new Vector2(100, Y), color); Y += 20;
            Program.CORE.SpriteBatch.DrawString(Font_normal, "Copyright (C) Ala Hadid 2010 - 2011.", new Vector2(100, Y), color); Y += 20;
            Program.CORE.SpriteBatch.DrawString(Font_normal, "E-Mail: ahdsoftwares@hotmail.com", new Vector2(100, Y), color);
            Y += 30;
            Program.CORE.SpriteBatch.DrawString(Font_normal, "My X Nes is free software; you can redistribute it and/or modify", new Vector2(100, Y), color); Y += 20;
            Program.CORE.SpriteBatch.DrawString(Font_normal, "it under the terms of the GNU General Public License as published by", new Vector2(100, Y), color); Y += 20;
            Program.CORE.SpriteBatch.DrawString(Font_normal, "the Free Software Foundation, either version 3 of the License, or", new Vector2(100, Y), color); Y += 20;
            Program.CORE.SpriteBatch.DrawString(Font_normal, "(at your option) any later version.", new Vector2(100, Y), color); Y += 25;
            Program.CORE.SpriteBatch.DrawString(Font_normal, "My X Nes is distributed in the hope that it will be useful,", new Vector2(100, Y), color); Y += 20;
            Program.CORE.SpriteBatch.DrawString(Font_normal, "but WITHOUT ANY WARRANTY; without even the implied warranty of", new Vector2(100, Y), color); Y += 20;
            Program.CORE.SpriteBatch.DrawString(Font_normal, "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the", new Vector2(100, Y), color); Y += 20;
            Program.CORE.SpriteBatch.DrawString(Font_normal, "GNU General Public License for more details.", new Vector2(100, Y), color); Y += 25;
            Program.CORE.SpriteBatch.DrawString(Font_normal, "You should have received a copy of the GNU General Public License", new Vector2(100, Y), color); Y += 20;
            Program.CORE.SpriteBatch.DrawString(Font_normal, "along with My X Nes.  If not, see <http://www.gnu.org/licenses/>.", new Vector2(100, Y), color);
            Y += 30;
            if (FadeIn)
                Program.CORE.SpriteBatch.DrawString(Font_normal, "NEVER REDISTRIBUTE WITH ROMS AND/OR SELL IT IN ANY FORM.", new Vector2(100, Y), color);
            else
                Program.CORE.SpriteBatch.DrawString(Font_normal, "NEVER REDISTRIBUTE WITH ROMS AND/OR SELL IT IN ANY FORM.", new Vector2(100, Y), Color.Red);
        }
    }
}

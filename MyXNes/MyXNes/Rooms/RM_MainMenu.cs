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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace MyXNes
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class RM_MainMenu : Microsoft.Xna.Framework.GameComponent
    {
        SpriteFont Font_normal;
        SpriteFont Font_large;
        SpriteFont Font_small;
        Texture2D tBackground;

        public int OptionIndex = 0;
        bool Pressed = false;
        int countdown = 0;

        public RM_MainMenu(Game game)
            : base(game)
        {
            tBackground = Program.CORE.Content.Load<Texture2D>("Background");
            Font_large = Program.CORE.Content.Load<SpriteFont>("Font_big");
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
            if (!Pressed)
            {
                countdown = 5;
                if (Keyboard.GetState().IsKeyDown(Keys.Down) |
                    GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                {
                    OptionIndex++; ;
                    if (OptionIndex > 4)
                        OptionIndex = 4;
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up) |
                    GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                {
                    OptionIndex--;
                    if (OptionIndex < 0)
                        OptionIndex = 0;
                    Pressed = true;
                }
            }
            else
            {
                if (countdown > 0)
                    countdown--;
                else
                    Pressed = false;
            }
            //ACTION
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) |
                GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
            {
                switch (OptionIndex)
                {
                    case 0://Browser
                        Program.CORE.ROOM = CurrentRoom.Browser;
                        break;
                    case 1://Oprions
                        Program.CORE.rSETTINGS.SelectedIndex = 0;
                        Program.CORE.ROOM = CurrentRoom.Options;
                        break;
                    case 2://Save manager
                        Program.CORE.rSAVEMANAGER.LoadFiles();
                        Program.CORE.ROOM = CurrentRoom.SaveManager;
                        break;
                    case 3://About
                        Program.CORE.ROOM = CurrentRoom.About;
                        break;
                    case 4://Exit
                        Program.CORE.Exit();
                        break;
                }
            }
            base.Update(gameTime);
        }

        public void Draw()
        {
            //Draw the background
            Program.CORE.DrawBackground();
            //Draw the buttons (or options)
            int Y = 270;
            int X = 40;
            if (OptionIndex == 0)
                Program.CORE.SpriteBatch.DrawString(Font_large, "Roms List",
                      new Vector2(X, Y), Color.Gold);
            else
                Program.CORE.SpriteBatch.DrawString(Font_large, "Roms List",
                   new Vector2(X, Y), Color.White);
            //options
            Y += 30;
            if (OptionIndex == 1)
                Program.CORE.SpriteBatch.DrawString(Font_large, "Settings", new Vector2(X, Y), Color.Gold);
            else
                Program.CORE.SpriteBatch.DrawString(Font_large, "Settings", new Vector2(X, Y), Color.White);
            //Save manager
            Y += 30;
            if (OptionIndex == 2)
                Program.CORE.SpriteBatch.DrawString(Font_large, "Save Manager", new Vector2(X, Y), Color.Gold);
            else
                Program.CORE.SpriteBatch.DrawString(Font_large, "Save Manager", new Vector2(X, Y), Color.White);
            Y += 30;
            //about
            if (OptionIndex == 3)
                Program.CORE.SpriteBatch.DrawString(Font_large, "About", new Vector2(X, Y), Color.Gold);
            else
                Program.CORE.SpriteBatch.DrawString(Font_large, "About", new Vector2(X, Y), Color.White);
            Y += 30;
            //exit
            if (OptionIndex == 4)
                Program.CORE.SpriteBatch.DrawString(Font_large, "Exit", new Vector2(X, Y), Color.Gold);
            else
                Program.CORE.SpriteBatch.DrawString(Font_large, "Exit", new Vector2(X, Y), Color.White);

            //Draw help
#if XBOX
            Program.CORE.SpriteBatch.DrawString(Font_small, @"""" + "Up/Down" + @"""" + "= select option, " + @"""" + "A" + @"""" + "= OK.", new Vector2(10, Program.CORE.GraphicsDevice.Viewport.Height - 30), Color.White);
#else
            Program.CORE.SpriteBatch.DrawString(Font_small, @"""" + "Up/Down" + @"""" + "= select option, " + @"""" + "Enter" + @"""" + "= OK.", new Vector2(10, Program.CORE.GraphicsDevice.Viewport.Height - 30), Color.White);
#endif
        }
    }
}

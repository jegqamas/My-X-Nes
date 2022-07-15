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
    public class RM_KeyConfig : Microsoft.Xna.Framework.GameComponent
    {
        Texture2D tFrame;
        SpriteFont Font_large;
        SpriteFont Font_normal;
        SpriteFont Font_small;
        int SelectedIndex = 0;
        public bool Pressed = true;
        public int countdown = 20;
        enum EditMode
        { None, EditingKey }
        EditMode MODE = EditMode.None;
        public RM_KeyConfig(Game game)
            : base(game)
        {
            //Load the textures
            tFrame = Program.CORE.Content.Load<Texture2D>("frame");
            Font_large = Program.CORE.Content.Load<SpriteFont>("Font_big");
            Font_normal = Program.CORE.Content.Load<SpriteFont>("FontNormalSmall");
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
            #region Not Editing
            if (MODE == EditMode.None)
            {

                if (!Pressed)
                {
                    countdown = 5;
                    //Exit
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape) |
                    GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    {
                        Program.CORE.rSETTINGS.countdown = 15;
                        Program.CORE.rSETTINGS.Pressed = true; 
                        Program.CORE.ROOM = CurrentRoom.Options;
                    }
                    //Move
                    if (Keyboard.GetState().IsKeyDown(Keys.Down) |
                        GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                    {
                        SelectedIndex++; ;
                        if (SelectedIndex > 17)
                            SelectedIndex = 0;
                        Pressed = true;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Up) |
                        GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                    {
                        SelectedIndex--;
                        if (SelectedIndex < 0)
                            SelectedIndex = 17;
                        Pressed = true;
                    }
                    //Action
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        switch (SelectedIndex)
                        {
                            case 16://Reset 
                                Program.Settings.P1_Left = Keys.Left;
                                Program.Settings.P1_Right = Keys.Right;
                                Program.Settings.P1_Up = Keys.Up;
                                Program.Settings.P1_Down = Keys.Down;
                                Program.Settings.P1_A = Keys.X;
                                Program.Settings.P1_B = Keys.Z;
                                Program.Settings.P1_Start = Keys.V;
                                Program.Settings.P1_Select = Keys.C;

                                Program.Settings.P2_Left = Keys.A;
                                Program.Settings.P2_Right = Keys.D;
                                Program.Settings.P2_Up = Keys.W;
                                Program.Settings.P2_Down = Keys.S;
                                Program.Settings.P2_A = Keys.K;
                                Program.Settings.P2_B = Keys.J;
                                Program.Settings.P2_Start = Keys.E;
                                Program.Settings.P2_Select = Keys.Q;

                                Program.CORE.WriteText("Keys reset to defaults", 120, TextStatus.NONE);
                                break;
                            case 17://Back
                                Program.CORE.rSETTINGS.Pressed = true;
                                Program.CORE.rSETTINGS.countdown = 15;
                                Program.CORE.ROOM = CurrentRoom.Options;
                                break;
                            default://Edit key
                                MODE = EditMode.EditingKey;
                                break;
                        }
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
            }
            #endregion
            else
            {
                if (!Pressed)
                {
                    countdown = 15;
                    //Exit editing
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        MODE = EditMode.None;
                        base.Update(gameTime); Pressed = true;
                        return;
                    }
                    //Pressing something
                    if (Keyboard.GetState().GetPressedKeys().Length > 0)
                    {
                        switch (SelectedIndex)
                        {
                            case 0: Program.Settings.P1_Up = Keyboard.GetState().GetPressedKeys()[0]; break;
                            case 1: Program.Settings.P1_Down = Keyboard.GetState().GetPressedKeys()[0]; break;
                            case 2: Program.Settings.P1_Right = Keyboard.GetState().GetPressedKeys()[0]; break;
                            case 3: Program.Settings.P1_Left = Keyboard.GetState().GetPressedKeys()[0]; break;
                            case 4: Program.Settings.P1_A = Keyboard.GetState().GetPressedKeys()[0]; break;
                            case 5: Program.Settings.P1_B = Keyboard.GetState().GetPressedKeys()[0]; break;
                            case 6: Program.Settings.P1_Start = Keyboard.GetState().GetPressedKeys()[0]; break;
                            case 7: Program.Settings.P1_Select = Keyboard.GetState().GetPressedKeys()[0]; break;

                            case 8: Program.Settings.P2_Up = Keyboard.GetState().GetPressedKeys()[0]; break;
                            case 9: Program.Settings.P2_Down = Keyboard.GetState().GetPressedKeys()[0]; break;
                            case 10: Program.Settings.P2_Right = Keyboard.GetState().GetPressedKeys()[0]; break;
                            case 11: Program.Settings.P2_Left = Keyboard.GetState().GetPressedKeys()[0]; break;
                            case 12: Program.Settings.P2_A = Keyboard.GetState().GetPressedKeys()[0]; break;
                            case 13: Program.Settings.P2_B = Keyboard.GetState().GetPressedKeys()[0]; break;
                            case 14: Program.Settings.P2_Start = Keyboard.GetState().GetPressedKeys()[0]; break;
                            case 15: Program.Settings.P2_Select = Keyboard.GetState().GetPressedKeys()[0]; break;
                        }
                        Pressed = true;
                        MODE = EditMode.None;
                    }
             
                }
                else
                {
                    if (countdown > 0)
                        countdown--;
                    else
                        Pressed = false;
                }
            }
            base.Update(gameTime);
        }

        public void Draw()
        {
            //Draw the background
            Program.CORE.DrawBackground();

            //Draw the frame
            Program.CORE.SpriteBatch.Draw(tFrame,
            new Rectangle(30, 189, Program.CORE.GraphicsDevice.Viewport.Width - 60,
            370), Color.White);
            //Draw the options
            int Y = 230;
            int X = 400;
            int add = 15;
            #region Player 1, UP
            if (SelectedIndex == 0)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <UP>",
                        new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_Up.ToString(),
                         new Vector2(X, Y), Color.Gold);

            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <UP>",
                 new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_Up.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Player 1, DOWN
            Y += add;
            if (SelectedIndex == 1)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <DOWN>",
                      new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_Down.ToString(),
                         new Vector2(X, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <DOWN>",
                  new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_Down.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Player 1, RIGHT
            Y += add;
            if (SelectedIndex == 2)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <RIGHT>",
                      new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_Right.ToString(),
                         new Vector2(X, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <RIGHT>",
                  new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_Right.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Player 1, LEFT
            Y += add;
            if (SelectedIndex == 3)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <LEFT>",
                      new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_Left.ToString(),
                         new Vector2(X, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <LEFT>",
                  new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_Left.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Player 1, A
            Y += add;
            if (SelectedIndex == 4)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <A>",
                      new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_A.ToString(),
                         new Vector2(X, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <A>",
                  new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_A.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Player 1, B
            Y += add;
            if (SelectedIndex == 5)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <B>",
                      new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_B.ToString(),
                         new Vector2(X, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <B>",
                  new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_B.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Player 1, START
            Y += add;
            if (SelectedIndex == 6)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <START>",
                      new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_Start.ToString(),
                         new Vector2(X, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <START>",
                  new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_Start.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Player 1, SELECT
            Y += add;
            if (SelectedIndex == 7)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <SELECT>",
                      new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_Select.ToString(),
                         new Vector2(X, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 1 <SELECT>",
                  new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P1_Select.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion

            #region Player 2, UP
            Y += add + 10;
            if (SelectedIndex == 8)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <UP>",
                        new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_Up.ToString(),
                         new Vector2(X, Y), Color.Gold);

            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <UP>",
                 new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_Up.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Player 2, DOWN
            Y += add;
            if (SelectedIndex == 9)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <DOWN>",
                      new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_Down.ToString(),
                         new Vector2(X, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <DOWN>",
                  new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_Down.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Player 2, RIGHT
            Y += add;
            if (SelectedIndex == 10)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <RIGHT>",
                      new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_Right.ToString(),
                         new Vector2(X, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <RIGHT>",
                  new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_Right.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Player 2, LEFT
            Y += add;
            if (SelectedIndex == 11)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <LEFT>",
                      new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_Left.ToString(),
                         new Vector2(X, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <LEFT>",
                  new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_Left.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Player 2, A
            Y += add;
            if (SelectedIndex == 12)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <A>",
                      new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_A.ToString(),
                         new Vector2(X, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <A>",
                  new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_A.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Player 2, B
            Y += add;
            if (SelectedIndex == 13)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <B>",
                      new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_B.ToString(),
                         new Vector2(X, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <B>",
                  new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_B.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Player 2, START
            Y += add;
            if (SelectedIndex == 14)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <START>",
                      new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_Start.ToString(),
                         new Vector2(X, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <START>",
                  new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_Start.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Player 2, SELECT
            Y += add;
            if (SelectedIndex == 15)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <SELECT>",
                      new Vector2(80, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_Select.ToString(),
                         new Vector2(X, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PLAYER 2 <SELECT>",
                  new Vector2(80, Y), Color.White);

                Program.CORE.SpriteBatch.DrawString(Font_normal, Program.Settings.P2_Select.ToString(),
                         new Vector2(X, Y), Color.White);
            }
            #endregion

            #region Reset to defaults
            Y += add + 10;
            if (SelectedIndex == 16)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "RESET TO DEFAULTS",
                      new Vector2(80, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "RESET TO DEFAULTS",
                  new Vector2(80, Y), Color.White);
            }
            #endregion
            #region Back
            Y += add;
            if (SelectedIndex == 17)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "BACK",
                      new Vector2(80, Y), Color.Gold);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "BACK",
                  new Vector2(80, Y), Color.White);
            }
            #endregion

            if (MODE == EditMode.EditingKey)
            {
                //Draw the frame
                Program.CORE.SpriteBatch.Draw(tFrame,
                new Rectangle(200, 220, 400, 70), Color.White);
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PRESS KEY ON YOUR KEYBOARD",
                    new Vector2(226, 240), Color.White);
                Program.CORE.SpriteBatch.DrawString(Font_small, "Press " + @"""" + "Escape" + @"""" + " to cancel.", new Vector2(10, Program.CORE.GraphicsDevice.Viewport.Height - 30), Color.White);
            }
            else
                Program.CORE.SpriteBatch.DrawString(Font_small, "Press " + @"""" + "Escape or Back" + @"""" + " to back to the settings page, " + @"""" + "Up/Down" + @"""" + " to select, " + @"""" + "Enter" + @"""" + " to edit selected.", new Vector2(10, Program.CORE.GraphicsDevice.Viewport.Height - 30), Color.White);
        }
    }
}

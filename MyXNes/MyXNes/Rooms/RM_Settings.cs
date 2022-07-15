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
using MyNes.Nes;

namespace MyXNes
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class RM_Settings : Microsoft.Xna.Framework.GameComponent
    {
        Texture2D tFrame;
        SpriteFont Font_large;
        SpriteFont Font_normal;
        SpriteFont Font_small;
       public  bool Pressed;
       public int countdown = 0;
        public int SelectedIndex = 1;
        //option holders
        bool startinfullscreen;
        TVFORMAT tv;
        MyNes.Nes.UseInternalPaletteMode palette;
        bool autoswitchtv;
        bool autosavesram;
        float Volume = 1.0f;
        bool SoundEnabled = true;

        public RM_Settings(Game game)
            : base(game)
        {
            //Load the textures
            tFrame = Program.CORE.Content.Load<Texture2D>("frame");
            Font_large = Program.CORE.Content.Load<SpriteFont>("Font_big");
            Font_normal = Program.CORE.Content.Load<SpriteFont>("Font_normal");
            Font_small = Program.CORE.Content.Load<SpriteFont>("Font_small");
            //load settings
            startinfullscreen = Program.Settings.StartInFullScreen;
            tv = Program.Settings.TV;
            palette = Program.Settings.PaletteFormat.UseInternalPaletteMode;
            autoswitchtv = Program.Settings.AutoSwitchEmulation;
            autosavesram = Program.Settings.AutoSaveSRAM;
            Volume = Program.Settings.VolumeLevel;
            SoundEnabled = Program.Settings.EnableSound;
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
                if (Keyboard.GetState().IsKeyDown(Keys.Escape) |
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    Program.CORE.ROOM = CurrentRoom.MainMenu;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down) |
                    GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                {
                    SelectedIndex++; ;
                    if (SelectedIndex > 9)
                        SelectedIndex = 0;
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up) |
                    GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                {
                    SelectedIndex--;
                    if (SelectedIndex < 0)
                        SelectedIndex = 9;
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right) |
                    GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)
                {
                    switch (SelectedIndex)
                    {
                        case 0://start in full screen
                            startinfullscreen = !startinfullscreen;
                            Pressed = true;
                            break;
                        case 1://emultion
                            if (tv == TVFORMAT.NTSC)
                                tv = TVFORMAT.PAL;
                            else
                                tv = TVFORMAT.NTSC;
                            Pressed = true;
                            break;
                        case 2://auto switch emulation
                            autoswitchtv = !autoswitchtv;
                            Pressed = true;
                            break;
                        case 3://palette
                            if (palette == UseInternalPaletteMode.Auto)
                                palette = UseInternalPaletteMode.NTSC;
                            else if (palette == UseInternalPaletteMode.NTSC)
                                palette = UseInternalPaletteMode.PAL;
                            else if (palette == UseInternalPaletteMode.PAL)
                                palette = UseInternalPaletteMode.Auto;
                            Pressed = true;
                            break;
                        case 4://save sram
                            autosavesram = !autosavesram;
                            Pressed = true;
                            break;
                        case 5://soundenabled
                            SoundEnabled = !SoundEnabled;
                            Pressed = true;
                            break;
                        case 6://volume level
                            Volume += 0.01f;
                            if (Volume >= 1.0f)
                                Volume = 1.0f;
                            Pressed = true;
                            break;
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left) |
                    GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)
                {
                    switch (SelectedIndex)
                    {
                        case 0://start in fullscreen
                            startinfullscreen = !startinfullscreen;
                            Pressed = true;
                            break;
                        case 1://emultion
                            if (tv == TVFORMAT.NTSC)
                                tv = TVFORMAT.PAL;
                            else
                                tv = TVFORMAT.NTSC;
                            Pressed = true;
                            break;
                        case 2://show fps
                            autoswitchtv = !autoswitchtv;
                            Pressed = true;
                            break;
                        case 3://palette
                            if (palette == UseInternalPaletteMode.Auto)
                                palette = UseInternalPaletteMode.PAL;
                            else if (palette == UseInternalPaletteMode.NTSC)
                                palette = UseInternalPaletteMode.Auto;
                            else if (palette == UseInternalPaletteMode.PAL)
                                palette = UseInternalPaletteMode.NTSC;
                            Pressed = true;
                            break;
                        case 4://save sram
                            autosavesram = !autosavesram;
                            Pressed = true;
                            break;
                        case 5://soundenabled
                            SoundEnabled = !SoundEnabled;
                            Pressed = true;
                            break;
                        case 6://volume level
                            Volume -= 0.01f;
                            if (Volume <= 0.0f)
                                Volume = 0.0f;
                            Pressed = true;
                            break;
                    }
                }

                //ACTION
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) |
                    GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                {
                    switch (SelectedIndex)
                    {
                        case 7://Keys configuration
                            Program.CORE.rKEYSCONFIGURATION.countdown = 20;
                            Program.CORE.rKEYSCONFIGURATION.Pressed = true;
                            Program.CORE.ROOM = CurrentRoom.KeysConfiguration;
                            break;
                        case 8://save
                            //Applay the settings
                            Program.Settings.StartInFullScreen = startinfullscreen;
                            Program.Settings.TV = tv;
                            Program.Settings.PaletteFormat.UseInternalPaletteMode = palette;
                            Program.Settings.AutoSwitchEmulation = autoswitchtv;
                            Program.Settings.AutoSaveSRAM = autosavesram;
                            Program.Settings.VolumeLevel = Volume;
                            Program.Settings.EnableSound = SoundEnabled;
                            //save
                            if (Program.SaveSerialize("Settings.mxn", "Settings", Program.Settings))
                            {
                                Program.CORE.WriteText("SETTINGS SAVED", 120, TextStatus.NONE);
                            }
                            //return to the main menu
                            Program.CORE.rMAINMENU.OptionIndex = -1;
                            Program.CORE.ROOM = CurrentRoom.MainMenu;
                            break;
                        case 9://reset
                            startinfullscreen = true;
                            tv = TVFORMAT.NTSC;
                            palette = UseInternalPaletteMode.Auto;
                            autoswitchtv = true;
                            autosavesram = true;
                            Volume = 0.95f;
                            SoundEnabled = true;
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

            #region FULL SCREEN
            if (SelectedIndex == 0)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "FULL SCREEN (WINDOWS ONLY)",
                        new Vector2(80, Y), Color.Gold);
                if (startinfullscreen)
                {
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "YES",
                         new Vector2(X, Y), Color.Gold);
                }
                else
                {
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "NO",
                      new Vector2(X, Y), Color.Gold);
                }
                Program.CORE.SpriteBatch.DrawString(Font_small,
                   "Specified if My X Nes should run in fullscreen at the next time you start My X Nes (Windows version only).",
                   new Vector2(80, Program.CORE.GraphicsDevice.Viewport.Height - 90), Color.White);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "FULL SCREEN (WINDOWS ONLY)",
                      new Vector2(80, Y), Color.White);
                if (startinfullscreen)
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "YES",
                        new Vector2(X, Y), Color.White);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "NO",
                      new Vector2(X, Y), Color.White);
            }
            #endregion
            #region EMULATION
            Y += 20;
            if (SelectedIndex == 1)
            {
                if (!autoswitchtv)
                {
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "EMULATION",
                           new Vector2(80, Y), Color.Gold);
                    Program.CORE.SpriteBatch.DrawString(Font_normal, tv.ToString(),
                    new Vector2(X, Y), Color.Gold);
                }
                else
                {
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "EMULATION",
                               new Vector2(80, Y), Color.LightGray);
                    Program.CORE.SpriteBatch.DrawString(Font_normal, tv.ToString(),
                    new Vector2(X, Y), Color.LightGray);
                }
                Program.CORE.SpriteBatch.DrawString(Font_small,
   "The NES emulation. NTSC : 60 FPS (frame per second), PAL : 50 FPS.",
   new Vector2(80, Program.CORE.GraphicsDevice.Viewport.Height - 90), Color.White);
            }
            else
            {
                if (!autoswitchtv)
                {
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "EMULATION",
                       new Vector2(80, Y), Color.White);
                    Program.CORE.SpriteBatch.DrawString(Font_normal, tv.ToString(),
                    new Vector2(X, Y), Color.White);
                }
                else
                {
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "EMULATION",
                         new Vector2(80, Y), Color.Gray);
                    Program.CORE.SpriteBatch.DrawString(Font_normal, tv.ToString(),
                    new Vector2(X, Y), Color.Gray);
                }
            }
            #endregion
            #region AUTO SWITCH EMULATION
            Y += 20;
            if (SelectedIndex == 2)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "AUTO SWITCH EMULATION",
                      new Vector2(80, Y), Color.Gold);
                if (autoswitchtv)
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "YES",
                        new Vector2(X, Y), Color.Gold);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "NO",
                      new Vector2(X, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_small,
"Specified if My X Nes should switch emulation automaticaly depending on rom region.",
new Vector2(80, Program.CORE.GraphicsDevice.Viewport.Height - 90), Color.White);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "AUTO SWITCH EMULATION",
                    new Vector2(80, Y), Color.White);
                if (autoswitchtv)
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "YES",
                        new Vector2(X, Y), Color.White);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "NO",
                      new Vector2(X, Y), Color.White);
            }
            #endregion
            #region PALETTE
            Y += 20;
            if (SelectedIndex == 3)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PALETTE",
                      new Vector2(80, Y), Color.Gold);
                Program.CORE.SpriteBatch.DrawString(Font_normal, palette.ToString(),
                new Vector2(X, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_small,
"Chose the palette you want My X Nes to use at video rendering.",
new Vector2(80, Program.CORE.GraphicsDevice.Viewport.Height - 90), Color.White);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "PALETTE",
                     new Vector2(80, Y), Color.White);
                Program.CORE.SpriteBatch.DrawString(Font_normal, palette.ToString(),
          new Vector2(X, Y), Color.White);
            }
            #endregion
            #region AUTO SAVE S-RAM
            Y += 20;
            if (SelectedIndex == 4)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "AUTO SAVE S-RAM",
                      new Vector2(80, Y), Color.Gold);
                if (autosavesram)
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "YES",
                        new Vector2(X, Y), Color.Gold);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "NO",
                      new Vector2(X, Y), Color.Gold);

                Program.CORE.SpriteBatch.DrawString(Font_small,
"Specified if you want My X Nes to save the S-RAM (save ram) automaticaly.",
new Vector2(80, Program.CORE.GraphicsDevice.Viewport.Height - 90), Color.White);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "AUTO SAVE S-RAM",
                    new Vector2(80, Y), Color.White);
                if (autosavesram)
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "YES",
                        new Vector2(X, Y), Color.White);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "NO",
                      new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Sound
            Y += 20;
            if (SelectedIndex == 5)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "SOUND",
                      new Vector2(80, Y), Color.Gold);
                if (SoundEnabled)
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "ON",
                        new Vector2(X, Y), Color.Gold);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "OFF",
                      new Vector2(X, Y), Color.Gold);
                Program.CORE.SpriteBatch.DrawString(Font_small,
"Specified if you want to enable the sound or not.",
new Vector2(80, Program.CORE.GraphicsDevice.Viewport.Height - 90), Color.White);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "SOUND",
                    new Vector2(80, Y), Color.White);
                if (SoundEnabled)
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "ON",
                        new Vector2(X, Y), Color.White);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "OFF",
                      new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Volume
            Y += 20;
            //Volume
            if (SelectedIndex == 6)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "SOUND LEVEL (VOLUME)",
                      new Vector2(80, Y), Color.Gold);
                Program.CORE.SpriteBatch.DrawString(Font_normal, (Volume * 100).ToString("F0") + " %",
                      new Vector2(X, Y), Color.Gold);
                Program.CORE.SpriteBatch.DrawString(Font_small,
"Specified the sound volume level.",
new Vector2(80, Program.CORE.GraphicsDevice.Viewport.Height - 90), Color.White);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "SOUND LEVEL (VOLUME)",
                    new Vector2(80, Y), Color.White);
                Program.CORE.SpriteBatch.DrawString(Font_normal, (Volume * 100).ToString("F0") + " %",
                     new Vector2(X, Y), Color.White);
            }
            #endregion
            #region Keys configuration
            Y += 20;
            //Volume
            if (SelectedIndex == 7)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "KEYS CONFIGURATION",
                      new Vector2(80, Y), Color.Gold);
                Program.CORE.SpriteBatch.DrawString(Font_small,
               "Config the Nes keys.",
                new Vector2(80, Program.CORE.GraphicsDevice.Viewport.Height - 90), Color.White);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "KEYS CONFIGURATION",
                    new Vector2(80, Y), Color.White);
            }
            #endregion
            #region SAVE
            Y += 70;
            if (SelectedIndex == 8)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "SAVE SETTINGS",
                       new Vector2(80, Y), Color.Gold);
                Program.CORE.SpriteBatch.DrawString(Font_small,
"Save the settings and applay them.",
new Vector2(80, Program.CORE.GraphicsDevice.Viewport.Height - 90), Color.White);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "SAVE SETTINGS",
                    new Vector2(80, Y), Color.White);
            }
            #endregion
            #region Reset
            Y += 20;
            if (SelectedIndex == 9)
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "RESET TO DEFAULTS",
                        new Vector2(80, Y), Color.Gold);
                Program.CORE.SpriteBatch.DrawString(Font_small,
"Reset the settings to the default values, you must save the settings to applay.",
new Vector2(80, Program.CORE.GraphicsDevice.Viewport.Height - 90), Color.White);
            }
            else
            {
                Program.CORE.SpriteBatch.DrawString(Font_normal, "RESET TO DEFAULTS",
                    new Vector2(80, Y), Color.White);
            }
            #endregion
#if WINDOWS
            Program.CORE.SpriteBatch.DrawString(Font_small, "Press " + @"""" + "Escape" + @"""" + " to back to main menu, " + @"""" + "Up/Down" + @"""" + " to select an option, " + @"""" + "Left/Right" + @"""" + " to edit selected option.", new Vector2(10, Program.CORE.GraphicsDevice.Viewport.Height - 30), Color.White);
#else
            Program.CORE.SpriteBatch.DrawString(Font_small, "Press " + @"""" + "Back" + @"""" + " to back to main menu without saving, " + @"""" + "Up/Down" + @"""" + " to select an option, " + @"""" + "Left/Right" + @"""" + " to edit selected option.", new Vector2(10, Program.CORE.GraphicsDevice.Viewport.Height - 30), Color.White);
#endif
        }
    }
}

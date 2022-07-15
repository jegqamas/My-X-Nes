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
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MyNes.Nes;
using MyNes.Nes.Output.Video;

namespace MyXNes
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class RM_GamePlay : Microsoft.Xna.Framework.GameComponent
    {
        public Thread mainThread;
        public NES Nes;
        Vid_XNA DR;
        Texture2D SCREEN;
        int SelectedIndex = 0;
        bool Pressed = false;
        int countdown = 0;
        int StateSlot = 0;
        //int prevTime = 0;
        //string fpsText = "0 FPS";
        MemoryStream QuikStateStream;

        Texture2D tFrame;
        Texture2D tTitle;
        SpriteFont Font_normal;
        SpriteFont Font_large;
        SpriteFont Font_small;

        Joypad Joypad1 = new Joypad();
        Joypad Joypad2 = new Joypad();

        Rectangle screenrt;
        int W = 0;
        int H = 0;
        public RM_GamePlay(Game game)
            : base(game)
        {
            Font_normal = Program.CORE.Content.Load<SpriteFont>("Font_normal");
            tFrame = Program.CORE.Content.Load<Texture2D>("frame");
            Font_large = Program.CORE.Content.Load<SpriteFont>("Font_big");
            Font_small = Program.CORE.Content.Load<SpriteFont>("Font_small");
            tTitle = Program.CORE.Content.Load<Texture2D>("Title");
            H = Program.CORE.GraphicsDevice.Viewport.Height;
            W = Program.CORE.GraphicsDevice.Viewport.Width;
            screenrt = new Rectangle(0, 0, W, H);
            CONSOLE.DebugRised += new EventHandler<DebugArg>(CONSOLE_DebugRised);
        }

        void CONSOLE_DebugRised(object sender, DebugArg e)
        {
            if (e.Status == DebugStatus.Notification)
                if (e.DebugLine.Contains("State") & !e.DebugLine.Contains("Quick"))
                    Program.CORE.WriteText(e.DebugLine + " [Slot " + StateSlot + "]", 100, TextStatus.NONE);
                else
                    Program.CORE.WriteText(e.DebugLine, 100, TextStatus.NONE);
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
            if (Nes != null)
            {
               // if (DateTime.Now.Second - prevTime >= 1)
               // {
              //      fpsText = Nes.FPS + " FPS";
               //     Nes.FPS = 0;
               // }
                //prevTime = DateTime.Now.Second;
                // Controls
                UpdatePlayer1();
                UpdatePlayer2();

                if (Keyboard.GetState().IsKeyDown(Keys.Escape) |
                    GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    Nes.PAUSE = true;
                }
                //options
                if (Nes.PAUSE)
                {
                    if (!Pressed)
                    {
                        countdown = 5;
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
                        //State slot
                        if ((Keyboard.GetState().IsKeyDown(Keys.Right) |
                            GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed) & SelectedIndex == 5)
                        {
                            StateSlot++;
                            if (StateSlot > 9)
                                StateSlot = 0;
                            Pressed = true;
                        }
                        if ((Keyboard.GetState().IsKeyDown(Keys.Left) |
                            GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed) & SelectedIndex == 5)
                        {
                            StateSlot--;
                            if (StateSlot < 0)
                                StateSlot = 9;
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
                        switch (SelectedIndex)
                        {
                            case 0://Resume
                                Nes.PAUSE = false;
                                break;
                            case 1://Quick Save state
                                 QuickSaveState();
                                break;
                             case 2://Quick Load state
                                 QuickLoadState();
                                 break;
                            case 3://Save state
                                SaveState();
                                break;
                            case 4://Load state
                                LoadState();
                                break;
                            case 5://State Slot
                                StateSlot++;
                                if (StateSlot > 9)
                                    StateSlot = 9;
                                break;
                            case 6://RESET (SOFT)
                                Nes.CPU.SoftReset();
                                Nes.PAUSE = false;
                                //Nes.APU.SoundEngine.Resume();
                                break;
                            case 7://RESET (HARD)
                                LoadCart(Nes.Cartridge.RomPath);
                                break;
                            case 8://back to browser
                                NesOff();
                                Program.CORE.ROOM = CurrentRoom.Browser;
                                break;
                            case 9://back to maim menu
                                NesOff();
                                Program.CORE.rMAINMENU.OptionIndex = -1;
                                Program.CORE.ROOM = CurrentRoom.MainMenu;
                                break;
                        }
                    }
                }
            }

            base.Update(gameTime);
        }
        public void Draw()
        {
            SCREEN.SetData<int>(DR.BUFFER, 0, DR.BUFFER.Length);
            Program.CORE.SpriteBatch.Draw(SCREEN, screenrt, Color.White);
          
            //We MUST draw something otherwise an exception will be thrown here !!
            //Program.CORE.SpriteBatch.DrawString(Font_large, fpsText,
            //new Vector2(30, 30), Color.White);
            Program.CORE.SpriteBatch.DrawString(Font_large, " ",
            new Vector2(30, 30), Color.White);
            if (Nes.PAUSE)
            {
                //Draw the menu!!
                //Draw the title
                Program.CORE.SpriteBatch.Draw(tTitle,
                new Vector2(220, 15), Color.White);
                //Draw the frame
                Program.CORE.SpriteBatch.Draw(tFrame,
                new Rectangle(200, 120, 400, 440), Color.White);
                Program.CORE.SpriteBatch.DrawString(Font_large, "IN GAME OPTIONS",
                          new Vector2(250, 150), Color.Blue);
                //Draw the buttons (or options)
                int y = 200;
                int add = 25;
                if (SelectedIndex == 0)
                    Program.CORE.SpriteBatch.DrawString(Font_large, "RESUME",
                          new Vector2(228, y), Color.Gold);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_large, "RESUME",
                        new Vector2(228, y), Color.White);
                y += add + 10;
                if (SelectedIndex == 1)
                    Program.CORE.SpriteBatch.DrawString(Font_large, "QUICK SAVE STATE",
                          new Vector2(228, y), Color.Gold);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_large, "QUICK SAVE STATE",
                        new Vector2(228, y), Color.White);
                y += add;
                if (SelectedIndex == 2)
                    Program.CORE.SpriteBatch.DrawString(Font_large, "QUICK LOAD STATE",
                          new Vector2(228, y), Color.Gold);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_large, "QUICK LOAD STATE",
                        new Vector2(228, y), Color.White);
                y += add + 10;
                if (SelectedIndex == 3)
                    Program.CORE.SpriteBatch.DrawString(Font_large, "SAVE STATE",
                          new Vector2(228, y), Color.Gold);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_large, "SAVE STATE",
                        new Vector2(228, y), Color.White);
                y += add;
                if (SelectedIndex == 4)
                    Program.CORE.SpriteBatch.DrawString(Font_large, "LOAD STATE",
                          new Vector2(228, y), Color.Gold);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_large, "LOAD STATE",
                        new Vector2(228, y), Color.White);
                y += add;
                if (SelectedIndex == 5)
                    Program.CORE.SpriteBatch.DrawString(Font_large, "STATE SLOT : ",
                          new Vector2(228, y), Color.Gold);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_large, "STATE SLOT : ",
                        new Vector2(228, y), Color.White);
                //Draw slot number
                if (SelectedIndex == 5)
                    Program.CORE.SpriteBatch.DrawString(Font_large, StateSlot.ToString(),
                          new Vector2(450, y), Color.Gold);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_large, StateSlot.ToString(),
                        new Vector2(450, y), Color.White);
                y += add + 10;
                if (SelectedIndex == 6)
                    Program.CORE.SpriteBatch.DrawString(Font_large, "RESET (SOFT)",
                          new Vector2(228, y), Color.Gold);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_large, "RESET (SOFT)",
                        new Vector2(228, y), Color.White);
                y += add;
                if (SelectedIndex == 7)
                    Program.CORE.SpriteBatch.DrawString(Font_large, "RESET (HARD)",
                          new Vector2(228, y), Color.Gold);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_large, "RESET (HARD)",
                        new Vector2(228, y), Color.White);
                y += add + 10;
                if (SelectedIndex == 8)
                    Program.CORE.SpriteBatch.DrawString(Font_large, "BACK TO BROWSER",
                          new Vector2(228, y), Color.Gold);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_large, "BACK TO BROWSER",
                        new Vector2(228, y), Color.White);
                y += add;
                if (SelectedIndex == 9)
                    Program.CORE.SpriteBatch.DrawString(Font_large, "BACK TO MAINMENU",
                          new Vector2(228, y), Color.Gold);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_large, "BACK TO MAINMENU",
                        new Vector2(228, y), Color.White);
                //Draw help
#if WINDOWS
                Program.CORE.SpriteBatch.DrawString(Font_small,
               @"""" + "Up/Down" + @"""" + "= select option, " + @"""" + "Enter" + @"""" + "= OK.",
               new Vector2(228, 513), Color.White);
#else
                Program.CORE.SpriteBatch.DrawString(Font_small,
                 @"""" + "Up/Down" + @"""" + "= select option, " + @"""" + "A" + @"""" + "= OK.",
                 new Vector2(228, 513), Color.White);
#endif
            }
        }

        public void LoadCart(string RomPath)
        {
            string Log = Path.GetFileNameWithoutExtension(RomPath);
            //Exit current thread
            if (mainThread != null)
            {
                mainThread.Abort();
            }
            if (Nes != null)
            {
                Nes.ShutDown();
                Nes = null;
            }
            //Start
            Nes = new NES(new Timer());
            Nes.LoadRom(RomPath, new FileStream(RomPath, FileMode.Open, FileAccess.Read),
              Program.OpenFile(Path.GetFileNameWithoutExtension(RomPath) + ".sav", "SRamSaves", FileMode.Open));
            //Setup
            Nes.AutoSaveSRAM = Program.Settings.AutoSaveSRAM;
            Nes.Memory.InputManager = new InputManager();
            Nes.Memory.Joypad1 = Joypad1;
            Nes.Memory.Joypad2 = Joypad2;

            if (Program.Settings.AutoSwitchEmulation)
            {
                if (Nes.Cartridge.IsPAL)
                {
                    Program.Settings.TV = TVFORMAT.PAL;
                }
                else
                {
                    Program.Settings.TV = TVFORMAT.NTSC;
                }
            }
            Log += ", " + Program.Settings.TV.ToString();
            Nes.PPU.SetPallete(Program.Settings.TV, Program.Settings.PaletteFormat);
            DR = new Vid_XNA(Program.Settings.TV);
            if (Program.Settings.TV== TVFORMAT.NTSC)
                SCREEN = new Texture2D(Program.CORE.GraphicsDevice, 256, 224, false, SurfaceFormat.Color);
            else
                SCREEN = new Texture2D(Program.CORE.GraphicsDevice, 256, 240, false, SurfaceFormat.Color);
            Nes.PPU.VIDEO = DR;
            //Sound setup
            Nes.SoundEnabled = Program.Settings.EnableSound;
            Nes.APU.Output = new Sound_XNA(Nes.APU);
            Nes.APU.Output.Initialize();
            ((Sound_XNA)Nes.APU.Output).SetVolume(Program.Settings.VolumeLevel);
            //launch
            Nes.TurnOn();
            //Write log
            Program.CORE.WriteText(Log, 120, TextStatus.NESLOG);
            mainThread = new Thread(new ThreadStart(Nes.RUN));
            mainThread.Start();
        }

        public void UpdatePlayer1()
        {
            Joypad1.Up = (Keyboard.GetState().IsKeyDown(Program.Settings.P1_Up)) | (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed);
            Joypad1.Down = (Keyboard.GetState().IsKeyDown(Program.Settings.P1_Down)) | (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed);
            Joypad1.Left = (Keyboard.GetState().IsKeyDown(Program.Settings.P1_Left)) | (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed);
            Joypad1.Right = (Keyboard.GetState().IsKeyDown(Program.Settings.P1_Right)) | (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed);
            Joypad1.A = (Keyboard.GetState().IsKeyDown(Program.Settings.P1_A)) | (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed);
            Joypad1.B = (Keyboard.GetState().IsKeyDown(Program.Settings.P1_B)) | (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed);
            Joypad1.Start = (Keyboard.GetState().IsKeyDown(Program.Settings.P1_Start)) | (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed);
            Joypad1.Select = (Keyboard.GetState().IsKeyDown(Program.Settings.P1_Select)) | (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed);
        }
        public void UpdatePlayer2()
        {
            Joypad2.Up = (Keyboard.GetState().IsKeyDown(Program.Settings.P2_Up)) | (GamePad.GetState(PlayerIndex.Two).DPad.Up == ButtonState.Pressed);
            Joypad2.Down = (Keyboard.GetState().IsKeyDown(Program.Settings.P2_Down)) | (GamePad.GetState(PlayerIndex.Two).DPad.Down == ButtonState.Pressed);
            Joypad2.Left = (Keyboard.GetState().IsKeyDown(Program.Settings.P2_Left)) | (GamePad.GetState(PlayerIndex.Two).DPad.Left == ButtonState.Pressed);
            Joypad2.Right = (Keyboard.GetState().IsKeyDown(Program.Settings.P2_Right)) | (GamePad.GetState(PlayerIndex.Two).DPad.Right == ButtonState.Pressed);
            Joypad2.A = (Keyboard.GetState().IsKeyDown(Program.Settings.P2_A)) | (GamePad.GetState(PlayerIndex.Two).Buttons.A == ButtonState.Pressed);
            Joypad2.B = (Keyboard.GetState().IsKeyDown(Program.Settings.P2_B)) | (GamePad.GetState(PlayerIndex.Two).Buttons.B == ButtonState.Pressed);
            Joypad2.Start = (Keyboard.GetState().IsKeyDown(Program.Settings.P2_Start)) | (GamePad.GetState(PlayerIndex.Two).Buttons.Start == ButtonState.Pressed);
            Joypad2.Select = (Keyboard.GetState().IsKeyDown(Program.Settings.P2_Select)) | (GamePad.GetState(PlayerIndex.Two).Buttons.LeftShoulder == ButtonState.Pressed);
        }
        void LoadState()
        {
            Stream str = Program.OpenFile(Path.GetFileNameWithoutExtension(Nes.Cartridge.RomPath) + "_" + StateSlot + ".mns",
            "StateSaves", FileMode.Open);
            Nes.LoadStateRequest(Path.GetFileNameWithoutExtension(Nes.Cartridge.RomPath) + "_" + StateSlot + ".mns", str, false);
        }
        void SaveState()
        {
            Stream str=Program.OpenFile(Path.GetFileNameWithoutExtension(Nes.Cartridge.RomPath) + "_" + StateSlot + ".mns", 
                "StateSaves", FileMode.Create);
            Nes.SaveStateRequest(Path.GetFileNameWithoutExtension(Nes.Cartridge.RomPath) + "_" + StateSlot + ".mns", str, false);
        }
        void QuickSaveState()
        {
            Nes.SaveStateRequest(Path.GetFileNameWithoutExtension(Nes.Cartridge.RomPath) + "_" + StateSlot + ".mns",
               QuikStateStream = new MemoryStream(), true);
        }
        void QuickLoadState()
        {
            if (QuikStateStream != null)
            {
                QuikStateStream.Position = 0;
                Nes.LoadStateRequest(Path.GetFileNameWithoutExtension(Nes.Cartridge.RomPath) + "_" + StateSlot + ".mns",
                       QuikStateStream, true);
            }
        }
        public void NesOff()
        {
            if (Nes != null)
            {
                Nes.ShutDown();
                Game.TargetElapsedTime = TimeSpan.FromMilliseconds(16.67);
                Nes = null;
            }
        }
    }
}

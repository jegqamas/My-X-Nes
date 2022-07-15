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
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using MyNes.Nes;

namespace MyXNes
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class RM_Browser : Microsoft.Xna.Framework.GameComponent
    {
        SpriteFont Font_normal;
        SpriteFont Font_small;
        Texture2D tFrame;
        Texture2D tBannerBack;

        public int ScrollIndex = 0;
        public int SelectedIndex = 0;
        bool Pressed = false;
        int countdown = 0;
        int PicViewDelay = 10;
        public MXN_ROMS_COLLECTION RomsHolder;
        public int PrecentCompleteLoading = 0;
        Texture2D Snapshot;
        public RM_Browser(Game game)
            : base(game)
        {
            tFrame = Program.CORE.Content.Load<Texture2D>("frame");
            Font_normal = Program.CORE.Content.Load<SpriteFont>("Font_normal");
            Font_small = Program.CORE.Content.Load<SpriteFont>("Font_small");
            tBannerBack = Program.CORE.Content.Load<Texture2D>("bannerback");
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
            if (PicViewDelay > 0)
                PicViewDelay--;

            if (!Pressed)//UP,DOWN
            {
                countdown = 5;
                if (Keyboard.GetState().IsKeyDown(Keys.Down) |
                    GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                {
                    if (SelectedIndex < RomsHolder.ROMS.Count)
                        SelectedIndex++;
                    if ((SelectedIndex - ScrollIndex) > 24)
                        ScrollIndex++;
                    if (RomsHolder.ROMS.Count > 24)
                        if (ScrollIndex > (RomsHolder.ROMS.Count - 1) - 24)
                            ScrollIndex = (RomsHolder.ROMS.Count - 1) - 24;
                    if (SelectedIndex > (RomsHolder.ROMS.Count - 1))
                        SelectedIndex = (RomsHolder.ROMS.Count - 1);
                    Pressed = true; PicViewDelay = 10; Snapshot = null;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up) |
                    GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                {
                    if (SelectedIndex > 0)
                        SelectedIndex--;
                    if ((SelectedIndex - ScrollIndex) < 0)
                        ScrollIndex--;
                    if (ScrollIndex < 0)
                        ScrollIndex = 0;
                    if (SelectedIndex < 0)
                        SelectedIndex = 0;
                    Pressed = true; PicViewDelay = 10; Snapshot = null;
                }

            }
            else
            {
                if (countdown > 0)
                    countdown--;
                else
                    Pressed = false;
            }
            //FAST MOVE
            if (Keyboard.GetState().IsKeyDown(Keys.Right) |
                GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)
            {
                PicViewDelay = 10;
                if (SelectedIndex < RomsHolder.ROMS.Count)
                    SelectedIndex++;
                if ((SelectedIndex - ScrollIndex) > 24)
                    ScrollIndex++;
                if (RomsHolder.ROMS.Count > 24)
                    if (ScrollIndex > (RomsHolder.ROMS.Count - 1) - 24)
                        ScrollIndex = (RomsHolder.ROMS.Count - 1) - 24;
                if (SelectedIndex > (RomsHolder.ROMS.Count - 1))
                    SelectedIndex = (RomsHolder.ROMS.Count - 1);
                Pressed = true; Snapshot = null;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) |
                GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)
            {
                PicViewDelay = 10;
                if (SelectedIndex > 0)
                    SelectedIndex--;
                if ((SelectedIndex - ScrollIndex) < 0)
                    ScrollIndex--;
                if (ScrollIndex < 0)
                    ScrollIndex = 0;
                if (SelectedIndex < 0)
                    SelectedIndex = 0;
                Pressed = true; Snapshot = null;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.PageUp) |
                GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed)
            {
                PicViewDelay = 10;
                ScrollIndex = 0;
                SelectedIndex = 0;
                Pressed = true; Snapshot = null;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.PageDown) |
                GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
            {
                PicViewDelay = 10;
                if (RomsHolder.ROMS.Count > 24)
                    ScrollIndex = (RomsHolder.ROMS.Count - 1) - 24;
                SelectedIndex = (RomsHolder.ROMS.Count - 1);
                Pressed = true; Snapshot = null;
            }
            //ACTION !!
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) |
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Program.CORE.ROOM = CurrentRoom.MainMenu;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space) |
                GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
            {
                Program.CORE.rGAMEPLAY.LoadCart(RomsHolder.ROMS[SelectedIndex].Path);
                Program.CORE.ROOM = CurrentRoom.GamePlay;
            }

            base.Update(gameTime);
        }

        public void Draw()
        {
            Program.CORE.DrawBackground();
            //Draw the frame of the game list
            Program.CORE.SpriteBatch.Draw(tFrame,
            new Rectangle(30, 189, Program.CORE.GraphicsDevice.Viewport.Width - 280,
            375), Color.White);
            //Draw the slider
            Program.CORE.SpriteBatch.Draw(tBannerBack,
            new Rectangle(490, 189, 30, 375), Color.White);

            //Draw the hand of the slider if we should
            if (RomsHolder.ROMS.Count > 16)
            {
                //calculations
                int height = (320 * 25) / RomsHolder.ROMS.Count;
                if (height < 30)
                    height = 30;
                int y = ((320 - height) * (ScrollIndex + 24)) / RomsHolder.ROMS.Count;
                y += 212;
                Program.CORE.SpriteBatch.Draw(tBannerBack,
                new Rectangle(492, y, 26, height), Color.White);
            }
            //Draw help
#if WINDOWS
            Program.CORE.SpriteBatch.DrawString(Font_small, @"""" + "Escape" + @"""" + "= back main menu, " + @"""" + "Up/Down/Left/Right/PageUp/PageDown" + @"""" + "= select game, " + @"""" + "Space" + @"""" + "= play selected.", new Vector2(10, Program.CORE.GraphicsDevice.Viewport.Height - 30), Color.White);
#else
            Program.CORE.SpriteBatch.DrawString(Font_small, @"""" + "Back" + @"""" + "= back main menu, " + @"""" + "Up/Down/Left/Right/LeftShoulder/RightShoulder" + @"""" + "= select game, " + @"""" + "Start" + @"""" + "= play selected.", new Vector2(10, Program.CORE.GraphicsDevice.Viewport.Height - 30), Color.White);
#endif
            //Draw the items   
            int Y = 212;
            if (RomsHolder.ROMS.Count > 0)
            {
                for (int i = ScrollIndex; i < ScrollIndex + 25; i++)
                {
                    if (i < RomsHolder.ROMS.Count)
                    {
                        if (SelectedIndex == i)
                        {
                            Program.CORE.SpriteBatch.DrawString(Font_small, (i + 1).ToString() + ". " + RomsHolder.ROMS[i].Name, new Vector2(80, Y), Color.Gold);
                        }
                        else
                        {
                            Program.CORE.SpriteBatch.DrawString(Font_small, (i + 1).ToString() + ". " + RomsHolder.ROMS[i].Name, new Vector2(80, Y), Color.White);
                        }
                        Y += 13;
                    }
                }
            }

            //Draw the frame of the snapshot
            int ix = 550;
            int iy = 189;
            Program.CORE.SpriteBatch.Draw(tFrame,
            new Rectangle(ix, iy, 240, 240), Color.White);
            //Draw snapshot !!
            if (PicViewDelay == 0)
            {
                if (Snapshot == null)
                {
                    if (File.Exists(RomsHolder.ROMS[SelectedIndex].SnapShotPath))
                    {
                        Snapshot = Texture2D.FromStream(Program.CORE.GraphicsDevice,
                               new FileStream(RomsHolder.ROMS[SelectedIndex].SnapShotPath, FileMode.Open, FileAccess.Read));
                    }
                }
                else
                {
                    Program.CORE.SpriteBatch.Draw(Snapshot,
                   new Rectangle(ix + 13, iy + 13, 213, 213), Color.White);
                }
                //Draw info
                Program.CORE.SpriteBatch.DrawString(Font_normal,
                    "MAPPER # " + RomsHolder.ROMS[SelectedIndex].Mapper.ToString(),
                    new Vector2(ix + 13, 440), Color.White);
                Program.CORE.SpriteBatch.DrawString(Font_normal,
                    "IS S-RAM: " + (RomsHolder.ROMS[SelectedIndex].IsSRAM ? "YES" : "NO"),
                    new Vector2(ix + 13, 470), Color.White);
                Program.CORE.SpriteBatch.DrawString(Font_normal,
                   "SYSTEM: " + (RomsHolder.ROMS[SelectedIndex].System),
                   new Vector2(ix + 13, 500), Color.White);
            }

        }

        public void LoadRoms()
        {
            if (File.Exists(".\\CACHE.XML"))
            {
                Stream stream = new FileStream(".\\CACHE.XML", FileMode.Open, FileAccess.Read);
                PrecentCompleteLoading = 100;
                if (LoadRomsAdvantage != null)
                    LoadRomsAdvantage(this, null);
                XmlSerializer ser = new XmlSerializer(typeof(MXN_ROMS_COLLECTION));
                RomsHolder = (MXN_ROMS_COLLECTION)ser.Deserialize(stream);
                stream.Close();
                Program.CORE.WriteText("ROMS LOADED FROM CACHE !!", 60, TextStatus.NONE);
                if (SelectedIndex >= RomsHolder.ROMS.Count)
                {
                    ScrollIndex = 0;
                    SelectedIndex = 0;
                }

                if (LoadRomsFinished != null)
                    LoadRomsFinished(this, null);
            }
            else
            {
                LoadRomsFromFolders();
            }
        }
        void LoadRomsFromFolders()
        {
            //Load the roms we can find in the dir
            int i = 0;
            if (Directory.Exists(".\\ROMS"))
            {
                string[] files = Directory.GetFiles(".\\ROMS");
                RomsHolder = new MXN_ROMS_COLLECTION();
                string[] imgexts = new string[] { ".jpg", ".bmp", ".png", ".gif" };
                string[] pictures = new string[0];
                if (Directory.Exists(".\\PICTURES"))
                    pictures = Directory.GetFiles(".\\PICTURES");
                foreach (string file in files)
                {
                    if (Path.GetExtension(file).ToLower() == ".nes")
                    {
                        Cartridge cart = new Cartridge(null);

                        if (cart.Load(file, new FileStream(file,FileMode.Open, FileAccess.Read),null, true) == LoadRomStatus.LoadSuccessed)
                        {
                            MXN_ROM ro = new MXN_ROM();
                            ro.Name = Path.GetFileNameWithoutExtension(file);
                            ro.Path = file;
                            //search for the snap shot
                            foreach (string pic in pictures)
                            {
                                if (Path.GetFileNameWithoutExtension(pic).Length >= ro.Name.Length)
                                {
                                    if (Path.GetFileNameWithoutExtension(pic).Substring(0, ro.Name.Length).ToLower() == ro.Name.ToLower())
                                    {
                                        foreach (string ext in imgexts)
                                        {
                                            if (Path.GetExtension(pic).ToLower() == ext)
                                            {
                                                ro.SnapShotPath = ".\\PICTURES\\" + Path.GetFileName(pic);
                                                break;
                                            }
                                        }
                                    }
                                }

                            }
                            //rom info
                            ro.Mapper = cart.MAPPER;
                            ro.IsSRAM = cart.IsBatteryBacked;
                            ro.System = cart.IsPAL ? "PAL" : "NTSC";
                            RomsHolder.ROMS.Add(ro);
                            PrecentCompleteLoading = (i * 100) / files.Length;
                            if (LoadRomsAdvantage != null)
                                LoadRomsAdvantage(this, null);
                        }
                    }
                    i++;
                }
                if (LoadRomsFinished != null)
                    LoadRomsFinished(this, null);
            }
            else
            {
                Program.CORE.WriteText("NO ROM FOUND, THE ROMS FOLDER ISN'T EXIST !!", 120, TextStatus.ERROR);
            }
        }
        public event EventHandler LoadRomsAdvantage;
        public event EventHandler LoadRomsFinished;
    }
}

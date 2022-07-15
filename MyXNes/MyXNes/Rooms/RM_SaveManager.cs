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
using Microsoft.Xna.Framework.Storage;
using System.IO;

namespace MyXNes
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class RM_SaveManager : Microsoft.Xna.Framework.GameComponent
    {
        SpriteFont Font_normal;
        SpriteFont Font_small;
        Texture2D tFrame;
        Texture2D tBannerBack;
        DeleteMode DELETEMODE = DeleteMode.None;
        public int ScrollIndex = 0;
        public int SelectedIndex = 0;
        bool Pressed = false;
        int countdown = 0;
        public List<MXN_ROM> FILES = new List<MXN_ROM>();
        public RM_SaveManager(Game game)
            : base(game)
        {
            //Load the textures
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
            if (DELETEMODE == DeleteMode.None)
            {
                if (!Pressed)
                {
                    countdown = 5;
                    if (Keyboard.GetState().IsKeyDown(Keys.Down) |
                        GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                    {
                        if (SelectedIndex < FILES.Count)
                            SelectedIndex++;
                        if ((SelectedIndex - ScrollIndex) > 15)
                            ScrollIndex++;
                        if (FILES.Count > 15)
                            if (ScrollIndex > (FILES.Count - 1) - 15)
                                ScrollIndex = (FILES.Count - 1) - 15;
                        if (SelectedIndex > (FILES.Count - 1))
                            SelectedIndex = (FILES.Count - 1);
                        Pressed = true;
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
                //FAST MOVE
                if (Keyboard.GetState().IsKeyDown(Keys.Right) |
                    GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)
                {
                    if (SelectedIndex < FILES.Count)
                        SelectedIndex++;
                    if ((SelectedIndex - ScrollIndex) > 15)
                        ScrollIndex++;
                    if (FILES.Count > 15)
                        if (ScrollIndex > (FILES.Count - 1) - 15)
                            ScrollIndex = (FILES.Count - 1) - 15;
                    if (SelectedIndex > (FILES.Count - 1))
                        SelectedIndex = (FILES.Count - 1);
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left) |
                    GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)
                {
                    if (SelectedIndex > 0)
                        SelectedIndex--;
                    if ((SelectedIndex - ScrollIndex) < 0)
                        ScrollIndex--;
                    if (ScrollIndex < 0)
                        ScrollIndex = 0;
                    if (SelectedIndex < 0)
                        SelectedIndex = 0;
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.PageUp) |
                    GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed)
                {
                    ScrollIndex = 0;
                    SelectedIndex = 0;
                    Pressed = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.PageDown) |
                    GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
                {
                    if (FILES.Count > 15)
                        ScrollIndex = (FILES.Count - 1) - 15;
                    SelectedIndex = (FILES.Count - 1);
                    Pressed = true;
                }
                //ACTION !!
                if (Keyboard.GetState().IsKeyDown(Keys.Escape) |
                    GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    Program.CORE.ROOM = CurrentRoom.MainMenu;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Delete) |
                    GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    if (FILES.Count > 0 & SelectedIndex >= 0)
                        DELETEMODE = DeleteMode.Selected;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.End) |
                    GamePad.GetState(PlayerIndex.One).Buttons.BigButton == ButtonState.Pressed)
                {
                    if (FILES.Count > 0 & SelectedIndex >= 0)
                        DELETEMODE = DeleteMode.All;
                }
            }
            switch (DELETEMODE)
            {
                case DeleteMode.Selected:
                    if (Keyboard.GetState().IsKeyDown(Keys.Y) |
                        GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed)
                    {
                        IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                        if (result.IsCompleted)
                        {
                            StorageDevice device = StorageDevice.EndShowSelector(result);
                            if (device != null && device.IsConnected)
                            {
                                if (FILES[SelectedIndex].Name.Substring(0, 8) == "[STATE ]")
                                    result = device.BeginOpenContainer("StateSaves", null, null);
                                else
                                    result = device.BeginOpenContainer("SRamSaves", null, null);

                                result.AsyncWaitHandle.WaitOne();

                                StorageContainer container = device.EndOpenContainer(result);
                                container.DeleteFile(FILES[SelectedIndex].Path);
                                SelectedIndex--;
                                if (SelectedIndex < 0)
                                    SelectedIndex = 0;
                                container.Dispose();
                            }
                        }
                        LoadFiles();
                        DELETEMODE = DeleteMode.None;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.N) |
                        GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed)
                    { DELETEMODE = DeleteMode.None; }
                    break;
                case DeleteMode.All:
                    if (Keyboard.GetState().IsKeyDown(Keys.Y) |
                        GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed)
                    {
                        IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                        if (result.IsCompleted)
                        {
                            StorageDevice device = StorageDevice.EndShowSelector(result);
                            if (device != null && device.IsConnected)
                            {
                                StorageContainer container;
                                //Delete all files
                                for (int i = 0; i < FILES.Count; i++)
                                {
                                    if (FILES[i].Name.Substring(0, 8) == "[STATE ]")
                                    {
                                        result = device.BeginOpenContainer("StateSaves", null, null);
                                        result.AsyncWaitHandle.WaitOne();
                                        container = device.EndOpenContainer(result);
                                        container.DeleteFile(FILES[i].Path);
                                    }
                                    else
                                    {
                                        result = device.BeginOpenContainer("SRamSaves", null, null);
                                        result.AsyncWaitHandle.WaitOne();
                                        container = device.EndOpenContainer(result);
                                        container.DeleteFile(FILES[i].Path);
                                    }
                                }
                                SelectedIndex--;
                                if (SelectedIndex < 0)
                                    SelectedIndex = 0;
                            }
                        }
                        SelectedIndex = 0;
                        LoadFiles();
                        DELETEMODE = DeleteMode.None;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.N) |
                        GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed)
                    { DELETEMODE = DeleteMode.None; }
                    break;
            }
            base.Update(gameTime);
        }
        public void Draw()
        {
            //Draw the background
            Program.CORE.DrawBackground();
            //Draw the frame of the state list
            Program.CORE.SpriteBatch.Draw(tFrame,
            new Rectangle(30, 189, Program.CORE.GraphicsDevice.Viewport.Width - 60,
            360), Color.White);
            //Draw the slider
            Program.CORE.SpriteBatch.Draw(tBannerBack,
            new Rectangle(720, 189, 30, 360), Color.White);
            //Draw the hand of the slider if we should
            if (FILES.Count > 16)
            {
                //calculations
                int height = (320 * 16) / FILES.Count;
                if (height < 30)
                    height = 30;
                int y = ((320 - height) * (ScrollIndex + 15)) / FILES.Count;
                y += 212;
                Program.CORE.SpriteBatch.Draw(tBannerBack,
                new Rectangle(722, y, 26, height), Color.White);
            }
            //Draw help
#if XBOX
            Program.CORE.SpriteBatch.DrawString(Font_small, "This page shows all the saved state files available in the storage unit.", new Vector2(10, Program.CORE.GraphicsDevice.Viewport.Height - 43), Color.White);
            Program.CORE.SpriteBatch.DrawString(Font_small, @"""" + "Back" + @"""" + "= back main menu, " + @"""" + "Up/Down/Left/Right/LeftShoulder/RightShoulder" + @"""" + "= select file, " + @"""" + "Start" + @"""" + "= delete selected, " + @"""" + "BigButton" + @"""" + "= delete all.", new Vector2(10, Program.CORE.GraphicsDevice.Viewport.Height - 30), Color.White);
#else
            Program.CORE.SpriteBatch.DrawString(Font_small, "This page shows all the saved state files available in the storage unit.", new Vector2(10, Program.CORE.GraphicsDevice.Viewport.Height - 43), Color.White);
            Program.CORE.SpriteBatch.DrawString(Font_small, @"""" + "Escape" + @"""" + "= back main menu, " + @"""" + "Up/Down/Left/Right/PageUp/PageDown" + @"""" + "= select file, " + @"""" + "Del" + @"""" + "= delete selected, " + @"""" + "End" + @"""" + "= delete all.", new Vector2(10, Program.CORE.GraphicsDevice.Viewport.Height - 30), Color.White);
#endif
            //Draw the items   
            int Y = 212;
            if (FILES.Count > 0)
            {
                for (int i = ScrollIndex; i < ScrollIndex + 16; i++)
                {
                    if (i < FILES.Count)
                    {
                        if (SelectedIndex == i)
                            Program.CORE.SpriteBatch.DrawString(Font_normal, (i + 1).ToString() + ". " + FILES[i].Name, new Vector2(80, Y), Color.Gold);
                        else
                            Program.CORE.SpriteBatch.DrawString(Font_normal, (i + 1).ToString() + ". " + FILES[i].Name, new Vector2(80, Y), Color.White);
                        Y += 20;
                    }
                }
            }
#if WINDOWS
            //Draw message if we have to
            switch (DELETEMODE)
            {
                case DeleteMode.Selected:
                    //Draw the frame
                    Program.CORE.SpriteBatch.Draw(tFrame,
                    new Rectangle(200, 220, 400, 70), Color.White);
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "ARE YOU SURE ? " + @"""" + "Y = Yes, N = No" + @"""",
                        new Vector2(226, 235), Color.White);
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "FILE WILL BE DELETED PERMANENTLY.",
                    new Vector2(226, 255), Color.Orange);
                    break;
                case DeleteMode.All:
                    //Draw the frame
                    Program.CORE.SpriteBatch.Draw(tFrame,
                    new Rectangle(200, 220, 400, 70), Color.White);
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "ARE YOU SURE ? " + @"""" + "Y = Yes, N = No" + @"""",
                        new Vector2(226, 235), Color.White);
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "FILES WILL BE DELETED PERMANENTLY.",
                    new Vector2(226, 255), Color.Orange);
                    break;
            }
#else
#if XBOX
            //Draw message if we have to
            switch (DELETEMODE)
            {
                case DeleteMode.Selected:
                    //Draw the frame
                    Program.CORE.SpriteBatch.Draw(tFrame,
                    new Rectangle(200, 220, 400, 70), Color.White);
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "ARE YOU SURE ? " + @"""" + "Y = Yes, X = No" + @"""",
                        new Vector2(226, 235), Color.White);
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "FILE WILL BE DELETED PERMANENTLY.",
                    new Vector2(226, 255), Color.Orange);
                    break;
                case DeleteMode.All:
                    //Draw the frame
                    Program.CORE.SpriteBatch.Draw(tFrame,
                    new Rectangle(200, 220, 400, 70), Color.White);
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "ARE YOU SURE ? " + @"""" + "Y = Yes, X = No" + @"""",
                        new Vector2(226, 235), Color.White);
                    Program.CORE.SpriteBatch.DrawString(Font_normal, "FILES WILL BE DELETED PERMANENTLY.",
                    new Vector2(226, 255), Color.Orange);
                    break;
            }
#endif
#endif
        }
        public void LoadFiles()
        {
            FILES.Clear();
            string[] files = new string[0];
            IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            if (result.IsCompleted)
            {
                StorageDevice device = StorageDevice.EndShowSelector(result);
                if (device != null && device.IsConnected)
                {
                    result = device.BeginOpenContainer("StateSaves", null, null);
                    result.AsyncWaitHandle.WaitOne();

                    StorageContainer container = device.EndOpenContainer(result);
                    files = container.GetFileNames("*");
                    foreach (string file in files)
                    {
                        if (Path.GetExtension(file).ToLower() == ".mns")
                        {
                            MXN_ROM ro = new MXN_ROM();
                            ro.Name = "[STATE ]" + Path.GetFileNameWithoutExtension(file);
                            ro.Path = file;
                            FILES.Add(ro);
                        }
                    }
                    result = device.BeginOpenContainer("SRamSaves", null, null);
                    result.AsyncWaitHandle.WaitOne();

                    container = device.EndOpenContainer(result);
                    files = container.GetFileNames("*");
                    //repeated twice ? this for arrange
                    foreach (string file in files)
                    {
                        if (Path.GetExtension(file).ToLower() == ".sav")
                        {
                            MXN_ROM ro = new MXN_ROM();
                            ro.Name = "[S-RAM ]" + Path.GetFileNameWithoutExtension(file);
                            ro.Path = file;
                            FILES.Add(ro);
                        }
                    }
                    container.Dispose();
                }
                else
                {
                    Program.CORE.WriteText("READ FAILED: STORAGE UNIT IS NOT CONNECTED OR DAMAGED !!", 120, TextStatus.ERROR);
                }
            }
        }
    }
    enum DeleteMode
    { All, Selected, None }
}

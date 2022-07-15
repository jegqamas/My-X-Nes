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
    /// This is the main type for your game
    /// </summary>
    public class Core : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch SpriteBatch;
        public CurrentRoom ROOM = CurrentRoom.StartUp;
        //draw stuff
        SpriteFont Font_large;
        Texture2D tBackground;
        SpriteFont Font_normal;
        SpriteFont Font_small;
        //ROOMS
        public RM_Loading rLOADING;
        public RM_MainMenu rMAINMENU;
        public RM_About rABOUT;
        public RM_Browser rBROWSER;
        public RM_Settings rSETTINGS;
        public RM_SaveManager rSAVEMANAGER;
        public RM_GamePlay rGAMEPLAY;
        public RM_StartUp rSTARTUP;
        public RM_KeyConfig rKEYSCONFIGURATION;
        //others
        string TextToDraw = "";
        int TextAppearance = 0;
        Color TextColor = Color.White;
        bool TextNesLog = false;
        GameTime time;

        public Core()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Exiting += new EventHandler<EventArgs>(Core_Exiting);
            graphics.IsFullScreen = Program.Settings.StartInFullScreen;
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);
#if WINDOWS
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
#endif
        }

        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            foreach (Microsoft.Xna.Framework.Graphics.DisplayMode displayMode
             in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (displayMode.Width == 800 && displayMode.Height == 600)
                {
                    e.GraphicsDeviceInformation.PresentationParameters.
                        BackBufferFormat = displayMode.Format;
                    e.GraphicsDeviceInformation.PresentationParameters.
                        BackBufferHeight = displayMode.Height;
                    e.GraphicsDeviceInformation.PresentationParameters.
                        BackBufferWidth = displayMode.Width;
                }
            }

        }

        void Core_Exiting(object sender, EventArgs e)
        {
            if (rGAMEPLAY.mainThread != null)
                rGAMEPLAY.mainThread.Abort();
            if (rGAMEPLAY.Nes != null)
                rGAMEPLAY.Nes.ShutDown();
            //save settings
            Program.Settings.LastGameScrollIndex = rBROWSER.ScrollIndex;
            Program.Settings.LastGameSelectedIndex = rBROWSER.SelectedIndex;
            Program.SaveSerialize("Settings.mxn", "Settings", Program.Settings);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            //Load rooms
            rBROWSER = new RM_Browser(this);
            rLOADING = new RM_Loading(this);
            rMAINMENU = new RM_MainMenu(this);
            rABOUT = new RM_About(this);
            rSETTINGS = new RM_Settings(this);
            rSAVEMANAGER = new RM_SaveManager(this);
            rGAMEPLAY = new RM_GamePlay(this);
            rSTARTUP = new RM_StartUp(this);
            rKEYSCONFIGURATION = new RM_KeyConfig(this);
            //load content
            Font_large = Content.Load<SpriteFont>("Font_big");
            tBackground = Program.CORE.Content.Load<Texture2D>("Background");
            Font_normal = Program.CORE.Content.Load<SpriteFont>("Font_normal");
            Font_small = Program.CORE.Content.Load<SpriteFont>("Font_small");
            //Load special settings
            rBROWSER.ScrollIndex = Program.Settings.LastGameScrollIndex;
            rBROWSER.SelectedIndex = Program.Settings.LastGameSelectedIndex;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            time = gameTime;
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            switch (ROOM)
            {
                case CurrentRoom.GamePlay: rGAMEPLAY.Update(gameTime); break;
                case CurrentRoom.About: rABOUT.Update(gameTime); break;
                case CurrentRoom.Loading: rLOADING.Update(gameTime); break;
                case CurrentRoom.MainMenu: rMAINMENU.Update(gameTime); break;
                case CurrentRoom.Browser: rBROWSER.Update(gameTime); break;
                case CurrentRoom.Options: rSETTINGS.Update(gameTime); break;
                case CurrentRoom.SaveManager: rSAVEMANAGER.Update(gameTime); break;
                case CurrentRoom.StartUp: rSTARTUP.Update(gameTime); break;
                case CurrentRoom.KeysConfiguration: rKEYSCONFIGURATION.Update(gameTime); break;
            }

            base.Update(gameTime);
        }
        /// <summary>
        /// Get the game time
        /// </summary>
        public GameTime gameTime
        { get { return time; } }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Immediate, null);
            switch (ROOM)
            {
                case CurrentRoom.GamePlay: rGAMEPLAY.Draw(); break;
                case CurrentRoom.About: rABOUT.Draw(); break;
                case CurrentRoom.Loading: rLOADING.Draw(); break;
                case CurrentRoom.MainMenu: rMAINMENU.Draw(); break;
                case CurrentRoom.Browser: rBROWSER.Draw(); break;
                case CurrentRoom.Options: rSETTINGS.Draw(); break;
                case CurrentRoom.SaveManager: rSAVEMANAGER.Draw(); break;
                case CurrentRoom.StartUp: rSTARTUP.Draw(); break;
                case CurrentRoom.KeysConfiguration: rKEYSCONFIGURATION.Draw(); break;
            }
            if (TextAppearance > 0)
            {
                if (!TextNesLog)
                    Program.CORE.SpriteBatch.DrawString(Font_large, TextToDraw,
                          new Vector2(30, Program.CORE.GraphicsDevice.Viewport.Height - 70), TextColor);
                else
                    Program.CORE.SpriteBatch.DrawString(Font_normal, TextToDraw, new Vector2(30, 30), Color.White);

                TextAppearance--;
            }
            SpriteBatch.End();
            base.Draw(gameTime);
        }
        public void WriteText(string Text, int Time, TextStatus Status)
        {
            TextToDraw = Text;
            TextAppearance = Time;
            TextNesLog = false;
            switch (Status)
            {
                case TextStatus.COOL: TextColor = Color.LightGreen; break;
                case TextStatus.ERROR: TextColor = Color.Red; break;
                case TextStatus.NONE: TextColor = Color.White; break;
                case TextStatus.WARNING: TextColor = Color.Yellow; break;
                case TextStatus.NESLOG:
                    TextColor = Color.White;
                    TextNesLog = true;
                    break;
            }
        }
        //DRAW METHODS
        /// <summary>
        /// Draw the background of My X Nes
        /// </summary>
        public void DrawBackground()
        {
            Program.CORE.SpriteBatch.Draw(tBackground,
            new Rectangle(0, 0, GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height), Color.White);

            SpriteBatch.DrawString(Font_small, Program.Version, new Vector2(620, 120), Color.White);
        }
    }
    public enum TextStatus
    { NONE, COOL, WARNING, ERROR, NESLOG }
}

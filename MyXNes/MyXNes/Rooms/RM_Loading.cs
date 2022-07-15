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
using System.Threading;

namespace MyXNes
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class RM_Loading : Microsoft.Xna.Framework.GameComponent
    {
        SpriteFont Font_normal;
        SpriteFont Font_large;
        SpriteFont Font_small;
        Texture2D tBackground;
        Texture2D tFrame;
        Texture2D tProgressBarBack;
        Thread MainThread;
        Texture2D tProgressBar;
        int CountDown = 60;
        int pr = 0;

        public RM_Loading(Game game)
            : base(game)
        {
            tBackground = Program.CORE.Content.Load<Texture2D>("Background");
            Font_large = Program.CORE.Content.Load<SpriteFont>("Font_big");
            tFrame = Program.CORE.Content.Load<Texture2D>("frame");
            Font_normal = Program.CORE.Content.Load<SpriteFont>("Font_normal");
            Font_small = Program.CORE.Content.Load<SpriteFont>("Font_small");
            tProgressBarBack = Program.CORE.Content.Load<Texture2D>("bannerback");
            tProgressBar = Program.CORE.Content.Load<Texture2D>("banner");
            Program.CORE.rBROWSER.LoadRomsAdvantage += new EventHandler(rBROWSER_LoadRomsAdvantage);
            Program.CORE.rBROWSER.LoadRomsFinished += new EventHandler(rBROWSER_LoadRomsFinished);
        }

        void rBROWSER_LoadRomsFinished(object sender, EventArgs e)
        {
            Program.CORE.ROOM = CurrentRoom.MainMenu;
        }

        void rBROWSER_LoadRomsAdvantage(object sender, EventArgs e)
        {
            pr = Program.CORE.rBROWSER.PrecentCompleteLoading;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (CountDown > 0)//We need some time to draw before we can load
                CountDown--;
            else if (CountDown == 0)
            {
                MainThread = new Thread(new ThreadStart(Program.CORE.rBROWSER.LoadRoms));
                MainThread.Start();
                CountDown = -1;//make it happen once
            }

            base.Update(gameTime);
        }
        public void Draw()
        {
            //Draw the background
            Program.CORE.DrawBackground();
            //Draw the frame of the game list
            Program.CORE.SpriteBatch.Draw(tFrame,
            new Rectangle(150, 280, Program.CORE.GraphicsDevice.Viewport.Width - 300,
            150), Color.White);

            //Draw loading
            Program.CORE.SpriteBatch.DrawString(Font_large, "LOADING, PLEASE WAIT ...",
            new Vector2(187, 300), Color.White);
            //Draw the progress bar
            Program.CORE.SpriteBatch.Draw(tProgressBarBack,
            new Rectangle(150, 386, Program.CORE.GraphicsDevice.Viewport.Width - 309, 40), Color.White);

            int X2 = ((Program.CORE.GraphicsDevice.Viewport.Width - 351) * pr) / 100;
            Program.CORE.SpriteBatch.Draw(tProgressBar,
            new Rectangle(178, 389, X2, 34), Color.White);

            //Draw help
            Program.CORE.SpriteBatch.DrawString(Font_small,
            "Now loading the roms, time long depends on the roms count.",
            new Vector2(190, 364), Color.White);

            Program.CORE.SpriteBatch.DrawString(Font_large, pr + "%",
            new Vector2((Program.CORE.GraphicsDevice.Viewport.Width / 2) - (Font_large.MeasureString(pr + "%").Length() / 2 - 10), 386), Color.LightBlue);
        }
    }
}

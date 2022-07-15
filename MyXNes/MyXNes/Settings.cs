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
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyNes.Nes;

namespace MyXNes
{
    public class Settings
    {
        public bool AutoSwitchEmulation = true;
        public TVFORMAT TV = TVFORMAT.NTSC;
        public PaletteFormat PaletteFormat = new PaletteFormat();
        public bool SoundEnabled = true;
        public bool AutoSaveSRAM = true;
        public bool StartInFullScreen = true;
        public float VolumeLevel = 0.95f;
        public bool EnableSound = true;
        public int LastGameSelectedIndex = 0;
        public int LastGameScrollIndex = 0;

        public Keys P1_Left = Keys.Left;
        public Keys P1_Right = Keys.Right;
        public Keys P1_Up = Keys.Up;
        public Keys P1_Down = Keys.Down;
        public Keys P1_A = Keys.X;
        public Keys P1_B = Keys.Z;
        public Keys P1_Start = Keys.V;
        public Keys P1_Select = Keys.C;

        public Keys P2_Left = Keys.A;
        public Keys P2_Right = Keys.D;
        public Keys P2_Up = Keys.W;
        public Keys P2_Down = Keys.S;
        public Keys P2_A = Keys.K;
        public Keys P2_B = Keys.J;
        public Keys P2_Start = Keys.E;
        public Keys P2_Select = Keys.Q;
    }
}

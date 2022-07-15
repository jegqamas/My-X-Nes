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
using MyNes.Nes.Input;

namespace MyXNes
{
    class Joypad : IJoypad
    {
        public bool A = false;
        public bool B = false;
        public bool Left = false;
        public bool Right = false;
        public bool Up = false;
        public bool Down = false;
        public bool Start = false;
        public bool Select = false;

        public byte GetData()
        {
            byte num = 0;

            if (A)
                num |= 1;

            if (B)
                num |= 2;

            if (Select)
                num |= 4;

            if (Start)
                num |= 8;

            if (Up)
                num |= 0x10;

            if (Down)
                num |= 0x20;

            if (Left)
                num |= 0x40;

            if (Right)
                num |= 0x80;

            return num;
        }
    }
}

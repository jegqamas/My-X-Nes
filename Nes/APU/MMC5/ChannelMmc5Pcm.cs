/*********************************************************************\
*This file is part of My Nes                                          *
*A Nintendo Entertainment System Emulator.                            *
*                                                                     *
*Copyright © Ala Hadid 2009 - 2011                                    *
*E-mail: mailto:ahdsoftwares@hotmail.com                              *
*                                                                     *
*My Nes is free software: you can redistribute it and/or modify       *
*it under the terms of the GNU General Public License as published by *
*the Free Software Foundation, either version 3 of the License, or    *
*(at your option) any later version.                                  *
*                                                                     *
*My Nes is distributed in the hope that it will be useful,            *
*but WITHOUT ANY WARRANTY; without even the implied warranty of       *
*MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
*GNU General Public License for more details.                         *
*                                                                     *
*You should have received a copy of the GNU General Public License    *
*along with this program.  If not, see <http://www.gnu.org/licenses/>.*
\*********************************************************************/
using System;
namespace MyNes.Nes
{
   
    public class ChannelMmc5Pcm : NesApuChannel
    {
        byte output = 0;

        public ChannelMmc5Pcm(NES nes)
            : base(nes)
        {
        }

        public override void ClockHalf()
        {
        }
        public override void ClockQuad()
        {
        }
        public override void Poke1(int addr, byte data)
        {
            output = data;
        }
        public override void Poke2(int addr, byte data)
        {
        }
        public override void Poke3(int addr, byte data)
        {
        }
        public override void Poke4(int addr, byte data)
        {
        }
        public override int RenderSample()
        {
            return output;
            //return 0;
        }

        public override void SaveState(System.IO.Stream stream)
        {
        }
        public override void LoadState(System.IO.Stream stream)
        {
        }
    }
}
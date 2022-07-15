﻿/*********************************************************************\
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyNes.Nes
{
   
    class Mapper229 : IMapper
    {
        CPUMemory map;
        public Mapper229(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            //if ((Address & 0x001E) == 0x001E)
            if ((Address & 0x0001) == 0x0000)
            {
                byte prg = (byte)(Address & 0x001F);

                map.Switch8kPrgRom((prg * 2 + 0) * 2, 0);
                map.Switch8kPrgRom((prg * 2 + 1) * 2, 1);
                map.Switch8kPrgRom((prg * 2 + 0) * 2, 2);
                map.Switch8kPrgRom((prg * 2 + 1) * 2, 3);

                map.Switch8kChrRom((Address & 0x0FFF) * 8);
            }
            else
            {
                map.Switch32kPrgRom(0);
                map.Switch8kChrRom((Address & 0x0FFF) * 8);
            }

            if ((Address & 0x0020) == 0x0020)
            {
                map.cartridge.Mirroring = Mirroring.Horizontal;
            }
            else
            {
                map.cartridge.Mirroring = Mirroring.Vertical;
            }
        }
        public void SetUpMapperDefaults()
        {
            map.Switch32kPrgRom(0);
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer(int cycles)
        {
        }
        public void SoftReset()
        { }
        public bool WriteUnder8000
        { get { return false; } }
        public bool WriteUnder6000
        { get { return false; } }
        public byte Read(ushort Address)
        {
            return 0;
        }
        public bool ScanlineTimerNotPauseAtVBLANK
        {
            get { return false; }
        }

        public void SaveState(System.IO.Stream stream)
        {

        }
        public void LoadState(System.IO.Stream stream)
        {

        }
    }
}

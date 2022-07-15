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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyNes.Nes
{
   
    class Mapper62 : IMapper
    {
        CPUMemory map;
        public Mapper62(CPUMemory MAP)
        { map = MAP; }
        public void Write(ushort address, byte data)
        {
            if (address >= 0x8000 & address <= 0xBFFF)
            {
                map.Switch8kChrRom(((address << 2) | (data & 0x3)) * 8);

                int data1 = ((address & 0x40) | (address >> 8 & 0x3F));
                int address1 = ~address >> 6 & 0x1;
                map.Switch16kPrgRom((data1 & ~address1) * 4, 0);
                map.Switch16kPrgRom((data1 | address1) * 4, 1);

                if ((address & 0x80) == 0x80)
                    map.cartridge.Mirroring = Mirroring.Horizontal;
                else
                    map.cartridge.Mirroring = Mirroring.Vertical;
            }
        }

        public byte Read(ushort Address)
        {
            return 0;
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
        {

        }

        public bool WriteUnder8000
        {
            get { return false; }
        }

        public bool WriteUnder6000
        {
            get { return false; }
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

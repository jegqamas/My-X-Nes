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
   
    class Mapper193 : IMapper
    {
        CPUMemory map;
        public Mapper193(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            switch (Address)
            {
                case 0x6000:
                    map.Switch2kChrRom((((data >> 1) & 0x7e) + 0) * 2, 0);
                    map.Switch2kChrRom((((data >> 1) & 0x7e) + 1) * 2, 1);
                    break;
                case 0x6001:
                    map.Switch2kChrRom((data >> 1) * 2, 2);
                    break;
                case 0x6002:
                    map.Switch2kChrRom((data >> 1) * 2, 3);
                    break;
                case 0x6003:
                    map.Switch32kPrgRom(data * 8);
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            map.Switch16kPrgRom((map.cartridge.PRG_PAGES - 2) * 4, 0);
            map.Switch16kPrgRom((map.cartridge.PRG_PAGES - 1) * 4, 1);
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
        { get { return true; } }
        public bool WriteUnder6000
        { get { return true; } }
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

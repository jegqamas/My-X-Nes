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
    class Mapper70 : IMapper
    {
        CPUMemory map;
        public Mapper70(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            map.Switch16kPrgRom(((data & 0x70) >> 4) * 4, 0);
            map.Switch8kChrRom((data & 0x0F)*8);


            if ((data & 0x80) == 0x80)
            {
                map.cartridge.Mirroring = Mirroring.One_Screen;
                map.cartridge.MirroringBase = 0x2400;
            }
            else
            {
                map.cartridge.Mirroring = Mirroring.One_Screen;
                map.cartridge.MirroringBase = 0x2000;
            }
        }
        public void SetUpMapperDefaults()
        {
            map.Switch16kPrgRom(0, 0);
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

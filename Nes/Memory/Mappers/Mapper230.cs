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
   
    class Mapper230 : IMapper
    {
        CPUMemory map;
        byte rom_sw = 0;
        public Mapper230(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            if (rom_sw == 1)
            {
                map.Switch8kPrgRom(((data & 0x07) * 2 + 0) * 2, 0);
                map.Switch8kPrgRom(((data & 0x07) * 2 + 1) * 2, 1);
            }
            else
            {
                if ((data & 0x20) == 0x20)
                {
                    map.Switch8kPrgRom(((data & 0x1F) * 2 + 16) * 2, 0);
                    map.Switch8kPrgRom(((data & 0x1F) * 2 + 17) * 2, 1);
                    map.Switch8kPrgRom(((data & 0x1F) * 2 + 16) * 2, 2);
                    map.Switch8kPrgRom(((data & 0x1F) * 2 + 17) * 2, 3);
                }
                else
                {
                    map.Switch8kPrgRom(((data & 0x1E) * 2 + 16) * 2, 0);
                    map.Switch8kPrgRom(((data & 0x1E) * 2 + 17) * 2, 1);
                    map.Switch8kPrgRom(((data & 0x1E) * 2 + 18) * 2, 2);
                    map.Switch8kPrgRom(((data & 0x1E) * 2 + 19) * 2, 3);
                }
                if ((data & 0x40) == 0x40)
                {
                    map.cartridge.Mirroring = Mirroring.Vertical;
                }
                else
                {
                    map.cartridge.Mirroring = Mirroring.Horizontal;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            map.IsSoftResetTrigger = true;
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);

            if (rom_sw == 1)
            {
                map.Switch16kPrgRom(0, 0);
                map.Switch16kPrgRom(14 * 4, 1);
            }
            else
            {
                map.Switch16kPrgRom(16 * 4, 0);
                map.Switch16kPrgRom((map.cartridge.PRG_PAGES - 1) * 4, 1);
            }
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer(int cycles)
        {
        }
        public void SoftReset()
        {
            if (rom_sw == 1)
            {
                rom_sw = 0;
            }
            else
            {
                rom_sw = 1;
            }
        }
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

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
    
    class Mapper22 : IMapper
    {
        CPUMemory Map;
        public Mapper22(CPUMemory Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if (address == 0x8000)
            {
                Map.Switch8kPrgRom(data * 2, 0);
            }
            else if (address == 0x9000)
            {
                switch (data & 0x3)
                {
                    case 0: Map.cartridge.Mirroring = Mirroring.Vertical; break;
                    case 1: Map.cartridge.Mirroring = Mirroring.Horizontal; break;
                    case 2:
                        Map.cartridge.Mirroring = Mirroring.One_Screen;
                        Map.cartridge.MirroringBase = 0x2400;
                        break;
                    case 3:
                        Map.cartridge.Mirroring = Mirroring.One_Screen;
                        Map.cartridge.MirroringBase = 0x2000;
                        break;
                }
            }
            else if (address == 0xA000)
            {
                Map.Switch8kPrgRom(data * 2, 1);
            }
            else if (address == 0xB000)
            {
                Map.Switch1kChrRom((data >> 1), 0);
            }
            else if (address == 0xB001)
            {
                Map.Switch1kChrRom((data >> 1), 1);
            }
            else if (address == 0xC000)
            {
                Map.Switch1kChrRom((data >> 1), 2);
            }
            else if (address == 0xC001)
            {
                Map.Switch1kChrRom((data >> 1), 3);
            }
            else if (address == 0xD000)
            {
                Map.Switch1kChrRom((data >> 1), 4);
            }
            else if (address == 0xD001)
            {
                Map.Switch1kChrRom((data >> 1), 5);
            }
            else if (address == 0xE000)
            {
                Map.Switch1kChrRom((data >> 1), 6);
            }
            else if (address == 0xE001)
            {
                Map.Switch1kChrRom((data >> 1), 7);
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom((Map.cartridge.PRG_PAGES - 1) * 4, 1);
            if (Map.cartridge.IsVRAM)
                Map.FillCHR(16);
            Map.Switch8kChrRom(0);
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

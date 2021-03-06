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
    
    class Mapper71 : IMapper
    {
        CPUMemory Map;
        public Mapper71(CPUMemory Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if ((address & 0xE000) == 0x6000)
                Map.Switch16kPrgRom(data * 4, 0);
            else
                switch (address & 0xF000)
                {
                    case 0xF000:
                    case 0xE000:
                    case 0xD000:
                    case 0xC000: Map.Switch16kPrgRom(data * 4, 0); break;
                    case 0x9000:
                        Map.cartridge.Mirroring = Mirroring.One_Screen;
                        if ((data & 0x10) != 0)
                            Map.cartridge.MirroringBase = 0x2400;
                        else
                            Map.cartridge.MirroringBase = 0x2000;
                        break;
                }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom((Map.cartridge.PRG_PAGES - 1) * 4, 1);
            Map.cartridge.Mirroring = Mirroring.One_Screen;
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
        { get { return true; } }
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

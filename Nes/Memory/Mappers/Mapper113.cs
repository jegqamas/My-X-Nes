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
    
    class Mapper113 : IMapper
    {
        CPUMemory Map;
        public Mapper113(CPUMemory map)
        { Map = map; }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x4100:
                case 0x4111:
                case 0x4120:
                case 0x4194:
                case 0x4195:
                case 0x4900:
                case 0x8008:
                case 0x8009:
                    Map.Switch32kPrgRom((data >> 3) * 8);
                    Map.Switch8kChrRom((((data >> 3) & 0x08) + (data & 0x07)) * 8);
                    break;
                case 0x8E66:
                case 0x8E67:
                    Map.Switch8kChrRom(((data & 0x07) == 0x07) ? 0 : 8);
                    break;
                case 0xE00A:
                    Map.cartridge.Mirroring = Mirroring.One_Screen;
                    Map.cartridge.MirroringBase = 0x2000;
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch32kPrgRom(0);
            if (Map.cartridge.IsVRAM)
                Map.FillCHR(8);
            Map.Switch8kChrRom(0);
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
            get { return true; }
        }
        public bool WriteUnder6000
        {
            get { return true; }
        }
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

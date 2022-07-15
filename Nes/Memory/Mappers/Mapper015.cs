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
   
    class Mapper15 : IMapper
    {
        CPUMemory Map;
        public Mapper15(CPUMemory Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000:
                    if ((data & 0x80) == 0x80)
                    {
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 1) * 2, 0);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 0) * 2, 1);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 3) * 2, 2);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 2) * 2, 3);
                    }
                    else
                    {
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 0) * 2, 0);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 1) * 2, 1);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 2) * 2, 2);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 3) * 2, 3);
                    }
                    if ((data & 0x40) == 0x40)
                        Map.cartridge.Mirroring = Mirroring.Horizontal;
                    else
                        Map.cartridge.Mirroring = Mirroring.Vertical;
                    break;
                case 0x8001:
                    if ((data & 0x80) == 0x80)
                    {
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 1) * 2, 2);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 0) * 2, 3);
                    }
                    else
                    {
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 0) * 2, 2);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 1) * 2, 3);
                    }
                    break;
                case 0x8002:
                    if ((data & 0x80) == 0x80)
                    {
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 1) * 2, 0);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 1) * 2, 1);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 1) * 2, 2);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 1) * 2, 3);
                    }
                    else
                    {
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 0) * 2, 0);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 0) * 2, 1);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 0) * 2, 2);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 0) * 2, 3);
                    }
                    break;
                case 0x8003:
                    if ((data & 0x80) == 0x80)
                    {
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 1) * 2, 2);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 0) * 2, 3);
                    }
                    else
                    {
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 0) * 2, 2);
                        Map.Switch8kPrgRom(((data & 0x3F) * 2 + 1) * 2, 3);
                    }
                    if ((data & 0x40) == 0x40)
                        Map.cartridge.Mirroring = Mirroring.Horizontal;
                    else
                        Map.cartridge.Mirroring = Mirroring.Vertical;
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

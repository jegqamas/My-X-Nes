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
   
    class Mapper80 : IMapper
    {
        CPUMemory Map;
        public Mapper80(CPUMemory map)
        { Map = map; }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x7EF0:
                    Map.Switch2kChrRom(((data >> 1) & 0x3F) * 2, 0);
                    if (Map.cartridge.PRGSizeInKB == 256)
                    {
                        if ((data & 0x80) == 0x80)
                        {
                            Map.Switch1kCHRToVRAM(1, 0);
                            Map.Switch1kCHRToVRAM(1, 1);
                        }
                        else
                        {
                            Map.Switch1kCHRToVRAM(0, 0);
                            Map.Switch1kCHRToVRAM(0, 1);
                        }
                    }
                    break;

                case 0x7EF1:
                    Map.Switch2kChrRom(((data >> 1) & 0x3F) * 2, 1);
                    if (Map.cartridge.PRGSizeInKB == 256)
                    {
                        if ((data & 0x80) == 0x80)
                        {
                            Map.Switch1kCHRToVRAM(1, 2);
                            Map.Switch1kCHRToVRAM(1, 3);
                        }
                        else
                        {
                            Map.Switch1kCHRToVRAM(0, 2);
                            Map.Switch1kCHRToVRAM(0, 3);
                        }
                    }
                    break;

                case 0x7EF2:
                    Map.Switch1kChrRom(data, 4);
                    break;
                case 0x7EF3:
                    Map.Switch1kChrRom(data, 5);
                    break;
                case 0x7EF4:
                    Map.Switch1kChrRom(data, 6);
                    break;
                case 0x7EF5:
                    Map.Switch1kChrRom(data, 7);
                    break;

                case 0x7EF6:
                    if ((data & 0x01) == 0x01)
                        Map.cartridge.Mirroring = Mirroring.Vertical;
                    else
                        Map.cartridge.Mirroring = Mirroring.Horizontal;
                    break;

                case 0x7EFA:
                case 0x7EFB:
                    Map.Switch8kPrgRom(data * 2, 0);
                    break;
                case 0x7EFC:
                case 0x7EFD:
                    Map.Switch8kPrgRom(data * 2, 1);
                    break;
                case 0x7EFE:
                case 0x7EFF:
                    Map.Switch8kPrgRom(data * 2, 2);
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.cartridge.PRG_PAGES - 1) * 4, 1);
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

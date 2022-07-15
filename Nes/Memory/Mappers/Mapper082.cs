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
    class Mapper82 : IMapper
    {
        bool Swapped;
        CPUMemory Map;
        public Mapper82(CPUMemory map)
        { Map = map; }
        public void Write(ushort address, byte data)
        {
            if (address == 0x7EF0)
            {
                if (Swapped)
                    Map.Switch2kChrRom((data >> 1) * 2, 2);
                else
                    Map.Switch2kChrRom((data >> 1) * 2, 0);
            }
            else if (address == 0x7EF1)
            {
                if (Swapped)
                    Map.Switch2kChrRom((data >> 1) * 2, 3);
                else
                    Map.Switch2kChrRom((data >> 1) * 2, 1);
            }
            else if (address == 0x7EF2)
            {
                if (Swapped)
                    Map.Switch1kChrRom(data, 0);
                else
                    Map.Switch1kChrRom(data, 4);
            }
            else if (address == 0x7EF3)
            {
                if (Swapped)
                    Map.Switch1kChrRom(data, 1);
                else
                    Map.Switch1kChrRom(data, 5);
            }
            else if (address == 0x7EF4)
            {
                if (Swapped)
                    Map.Switch1kChrRom(data, 2);
                else
                    Map.Switch1kChrRom(data, 6);
            }
            else if (address == 0x7EF5)
            {
                if (Swapped)
                    Map.Switch1kChrRom(data, 3);
                else
                    Map.Switch1kChrRom(data, 7);
            }
            else if (address == 0x7EF6)
            {
                Swapped = ((data & 0x2) >> 1) != 0;
                if ((data & 0x01) == 0)
                    Map.cartridge.Mirroring = Mirroring.Horizontal;
                else
                    Map.cartridge.Mirroring = Mirroring.Vertical;
            }
            else if (address == 0x7EFA)
                Map.Switch8kPrgRom((data >> 2) * 2, 0);
            else if (address == 0x7EFB)
                Map.Switch8kPrgRom((data >> 2) * 2, 1);
            else if (address == 0x7EFC)
                Map.Switch8kPrgRom((data >> 2) * 2, 2);
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom((Map.cartridge.PRG_PAGES - 1) * 4, 1);
            Map.Switch16kPrgRom((Map.cartridge.PRG_PAGES - 2) * 4, 0);
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
            get { return false; }
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
            stream.WriteByte((byte)(Swapped ? 1 : 0));
        }
        public void LoadState(System.IO.Stream stream)
        {
            Swapped = stream.ReadByte() == 1;
        }
    }
}

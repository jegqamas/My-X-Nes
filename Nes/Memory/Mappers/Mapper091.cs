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
    
    class Mapper91 : IMapper
    {
        CPUMemory Map;
        public bool IRQEnabled;
        public int IRQCount = 0;
        public Mapper91(CPUMemory map)
        { Map = map; }
        public void Write(ushort address, byte data)
        {
            if (address == 0x6000)
                Map.Switch2kChrRom(data * 2, 0);
            else if (address == 0x6001)
                Map.Switch2kChrRom(data * 2, 1);
            else if (address == 0x6002)
                Map.Switch2kChrRom(data * 2, 2);
            else if (address == 0x6003)
                Map.Switch2kChrRom(data * 2, 3);
            else if (address == 0x7000)
                Map.Switch8kPrgRom(data * 2, 0);
            else if (address == 0x7001)
                Map.Switch8kPrgRom(data * 2, 1);
            else if (address == 0x7006)
            {
                IRQEnabled = false;
                IRQCount = 0;
            }
            else if (address == 0x7007)
                IRQEnabled = true;
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom((Map.cartridge.PRG_PAGES - 1) * 4, 1);
            if (Map.cartridge.IsVRAM)
                Map.FillCHR(8);
            Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
            if (IRQCount < 8 & IRQEnabled)
            {
                IRQCount++;
                if (IRQCount >= 8)
                {
                    Map.cpu.SetIRQ(true);
                }
            }
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

        }
        public void LoadState(System.IO.Stream stream)
        {

        }
    }
}

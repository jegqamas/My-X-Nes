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
   
    class Mapper212 : IMapper
    {
        CPUMemory mem;
        public Mapper212(CPUMemory MEM)
        {
            mem = MEM;
        }

        public void Write(ushort address, byte data)
        {
            mem.cartridge.Mirroring = ((address & 0x8) != 0) ? Mirroring.Horizontal : Mirroring.Vertical;
            mem.Switch8kChrRom((address & 0x7) * 8);
            if ((address & 0x4000) != 0)
            {
                mem.Switch32kPrgRom(((address & 0x6) >> 1) * 8);
            }
            else
            {
                mem.Switch16kPrgRom((address & 0x7) * 4, 0);
                mem.Switch16kPrgRom((address & 0x7) * 4, 1);
            }
        }

        public void SetUpMapperDefaults()
        {
            mem.Switch32kPrgRom(0);
            if (mem.cartridge.IsVRAM)
                mem.FillCHR(16);
            mem.Switch8kChrRom(0);
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
            get { return false; }
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

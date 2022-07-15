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
    
    class Mapper61 : IMapper
    {
        CPUMemory _Map;
        public Mapper61(CPUMemory map)
        { _Map = map; }
        public void Write(ushort address, byte data)
        {
            if (address >= 0x8000 & address <= 0xFFFF)
            {
                if ((address & 0x10) == 0)
                    _Map.Switch32kPrgRom((address & 0xF) * 8);
                else
                {
                    _Map.Switch16kPrgRom((((address & 0xF) << 1) | (((address & 0x20) >> 5))) * 4, 0);
                    _Map.Switch16kPrgRom((((address & 0xF) << 1) | (((address & 0x20) >> 5))) * 4, 1);
                }

                _Map.cartridge.Mirroring = ((address & 0x80) != 0) ? Mirroring.Horizontal : Mirroring.Vertical;
            }
        }
        public void SetUpMapperDefaults()
        {
            _Map.Switch32kPrgRom(0);
            if (_Map.cartridge.IsVRAM)
                _Map.FillCHR(16);
            _Map.Switch8kChrRom(0);
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

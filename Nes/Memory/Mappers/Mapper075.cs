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
    
    class Mapper75 : IMapper
    {
        CPUMemory _Map;
        public int[] reg = new int[2];
        public Mapper75(CPUMemory Map)
        {
            _Map = Map;
        }

        public void Write(ushort address, byte data)
        {
            switch (address & 0xF000)
            {
                case 0x8000:
                    _Map.Switch8kPrgRom((data & 0x0F) * 2, 0);
                    break;

                case 0x9000:
                    if ((data & 0x01) != 0)
                        _Map.cartridge.Mirroring = Mirroring.Horizontal;
                    else
                        _Map.cartridge.Mirroring = Mirroring.Vertical;

                    reg[0] = (reg[0] & 0x0F) | ((data & 0x02) << 3);
                    reg[1] = (reg[1] & 0x0F) | ((data & 0x04) << 2);
                    _Map.Switch4kChrRom(reg[0] * 4, 0);
                    _Map.Switch4kChrRom(reg[1] * 4, 1);
                    break;

                case 0xA000:
                    _Map.Switch8kPrgRom((data & 0x0F) * 2, 1);
                    break;
                case 0xC000:
                    _Map.Switch8kPrgRom((data & 0x0F) * 2, 2);
                    break;

                case 0xE000:
                    reg[0] = (reg[0] & 0x10) | (data & 0x0F);
                    _Map.Switch4kChrRom(reg[0] * 4, 0);
                    break;

                case 0xF000:
                    reg[1] = (reg[1] & 0x10) | (data & 0x0F);
                    _Map.Switch4kChrRom(reg[1] * 4, 1);
                    break;
            }
        }

        public void SetUpMapperDefaults()
        {
            _Map.Switch16kPrgRom(0, 0);
            _Map.Switch16kPrgRom((_Map.cartridge.PRG_PAGES - 1) * 4, 1);
            if (_Map.cartridge.IsVRAM)
                _Map.FillCHR(8);
            _Map.Switch8kChrRom(0);
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

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
using System.IO;
namespace MyNes.Nes
{

    class Mapper78 : IMapper
    {
        CPUMemory _Map;
        bool IREM = true;
        public Mapper78(CPUMemory Map)
        {
            _Map = Map;
        }
        public void Write(ushort address, byte data)
        {
            if (address >= 0x8000 & address <= 0xFFFF)
            {
                _Map.Switch16kPrgRom((data & 0x7) * 4, 0);
                _Map.Switch8kChrRom(((data & 0xF0) >> 4) * 8);
                if ((address & 0xFE00) != 0xFE00)
                {
                    if (IREM)
                    {
                        if ((data & 0x8) != 0)
                            _Map.cartridge.Mirroring = Mirroring.Horizontal;
                        else
                            _Map.cartridge.Mirroring = Mirroring.Vertical;
                    }
                    else
                    {
                        _Map.cartridge.Mirroring = Mirroring.One_Screen;
                        if ((data & 0x8) != 0)
                            _Map.cartridge.MirroringBase = 0x2000;
                        else
                            _Map.cartridge.MirroringBase = 0x2400;
                    }
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            _Map.Switch16kPrgRom(0, 0);
            _Map.Switch16kPrgRom((_Map.cartridge.CHR_PAGES - 1) * 4, 1);
            try
            {
                if (!_Map.cartridge.RomPath.Contains("Holy Diver"))
                    IREM = false;
            }
            catch
            { }
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

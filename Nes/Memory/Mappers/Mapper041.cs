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
   
    class Mapper41 : IMapper
    {
        CPUMemory Map;
        byte[] reg = new byte[2];
        public Mapper41(CPUMemory Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            if (address >= 0x6000 & address < 0x6800)
            {
                Map.Switch32kPrgRom((address & 0x07) * 8);
                reg[0] = (byte)(address & 0x04);
                reg[1] = (byte)(((address >> 1) & 0x0C) | reg[1] & 0x03);
                Map.Switch8kChrRom(reg[1] * 8);
                if ((address & 0x20) == 0x20)
                    Map.cartridge.Mirroring = Mirroring.Horizontal;
                else
                    Map.cartridge.Mirroring = Mirroring.Vertical;
            }
            else if (address >= 0x8000 & address < 0xFFFF)
            {
                if (reg[0] == 0x04)
                {
                    reg[1] = (byte)(reg[1] & 0x0C | address & 0x03);
                    Map.Switch8kChrRom(reg[1] * 8);
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            reg[0] = 0x04;
            Map.Switch32kPrgRom(0);
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
        { get { return true; } }
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
            stream.Write(reg, 0, reg.Length);
        }
        public void LoadState(System.IO.Stream stream)
        {
            stream.Read(reg, 0, reg.Length);
        }
    }
}

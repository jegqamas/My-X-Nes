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
   
    class Mapper88 : IMapper
    {
        CPUMemory map;
        byte reg = 0;
        public Mapper88(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            switch (Address)
            {
                case 0x8000:
                    reg = data;
                    break;
                case 0x8001:
                    switch (reg & 0x07)
                    {
                        case 0:
                            map.Switch2kChrRom((data >> 1) * 2, 0);
                            break;
                        case 1:
                            map.Switch2kChrRom((data >> 1) * 2, 1);
                            break;
                        case 2:
                            map.Switch1kChrRom(data + 0x40, 4);
                            break;
                        case 3:
                            map.Switch1kChrRom(data + 0x40, 5);
                            break;
                        case 4:
                            map.Switch1kChrRom(data + 0x40, 6);
                            break;
                        case 5:
                            map.Switch1kChrRom(data + 0x40, 7);
                            break;
                        case 6:
                            map.Switch8kPrgRom(data * 2, 0);
                            break;
                        case 7:
                            map.Switch8kPrgRom(data * 2, 1);
                            break;
                    }
                    break;
                case 0xC000:
                    if (data != 0)
                    {
                        map.cartridge.Mirroring = Mirroring.One_Screen;
                        map.cartridge.MirroringBase = 0x2400;
                    }
                    else
                    {
                        map.cartridge.Mirroring = Mirroring.One_Screen;
                        map.cartridge.MirroringBase = 0x2000;
                    }
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            map.Switch16kPrgRom(0, 0);
            map.Switch16kPrgRom((map.cartridge.PRG_PAGES - 1) * 4, 1);
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
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
            stream.WriteByte(reg);
        }
        public void LoadState(System.IO.Stream stream)
        {
            reg = (byte)stream.ReadByte();
        }
    }
}

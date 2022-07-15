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
   
    class Mapper243 : IMapper
    {
        CPUMemory map;
        byte[] reg = new byte[4];
        public Mapper243(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            if ((Address & 0x4101) == 0x4100)
            {
                reg[0] = data;
            }
            else if ((Address & 0x4101) == 0x4101)
            {
                switch (reg[0] & 0x07)
                {
                    case 0:
                        reg[1] = 0;
                        reg[2] = 3;
                        break;
                    case 4:
                        reg[2] = (byte)((reg[2] & 0x06) | (data & 0x01));
                        break;
                    case 5:
                        reg[1] = (byte)(data & 0x01);
                        break;
                    case 6:
                        reg[2] = (byte)((reg[2] & 0x01) | ((data & 0x03) << 1));
                        break;
                    case 7:
                        reg[3] = (byte)(data & 0x01);
                        break;
                    default:
                        break;
                }

                map.Switch32kPrgRom(reg[1]*8);

                map.Switch1kChrRom(reg[2] * 8 + 0,0);
                map.Switch1kChrRom(reg[2] * 8 + 1, 1);
                map.Switch1kChrRom(reg[2] * 8 + 2, 2);
                map.Switch1kChrRom(reg[2] * 8 + 3, 3);
                map.Switch1kChrRom(reg[2] * 8 + 4, 4);
                map.Switch1kChrRom(reg[2] * 8 + 5, 5);
                map.Switch1kChrRom(reg[2] * 8 + 6, 6);
                map.Switch1kChrRom(reg[2] * 8 + 7, 7);

                if (reg[3]!=0)
                {
                    map.cartridge.Mirroring = Mirroring.Vertical;
                }
                else
                {
                    map.cartridge.Mirroring = Mirroring.Horizontal;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            map.Switch32kPrgRom(0);
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);

            reg[0] = 0;
            reg[1] = 0;
            reg[2] = 3;
            reg[3] = 0;
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

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

    class Mapper251 : IMapper
    {
        CPUMemory map;
        byte[] reg = new byte[11];
        byte[] breg = new byte[4];
        public Mapper251(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            if (Address < 0x8000)
            {
                if ((Address & 0xE001) == 0x6000)
                {
                    if (reg[9] != 0)
                    {
                        breg[reg[10]++] = data;
                        if (reg[10] == 4)
                        {
                            reg[10] = 0;
                            SetBank();
                        }
                    }
                }
            }
            else
            {
                switch (Address & 0xE001)
                {
                    case 0x8000:
                        reg[8] = data;
                        SetBank();
                        break;
                    case 0x8001:
                        reg[reg[8] & 0x07] = data;
                        SetBank();
                        break;
                    case 0xA001:
                        if ((data & 0x80) == 0x80)
                        {
                            reg[9] = 1;
                            reg[10] = 0;
                        }
                        else
                        {
                            reg[9] = 0;
                        }
                        break;
                }
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
        void SetBank()
        {
            int[] chr = new int[6];
            int[] prg = new int[4];

            for (int i = 0; i < 6; i++)
            {
                chr[i] = (reg[i] | (breg[1] << 4)) & ((breg[2] << 4) | 0x0F);
            }

            if ((reg[8] & 0x80) == 0x80)
            {
                map.Switch1kChrRom(chr[2], 0);
                map.Switch1kChrRom(chr[3], 1);
                map.Switch1kChrRom(chr[4], 2);
                map.Switch1kChrRom(chr[5], 3);
                map.Switch1kChrRom(chr[0], 4);
                map.Switch1kChrRom(chr[0] + 1, 5);
                map.Switch1kChrRom(chr[1], 6);
                map.Switch1kChrRom(chr[1] + 1, 7);
            }
            else
            {

                map.Switch1kChrRom(chr[0], 0);
                map.Switch1kChrRom(chr[0] + 1, 1);
                map.Switch1kChrRom(chr[1], 2);
                map.Switch1kChrRom(chr[1] + 1, 3);
                map.Switch1kChrRom(chr[2], 4);
                map.Switch1kChrRom(chr[3], 5);
                map.Switch1kChrRom(chr[4], 6);
                map.Switch1kChrRom(chr[5], 7);
            }

            prg[0] = (reg[6] & ((breg[3] & 0x3F) ^ 0x3F)) | (breg[1]);
            prg[1] = (reg[7] & ((breg[3] & 0x3F) ^ 0x3F)) | (breg[1]);
            prg[2] = prg[3] = ((breg[3] & 0x3F) ^ 0x3F) | (breg[1]);
            prg[2] &= map.cartridge.PRG_PAGES - 1;

            if ((reg[8] & 0x40) == 0x40)
            {
                map.Switch8kPrgRom(prg[2] * 2, 0);
                map.Switch8kPrgRom(prg[1] * 2, 1);
                map.Switch8kPrgRom(prg[0] * 2, 2);
                map.Switch8kPrgRom(prg[3] * 2, 3);
            }
            else
            {
                map.Switch8kPrgRom(prg[0] * 2, 0);
                map.Switch8kPrgRom(prg[1] * 2, 1);
                map.Switch8kPrgRom(prg[2] * 2, 2);
                map.Switch8kPrgRom(prg[3] * 2, 3);
            }
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
            stream.Write(reg, 0, reg.Length); stream.Write(breg, 0, reg.Length);
        }
        public void LoadState(System.IO.Stream stream)
        {
            stream.Read(reg, 0, reg.Length); stream.Read(breg, 0, reg.Length);
        }
    }
}

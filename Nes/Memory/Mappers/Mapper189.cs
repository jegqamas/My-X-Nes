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

    class Mapper189 : IMapper
    {
        CPUMemory map;
        byte[] reg = new byte[2];
        byte chr01 = 0;
        byte chr23 = 2;
        byte chr4 = 4;
        byte chr5 = 5;
        byte chr6 = 6;
        byte chr7 = 7;
        byte irq_enable = 0;
        byte irq_counter = 0;
        byte irq_latch = 0;
        byte[] protect_dat = new byte[4];
        public Mapper189(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            if (Address < 0x8000)
            {
                if ((Address & 0xFF00) == 0x4100)
                {
                    map.Switch32kPrgRom(((data & 0x30) >> 4) * 8);
                }
                else if ((Address & 0xFF00) == 0x6100)
                {
                    map.Switch32kPrgRom((data & 0x30) * 8);
                }
            }
            else
            {
                switch (Address & 0xE001)
                {
                    case 0x8000:
                        reg[0] = data;
                        SetBank_PPU();
                        break;

                    case 0x8001:
                        reg[1] = data;
                        SetBank_PPU();
                        switch (reg[0] & 0x07)
                        {
                            case 0x00:
                                chr01 = (byte)(data & 0xFE);
                                SetBank_PPU();
                                break;
                            case 0x01:
                                chr23 = (byte)(data & 0xFE);
                                SetBank_PPU();
                                break;
                            case 0x02:
                                chr4 = data;
                                SetBank_PPU();
                                break;
                            case 0x03:
                                chr5 = data;
                                SetBank_PPU();
                                break;
                            case 0x04:
                                chr6 = data;
                                SetBank_PPU();
                                break;
                            case 0x05:
                                chr7 = data;
                                SetBank_PPU();
                                break;
                        }
                        break;

                    case 0xA000:
                        if ((data & 0x01) == 0x01)
                            map.cartridge.Mirroring = Mirroring.Horizontal;
                        else
                            map.cartridge.Mirroring = Mirroring.Vertical;
                        break;

                    case 0xC000:
                        irq_counter = data;
                        break;
                    case 0xC001:
                        irq_latch = data;
                        break;
                    case 0xE000:
                        irq_enable = 0;
                        map.cpu.SetIRQ(false);
                        break;
                    case 0xE001:
                        irq_enable = 0xFF;
                        break;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            map.Switch16kPrgRom((map.cartridge.PRG_PAGES - 2) * 4, 0);
            map.Switch16kPrgRom((map.cartridge.PRG_PAGES - 1) * 4, 1);
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            SetBank_PPU();
            if (map.cartridge.PRG_PAGES == 1)
            {
                map.Switch16kPrgRom(0, 1);
            }
        }
        void SetBank_PPU()
        {

            if (!map.cartridge.IsVRAM)
            {
                if ((reg[0] & 0x80) == 0x80)
                {
                    map.Switch1kChrRom(chr4, 0);
                    map.Switch1kChrRom(chr5, 1);
                    map.Switch1kChrRom(chr6, 2);
                    map.Switch1kChrRom(chr7, 3);
                    map.Switch1kChrRom(chr01, 4);
                    map.Switch1kChrRom(chr01 + 1, 5);
                    map.Switch1kChrRom(chr23, 6);
                    map.Switch1kChrRom(chr23 + 1, 7);
                }
                else
                {
                    map.Switch1kChrRom(chr01, 0);
                    map.Switch1kChrRom(chr01 + 1, 1);
                    map.Switch1kChrRom(chr23, 2);
                    map.Switch1kChrRom(chr23 + 1, 3);
                    map.Switch1kChrRom(chr4, 4);
                    map.Switch1kChrRom(chr5, 5);
                    map.Switch1kChrRom(chr6, 6);
                    map.Switch1kChrRom(chr7, 7);
                }
            }
            else
            {
                if ((reg[0] & 0x80) == 0x80)
                {
                    map.Switch1kChrRom((chr01 + 0) & 0x07, 4);
                    map.Switch1kChrRom((chr01 + 1) & 0x07, 5);
                    map.Switch1kChrRom((chr23 + 0) & 0x07, 6);
                    map.Switch1kChrRom((chr23 + 1) & 0x07, 7);
                    map.Switch1kChrRom(chr4 & 0x07, 0);
                    map.Switch1kChrRom(chr5 & 0x07, 1);
                    map.Switch1kChrRom(chr6 & 0x07, 2);
                    map.Switch1kChrRom(chr7 & 0x07, 3);
                }
                else
                {
                    map.Switch1kChrRom((chr01 + 0) & 0x07, 0);
                    map.Switch1kChrRom((chr01 + 1) & 0x07, 1);
                    map.Switch1kChrRom((chr23 + 0) & 0x07, 2);
                    map.Switch1kChrRom((chr23 + 1) & 0x07, 3);
                    map.Switch1kChrRom(chr4 & 0x07, 4);
                    map.Switch1kChrRom(chr5 & 0x07, 5);
                    map.Switch1kChrRom(chr6 & 0x07, 6);
                    map.Switch1kChrRom(chr7 & 0x07, 7);
                }
            }
        }
        public void TickScanlineTimer()
        {
            if (irq_enable != 0)
            {
                if ((--irq_counter) == 0)
                {
                    irq_counter = irq_latch;
                    map.cpu.SetIRQ(true);
                }
            }
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
            stream.WriteByte(chr01);
            stream.WriteByte(chr23);
            stream.WriteByte(chr4);
            stream.WriteByte(chr5);
            stream.WriteByte(chr6);
            stream.WriteByte(chr7);
            stream.WriteByte(irq_enable);
            stream.WriteByte(irq_counter);
            stream.WriteByte(irq_latch);
            stream.Write(protect_dat, 0, protect_dat.Length);
        }
        public void LoadState(System.IO.Stream stream)
        {
            stream.Read(reg, 0, reg.Length);
            chr01 = (byte)stream.ReadByte();
            chr23 = (byte)stream.ReadByte();
            chr4 = (byte)stream.ReadByte();
            chr5 = (byte)stream.ReadByte();
            chr6 = (byte)stream.ReadByte();
            chr7 = (byte)stream.ReadByte();
            irq_enable = (byte)stream.ReadByte();
            irq_counter = (byte)stream.ReadByte();
            irq_latch = (byte)stream.ReadByte();
            stream.Read(protect_dat, 0, protect_dat.Length);
        }
    }
}

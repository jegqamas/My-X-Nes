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
    class Mapper249 : IMapper
    {
        CPUMemory map;
        byte[] reg = new byte[8];
        byte prg0 = 0;
        byte prg1 = 1;
        byte chr01 = 0;
        byte chr23 = 2;
        byte chr4 = 4;
        byte chr5 = 5;
        byte chr6 = 6;
        byte chr7 = 7;
        byte we_sram = 0;
        byte irq_enable = 0;
        byte irq_counter = 0;
        byte irq_latch = 0;
        byte irq_request = 0;
        byte spdata = 0;
        public Mapper249(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            if (Address == 0x5000)
            {
                switch (data)
                {
                    case 0x00:
                        spdata = 0;
                        break;
                    case 0x02:
                        spdata = 1;
                        break;
                }
            }
            if (Address >= 0x8000)
            {
                int m0, m1, m2, m3, m4, m5, m6, m7;

                switch (Address & 0xFF01)
                {
                    case 0x8000:
                    case 0x8800:
                        reg[0] = data;
                        break;
                    case 0x8001:
                    case 0x8801:
                        switch (reg[0] & 0x07)
                        {
                            case 0x00:
                                if (spdata == 1)
                                {
                                    m0 = data & 0x1;
                                    m1 = (data & 0x02) >> 1;
                                    m2 = (data & 0x04) >> 2;
                                    m3 = (data & 0x08) >> 3;
                                    m4 = (data & 0x10) >> 4;
                                    m5 = (data & 0x20) >> 5;
                                    m6 = (data & 0x40) >> 6;
                                    m7 = (data & 0x80) >> 7;
                                    data = (byte)((m5 << 7) | (m4 << 6) | (m2 << 5) | (m6 << 4) | (m7 << 3) | (m3 << 2) | (m1 << 1) | m0);
                                }
                                map.Switch1kChrRom(data & 0xFE, 0);
                                map.Switch1kChrRom(data | 0x01, 1);
                                break;
                            case 0x01:
                                if (spdata == 1)
                                {
                                    m0 = data & 0x1;
                                    m1 = (data & 0x02) >> 1;
                                    m2 = (data & 0x04) >> 2;
                                    m3 = (data & 0x08) >> 3;
                                    m4 = (data & 0x10) >> 4;
                                    m5 = (data & 0x20) >> 5;
                                    m6 = (data & 0x40) >> 6;
                                    m7 = (data & 0x80) >> 7;
                                    data = (byte)((m5 << 7) | (m4 << 6) | (m2 << 5) | (m6 << 4) | (m7 << 3) | (m3 << 2) | (m1 << 1) | m0);
                                }
                                map.Switch1kChrRom(data & 0xFE, 2);
                                map.Switch1kChrRom(data | 0x01, 3);
                                break;
                            case 0x02:
                                if (spdata == 1)
                                {
                                    m0 = data & 0x1;
                                    m1 = (data & 0x02) >> 1;
                                    m2 = (data & 0x04) >> 2;
                                    m3 = (data & 0x08) >> 3;
                                    m4 = (data & 0x10) >> 4;
                                    m5 = (data & 0x20) >> 5;
                                    m6 = (data & 0x40) >> 6;
                                    m7 = (data & 0x80) >> 7;
                                    data = (byte)((m5 << 7) | (m4 << 6) | (m2 << 5) | (m6 << 4) | (m7 << 3) | (m3 << 2) | (m1 << 1) | m0);
                                }
                                map.Switch1kChrRom(data, 4);
                                break;
                            case 0x03:
                                if (spdata == 1)
                                {
                                    m0 = data & 0x1;
                                    m1 = (data & 0x02) >> 1;
                                    m2 = (data & 0x04) >> 2;
                                    m3 = (data & 0x08) >> 3;
                                    m4 = (data & 0x10) >> 4;
                                    m5 = (data & 0x20) >> 5;
                                    m6 = (data & 0x40) >> 6;
                                    m7 = (data & 0x80) >> 7;
                                    data = (byte)((m5 << 7) | (m4 << 6) | (m2 << 5) | (m6 << 4) | (m7 << 3) | (m3 << 2) | (m1 << 1) | m0);
                                }
                                map.Switch1kChrRom(data, 5);
                                break;
                            case 0x04:
                                if (spdata == 1)
                                {
                                    m0 = data & 0x1;
                                    m1 = (data & 0x02) >> 1;
                                    m2 = (data & 0x04) >> 2;
                                    m3 = (data & 0x08) >> 3;
                                    m4 = (data & 0x10) >> 4;
                                    m5 = (data & 0x20) >> 5;
                                    m6 = (data & 0x40) >> 6;
                                    m7 = (data & 0x80) >> 7;
                                    data = (byte)((m5 << 7) | (m4 << 6) | (m2 << 5) | (m6 << 4) | (m7 << 3) | (m3 << 2) | (m1 << 1) | m0);
                                }
                                map.Switch1kChrRom(data, 6);
                                break;
                            case 0x05:
                                if (spdata == 1)
                                {
                                    m0 = data & 0x1;
                                    m1 = (data & 0x02) >> 1;
                                    m2 = (data & 0x04) >> 2;
                                    m3 = (data & 0x08) >> 3;
                                    m4 = (data & 0x10) >> 4;
                                    m5 = (data & 0x20) >> 5;
                                    m6 = (data & 0x40) >> 6;
                                    m7 = (data & 0x80) >> 7;
                                    data = (byte)((m5 << 7) | (m4 << 6) | (m2 << 5) | (m6 << 4) | (m7 << 3) | (m3 << 2) | (m1 << 1) | m0);
                                }
                                map.Switch1kChrRom(data, 7);
                                break;
                            case 0x06:
                                if (spdata == 1)
                                {
                                    if (data < 0x20)
                                    {
                                        m0 = data & 0x1;
                                        m1 = (data & 0x02) >> 1;
                                        m2 = (data & 0x04) >> 2;
                                        m3 = (data & 0x08) >> 3;
                                        m4 = (data & 0x10) >> 4;
                                        m5 = 0;
                                        m6 = 0;
                                        m7 = 0;
                                        data = (byte)((m7 << 7) | (m6 << 6) | (m5 << 5) | (m2 << 4) | (m1 << 3) | (m3 << 2) | (m4 << 1) | m0);
                                    }
                                    else
                                    {
                                        data -= 0x20;
                                        m0 = data & 0x1;
                                        m1 = (data & 0x02) >> 1;
                                        m2 = (data & 0x04) >> 2;
                                        m3 = (data & 0x08) >> 3;
                                        m4 = (data & 0x10) >> 4;
                                        m5 = (data & 0x20) >> 5;
                                        m6 = (data & 0x40) >> 6;
                                        m7 = (data & 0x80) >> 7;
                                        data = (byte)((m5 << 7) | (m4 << 6) | (m2 << 5) | (m6 << 4) | (m7 << 3) | (m3 << 2) | (m1 << 1) | m0);
                                    }
                                }
                                map.Switch8kPrgRom(data * 2, 0);
                                break;
                            case 0x07:
                                if (spdata == 1)
                                {
                                    if (data < 0x20)
                                    {
                                        m0 = data & 0x1;
                                        m1 = (data & 0x02) >> 1;
                                        m2 = (data & 0x04) >> 2;
                                        m3 = (data & 0x08) >> 3;
                                        m4 = (data & 0x10) >> 4;
                                        m5 = 0;
                                        m6 = 0;
                                        m7 = 0;
                                        data = (byte)((m7 << 7) | (m6 << 6) | (m5 << 5) | (m2 << 4) | (m1 << 3) | (m3 << 2) | (m4 << 1) | m0);
                                    }
                                    else
                                    {
                                        data -= 0x20;
                                        m0 = data & 0x1;
                                        m1 = (data & 0x02) >> 1;
                                        m2 = (data & 0x04) >> 2;
                                        m3 = (data & 0x08) >> 3;
                                        m4 = (data & 0x10) >> 4;
                                        m5 = (data & 0x20) >> 5;
                                        m6 = (data & 0x40) >> 6;
                                        m7 = (data & 0x80) >> 7;
                                        data = (byte)((m5 << 7) | (m4 << 6) | (m2 << 5) | (m6 << 4) | (m7 << 3) | (m3 << 2) | (m1 << 1) | m0);
                                    }
                                }
                                map.Switch8kPrgRom(data * 2, 1);
                                break;
                        }
                        break;
                    case 0xA000:
                    case 0xA800:
                        reg[2] = data;

                        if ((data & 0x01) == 0x01)
                            map.cartridge.Mirroring = Mirroring.Horizontal;
                        else
                            map.cartridge.Mirroring = Mirroring.Vertical;
                        break;
                    case 0xA001:
                    case 0xA801:
                        reg[3] = data;
                        break;
                    case 0xC000:
                    case 0xC800:
                        reg[4] = data;
                        irq_counter = data;
                        irq_request = 0;
                        map.cpu.SetIRQ(false);
                        break;
                    case 0xC001:
                    case 0xC801:
                        reg[5] = data;
                        irq_latch = data;
                        irq_request = 0;
                        map.cpu.SetIRQ(false);
                        break;
                    case 0xE000:
                    case 0xE800:
                        reg[6] = data;
                        irq_enable = 0;
                        irq_request = 0;
                        map.cpu.SetIRQ(false);
                        break;
                    case 0xE001:
                    case 0xE801:
                        reg[7] = data;
                        irq_enable = 1;
                        irq_request = 0;
                        map.cpu.SetIRQ(false);
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
        public void TickScanlineTimer()
        {
            if (irq_enable != 0 && irq_request == 0)
            {
                if (map.ppu.ScanLine == map.ppu.ScanlineOfEndOfVblank)
                {
                    if (irq_counter != 0)
                    {
                        irq_counter--;
                    }
                }
                if ((irq_counter--) == 0)
                {
                    irq_request = 0xFF;
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
            stream.WriteByte(prg0);
            stream.WriteByte(prg1);
            stream.WriteByte(chr01);
            stream.WriteByte(chr23);
            stream.WriteByte(chr4);
            stream.WriteByte(chr5);
            stream.WriteByte(chr6);
            stream.WriteByte(chr7);
            stream.WriteByte(we_sram);
            stream.WriteByte(irq_enable);
            stream.WriteByte(irq_counter);
            stream.WriteByte(irq_latch);
            stream.WriteByte(irq_request);
            stream.WriteByte(spdata);
        }
        public void LoadState(System.IO.Stream stream)
        {
            stream.Read(reg, 0, reg.Length);
            prg0 = (byte)stream.ReadByte();
            prg1 = (byte)stream.ReadByte();
            chr01 = (byte)stream.ReadByte();
            chr23 = (byte)stream.ReadByte();
            chr4 = (byte)stream.ReadByte();
            chr5 = (byte)stream.ReadByte();
            chr6 = (byte)stream.ReadByte();
            chr7 = (byte)stream.ReadByte();
            we_sram = (byte)stream.ReadByte();
            irq_enable = (byte)stream.ReadByte();
            irq_counter = (byte)stream.ReadByte();
            irq_latch = (byte)stream.ReadByte();
            irq_request = (byte)stream.ReadByte();
            spdata = (byte)stream.ReadByte();
        }
    }
}

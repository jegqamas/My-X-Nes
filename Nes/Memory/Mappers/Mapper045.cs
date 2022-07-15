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

    class Mapper45 : IMapper
    {
        CPUMemory map;
        byte[] reg = new byte[8];
        byte[] p = new byte[4];
        int[] c = new int[8];
        int mode = 0;
        byte prg0 = 0;
        byte prg1 = 1;
        byte prg2 = 0;
        byte prg3 = 0;
        int irq_enable = 0;
        int irq_counter = 0;
        int irq_latch = 0;
        int irq_latched = 0;
        byte irq_reset = 0;
        byte chr0 = 0;
        byte chr1 = 1;
        byte chr2 = 2;
        byte chr3 = 3;
        byte chr4 = 4;
        byte chr5 = 5;
        byte chr6 = 6;
        byte chr7 = 7;

        public Mapper45(CPUMemory MAP)
        { map = MAP; }
        public void Write(ushort address, byte data)
        {
            if (address < 0x8000)
            {
                if ((reg[3] & 0x40) == 0)
                {
                    reg[reg[5]] = data;
                    reg[5] = (byte)((reg[5] + 1) & 0x03);

                    SetBank_CPU_4(prg0);
                    SetBank_CPU_5(prg1);
                    SetBank_CPU_6(prg2);
                    SetBank_CPU_7(prg3);
                    SetBank_PPU();
                }
            }
            else
            {
                switch (address & 0xE001)
                {
                    case 0x8000:
                        if ((data & 0x40) != (reg[6] & 0x40))
                        {
                            byte swp;
                            swp = prg0; prg0 = prg2; prg2 = swp;
                            swp = p[0]; p[0] = p[2]; p[2] = swp;
                            SetBank_CPU_4(p[0]);
                            SetBank_CPU_5(p[1]);
                        }
                        if (!map.cartridge.IsVRAM)
                        {
                            if ((data & 0x80) != (reg[6] & 0x80))
                            {
                                byte swp;
                                swp = chr4; chr4 = chr0; chr0 = swp;
                                swp = chr5; chr5 = chr1; chr1 = swp;
                                swp = chr6; chr6 = chr2; chr2 = swp;
                                swp = chr7; chr7 = chr3; chr3 = swp;
                                swp = (byte)c[4]; c[4] = c[0]; c[0] = swp;
                                swp = (byte)c[5]; c[5] = c[1]; c[1] = swp;
                                swp = (byte)c[6]; c[6] = c[2]; c[2] = swp;
                                swp = (byte)c[7]; c[7] = c[3]; c[3] = swp;
                                map.Switch1kChrRom(c[0], 0);
                                map.Switch1kChrRom(c[1], 1);
                                map.Switch1kChrRom(c[2], 2);
                                map.Switch1kChrRom(c[3], 3);
                                map.Switch1kChrRom(c[4], 4);
                                map.Switch1kChrRom(c[5], 5);
                                map.Switch1kChrRom(c[6], 6);
                                map.Switch1kChrRom(c[7], 7);
                            }
                        }
                        reg[6] = data;
                        break;
                    case 0x8001:
                        switch (reg[6] & 0x07)
                        {
                            case 0x00:
                                chr0 = (byte)((data & 0xFE) + 0);
                                chr1 = (byte)((data & 0xFE) + 1);
                                SetBank_PPU();
                                break;
                            case 0x01:
                                chr2 = (byte)((data & 0xFE) + 0);
                                chr3 = (byte)((data & 0xFE) + 1);
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
                            case 0x06:
                                if ((reg[6] & 0x40) == 0x40)
                                {
                                    prg2 = (byte)(data & 0x3F);
                                    SetBank_CPU_6(data);
                                }
                                else
                                {
                                    prg0 = (byte)(data & 0x3F);
                                    SetBank_CPU_4(data);
                                }
                                break;
                            case 0x07:
                                prg1 = (byte)(data & 0x3F);
                                SetBank_CPU_5(data);
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
                        if (mode == 2)
                        {
                            if (data == 0x29 || data == 0x70)
                                data = 0x07;
                        }
                        irq_latch = data;
                        irq_latched = 1;
                        if (irq_reset > 0)
                        {
                            irq_counter = data;
                            irq_latched = 0;
                        }
                        //			irq_counter = data;
                        break;
                    case 0xC001:
                        //			irq_latch = data;
                        irq_counter = irq_latch;
                        break;
                    case 0xE000:
                        irq_enable = 0;
                        irq_reset = 1;

                        break;
                    case 0xE001:
                        irq_enable = 1;
                        if (irq_latched > 0)
                        {
                            irq_counter = irq_latch;
                        }
                        break;
                }
            }
        }
        public byte Read(ushort Address)
        {
            return 0;
        }
        public void SetUpMapperDefaults()
        {
            prg2 = (byte)((map.cartridge.PRG_PAGES * 2) - 2);
            prg3 = (byte)((map.cartridge.PRG_PAGES * 2) - 1);
            string name = map.cartridge.RomPath;

            if (name.Contains("Kunio 8-in-1"))
            {
                mode = 1;
                prg2 = 62;
                prg3 = 63;
            }

            if (name.Contains("HIK 7-in-1"))
            {
                mode = 1;
                prg2 = 62;
                prg3 = 63;
            }

            if (name.Contains("Super 8-in-1"))
            {
                mode = 1;
                prg2 = 62;
                prg3 = 63;
            }

            if (name.Contains("Super 3-in-1"))
            {
                mode = 2;
            }

            map.Switch16kPrgRom(0, 0);
            map.Switch8kPrgRom(prg2 * 2, 2);
            map.Switch8kPrgRom(prg3 * 2, 3);
            p[0] = prg0;
            p[1] = prg1;
            p[2] = prg2;
            p[3] = prg3;

            c[0] = 0;
            c[1] = 1;
            c[2] = 2;
            c[3] = 3;
            c[4] = 4;
            c[5] = 5;
            c[6] = 6;
            c[7] = 7;
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
            irq_reset = 0;

            if (irq_counter > 0)
            {
                irq_counter--;
                if (irq_counter == 0)
                {
                    if (irq_enable > 0)
                    {
                        map.cpu.SetIRQ(true);
                    }
                }
            }
        }
        public void TickCycleTimer(int cycles)
        {

        }
        public void SoftReset()
        {

        }
        public bool WriteUnder8000
        {
            get { return true; }
        }
        public bool WriteUnder6000
        {
            get { return true; }
        }
        public bool ScanlineTimerNotPauseAtVBLANK
        {
            get { return false; }
        }
        void SetBank_CPU_4(byte data)
        {
            data &= (byte)((reg[3] & 0x3F) ^ 0xFF);
            data &= 0x3F;
            data |= reg[1];
            map.Switch8kPrgRom(data * 2, 0);
            p[0] = data;
        }
        void SetBank_CPU_5(byte data)
        {
            data &= (byte)((reg[3] & 0x3F) ^ 0xFF);
            data &= 0x3F;
            data |= reg[1];
            map.Switch8kPrgRom(data * 2, 1);
            p[1] = data;
        }
        void SetBank_CPU_6(byte data)
        {
            data &= (byte)((reg[3] & 0x3F) ^ 0xFF);
            data &= 0x3F;
            data |= reg[1];
            map.Switch8kPrgRom(data * 2, 2);
            p[2] = data;
        }
        void SetBank_CPU_7(byte data)
        {
            data &= (byte)((reg[3] & 0x3F) ^ 0xFF);
            data &= 0x3F;
            data |= reg[1];
            map.Switch8kPrgRom(data * 2, 3);
            p[3] = data;
        }
        void SetBank_PPU()
        {
            byte[] table = {
		0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
		0x01,0x03,0x07,0x0F,0x1F,0x3F,0x7F,0xFF};

            c[0] = chr0;
            c[1] = chr1;
            c[2] = chr2;
            c[3] = chr3;
            c[4] = chr4;
            c[5] = chr5;
            c[6] = chr6;
            c[7] = chr7;

            for (int i = 0; i < 8; i++)
            {
                c[i] &= table[reg[2] & 0x0F];
                c[i] |= (reg[0] & ((mode != 1) ? 0xFF : 0xC0));
                c[i] += ((reg[2] & ((mode != 1) ? 0x10 : 0x30)) << 4);
            }

            if ((reg[6] & 0x80) == 0x80)
            {
                map.Switch1kChrRom(c[4], 0);
                map.Switch1kChrRom(c[5], 1);
                map.Switch1kChrRom(c[6], 2);
                map.Switch1kChrRom(c[7], 3);
                map.Switch1kChrRom(c[0], 4);
                map.Switch1kChrRom(c[1], 5);
                map.Switch1kChrRom(c[2], 6);
                map.Switch1kChrRom(c[3], 7);
            }
            else
            {
                map.Switch1kChrRom(c[0], 0);
                map.Switch1kChrRom(c[1], 1);
                map.Switch1kChrRom(c[2], 2);
                map.Switch1kChrRom(c[3], 3);
                map.Switch1kChrRom(c[4], 4);
                map.Switch1kChrRom(c[5], 5);
                map.Switch1kChrRom(c[6], 6);
                map.Switch1kChrRom(c[7], 7);
            }
        }

        public void SaveState(System.IO.Stream stream)
        {
            stream.Write(reg, 0, reg.Length);
            stream.Write(p, 0, p.Length);
            for (int i = 0; i < c.Length; i++)
            {
                stream.WriteByte((byte)((c[i] & 0xFF000000) >> 24));
                stream.WriteByte((byte)((c[i] & 0x00FF0000) >> 16));
                stream.WriteByte((byte)((c[i] & 0x0000FF00) >> 8));
                stream.WriteByte((byte)((c[i] & 0x000000FF)));
            }
            stream.WriteByte((byte)((mode & 0xFF000000) >> 24));
            stream.WriteByte((byte)((mode & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((mode & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((mode & 0x000000FF)));

            stream.WriteByte(prg0);
            stream.WriteByte(prg1);
            stream.WriteByte(prg2);
            stream.WriteByte(prg3);

            stream.WriteByte((byte)((irq_enable & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_enable & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_enable & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_enable & 0x000000FF)));

            stream.WriteByte((byte)((irq_counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_counter & 0x000000FF)));

            stream.WriteByte((byte)((irq_latch & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_latch & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_latch & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_latch & 0x000000FF)));

            stream.WriteByte((byte)((irq_latched & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_latched & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_latched & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_latched & 0x000000FF)));

            stream.WriteByte(irq_reset);
            stream.WriteByte(chr0);
            stream.WriteByte(chr1);
            stream.WriteByte(chr2);
            stream.WriteByte(chr3);
            stream.WriteByte(chr4);
            stream.WriteByte(chr5);
            stream.WriteByte(chr6);
            stream.WriteByte(chr7);
        }
        public void LoadState(System.IO.Stream stream)
        {
            stream.Read(reg, 0, reg.Length);
            stream.Read(p, 0, p.Length);
            for (int i = 0; i < c.Length; i++)
            {
                c[i] = (int)(stream.ReadByte() << 24);
                c[i] |= (int)(stream.ReadByte() << 16);
                c[i] |= (int)(stream.ReadByte() << 8);
                c[i] |= stream.ReadByte();
            }
            mode = (int)(stream.ReadByte() << 24);
            mode |= (int)(stream.ReadByte() << 16);
            mode |= (int)(stream.ReadByte() << 8);
            mode |= stream.ReadByte();

            prg0 = (byte)stream.ReadByte();
            prg1 = (byte)stream.ReadByte();
            prg2 = (byte)stream.ReadByte();
            prg3 = (byte)stream.ReadByte();

            irq_enable = (int)(stream.ReadByte() << 24);
            irq_enable |= (int)(stream.ReadByte() << 16);
            irq_enable |= (int)(stream.ReadByte() << 8);
            irq_enable |= stream.ReadByte();

            irq_counter = (int)(stream.ReadByte() << 24);
            irq_counter |= (int)(stream.ReadByte() << 16);
            irq_counter |= (int)(stream.ReadByte() << 8);
            irq_counter |= stream.ReadByte();

            irq_latch = (int)(stream.ReadByte() << 24);
            irq_latch |= (int)(stream.ReadByte() << 16);
            irq_latch |= (int)(stream.ReadByte() << 8);
            irq_latch |= stream.ReadByte();

            irq_latched = (int)(stream.ReadByte() << 24);
            irq_latched |= (int)(stream.ReadByte() << 16);
            irq_latched |= (int)(stream.ReadByte() << 8);
            irq_latched |= stream.ReadByte();

            irq_reset = (byte)stream.ReadByte();
            chr0 = (byte)stream.ReadByte();
            chr1 = (byte)stream.ReadByte();
            chr2 = (byte)stream.ReadByte();
            chr3 = (byte)stream.ReadByte();
            chr4 = (byte)stream.ReadByte();
            chr5 = (byte)stream.ReadByte();
            chr6 = (byte)stream.ReadByte();
            chr7 = (byte)stream.ReadByte();
        }
    }
}

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

    class Mapper187 : IMapper
    {
        int[] chr = new int[8];
        byte[] bank = new byte[8];
        byte[] prg = new byte[4];
        byte ext_mode = 0;
        byte chr_mode = 0;
        byte ext_enable = 0;

        byte irq_enable = 0;
        byte irq_counter = 0;
        byte irq_latch = 0;
        byte irq_occur = 0;

        byte last_write = 0;

        CPUMemory map;
        public Mapper187(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            if (Address < 0x8000)
            {
                last_write = data;
                if (Address == 0x5000)
                {
                    ext_mode = data;
                    if ((data & 0x80) == 0x80)
                    {
                        if ((data & 0x20) == 0x20)
                        {
                            prg[0] = (byte)(((data & 0x1E) << 1) + 0);
                            prg[1] = (byte)(((data & 0x1E) << 1) + 1);
                            prg[2] = (byte)(((data & 0x1E) << 1) + 2);
                            prg[3] = (byte)(((data & 0x1E) << 1) + 3);
                        }
                        else
                        {
                            prg[2] = (byte)(((data & 0x1F) << 1) + 0);
                            prg[3] = (byte)(((data & 0x1F) << 1) + 1);
                        }
                    }
                    else
                    {
                        prg[0] = bank[6];
                        prg[1] = bank[7];
                        prg[2] = (byte)(map.cartridge.PRGSizeInKB - 2);
                        prg[3] = (byte)(map.cartridge.PRGSizeInKB - 1);
                    }
                    SetBank_CPU();
                }
            }
            else
            {
                last_write = data;
                switch (Address)
                {
                    case 0x8003:
                        ext_enable = 0xFF;
                        chr_mode = data;
                        if ((data & 0xF0) == 0)
                        {
                            prg[2] = (byte)(map.cartridge.PRGSizeInKB - 2);
                            SetBank_CPU();
                        }
                        break;

                    case 0x8000:
                        ext_enable = 0;
                        chr_mode = data;
                        break;

                    case 0x8001:
                        if (ext_enable == 0)
                        {
                            switch (chr_mode & 7)
                            {
                                case 0:
                                    data &= 0xFE;
                                    chr[4] = data + 0x100;
                                    chr[5] = data + 0x100 + 1;
                                    SetBank_PPU();
                                    break;
                                case 1:
                                    data &= 0xFE;
                                    chr[6] = data + 0x100;
                                    chr[7] = data + 0x100 + 1;
                                    SetBank_PPU();
                                    break;
                                case 2:
                                    chr[0] = data;
                                    SetBank_PPU();
                                    break;
                                case 3:
                                    chr[1] = data;
                                    SetBank_PPU();
                                    break;
                                case 4:
                                    chr[2] = data;
                                    SetBank_PPU();
                                    break;
                                case 5:
                                    chr[3] = data;
                                    SetBank_PPU();
                                    break;
                                case 6:
                                    if ((ext_mode & 0xA0) != 0xA0)
                                    {
                                        prg[0] = data;
                                        SetBank_CPU();
                                    }
                                    break;
                                case 7:
                                    if ((ext_mode & 0xA0) != 0xA0)
                                    {
                                        prg[1] = data;
                                        SetBank_CPU();
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            switch (chr_mode)
                            {
                                case 0x2A:
                                    prg[1] = 0x0F;
                                    break;
                                case 0x28:
                                    prg[2] = 0x17;
                                    break;
                                case 0x26:
                                    break;
                                default:
                                    break;
                            }
                            SetBank_CPU();
                        }
                        bank[chr_mode & 7] = data;
                        break;

                    case 0xA000:
                        if ((data & 0x01) == 0x01)
                        {
                            map.cartridge.Mirroring = Mirroring.Horizontal;
                        }
                        else
                        {
                            map.cartridge.Mirroring = Mirroring.Vertical;
                        }
                        break;
                    case 0xA001:
                        break;

                    case 0xC000:
                        irq_counter = data;
                        irq_occur = 0;
                        map.cpu.SetIRQ(false);
                        break;
                    case 0xC001:
                        irq_latch = data;
                        irq_occur = 0;
                        map.cpu.SetIRQ(false);
                        break;
                    case 0xE000:
                    case 0xE002:
                        irq_enable = 0;
                        irq_occur = 0;
                        map.cpu.SetIRQ(false);
                        break;
                    case 0xE001:
                    case 0xE003:
                        irq_enable = 1;
                        irq_occur = 0;
                        map.cpu.SetIRQ(false);
                        break;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            map.Switch32kPrgRom((map.cartridge.PRG_PAGES - 1) * 4 - 4);
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
            prg[0] = (byte)(map.cartridge.PRGSizeInKB - 4);
            prg[1] = (byte)(map.cartridge.PRGSizeInKB - 3);
            prg[2] = (byte)(map.cartridge.PRGSizeInKB - 2);
            prg[3] = (byte)(map.cartridge.PRGSizeInKB - 1);
            //SetBank_CPU();
        }
        void SetBank_CPU()
        {
            map.Switch8kPrgRom(prg[0] * 2, 0);
            map.Switch8kPrgRom(prg[1] * 2, 1);
            map.Switch8kPrgRom(prg[2] * 2, 2);
            map.Switch8kPrgRom(prg[3] * 2, 3);
        }
        void SetBank_PPU()
        {
            map.Switch1kChrRom(chr[0], 0);
            map.Switch1kChrRom(chr[1], 1);
            map.Switch1kChrRom(chr[2], 2);
            map.Switch1kChrRom(chr[3], 3);
            map.Switch1kChrRom(chr[4], 4);
            map.Switch1kChrRom(chr[5], 5);
            map.Switch1kChrRom(chr[6], 6);
            map.Switch1kChrRom(chr[7], 7);
        }
        public void TickScanlineTimer()
        {

            if (irq_enable != 0)
            {
                if (irq_counter == 0)
                {
                    irq_counter--;
                    irq_enable = 0;
                    irq_occur = 0xFF;
                    map.cpu.SetIRQ(true);
                }
                else
                {
                    irq_counter--;
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
            if (Address < 0x8000)
            {
                switch (last_write & 0x03)
                {
                    case 0:
                        return 0x83;
                    case 1:
                        return 0x83;
                    case 2:
                        return 0x42;
                    case 3:
                        return 0x00;
                }
            }
            return 0;
        }
        public bool ScanlineTimerNotPauseAtVBLANK
        {
            get { return false; }
        }

        public void SaveState(System.IO.Stream stream)
        {
            for (int i = 0; i < chr.Length; i++)
            {
                stream.WriteByte((byte)((chr[i] & 0xFF000000) >> 24));
                stream.WriteByte((byte)((chr[i] & 0x00FF0000) >> 16));
                stream.WriteByte((byte)((chr[i] & 0x0000FF00) >> 8));
                stream.WriteByte((byte)((chr[i] & 0x000000FF)));
            }
            stream.Write(bank, 0, bank.Length);
            stream.Write(prg, 0, prg.Length);
            stream.WriteByte(ext_mode);
            stream.WriteByte(chr_mode);
            stream.WriteByte(ext_enable);
            stream.WriteByte(irq_enable);
            stream.WriteByte(irq_counter);
            stream.WriteByte(irq_latch);
            stream.WriteByte(irq_occur);
            stream.WriteByte(last_write);
        }
        public void LoadState(System.IO.Stream stream)
        {
            for (int i = 0; i < chr.Length; i++)
            {
                chr[i] = (int)(stream.ReadByte() << 24);
                chr[i] |= (int)(stream.ReadByte() << 16);
                chr[i] |= (int)(stream.ReadByte() << 8);
                chr[i] |= stream.ReadByte();
            }
            stream.Read(bank, 0, bank.Length);
            stream.Read(prg, 0, prg.Length);
            ext_mode = (byte)stream.ReadByte();
            chr_mode = (byte)stream.ReadByte();
            ext_enable = (byte)stream.ReadByte();
            irq_enable = (byte)stream.ReadByte();
            irq_counter = (byte)stream.ReadByte();
            irq_latch = (byte)stream.ReadByte();
            irq_occur = (byte)stream.ReadByte();
            last_write = (byte)stream.ReadByte();
        }
    }
}

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

    class Mapper115 : IMapper
    {
        byte[] reg = new byte[8];
        byte prg0 = 0;
        byte prg0L = 0;
        byte prg1 = 0;
        byte prg1L = 0;
        byte prg2 = 0;
        byte prg3 = 0;
        byte ExPrgSwitch = 0;
        byte ExChrSwitch = 0;
        byte chr0 = 0;
        byte chr1 = 1;
        byte chr2 = 2;
        byte chr3 = 3;
        byte chr4 = 4;
        byte chr5 = 5;
        byte chr6 = 6;
        byte chr7 = 7;
        byte irq_enable = 0;
        byte irq_counter = 0;
        byte irq_latch = 0;
        CPUMemory map;
        public Mapper115(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            if (Address < 0x8000)
            {
                switch (Address)
                {
                    case 0x6000:
                        ExPrgSwitch = data; //data
                        SetBank_CPU();
                        break;
                    case 0x6001:
                        ExChrSwitch = (byte)(data & 0x1);
                        SetBank_PPU();
                        break;
                }
            }
            else
            {
                switch (Address & 0xE001)
                {
                    case 0x8000:
                        reg[0] = data;
                        SetBank_CPU();
                        SetBank_PPU();
                        break;
                    case 0x8001:
                        reg[1] = data;
                        switch (reg[0] & 0x07)
                        {
                            case 0x00:
                                chr0 = (byte)(data & 0xFE);
                                chr1 = (byte)(chr0 + 1);
                                SetBank_PPU();
                                break;
                            case 0x01:
                                chr2 = (byte)(data & 0xFE);
                                chr3 = (byte)(chr2 + 1);
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
                                prg0 = prg0L = data;
                                SetBank_CPU();
                                break;
                            case 0x07:
                                prg1 = prg1L = data;
                                SetBank_CPU();
                                break;
                        }
                        break;
                    case 0xA000:
                        reg[2] = data;
                        if (map.cartridge.Mirroring != Mirroring.Four_Screen)
                        {
                            if ((data & 0x01) == 0x01)
                                map.cartridge.Mirroring = Mirroring.Horizontal;
                            else
                                map.cartridge.Mirroring = Mirroring.Vertical;
                        }
                        break;
                    case 0xA001:
                        reg[3] = data;
                        break;
                    case 0xC000:
                        reg[4] = data;
                        irq_counter = data;
                        irq_enable = 0xFF;
                        break;
                    case 0xC001:
                        reg[5] = data;
                        irq_latch = data;
                        break;
                    case 0xE000:
                        reg[6] = data;
                        irq_enable = 0;
                        map.cpu.SetIRQ(false);
                        break;
                    case 0xE001:
                        reg[7] = data;
                        irq_enable = 0xFF;
                        break;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            prg0 = prg0L = 0;
            prg1 = prg1L = 1;
            prg2 = (byte)((map.cartridge.PRG_PAGES * 2) - 2);
            prg3 = (byte)((map.cartridge.PRG_PAGES * 2) - 1);

            ExPrgSwitch = 0;
            ExChrSwitch = 0;
            if (!map.cartridge.IsVRAM)
            {
                chr0 = 0;
                chr1 = 1;
                chr2 = 2;
                chr3 = 3;
                chr4 = 4;
                chr5 = 5;
                chr6 = 6;
                chr7 = 7;
            }
            else
            {
                chr0 = chr2 = chr4 = chr5 = chr6 = chr7 = 0;
                chr1 = chr3 = 1;
            }
            SetBank_PPU();
            SetBank_CPU();
        }
        public void TickScanlineTimer()
        {
            if (irq_enable != 0)
            {
                if ((irq_counter--) == 0)
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
        void SetBank_CPU()
        {
            if ((ExPrgSwitch & 0x80) == 0x80)
            {
                prg0 = (byte)((ExPrgSwitch << 1) & 0x1e);
                prg1 = (byte)(prg0 + 1);

                map.Switch8kPrgRom(prg0 * 2, 0);
                map.Switch8kPrgRom(prg1 * 2, 1);
                map.Switch8kPrgRom((prg0 + 2) * 2, 2);
                map.Switch8kPrgRom((prg1 + 2) * 2, 3);
            }
            else
            {
                prg0 = prg0L;
                prg1 = prg1L;
                if ((reg[0] & 0x40) == 0x40)
                {
                    map.Switch8kPrgRom(((map.cartridge.PRG_PAGES * 2) - 2) * 2, 0);
                    map.Switch8kPrgRom(prg1 * 2, 1);
                    map.Switch8kPrgRom(prg0 * 2, 2);
                    map.Switch8kPrgRom(((map.cartridge.PRG_PAGES * 2) - 1) * 2, 3);
                }
                else
                {
                    map.Switch8kPrgRom(prg0 * 2, 0);
                    map.Switch8kPrgRom(prg1 * 2, 1);
                    map.Switch8kPrgRom(((map.cartridge.PRG_PAGES * 2) - 2) * 2, 2);
                    map.Switch8kPrgRom(((map.cartridge.PRG_PAGES * 2) - 1) * 2, 3);
                }
            }
        }
        void SetBank_PPU()
        {
            if (!map.cartridge.IsVRAM)
            {
                if ((reg[0] & 0x80) == 0x80)
                {
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr4, 0);
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr5, 1);
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr6, 2);
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr7, 3);
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr0, 4);
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr1, 5);
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr2, 6);
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr3, 7);
                }
                else
                {
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr0, 0);
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr1, 1);
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr2, 2);
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr3, 3);
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr4, 4);
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr5, 5);
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr6, 6);
                    map.Switch1kChrRom((ExChrSwitch << 8) + chr7, 7);
                }
            }
        }

        public void SaveState(System.IO.Stream stream)
        {
            stream.Write(reg, 0, reg.Length);
            stream.WriteByte(prg0);
            stream.WriteByte(prg0L);
            stream.WriteByte(prg1);
            stream.WriteByte(prg1L);
            stream.WriteByte(prg2);
            stream.WriteByte(prg3);
            stream.WriteByte(ExPrgSwitch);
            stream.WriteByte(ExChrSwitch);
            stream.WriteByte(chr0);
            stream.WriteByte(chr1);
            stream.WriteByte(chr2);
            stream.WriteByte(chr3);
            stream.WriteByte(chr4);
            stream.WriteByte(chr5);
            stream.WriteByte(chr6);
            stream.WriteByte(chr7);
            stream.WriteByte(irq_enable);
            stream.WriteByte(irq_counter);
            stream.WriteByte(irq_latch);
        }
        public void LoadState(System.IO.Stream stream)
        {
            stream.Read(reg, 0, reg.Length);
            prg0 = (byte)stream.ReadByte();
            prg0L = (byte)stream.ReadByte();
            prg1 = (byte)stream.ReadByte();
            prg1L = (byte)stream.ReadByte();
            prg2 = (byte)stream.ReadByte();
            prg3 = (byte)stream.ReadByte();
            ExPrgSwitch = (byte)stream.ReadByte();
            ExChrSwitch = (byte)stream.ReadByte();
            chr0 = (byte)stream.ReadByte();
            chr1 = (byte)stream.ReadByte();
            chr2 = (byte)stream.ReadByte();
            chr3 = (byte)stream.ReadByte();
            chr4 = (byte)stream.ReadByte();
            chr5 = (byte)stream.ReadByte();
            chr6 = (byte)stream.ReadByte();
            chr7 = (byte)stream.ReadByte();
            irq_enable = (byte)stream.ReadByte();
            irq_counter = (byte)stream.ReadByte();
            irq_latch = (byte)stream.ReadByte();
        }
    }
}

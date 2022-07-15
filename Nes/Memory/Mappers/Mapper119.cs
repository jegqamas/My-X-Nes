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

    class Mapper119 : IMapper
    {
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
        CPUMemory map;
        public Mapper119(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
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
                            if (!map.cartridge.IsVRAM)
                            {
                                chr01 = (byte)(data & 0xFE);
                                SetBank_PPU();
                            }
                            break;
                        case 0x01:
                            if (!map.cartridge.IsVRAM)
                            {
                                chr23 = (byte)(data & 0xFE);
                                SetBank_PPU();
                            }
                            break;
                        case 0x02:
                            if (!map.cartridge.IsVRAM)
                            {
                                chr4 = data;
                                SetBank_PPU();
                            }
                            break;
                        case 0x03:
                            if (!map.cartridge.IsVRAM)
                            {
                                chr5 = data;
                                SetBank_PPU();
                            }
                            break;
                        case 0x04:
                            if (!map.cartridge.IsVRAM)
                            {
                                chr6 = data;
                                SetBank_PPU();
                            }
                            break;
                        case 0x05:
                            if (!map.cartridge.IsVRAM)
                            {
                                chr7 = data;
                                SetBank_PPU();
                            }
                            break;
                        case 0x06:
                            prg0 = data;
                            SetBank_CPU();
                            break;
                        case 0x07:
                            prg1 = data;
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
                    break;
                case 0xC001:
                    reg[5] = data;
                    irq_latch = data;
                    break;
                case 0xE000:
                    reg[6] = data;
                    irq_enable = 0;
                    irq_counter = irq_latch;
                    map.cpu.SetIRQ(false);
                    break;
                case 0xE001:
                    reg[7] = data;
                    irq_enable = 1;
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.CloneCHRtoCRAM();
            SetBank_CPU();
            SetBank_PPU();
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
        void SetBank_CPU()
        {
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
        void SetBank_PPU()
        {
            if ((reg[0] & 0x80) == 0x80)
            {
                if ((chr4 & 0x40) == 0x40)
                    map.Switch1kCRAMEX(chr4, 0);
                else
                    map.Switch1kChrRom(chr4, 0);

                if ((chr5 & 0x40) == 0x40)
                    map.Switch1kCRAMEX(chr5, 1);
                else
                    map.Switch1kChrRom(chr5, 1);

                if ((chr6 & 0x40) == 0x40)
                    map.Switch1kCRAMEX(chr6, 2);
                else
                    map.Switch1kChrRom(chr6, 2);

                if ((chr7 & 0x40) == 0x40)
                    map.Switch1kCRAMEX(chr7, 3);
                else
                    map.Switch1kChrRom(chr7, 3);

                if (((chr01 + 0) & 0x40) == 0x40)
                    map.Switch1kCRAMEX((chr01 + 0), 4);
                else
                    map.Switch1kChrRom((chr01 + 0), 4);

                if (((chr01 + 1) & 0x40) == 0x40)
                    map.Switch1kCRAMEX((chr01 + 1), 5);
                else
                    map.Switch1kChrRom((chr01 + 1), 5);

                if (((chr23 + 0) & 0x40) == 0x40)
                    map.Switch1kCRAMEX((chr23 + 0), 6);
                else
                    map.Switch1kChrRom((chr23 + 0), 6);

                if (((chr23 + 1) & 0x40) == 0x40)
                    map.Switch1kCRAMEX((chr23 + 1), 7);
                else
                    map.Switch1kChrRom((chr23 + 1), 7);
            }
            else
            {
                if (((chr01 + 0) & 0x40) == 0x40)
                    map.Switch1kCRAMEX((chr01 + 0), 0);
                else
                    map.Switch1kChrRom((chr01 + 0), 0);

                if (((chr01 + 1) & 0x40) == 0x40)
                    map.Switch1kCRAMEX((chr01 + 1), 1);
                else
                    map.Switch1kChrRom((chr01 + 1), 1);

                if (((chr23 + 0) & 0x40) == 0x40)
                    map.Switch1kCRAMEX((chr23 + 0), 2);
                else
                    map.Switch1kChrRom((chr23 + 0), 2);

                if (((chr23 + 1) & 0x40) == 0x40)
                    map.Switch1kCRAMEX((chr23 + 1), 3);
                else
                    map.Switch1kChrRom((chr23 + 1), 3);

                if ((chr4 & 0x40) == 0x40)
                    map.Switch1kCRAMEX(chr4, 4);
                else
                    map.Switch1kChrRom(chr4, 4);

                if ((chr5 & 0x40) == 0x40)
                    map.Switch1kCRAMEX(chr5, 5);
                else
                    map.Switch1kChrRom(chr5, 5);

                if ((chr6 & 0x40) == 0x40)
                    map.Switch1kCRAMEX(chr6, 6);
                else
                    map.Switch1kChrRom(chr6, 6);

                if ((chr7 & 0x40) == 0x40)
                    map.Switch1kCRAMEX(chr7, 7);
                else
                    map.Switch1kChrRom(chr7, 7);
            }
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
        }
    }
}

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

    class Mapper74 : IMapper
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
        byte irq_request = 0;
        CPUMemory map;
        public Mapper74(CPUMemory mem)
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
                    if ((data & 0x01) == 0x01)
                        map.cartridge.Mirroring = Mirroring.Horizontal;
                    else
                        map.cartridge.Mirroring = Mirroring.Vertical;
                    break;
                case 0xA001:
                    reg[3] = data;
                    break;
                case 0xC000:
                    reg[4] = data;
                    irq_counter = data;
                    irq_request = 0;
                    break;
                case 0xC001:
                    reg[5] = data;
                    irq_latch = data;
                    irq_request = 0;
                    break;
                case 0xE000:
                    reg[6] = data;
                    irq_enable = 0;
                    irq_request = 0;
                    map.cpu.SetIRQ(false);
                    break;
                case 0xE001:
                    reg[7] = data;
                    irq_enable = 1;
                    irq_request = 0;
                    break;
            }

        }
        public void SetUpMapperDefaults()
        {
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.CloneCHRtoCRAM();
            SetBank_PPU();
            SetBank_CPU();
        }
        public void TickScanlineTimer()
        {
            if (irq_enable != 0 && irq_request == 0)
            {
                if (map.ppu.ScanLine == map.ppu.ScanlineOfEndOfVblank)
                {
                    if (irq_counter > 0)
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
            if (!map.cartridge.IsVRAM)
            {
                if ((reg[0] & 0x80) == 0x80)
                {
                    SetBank_PPUSUB(4, chr01 + 0);
                    SetBank_PPUSUB(5, chr01 + 1);
                    SetBank_PPUSUB(6, chr23 + 0);
                    SetBank_PPUSUB(7, chr23 + 1);
                    SetBank_PPUSUB(0, chr4);
                    SetBank_PPUSUB(1, chr5);
                    SetBank_PPUSUB(2, chr6);
                    SetBank_PPUSUB(3, chr7);
                }
                else
                {
                    SetBank_PPUSUB(0, chr01 + 0);
                    SetBank_PPUSUB(1, chr01 + 1);
                    SetBank_PPUSUB(2, chr23 + 0);
                    SetBank_PPUSUB(3, chr23 + 1);
                    SetBank_PPUSUB(4, chr4);
                    SetBank_PPUSUB(5, chr5);
                    SetBank_PPUSUB(6, chr6);
                    SetBank_PPUSUB(7, chr7);
                }
            }
            else
            {
                if ((reg[0] & 0x80) == 0x80)
                {
                    map.Switch1kCRAMEX((chr01 + 0) & 0x07, 4);
                    map.Switch1kCRAMEX((chr01 + 1) & 0x07, 5);
                    map.Switch1kCRAMEX((chr23 + 0) & 0x07, 6);
                    map.Switch1kCRAMEX((chr23 + 1) & 0x07, 7);
                    map.Switch1kCRAMEX(chr4 & 0x07, 0);
                    map.Switch1kCRAMEX(chr5 & 0x07, 1);
                    map.Switch1kCRAMEX(chr6 & 0x07, 2);
                    map.Switch1kCRAMEX(chr7 & 0x07, 3);
                }
                else
                {
                    map.Switch1kCRAMEX((chr01 + 0) & 0x07, 0);
                    map.Switch1kCRAMEX((chr01 + 1) & 0x07, 1);
                    map.Switch1kCRAMEX((chr23 + 0) & 0x07, 2);
                    map.Switch1kCRAMEX((chr23 + 1) & 0x07, 3);
                    map.Switch1kCRAMEX(chr4 & 0x07, 4);
                    map.Switch1kCRAMEX(chr5 & 0x07, 5);
                    map.Switch1kCRAMEX(chr6 & 0x07, 6);
                    map.Switch1kCRAMEX(chr7 & 0x07, 7);
                }
            }
        }
        void SetBank_PPUSUB(int bank, int page)
        {
            if ((page == 8 || page == 9))
            {
                map.Switch1kCRAMEX(page & 7, bank);
            }
            // else if (patch == 1 && page >= 128)
            // {
            //      SetCRAM_1K_Bank(bank, page & 7);
            //  }
            else
            {
                map.Switch1kChrRom(page, bank);
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
            stream.WriteByte(irq_request);
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
        }
    }
}

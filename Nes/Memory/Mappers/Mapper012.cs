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

    class Mapper12 : IMapper
    {
        CPUMemory map;
        byte[] reg = new byte[8];
        byte prg0 = 0;
        byte prg1 = 1;
        ushort vb0 = 0;
        ushort vb1 = 0;
        byte chr01 = 0;
        byte chr23 = 2;
        byte chr4 = 4;
        byte chr5 = 5;
        byte chr6 = 6;
        byte chr7 = 7;
        byte we_sram = 0;
        byte irq_enable = 0;
        byte irq_counter = 0;
        byte irq_latch = 0xFF;
        byte irq_request = 0;
        byte irq_preset = 0;
        byte irq_preset_vbl = 0;

        public Mapper12(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            if (Address > 0x4100 && Address < 0x6000)
            {
                vb0 = (ushort)((data & 0x01) << 8);
                vb1 = (ushort)((data & 0x10) << 4);
                SetBank_PPU();
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
                        irq_latch = data;
                        break;
                    case 0xC001:
                        reg[5] = data;
                        if (map.ppu.ScanLine >= map.ppu.ScanlineOfEndOfVblank &
                            map.ppu.ScanLine <= 239 + map.ppu.ScanlineOfEndOfVblank)
                        {
                            irq_counter |= 0x80;
                            irq_preset = 0xFF;
                        }
                        else
                        {
                            irq_counter |= 0x80;
                            irq_preset_vbl = 0xFF;
                            irq_preset = 0;
                        }
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
        }
        public void SetUpMapperDefaults()
        {
            if (map.NES.Cartridge.IsVRAM)
                map.FillCHR(256);
            SetBank_CPU();
            SetBank_PPU();
        }
        public void TickScanlineTimer()
        {
            if (irq_preset_vbl != 0)
            {
                irq_counter = irq_latch;
                irq_preset_vbl = 0;
            }
            if (irq_preset != 0)
            {
                irq_counter = irq_latch;
                irq_preset = 0;
            }
            else if (irq_counter > 0)
            {
                irq_counter--;
            }

            if (irq_counter == 0)
            {
                // Some game set irq_latch to zero to disable irq. So check it here.
                if ((irq_enable != 0) && (irq_latch != 0))
                {
                    irq_request = 0xFF;
                    map.cpu.SetIRQ(true);
                }
                irq_preset = 0xFF;
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
                return 0x01;
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
                    map.Switch1kChrRom(vb0 + chr4, 0);
                    map.Switch1kChrRom(vb0 + chr5, 1);
                    map.Switch1kChrRom(vb0 + chr6, 2);
                    map.Switch1kChrRom(vb0 + chr7, 3);
                    map.Switch1kChrRom(vb1 + chr01, 4);
                    map.Switch1kChrRom(vb1 + chr01 + 1, 5);
                    map.Switch1kChrRom(vb1 + chr23, 6);
                    map.Switch1kChrRom(vb1 + chr23 + 1, 7);
                }
                else
                {
                    map.Switch1kChrRom(vb0 + chr01, 0);
                    map.Switch1kChrRom(vb0 + chr01 + 1, 1);
                    map.Switch1kChrRom(vb0 + chr23, 2);
                    map.Switch1kChrRom(vb0 + chr23 + 1, 3);
                    map.Switch1kChrRom(vb1 + chr4, 4);
                    map.Switch1kChrRom(vb1 + chr5, 5);
                    map.Switch1kChrRom(vb1 + chr6, 6);
                    map.Switch1kChrRom(vb1 + chr7, 7);
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
        public void SaveState(System.IO.Stream stream)
        {

        }
        public void LoadState(System.IO.Stream stream)
        {

        }
    }
}

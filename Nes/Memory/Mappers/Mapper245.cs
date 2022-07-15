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

    class Mapper245 : IMapper
    {
        CPUMemory map;
        byte[] reg = new byte[8];
        byte prg0 = 0;
        byte prg1 = 1;
        byte we_sram = 0;	// Disable
        byte irq_enable = 0;	// Disable
        byte irq_counter = 0;
        byte irq_latch = 0;
        byte irq_request = 0;
        public Mapper245(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            switch (Address & 0xF7FF)
            {
                case 0x8000:
                    reg[0] = data;
                    break;
                case 0x8001:
                    reg[1] = data;
                    switch (reg[0])
                    {
                        case 0x00:
                            reg[3] = (byte)((data & 2) << 5);
                            map.Switch8kPrgRom((0x3E | reg[3]) * 2, 2);
                            map.Switch8kPrgRom((0x3F | reg[3]) * 2, 3);
                            break;
                        case 0x06:
                            prg0 = data;
                            break;
                        case 0x07:
                            prg1 = data;
                            break;
                    }

                    map.Switch8kPrgRom((prg0 | reg[3]) * 2, 0);
                    map.Switch8kPrgRom((prg1 | reg[3]) * 2, 1);
                    break;
                case 0xA000:
                    reg[2] = data;
                    //if (map.cartridge.Mirroring != Mirroring.Four_Screen)
                    {
                        if ((data & 0x01) == 0x01)
                            map.cartridge.Mirroring = Mirroring.Horizontal;
                        else
                            map.cartridge.Mirroring = Mirroring.Vertical;
                    }
                    break;
                case 0xA001:

                    break;
                case 0xC000:
                    reg[4] = data;
                    irq_counter = data;
                    irq_request = 0;
                    map.cpu.SetIRQ(false);
                    break;
                case 0xC001:
                    reg[5] = data;
                    irq_latch = data;
                    irq_request = 0;
                    map.cpu.SetIRQ(false);
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
                    map.cpu.SetIRQ(false);
                    break;
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
        public void SaveState(System.IO.Stream stream)
        {
            stream.Write(reg, 0, reg.Length);
            stream.WriteByte(prg0);
            stream.WriteByte(prg1);
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
            we_sram = (byte)stream.ReadByte();
            irq_enable = (byte)stream.ReadByte();
            irq_counter = (byte)stream.ReadByte();
            irq_latch = (byte)stream.ReadByte();
            irq_request = (byte)stream.ReadByte();
        }
    }
}

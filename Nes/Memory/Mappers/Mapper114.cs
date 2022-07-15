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

    class Mapper114 : IMapper
    {
        byte irq_counter = 0;
        byte irq_occur = 0;
        byte reg_a = 0;
        byte[] reg_b = new byte[8];
        byte reg_c = 0;
        byte reg_m = 0;
        CPUMemory map;
        public Mapper114(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            if (Address < 0x8000)
            {
                reg_m = data;
                SetBank_CPU();
            }
            else
            {
                if (Address == 0xE003)
                {
                    irq_counter = data;
                }
                else
                    if (Address == 0xE002)
                    {
                        irq_occur = 0;
                        map.cpu.SetIRQ(false);
                    }
                    else
                    {
                        switch (Address & 0xE000)
                        {
                            case 0x8000:
                                if ((data & 0x01) == 0x01)
                                    map.cartridge.Mirroring = Mirroring.Horizontal;
                                else
                                    map.cartridge.Mirroring = Mirroring.Vertical;
                                break;
                            case 0xA000:
                                reg_c = 1;
                                reg_a = data;
                                break;
                            case 0xC000:
                                if (reg_c == 0)
                                {
                                    break;
                                }
                                reg_b[reg_a & 0x07] = data;
                                switch (reg_a & 0x07)
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 6:
                                    case 7:
                                        SetBank_PPU();
                                        break;
                                    case 4:
                                    case 5:
                                        SetBank_CPU();
                                        break;
                                }
                                reg_c = 0;
                                break;
                        }
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
            if (irq_counter != 0)
            {
                irq_counter--;
                if (irq_counter == 0)
                {
                    irq_occur = 0xFF;
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
            if ((reg_m & 0x80) == 0x80)
            {
                map.Switch16kPrgRom((reg_m & 0x1F) * 4, 0);
            }
            else
            {
                map.Switch8kPrgRom(reg_b[4] * 2, 0);
                map.Switch8kPrgRom(reg_b[5] * 2, 1);
            }
        }
        void SetBank_PPU()
        {
            map.Switch2kChrRom((reg_b[0] >> 1) * 2, 0);
            map.Switch2kChrRom((reg_b[2] >> 1) * 2, 1);
            map.Switch1kChrRom(reg_b[6], 4);
            map.Switch1kChrRom(reg_b[1], 5);
            map.Switch1kChrRom(reg_b[7], 6);
            map.Switch1kChrRom(reg_b[3], 7);
        }

        public void SaveState(System.IO.Stream stream)
        {
            stream.WriteByte(irq_counter);
            stream.WriteByte(irq_occur);
            stream.WriteByte(reg_a);
            stream.Write(reg_b, 0, reg_b.Length);
            stream.WriteByte(reg_c);
            stream.WriteByte(reg_m);
        }
        public void LoadState(System.IO.Stream stream)
        {
            irq_counter = (byte)stream.ReadByte();
            irq_occur = (byte)stream.ReadByte();
            reg_a = (byte)stream.ReadByte();
            stream.Read(reg_b, 0, reg_b.Length);
            reg_c = (byte)stream.ReadByte();
            reg_m = (byte)stream.ReadByte();
        }
    }
}

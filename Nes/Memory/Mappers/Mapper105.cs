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

    class Mapper105 : IMapper
    {
        byte[] reg = new byte[4];
        byte bits = 0;
        byte write_count = 0;
        int irq_counter = 0;
        byte irq_enable = 0;
        byte init_state = 0;

        CPUMemory map;
        public Mapper105(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            short reg_num = (short)((Address & 0x7FFF) >> 13);

            if ((data & 0x80) == 0x80)
            {
                bits = write_count = 0;
                if (reg_num == 0)
                {
                    reg[reg_num] |= 0x0C;
                }
            }
            else
            {
                bits |= (byte)((data & 1) << write_count++);
                if (write_count == 5)
                {
                    reg[reg_num] = (byte)(bits & 0x1F);
                    bits = write_count = 0;
                }
            }

            if ((reg[0] & 0x02) == 0x02)
            {
                if ((reg[0] & 0x01) == 0x01)
                {
                    map.cartridge.Mirroring = Mirroring.Horizontal;
                }
                else
                {
                    map.cartridge.Mirroring = Mirroring.Vertical;
                }
            }
            else
            {
                if ((reg[0] & 0x01) == 0x01)
                {
                    map.cartridge.Mirroring = Mirroring.One_Screen;
                    map.cartridge.MirroringBase = 0x2400;
                }
                else
                {
                    map.cartridge.Mirroring = Mirroring.One_Screen;
                    map.cartridge.MirroringBase = 0x2000;
                }
            }

            switch (init_state)
            {
                case 0:
                case 1:
                    init_state++;
                    break;
                case 2:
                    if ((reg[1] & 0x08) == 0x08)
                    {
                        if ((reg[0] & 0x08) == 0x08)
                        {
                            if ((reg[0] & 0x04) == 0x04)
                            {
                                map.Switch8kPrgRom(((reg[3] & 0x07) * 2 + 16) * 2, 0);
                                map.Switch8kPrgRom(((reg[3] & 0x07) * 2 + 17) * 2, 1);
                                map.Switch8kPrgRom(30 * 2, 2);
                                map.Switch8kPrgRom(31 * 2, 3);
                            }
                            else
                            {
                                map.Switch8kPrgRom(16 * 2, 0);
                                map.Switch8kPrgRom(17 * 2, 1);
                                map.Switch8kPrgRom(((reg[3] & 0x07) * 2 + 16) * 2, 2);
                                map.Switch8kPrgRom(((reg[3] & 0x07) * 2 + 17) * 2, 3);
                            }
                        }
                        else
                        {
                            map.Switch8kPrgRom(((reg[3] & 0x06) * 2 + 16) * 2, 0);
                            map.Switch8kPrgRom(((reg[3] & 0x06) * 2 + 17) * 2, 1);
                            map.Switch8kPrgRom(((reg[3] & 0x06) * 2 + 18) * 2, 2);
                            map.Switch8kPrgRom(((reg[3] & 0x06) * 2 + 19) * 2, 3);
                        }
                    }
                    else
                    {
                        map.Switch8kPrgRom(((reg[1] & 0x06) * 2 + 0) * 2, 0);
                        map.Switch8kPrgRom(((reg[1] & 0x06) * 2 + 1) * 2, 1);
                        map.Switch8kPrgRom(((reg[1] & 0x06) * 2 + 2) * 2, 2);
                        map.Switch8kPrgRom(((reg[1] & 0x06) * 2 + 3) * 2, 3);
                    }

                    if ((reg[1] & 0x10) == 0x10)
                    {
                        irq_counter = 0;
                        irq_enable = 0;
                    }
                    else
                    {
                        irq_enable = 1;
                    }
                    break;
                default:
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            map.Switch32kPrgRom(0);
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
            if (map.ppu.ScanLine == map.ppu.ScanlineOfEndOfVblank)
            {
                if (irq_enable > 0)
                {
                    irq_counter += 29781;
                }
                if (((irq_counter | 0x21FFFFFF) & 0x3E000000) == 0x3E000000)
                {
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
            get { return true; }
        }

        public void SaveState(System.IO.Stream stream)
        {
            stream.Write(reg, 0, reg.Length);
            stream.WriteByte(bits);
            stream.WriteByte(write_count);
            stream.WriteByte(irq_enable);
            stream.WriteByte(init_state);

            stream.WriteByte((byte)((irq_counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_counter & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            stream.Read(reg, 0, reg.Length);
            bits = (byte)stream.ReadByte();
            write_count = (byte)stream.ReadByte();
            irq_enable = (byte)stream.ReadByte();
            init_state = (byte)stream.ReadByte();

            irq_counter = (int)(stream.ReadByte() << 24);
            irq_counter |= (int)(stream.ReadByte() << 16);
            irq_counter |= (int)(stream.ReadByte() << 8);
            irq_counter |= stream.ReadByte();
        }
    }
}

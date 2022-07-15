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
   
    class Mapper64 : IMapper
    {
        CPUMemory Map;
        int[] reg = new int[3];
        int irq_enable = 0;
        int irq_mode = 0;
        int irq_counter = 0;
        int irq_counter2 = 0;
        int irq_latch = 0;
        int irq_reset = 0;
        public Mapper64(CPUMemory Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xF003)
            {
                case 0x8000:
                    reg[0] = data & 0x0F;
                    reg[1] = data & 0x40;
                    reg[2] = data & 0x80;
                    break;

                case 0x8001:
                    switch (reg[0])
                    {
                        case 0x00:
                            if (reg[2] != 0)
                            {
                                Map.Switch1kChrRom(data + 0, 4);
                                Map.Switch1kChrRom(data + 1, 5);
                            }
                            else
                            {
                                Map.Switch1kChrRom(data + 0, 0);
                                Map.Switch1kChrRom(data + 1, 1);
                            }
                            break;
                        case 0x01:
                            if (reg[2] != 0)
                            {
                                Map.Switch1kChrRom(data + 0, 6);
                                Map.Switch1kChrRom(data + 1, 7);
                            }
                            else
                            {
                                Map.Switch1kChrRom(data + 0, 2);
                                Map.Switch1kChrRom(data + 1, 3);
                            }
                            break;
                        case 0x02:
                            if (reg[2] != 0)
                            {
                                Map.Switch1kChrRom(data, 0);
                            }
                            else
                            {
                                Map.Switch1kChrRom(data, 4);
                            }
                            break;
                        case 0x03:
                            if (reg[2] != 0)
                            {
                                Map.Switch1kChrRom(data, 1);
                            }
                            else
                            {
                                Map.Switch1kChrRom(data, 5);
                            }
                            break;
                        case 0x04:
                            if (reg[2] != 0)
                            {
                                Map.Switch1kChrRom(data, 2);
                            }
                            else
                            {
                                Map.Switch1kChrRom(data, 6);
                            }
                            break;
                        case 0x05:
                            if (reg[2] != 0)
                            {
                                Map.Switch1kChrRom(data, 3);
                            }
                            else
                            {
                                Map.Switch1kChrRom(data, 7);
                            }
                            break;
                        case 0x06:
                            if (reg[1] != 0)
                            {
                                Map.Switch8kPrgRom(data * 2, 1);
                            }
                            else
                            {
                                Map.Switch8kPrgRom(data * 2, 0);
                            }
                            break;
                        case 0x07:
                            if (reg[1] != 0)
                            {
                                Map.Switch8kPrgRom(data * 2, 2);
                            }
                            else
                            {
                                Map.Switch8kPrgRom(data * 2, 1);
                            }
                            break;
                        case 0x08:
                            Map.Switch1kChrRom(data, 1);
                            break;
                        case 0x09:
                            Map.Switch1kChrRom(data, 3);
                            break;
                        case 0x0F:
                            if (reg[1] != 0)
                            {
                                Map.Switch8kPrgRom(data * 2, 0);
                            }
                            else
                            {
                                Map.Switch8kPrgRom(data * 2, 2);
                            }
                            break;
                    }
                    break;

                case 0xA000:
                    if ((data & 0x01) == 0x01)
                        Map.cartridge.Mirroring = Mirroring.Horizontal;
                    else
                        Map.cartridge.Mirroring = Mirroring.Vertical;
                    break;

                case 0xC000:
                    irq_latch = data;
                    if (irq_reset != 0)
                    {
                        irq_counter = irq_latch;
                    }
                    break;
                case 0xC001:
                    irq_reset = 0xFF;
                    irq_counter = irq_latch;
                    irq_mode = data & 0x01;
                    break;
                case 0xE000:
                    irq_enable = 0;
                    if (irq_reset != 0)
                    {
                        irq_counter = irq_latch;
                    }
                    break;
                case 0xE001:
                    irq_enable = 0xFF;
                    if (irq_reset != 0)
                    {
                        irq_counter = irq_latch;
                    }
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch8kPrgRom(((Map.cartridge.PRG_PAGES * 2) - 1) * 2, 0);
            Map.Switch8kPrgRom(((Map.cartridge.PRG_PAGES * 2) - 1) * 2, 1);
            Map.Switch8kPrgRom(((Map.cartridge.PRG_PAGES * 2) - 1) * 2, 2);
            Map.Switch8kPrgRom(((Map.cartridge.PRG_PAGES * 2) - 1) * 2, 3);
            if (Map.cartridge.IsVRAM)
                Map.FillCHR(16);
            Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
            if (irq_mode == 1)
                return;
            irq_reset = 0;
            if (irq_counter >= 0)
            {
                irq_counter--;
                if (irq_counter < 0)
                {
                    if (irq_enable != 0)
                    {
                        irq_reset = 1;
                        Map.cpu.SetIRQ(true);
                    }
                }
            }
        }
        public void TickCycleTimer(int cycles)
        {
            if (irq_mode == 0)
                return;

            irq_counter2 += cycles;
            while (irq_counter2 >= 4)
            {
                irq_counter2 -= 4;
                if (irq_counter >= 0)
                {
                    irq_counter--;
                    if (irq_counter < 0)
                    {
                        if (irq_enable != 0)
                        {
                            Map.cpu.SetIRQ(true);
                        }
                    }
                }
            }
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
            for (int i = 0; i < reg.Length; i++)
            {
                stream.WriteByte((byte)((reg[i] & 0xFF000000) >> 24));
                stream.WriteByte((byte)((reg[i] & 0x00FF0000) >> 16));
                stream.WriteByte((byte)((reg[i] & 0x0000FF00) >> 8));
                stream.WriteByte((byte)((reg[i] & 0x000000FF)));
            }
            stream.WriteByte((byte)((irq_enable & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_enable & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_enable & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_enable & 0x000000FF)));
            stream.WriteByte((byte)((irq_mode & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_mode & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_mode & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_mode & 0x000000FF)));
            stream.WriteByte((byte)((irq_counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_counter & 0x000000FF)));
            stream.WriteByte((byte)((irq_counter2 & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_counter2 & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_counter2 & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_counter2 & 0x000000FF)));
            stream.WriteByte((byte)((irq_latch & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_latch & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_latch & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_latch & 0x000000FF)));
            stream.WriteByte((byte)((irq_reset & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_reset & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_reset & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_reset & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            for (int i = 0; i < reg.Length; i++)
            {
                reg[i] = (int)(stream.ReadByte() << 24);
                reg[i] |= (int)(stream.ReadByte() << 16);
                reg[i] |= (int)(stream.ReadByte() << 8);
                reg[i] |= stream.ReadByte();
            }
            irq_enable = (int)(stream.ReadByte() << 24);
            irq_enable |= (int)(stream.ReadByte() << 16);
            irq_enable |= (int)(stream.ReadByte() << 8);
            irq_enable |= stream.ReadByte();
            irq_mode = (int)(stream.ReadByte() << 24);
            irq_mode |= (int)(stream.ReadByte() << 16);
            irq_mode |= (int)(stream.ReadByte() << 8);
            irq_mode |= stream.ReadByte();
            irq_counter = (int)(stream.ReadByte() << 24);
            irq_counter |= (int)(stream.ReadByte() << 16);
            irq_counter |= (int)(stream.ReadByte() << 8);
            irq_counter |= stream.ReadByte();
            irq_counter2 = (int)(stream.ReadByte() << 24);
            irq_counter2 |= (int)(stream.ReadByte() << 16);
            irq_counter2 |= (int)(stream.ReadByte() << 8);
            irq_counter2 |= stream.ReadByte();
            irq_latch = (int)(stream.ReadByte() << 24);
            irq_latch |= (int)(stream.ReadByte() << 16);
            irq_latch |= (int)(stream.ReadByte() << 8);
            irq_latch |= stream.ReadByte();
            irq_reset = (int)(stream.ReadByte() << 24);
            irq_reset |= (int)(stream.ReadByte() << 16);
            irq_reset |= (int)(stream.ReadByte() << 8);
            irq_reset |= stream.ReadByte();
        }
    }
}

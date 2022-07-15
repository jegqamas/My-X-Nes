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

    class Mapper18 : IMapper
    {
        CPUMemory Map;
        int irq_enable = 0;
        int irq_mode = 0;
        int irq_counter = 0xFFFF;
        int irq_latch = 0xFFFF;
        int[] reg = new int[11];
        public Mapper18(CPUMemory Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000:
                    reg[0] = (reg[0] & 0xF0) | (data & 0x0F);
                    Map.Switch8kPrgRom(reg[0] * 2, 0);
                    break;
                case 0x8001:
                    reg[0] = (reg[0] & 0x0F) | ((data & 0x0F) << 4);
                    Map.Switch8kPrgRom(reg[0] * 2, 0);
                    break;
                case 0x8002:
                    reg[1] = (reg[1] & 0xF0) | (data & 0x0F);
                    Map.Switch8kPrgRom(reg[1] * 2, 1);
                    break;
                case 0x8003:
                    reg[1] = (reg[1] & 0x0F) | ((data & 0x0F) << 4);
                    Map.Switch8kPrgRom(reg[1] * 2, 1);
                    break;
                case 0x9000:
                    reg[2] = (reg[2] & 0xF0) | (data & 0x0F);
                    Map.Switch8kPrgRom(reg[2] * 2, 2);
                    break;
                case 0x9001:
                    reg[2] = (reg[2] & 0x0F) | ((data & 0x0F) << 4);
                    Map.Switch8kPrgRom(reg[2] * 2, 2);
                    break;

                case 0xA000:
                    reg[3] = (reg[3] & 0xF0) | (data & 0x0F);
                    Map.Switch1kChrRom(reg[3], 0);
                    break;
                case 0xA001:
                    reg[3] = (reg[3] & 0x0F) | ((data & 0x0F) << 4);
                    Map.Switch1kChrRom(reg[3], 0);
                    break;
                case 0xA002:
                    reg[4] = (reg[4] & 0xF0) | (data & 0x0F);
                    Map.Switch1kChrRom(reg[4], 1);
                    break;
                case 0xA003:
                    reg[4] = (reg[4] & 0x0F) | ((data & 0x0F) << 4);
                    Map.Switch1kChrRom(reg[4], 1);
                    break;

                case 0xB000:
                    reg[5] = (reg[5] & 0xF0) | (data & 0x0F);
                    Map.Switch1kChrRom(reg[5], 2);
                    break;
                case 0xB001:
                    reg[5] = (reg[5] & 0x0F) | ((data & 0x0F) << 4);
                    Map.Switch1kChrRom(reg[5], 2);
                    break;
                case 0xB002:
                    reg[6] = (reg[6] & 0xF0) | (data & 0x0F);
                    Map.Switch1kChrRom(reg[6], 3);
                    break;
                case 0xB003:
                    reg[6] = (reg[6] & 0x0F) | ((data & 0x0F) << 4);
                    Map.Switch1kChrRom(reg[6], 3);
                    break;

                case 0xC000:
                    reg[7] = (reg[7] & 0xF0) | (data & 0x0F);
                    Map.Switch1kChrRom(reg[7], 4);
                    break;
                case 0xC001:
                    reg[7] = (reg[7] & 0x0F) | ((data & 0x0F) << 4);
                    Map.Switch1kChrRom(reg[7], 4);
                    break;
                case 0xC002:
                    reg[8] = (reg[8] & 0xF0) | (data & 0x0F);
                    Map.Switch1kChrRom(reg[8], 5);
                    break;
                case 0xC003:
                    reg[8] = (reg[8] & 0x0F) | ((data & 0x0F) << 4);
                    Map.Switch1kChrRom(reg[8], 5);
                    break;

                case 0xD000:
                    reg[9] = (reg[9] & 0xF0) | (data & 0x0F);
                    Map.Switch1kChrRom(reg[9], 6);
                    break;
                case 0xD001:
                    reg[9] = (reg[9] & 0x0F) | ((data & 0x0F) << 4);
                    Map.Switch1kChrRom(reg[9], 6);
                    break;
                case 0xD002:
                    reg[10] = (reg[10] & 0xF0) | (data & 0x0F);
                    Map.Switch1kChrRom(reg[10], 7);
                    break;
                case 0xD003:
                    reg[10] = (reg[10] & 0x0F) | ((data & 0x0F) << 4);
                    Map.Switch1kChrRom(reg[10], 7);
                    break;

                case 0xE000:
                    irq_latch = (irq_latch & 0xFFF0) | (data & 0x0F);
                    break;
                case 0xE001:
                    irq_latch = (irq_latch & 0xFF0F) | ((data & 0x0F) << 4);
                    break;
                case 0xE002:
                    irq_latch = (irq_latch & 0xF0FF) | ((data & 0x0F) << 8);
                    break;
                case 0xE003:
                    irq_latch = (irq_latch & 0x0FFF) | ((data & 0x0F) << 12);
                    break;

                case 0xF000:
                    irq_counter = irq_latch;
                    break;
                case 0xF001:
                    irq_mode = (data >> 1) & 0x07;
                    irq_enable = (data & 0x01);
                    break;

                case 0xF002:
                    data &= 0x03;
                    if (data == 0)
                        Map.cartridge.Mirroring = Mirroring.Horizontal;
                    else if (data == 1)
                        Map.cartridge.Mirroring = Mirroring.Vertical;
                    else
                    {
                        Map.cartridge.Mirroring = Mirroring.One_Screen;
                        Map.cartridge.MirroringBase = 0x2000;
                    }
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.cartridge.PRG_PAGES - 1) * 4, 1);
            if (Map.cartridge.IsVRAM)
                Map.FillCHR(16);
            Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer(int cycles)
        {
            bool bIRQ = false;
            int irq_counter_old = irq_counter;

            if (irq_enable > 0 && irq_counter > 0)
            {
                irq_counter -= cycles;

                switch (irq_mode)
                {
                    case 0:
                        if (irq_counter <= 0)
                        {
                            bIRQ = true;
                        }
                        break;
                    case 1:
                        if ((irq_counter & 0xF000) != (irq_counter_old & 0xF000))
                        {
                            bIRQ = true;
                        }
                        break;
                    case 2:
                    case 3:
                        if ((irq_counter & 0xFF00) != (irq_counter_old & 0xFF00))
                        {
                            bIRQ = true;
                        }
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        if ((irq_counter & 0xFFF0) != (irq_counter_old & 0xFFF0))
                        {
                            bIRQ = true;
                        }
                        break;
                }

                if (bIRQ)
                {
                    irq_counter = 0;
                    irq_enable = 0;
                    Map.cpu.SetIRQ(true);
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

            stream.WriteByte((byte)((irq_latch & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_latch & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_latch & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_latch & 0x000000FF)));

            for (int i = 0; i < reg.Length; i++)
            {
                stream.WriteByte((byte)((reg[i] & 0xFF000000) >> 24));
                stream.WriteByte((byte)((reg[i] & 0x00FF0000) >> 16));
                stream.WriteByte((byte)((reg[i] & 0x0000FF00) >> 8));
                stream.WriteByte((byte)((reg[i] & 0x000000FF)));
            }
        }
        public void LoadState(System.IO.Stream stream)
        {
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

            irq_latch = (int)(stream.ReadByte() << 24);
            irq_latch |= (int)(stream.ReadByte() << 16);
            irq_latch |= (int)(stream.ReadByte() << 8);
            irq_latch |= stream.ReadByte();

            for (int i = 0; i < reg.Length; i++)
            {
                reg[i] = (int)(stream.ReadByte() << 24);
                reg[i] |= (int)(stream.ReadByte() << 16);
                reg[i] |= (int)(stream.ReadByte() << 8);
                reg[i] |= stream.ReadByte();
            }
        }
    }
}

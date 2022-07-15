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
   
    class Mapper19 : IMapper
    {
        CPUMemory Map;
        byte[] reg = new byte[3];
        byte[] exram = new byte[128];
        int irq_enable = 0;
        int irq_counter = 0;

        public Mapper19(CPUMemory map)
        { Map = map; }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xF800)
            {
                case 0x4800:
                    if (address == 0x4800)
                    {
                        if ((reg[2] & 0x80) == 0x80)
                            reg[2] = (byte)((reg[2] + 1) | 0x80);
                    }
                    break;
                case 0x5000:
                    irq_counter = (irq_counter & 0xFF00) | data;
                    break;
                case 0x5800:
                    irq_counter = (irq_counter & 0x00FF) | ((data & 0x7F) << 8);
                    irq_enable = data & 0x80;
                    break;

                case 0x8000:
                    if ((data < 0xE0) || (reg[0] != 0))
                    {
                        Map.Switch1kChrRom(data, 0);
                    }
                    else
                    {
                        Map.Switch1kCRAM(data & 0x1F, 0);
                    }
                    break;
                case 0x8800:
                    if ((data < 0xE0) || (reg[0] != 0))
                    {
                        Map.Switch1kChrRom(data, 1);
                    }
                    else
                    {
                        Map.Switch1kCRAM(data & 0x1F, 1);
                    }
                    break;
                case 0x9000:
                    if ((data < 0xE0) || (reg[0] != 0))
                    {
                        Map.Switch1kChrRom(data, 2);
                    }
                    else
                    {
                        Map.Switch1kCRAM(data & 0x1F, 2);
                    }
                    break;
                case 0x9800:
                    if ((data < 0xE0) || (reg[0] != 0))
                    {
                        Map.Switch1kChrRom(data, 3);
                    }
                    else
                    {
                        Map.Switch1kCRAM(data & 0x1F, 3);
                    }
                    break;
                case 0xA000:
                    if ((data < 0xE0) || (reg[1] != 0))
                    {
                        Map.Switch1kChrRom(data, 4);
                    }
                    else
                    {
                        Map.Switch1kCRAM(data & 0x1F, 4);
                    }
                    break;
                case 0xA800:
                    if ((data < 0xE0) || (reg[1] != 0))
                    {
                        Map.Switch1kChrRom(data, 5);
                    }
                    else
                    {
                        Map.Switch1kCRAM(data & 0x1F, 5);
                    }
                    break;
                case 0xB000:
                    if ((data < 0xE0) || (reg[1] != 0))
                    {
                        Map.Switch1kChrRom(data, 6);
                    }
                    else
                    {
                        Map.Switch1kCRAM(data & 0x1F, 6);
                    }
                    break;
                case 0xB800:
                    if ((data < 0xE0) || (reg[1] != 0))
                    {
                        Map.Switch1kChrRom(data, 7);
                    }
                    else
                    {
                        Map.Switch1kCRAM(data & 0x1F, 7);
                    }
                    break;
                case 0xC000:
                    if (data <= 0xDF)
                    {
                       Map.Switch1kCHRToVRAM(data, 0);
                    }
                    else
                    {
                        Map.SwitchVRAM((byte)(data & 0x01), 0);
                    }
                    break;
                case 0xC800:

                    if (data <= 0xDF)
                    {
                       Map.Switch1kCHRToVRAM(data, 1);
                    }
                    else
                    {
                        Map.SwitchVRAM((byte)(data & 0x01), 1);
                    }
                    break;
                case 0xD000:
                    if (data <= 0xDF)
                    {
                       Map.Switch1kCHRToVRAM(data, 2);
                    }
                    else
                    {
                        Map.SwitchVRAM((byte)(data & 0x01), 2);
                    }
                    break;
                case 0xD800:
                    if (data <= 0xDF)
                    {
                        Map.Switch1kCHRToVRAM(data, 3);
                    }
                    else
                    {
                        Map.SwitchVRAM((byte)(data & 0x01), 3);
                    }
                    break;
                case 0xE000:
                    Map.Switch8kPrgRom((data & 0x3F) * 2, 0);
                    break;
                case 0xE800:
                    reg[0] = (byte)(data & 0x40);
                    reg[1] = (byte)(data & 0x80);
                    Map.Switch8kPrgRom((data & 0x3F) * 2, 1);
                    break;
                case 0xF000:
                    Map.Switch8kPrgRom((data & 0x3F) * 2, 2);
                    break;
                case 0xF800:
                    if (address == 0xF800)
                    {
                        reg[2] = data;
                    }
                    break;
            }
        }
        public byte Read(ushort Address)
        {
            switch (Address & 0xF800)
            {
                case 0x4800:
                    if (Address == 0x4800)
                    {
                        if ((reg[2] & 0x80) == 0x80)
                            reg[2] = (byte)((reg[2] + 1) | 0x80);
                        //return data;
                    }
                    break;
                case 0x5000:
                    return (byte)(irq_counter & 0x00FF);
                case 0x5800:
                    return (byte)((irq_counter >> 8) & 0x7F);
            }
            return 0;
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.cartridge.PRG_PAGES - 1) * 4, 1);
            Map.FillCRAM(32);//we'll need these 32k
            if (Map.cartridge.IsVRAM)
                Map.FillCHR(16);
            Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer(int cycles)
        {
            if (irq_enable > 0)
            {
                if ((irq_counter += cycles) >= 0x7FFF)
                {
                    irq_enable = 0;
                    irq_counter = 0x7FFF;
                    Map.cpu.SetIRQ(true);
                }
            }
        }
        public void SoftReset()
        {

        }
        public bool WriteUnder8000
        {
            get { return true; }
        }
        public bool WriteUnder6000
        {
            get { return true; }
        }
        public bool ScanlineTimerNotPauseAtVBLANK
        {
            get { return false; }
        }

        public void SaveState(System.IO.Stream stream)
        {
            stream.Write(reg, 0, reg.Length);
            stream.Write(exram, 0, exram.Length);
            stream.WriteByte((byte)((irq_enable & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_enable & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_enable & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_enable & 0x000000FF)));
            stream.WriteByte((byte)((irq_counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_counter & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            stream.Read(reg, 0, reg.Length);
            stream.Read(exram, 0, exram.Length);
            irq_enable = (int)(stream.ReadByte() << 24);
            irq_enable |= (int)(stream.ReadByte() << 16);
            irq_enable |= (int)(stream.ReadByte() << 8);
            irq_enable |= stream.ReadByte();

            irq_counter = (int)(stream.ReadByte() << 24);
            irq_counter |= (int)(stream.ReadByte() << 16);
            irq_counter |= (int)(stream.ReadByte() << 8);
            irq_counter |= stream.ReadByte();
        }
    }
}

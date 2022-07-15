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

    class Mapper24 : IMapper
    {
        CPUMemory Map;
        int irq_latch = 0;
        int irq_enable = 0;
        int irq_counter = 0;
        int irq_clock = 0;
        public Mapper24(CPUMemory MAP)
        {
            Map = MAP;
        }
        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0x8000: Map.Switch16kPrgRom(data * 4, 0); break;
                case 0xB003:
                    switch (data & 0x0C)
                    {
                        case 0: Map.cartridge.Mirroring = Mirroring.Vertical; break;
                        case 0x04: Map.cartridge.Mirroring = Mirroring.Horizontal; break;
                        case 0x08:
                            Map.cartridge.Mirroring = Mirroring.One_Screen;
                            Map.cartridge.MirroringBase = 0x2000;
                            break;
                        case 0x0C:
                            Map.cartridge.Mirroring = Mirroring.One_Screen;
                            Map.cartridge.MirroringBase = 0x2400;
                            break;
                    }
                    break;

                case 0xC000: Map.Switch8kPrgRom(data * 2, 2); break;
                case 0xD000: Map.Switch1kChrRom(data, 0); break;
                case 0xD001: Map.Switch1kChrRom(data, 1); break;
                case 0xD002: Map.Switch1kChrRom(data, 2); break;
                case 0xD003: Map.Switch1kChrRom(data, 3); break;
                case 0xE000: Map.Switch1kChrRom(data, 4); break;
                case 0xE001: Map.Switch1kChrRom(data, 5); break;
                case 0xE002: Map.Switch1kChrRom(data, 6); break;
                case 0xE003: Map.Switch1kChrRom(data, 7); break;

                case 0xF000:
                    irq_latch = data;
                    break;
                case 0xF001:
                    irq_enable = (data & 0x03);
                    if ((irq_enable & 0x02) == 0x02)
                    {
                        irq_counter = irq_latch;
                        irq_clock = 0;
                    }
                    break;
                case 0xF002:
                    irq_enable = (irq_enable & 0x01) * 3;
                    break;

                //Sound
                //Pulse 1
                case 0x9000:
                    ((Vrc6ExternalComponent)Map.apu.External).ChannelSq1.Poke1(0x9000, data);
                    break;
                case 0x9001:
                    ((Vrc6ExternalComponent)Map.apu.External).ChannelSq1.Poke2(0x9001, data);
                    break;
                case 0x9002:
                    ((Vrc6ExternalComponent)Map.apu.External).ChannelSq1.Poke3(0x9002, data);
                    break;
                //Pulse 2
                case 0xA000:
                    ((Vrc6ExternalComponent)Map.apu.External).ChannelSq2.Poke1(0xA000, data);
                    break;
                case 0xA001:
                    ((Vrc6ExternalComponent)Map.apu.External).ChannelSq2.Poke2(0xA001, data);
                    break;
                case 0xA002:
                    ((Vrc6ExternalComponent)Map.apu.External).ChannelSq2.Poke3(0xA002, data);
                    break;
                //Sawtooth
                case 0xB000:
                    ((Vrc6ExternalComponent)Map.apu.External).ChannelSaw.Poke1(0xB000, data);
                    break;
                case 0xB001:
                    ((Vrc6ExternalComponent)Map.apu.External).ChannelSaw.Poke2(0xB001, data);
                    break;
                case 0xB002:
                    ((Vrc6ExternalComponent)Map.apu.External).ChannelSaw.Poke3(0xB002, data);
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
            Map.NES.APU.External = new Vrc6ExternalComponent(Map.NES);
        }
        public void TickScanlineTimer()
        {

        }
        public void TickCycleTimer(int cycles)
        {
            if ((irq_enable & 0x02) == 0x02)
            {
                if ((irq_clock += cycles) >= 0x72)
                {
                    irq_clock -= 0x72;
                    if (irq_counter == 0xFF)
                    {
                        irq_counter = irq_latch;
                        Map.cpu.SetIRQ(true);
                    }
                    else
                    {
                        irq_counter++;
                    }
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
            stream.WriteByte((byte)((irq_latch & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_latch & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_latch & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_latch & 0x000000FF)));
            stream.WriteByte((byte)((irq_enable & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_enable & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_enable & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_enable & 0x000000FF)));
            stream.WriteByte((byte)((irq_counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_counter & 0x000000FF)));
            stream.WriteByte((byte)((irq_clock & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_clock & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_clock & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_clock & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            irq_latch = (int)(stream.ReadByte() << 24);
            irq_latch |= (int)(stream.ReadByte() << 16);
            irq_latch |= (int)(stream.ReadByte() << 8);
            irq_latch |= stream.ReadByte();
            irq_enable = (int)(stream.ReadByte() << 24);
            irq_enable |= (int)(stream.ReadByte() << 16);
            irq_enable |= (int)(stream.ReadByte() << 8);
            irq_enable |= stream.ReadByte();
            irq_counter = (int)(stream.ReadByte() << 24);
            irq_counter |= (int)(stream.ReadByte() << 16);
            irq_counter |= (int)(stream.ReadByte() << 8);
            irq_counter |= stream.ReadByte();
            irq_clock = (int)(stream.ReadByte() << 24);
            irq_clock |= (int)(stream.ReadByte() << 16);
            irq_clock |= (int)(stream.ReadByte() << 8);
            irq_clock |= stream.ReadByte();
        }
    }
}

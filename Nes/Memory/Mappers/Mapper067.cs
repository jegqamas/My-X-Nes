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
   
    class Mapper67 : IMapper
    {
        CPUMemory Map;
        public Mapper67(CPUMemory map)
        { Map = map; }
        int irq_enable = 0;
        int irq_counter = 0;
        int irq_occur = 0; 
        int irq_toggle = 0;
        public void Write(ushort address, byte data)
        {
            switch (address & 0xF800)
            {
                case 0x8800:
                    Map.Switch2kChrRom(data * 2, 0);
                    break;
                case 0x9800:
                    Map.Switch2kChrRom(data * 2, 1);
                    break;
                case 0xA800:
                    Map.Switch2kChrRom(data * 2, 2);
                    break;
                case 0xB800:
                    Map.Switch2kChrRom(data * 2, 3);
                    break;

                case 0xC800:
                    if (irq_toggle == 0)
                    {
                        irq_counter = (irq_counter & 0x00FF) | (data << 8);
                    }
                    else
                    {
                        irq_counter = (irq_counter & 0xFF00) | (data & 0xFF);
                    }
                    irq_toggle ^= 1;
                    irq_occur = 0;
                    break;
                case 0xD800:
                    irq_enable = data & 0x10;
                    irq_toggle = 0;
                    irq_occur = 0;
                    break;

                case 0xE800:
                    data &= 0x03;
                    if (data == 0)
                        Map.cartridge.Mirroring = Mirroring.Vertical;
                    else if (data == 1)
                        Map.cartridge.Mirroring = Mirroring.Horizontal;
                    else if (data == 2)
                    {
                        Map.cartridge.Mirroring = Mirroring.One_Screen;
                        Map.cartridge.MirroringBase = 0x2000;
                    }
                    else
                    {
                        Map.cartridge.Mirroring = Mirroring.One_Screen;
                        Map.cartridge.MirroringBase = 0x2400;
                    }
                    break;

                case 0xF800:
                    Map.Switch16kPrgRom(data * 4, 0);
                    break;
            }
        }

        public byte Read(ushort Address)
        {
            return 0;
        }

        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom((Map.cartridge.CHR_PAGES - 1) * 4, 1);
            if (Map.cartridge.IsVRAM)
                Map.FillCHR(16);
            Map.Switch8kChrRom(0);
        }

        public void TickScanlineTimer()
        {

        }

        public void TickCycleTimer(int cycles)
        {
            if (irq_enable != 0)
            {
                if ((irq_counter -= cycles) <= 0)
                {
                    irq_enable = 0;
                    irq_occur = 0xFF;
                    irq_counter = 0xFFFF;
                    Map.cpu.SetIRQ(true);
                }
            }
        }

        public void SoftReset()
        {

        }

        public bool WriteUnder8000
        {
            get { return false; }
        }

        public bool WriteUnder6000
        {
            get { return false; }
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

            stream.WriteByte((byte)((irq_counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_counter & 0x000000FF)));

            stream.WriteByte((byte)((irq_occur & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_occur & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_occur & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_occur & 0x000000FF)));

            stream.WriteByte((byte)((irq_toggle & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_toggle & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_toggle & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_toggle & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            irq_enable = (int)(stream.ReadByte() << 24);
            irq_enable |= (int)(stream.ReadByte() << 16);
            irq_enable |= (int)(stream.ReadByte() << 8);
            irq_enable |= stream.ReadByte();

            irq_counter = (int)(stream.ReadByte() << 24);
            irq_counter |= (int)(stream.ReadByte() << 16);
            irq_counter |= (int)(stream.ReadByte() << 8);
            irq_counter |= stream.ReadByte();

            irq_occur = (int)(stream.ReadByte() << 24);
            irq_occur |= (int)(stream.ReadByte() << 16);
            irq_occur |= (int)(stream.ReadByte() << 8);
            irq_occur |= stream.ReadByte();

            irq_toggle = (int)(stream.ReadByte() << 24);
            irq_toggle |= (int)(stream.ReadByte() << 16);
            irq_toggle |= (int)(stream.ReadByte() << 8);
            irq_toggle |= stream.ReadByte();
        }
    }
}

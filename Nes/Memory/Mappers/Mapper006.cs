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
   
    class Mapper06 : IMapper
    {
        CPUMemory Map;
        public bool IRQEnabled = false;
        public int irq_counter = 0;
        public Mapper06(CPUMemory map)
        { Map = map; }
        public void Write(ushort address, byte data)
        {
            if (address >= 0x8000 & address <= 0xFFFF)
            {
                Map.Switch16kPrgRom(((data & 0x3C) >> 2) * 4, 0);
                Map.Switch8kChrRom((data & 0x3) * 8);
            }
            else
            {
                switch (address)
                {
                    case 0x42FE:
                        Map.cartridge.Mirroring = Mirroring.One_Screen;
                        if ((data & 0x10) != 0)
                            Map.cartridge.MirroringBase = 0x2400;
                        else
                            Map.cartridge.MirroringBase = 0x2000;
                        break;
                    case 0x42FF:
                        if ((data & 0x10) != 0)
                            Map.cartridge.Mirroring = Mirroring.Horizontal;
                        else
                            Map.cartridge.Mirroring = Mirroring.Vertical;
                        break;
                    case 0x4501: IRQEnabled = false; Map.cpu.SetIRQ(false); break;
                    case 0x4502: irq_counter = (short)((irq_counter & 0xFF00) | data); break;
                    case 0x4503: irq_counter = (short)((data << 8) | (irq_counter & 0x00FF)); IRQEnabled = true; Map.cpu.SetIRQ(false); break;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            Map.Switch16kPrgRom(0, 0);
            Map.Switch16kPrgRom(7 * 4, 1);
            if (Map.cartridge.IsVRAM)
                Map.FillCHR(512);
            Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
            if (IRQEnabled)
            {
                irq_counter += 133;
                if (irq_counter >= 0xFFFF)
                {
                    irq_counter = 0;
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
        public void TickCycleTimer(int cycles)
        {
          
        }
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
            stream.WriteByte((byte)(IRQEnabled ? 1 : 0));
            stream.WriteByte((byte)((irq_counter & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_counter & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_counter & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_counter & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            IRQEnabled = stream.ReadByte() == 1;
            irq_counter = (int)(stream.ReadByte() << 24);
            irq_counter |= (int)(stream.ReadByte() << 16);
            irq_counter |= (int)(stream.ReadByte() << 8);
            irq_counter |= stream.ReadByte();
        }
    }
}

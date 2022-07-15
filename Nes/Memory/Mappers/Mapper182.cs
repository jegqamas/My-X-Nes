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

    class Mapper182 : IMapper
    {
        CPUMemory map;
        byte reg = 0;
        byte irq_enable = 0;
        byte irq_counter = 0;
        public Mapper182(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            switch (Address & 0xF003)
            {
                case 0x8001:
                    if ((data & 0x01) == 0x01)
                        map.cartridge.Mirroring = Mirroring.Horizontal;
                    else
                        map.cartridge.Mirroring = Mirroring.Vertical;
                    break;
                case 0xA000:
                    reg = (byte)(data & 0x07);
                    break;
                case 0xC000:
                    switch (reg)
                    {
                        case 0:
                            map.Switch1kChrRom((data & 0xFE) + 0, 0);
                            map.Switch1kChrRom((data & 0xFE) + 1, 1);
                            break;
                        case 1:
                            map.Switch1kChrRom(data, 5);
                            break;
                        case 2:
                            map.Switch1kChrRom((data & 0xFE) + 0, 2);
                            map.Switch1kChrRom((data & 0xFE) + 1, 3);
                            break;
                        case 3:
                            map.Switch1kChrRom(data, 7);
                            break;
                        case 4:
                            map.Switch8kPrgRom(data * 2, 0);
                            break;
                        case 5:
                            map.Switch8kPrgRom(data * 2, 1);
                            break;
                        case 6:
                            map.Switch1kChrRom(data, 4);
                            break;
                        case 7:
                            map.Switch1kChrRom(data, 6);
                            break;
                    }
                    break;
                case 0xE003:
                    irq_enable = data;
                    irq_counter = data;
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
            if ((--irq_counter) == 0)
            {
                irq_enable = 0;
                irq_counter = 0;
                map.cpu.SetIRQ(true);
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
            stream.WriteByte(reg);
            stream.WriteByte(irq_enable);
            stream.WriteByte(irq_enable);
        }
        public void LoadState(System.IO.Stream stream)
        {
            reg = (byte)stream.ReadByte();
            irq_enable = (byte)stream.ReadByte();
            irq_enable = (byte)stream.ReadByte();
        }
    }
}

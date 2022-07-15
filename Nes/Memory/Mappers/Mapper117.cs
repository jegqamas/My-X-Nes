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

    class Mapper117 : IMapper
    {
        CPUMemory map;
        byte irq_counter = 0;
        byte irq_enable = 0;
        public Mapper117(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            switch (Address)
            {
                case 0x8000:
                    map.Switch8kPrgRom(data * 2, 0);
                    break;
                case 0x8001:
                    map.Switch8kPrgRom(data * 2, 1);
                    break;
                case 0x8002:
                    map.Switch8kPrgRom(data * 2, 2);
                    break;
                case 0xA000:
                    map.Switch1kChrRom(data, 0);
                    break;
                case 0xA001:
                    map.Switch1kChrRom(data, 1);
                    break;
                case 0xA002:
                    map.Switch1kChrRom(data, 2);
                    break;
                case 0xA003:
                    map.Switch1kChrRom(data, 3);
                    break;
                case 0xA004:
                    map.Switch1kChrRom(data, 4);
                    break;
                case 0xA005:
                    map.Switch1kChrRom(data, 5);
                    break;
                case 0xA006:
                    map.Switch1kChrRom(data, 6);
                    break;
                case 0xA007:
                    map.Switch1kChrRom(data, 7);
                    break;
                case 0xC001:
                case 0xC002:
                case 0xC003:
                    irq_counter = data;
                    break;
                case 0xE000:
                    irq_enable = (byte)(data & 1);
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
            if (irq_enable != 0)
            {
                if (irq_counter == map.ppu.ScanLine - map.ppu.ScanlineOfEndOfVblank)
                {
                    irq_counter = 0;
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
            stream.WriteByte(irq_counter); stream.WriteByte(irq_enable);
        }
        public void LoadState(System.IO.Stream stream)
        {
            irq_counter = (byte)stream.ReadByte();
            irq_enable = (byte)stream.ReadByte();
        }
    }
}

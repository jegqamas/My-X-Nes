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

    class Mapper85 : IMapper
    {
        CPUMemory _Map;
        byte irq_latch = 0;
        byte irq_enable = 0;
        byte irq_counter = 0;
        int irq_clock = 0;
        public Mapper85(CPUMemory map)
        {
            _Map = map;
        }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xF038)
            {
                case 0x8000: _Map.Switch8kPrgRom(data * 2, 0); break;
                case 0x8010:
                case 0x8008: _Map.Switch8kPrgRom(data * 2, 1); break;
                case 0x9000: _Map.Switch8kPrgRom(data * 2, 2); break;

                case 0x9010:
                case 0x9030://ex sound, later ...
                    break;
                case 0xA000: _Map.Switch1kChrRom(data, 0); break;
                case 0xA008:
                case 0xA010: _Map.Switch1kChrRom(data, 1); break;
                case 0xB000: _Map.Switch1kChrRom(data, 2); break;
                case 0xB008:
                case 0xB010: _Map.Switch1kChrRom(data, 3); break;
                case 0xC000: _Map.Switch1kChrRom(data, 4); break;
                case 0xC008:
                case 0xC010: _Map.Switch1kChrRom(data, 5); break;
                case 0xD000: _Map.Switch1kChrRom(data, 6); break;
                case 0xD008:
                case 0xD010: _Map.Switch1kChrRom(data, 7); break;

                case 0xE000:
                    data &= 0x03;
                    if (data == 0)
                        _Map.cartridge.Mirroring = Mirroring.Vertical;
                    else if (data == 1)
                        _Map.cartridge.Mirroring = Mirroring.Horizontal;
                    else if (data == 2)
                    {
                        _Map.cartridge.Mirroring = Mirroring.One_Screen;
                        _Map.cartridge.MirroringBase = 0x2000;
                    }
                    else
                    {
                        _Map.cartridge.Mirroring = Mirroring.One_Screen;
                        _Map.cartridge.MirroringBase = 0x2400;
                    }
                    break;
                case 0xE008:
                case 0xE010:
                    irq_latch = data;
                    break;

                case 0xF000:
                    irq_enable = (byte)(data & 0x03);
                    irq_counter = irq_latch;
                    irq_clock = 0;
                    _Map.cpu.SetIRQ(false);
                    break;

                case 0xF008:
                case 0xF010:
                    irq_enable = (byte)((irq_enable & 0x01) * 3);
                    _Map.cpu.SetIRQ(false);
                    break;
            }
        }
        public void SetUpMapperDefaults()
        {
            _Map.Switch16kPrgRom(0, 0);
            _Map.Switch16kPrgRom((_Map.cartridge.PRG_PAGES - 1) * 4, 1);
            if (_Map.cartridge.IsVRAM)
                _Map.FillCHR(16);
            _Map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {

        }
        public void TickCycleTimer(int cycles)
        {
            if ((irq_enable & 0x02) != 0)
            {
                irq_clock += cycles * 4;
                while (irq_clock >= 455)
                {
                    irq_clock -= 455;
                    irq_counter++;
                    if (irq_counter == 0)
                    {
                        irq_counter = irq_latch;
                        _Map.cpu.SetIRQ(true);
                    }
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
            stream.WriteByte(irq_latch);
            stream.WriteByte(irq_enable);
            stream.WriteByte(irq_counter);
            stream.WriteByte((byte)((irq_clock & 0xFF000000) >> 24));
            stream.WriteByte((byte)((irq_clock & 0x00FF0000) >> 16));
            stream.WriteByte((byte)((irq_clock & 0x0000FF00) >> 8));
            stream.WriteByte((byte)((irq_clock & 0x000000FF)));
        }
        public void LoadState(System.IO.Stream stream)
        {
            irq_latch = (byte)stream.ReadByte();
            irq_enable = (byte)stream.ReadByte();
            irq_counter = (byte)stream.ReadByte();

            irq_clock = (int)(stream.ReadByte() << 24);
            irq_clock |= (int)(stream.ReadByte() << 16);
            irq_clock |= (int)(stream.ReadByte() << 8);
            irq_clock |= stream.ReadByte();
        }
    }
}

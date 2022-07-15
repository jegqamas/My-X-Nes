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
   
    class Mapper33 : IMapper
    {
        CPUMemory Map;
        bool type1 = true;
        byte IRQCounter = 0;
        byte IRQLatch = 0;
        bool IRQEabled;
        public Mapper33(CPUMemory Maps)
        {
            Map = Maps;
        }
        public void Write(ushort address, byte data)
        {
            if (address == 0x8000)
            {
                Map.Switch8kPrgRom((data & 0x1F) * 2, 0);
                if (type1)
                {
                    if ((data & 0x40) == 0x40)
                    {
                        Map.cartridge.Mirroring = Mirroring.Horizontal;

                    }
                    else
                    {
                        Map.cartridge.Mirroring = Mirroring.Vertical;
                    }
                }
            }
            else if (address == 0x8001)
            {
                Map.Switch8kPrgRom(data * 2, 1);
            }
            else if (address == 0x8002)
            {
                Map.Switch2kChrRom(data * 2, 0);
            }
            else if (address == 0x8003)
            {
                Map.Switch2kChrRom(data * 2, 1);
            }
            else if (address == 0xA000)
            {
                Map.Switch1kChrRom(data, 4);
            }
            else if (address == 0xA001)
            {
                Map.Switch1kChrRom(data, 5);
            }
            else if (address == 0xA002)
            {
                Map.Switch1kChrRom(data, 6);
            }
            else if (address == 0xA003)
            {
                Map.Switch1kChrRom(data, 7);
            }
            //Type 2 registers
            else if (address == 0xC000)
            {
                type1 = false;
                IRQLatch = data;
                IRQCounter = IRQLatch;
            }
            else if (address == 0xC001)
            {
                type1 = false;
                IRQCounter = IRQLatch;
            }
            else if (address == 0xC002)
            {
                type1 = false;
                IRQEabled = true;
            }
            else if (address == 0xC003)
            {
                type1 = false;
                IRQEabled = false;
            }
            else if (address == 0xE000)
            {
                type1 = false;
                if ((data & 0x40) == 0x40)
                {
                    Map.cartridge.Mirroring = Mirroring.Horizontal;

                }
                else
                {
                    Map.cartridge.Mirroring = Mirroring.Vertical;
                }
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
            if (IRQEabled)
            {
                if (++IRQCounter == 0)
                {
                    IRQEabled = false;
                    IRQCounter = 0;
                    Map.cpu.SetIRQ(true);
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
            stream.WriteByte((byte)(type1 ? 1 : 0));
            stream.WriteByte(IRQCounter);
            stream.WriteByte(IRQLatch);
            stream.WriteByte((byte)(IRQEabled ? 1 : 0));
        }
        public void LoadState(System.IO.Stream stream)
        {
            type1 = stream.ReadByte() == 1;
            IRQCounter = (byte)stream.ReadByte();
            IRQLatch = (byte)stream.ReadByte();
            IRQEabled = stream.ReadByte() == 1;
        }
    }
}

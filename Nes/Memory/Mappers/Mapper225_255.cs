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

    class Mapper225_255 : IMapper
    {
        CPUMemory _Map;
        byte Mapper225_reg0 = 0xF;
        byte Mapper225_reg1 = 0xF;
        byte Mapper225_reg2 = 0xF;
        byte Mapper225_reg3 = 0xF;
        public Mapper225_255(CPUMemory Map)
        {
            _Map = Map;
        }
        public void Write(ushort address, byte data)
        {
            if ((address >= 0x8000) && (address <= 0xFFFF))
            {
                int banks = ((address & 0x4000) != 0) ? 1 : 0;
                ushort wher = (ushort)((address & 0x0Fc0) >> 7);
                _Map.Switch8kChrRom(((address & 0x003F) + (banks << 6)) * 8);
                if ((address & 0x1000) != 0)//A12
                {
                    if ((address & 0x0040) != 0)//A6
                    {
                        _Map.Switch16kPrgRom((((wher + (banks << 5)) << 1) + 1) * 4, 0);
                        _Map.Switch16kPrgRom((((wher + (banks << 5)) << 1) + 1) * 4, 1);
                    }
                    else
                    {
                        _Map.Switch16kPrgRom(((wher + (banks << 5)) << 1) * 4, 0);
                        _Map.Switch16kPrgRom(((wher + (banks << 5)) << 1) * 4, 1);
                    }
                }
                else
                {
                    _Map.Switch32kPrgRom((wher + (banks << 5)) * 8);//ignore A6
                }
                _Map.cartridge.Mirroring = ((address >> 13) & 1) == 0 ? Mirroring.Vertical : Mirroring.Horizontal;
            }
            else if ((address >= 0x5800) && (address <= 0x5FFF))
            {
                switch (address & 0x3)
                {
                    case 0: Mapper225_reg0 = (byte)(data & 0xf); break;
                    case 1: Mapper225_reg1 = (byte)(data & 0xf); break;
                    case 2: Mapper225_reg2 = (byte)(data & 0xf); break;
                    case 3: Mapper225_reg3 = (byte)(data & 0xf); break;
                }
            }
        }
        public void SetUpMapperDefaults()
        {
            _Map.Switch32kPrgRom(0);
            if (_Map.cartridge.IsVRAM)
                _Map.FillCHR(8);
            _Map.Switch8kChrRom(0);
            Mapper225_reg0 = 0xF;
            Mapper225_reg1 = 0xF;
            Mapper225_reg2 = 0xF;
            Mapper225_reg3 = 0xF;
        }
        public void TickScanlineTimer()
        {
        }
        public void TickCycleTimer(int cycles)
        {
        }
        public byte ReadRegisters(ushort address)
        {
            if ((address >= 0x5800) && (address <= 0x5FFF))
            {
                switch (address & 0x3)
                {
                    case 0: return Mapper225_reg0;
                    case 1: return Mapper225_reg1;
                    case 2: return Mapper225_reg2;
                    case 3: return Mapper225_reg3;
                }
            }
            return 0xF;
        }
        public void SoftReset()
        {

        }
        public bool WriteUnder8000
        { get { return true; } }
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
            stream.WriteByte(Mapper225_reg0);
            stream.WriteByte(Mapper225_reg1);
            stream.WriteByte(Mapper225_reg2);
            stream.WriteByte(Mapper225_reg3);
        }
        public void LoadState(System.IO.Stream stream)
        {
            Mapper225_reg0 = (byte)stream.ReadByte();
            Mapper225_reg1 = (byte)stream.ReadByte();
            Mapper225_reg2 = (byte)stream.ReadByte();
            Mapper225_reg3 = (byte)stream.ReadByte();
        }
    }
}

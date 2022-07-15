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
    
    class Mapper32 : IMapper
    {
        CPUMemory Map;
        bool mapper32SwitchingMode = false;
        public Mapper32(CPUMemory Maps)
        { Map = Maps; }
        public void Write(ushort address, byte data)
        {
            switch (address & 0xF000)
            {
                case 0x8000:
                    if (mapper32SwitchingMode)
                    {
                        Map.Switch8kPrgRom(data * 2, 2);
                    }
                    else
                    {
                        Map.Switch8kPrgRom(data * 2, 0);
                    }
                    break;

                case 0x9000:
                    mapper32SwitchingMode = ((data & 0x02) == 0x02);
                    Map.cartridge.Mirroring = ((data & 0x01) == 0) ? Mirroring.Vertical : Mirroring.Horizontal;
                    break;

                case 0xA000:
                    Map.Switch8kPrgRom(data * 2, 1);
                    break;
            }
            switch (address & 0xF007)
            {
                case 0xB000:
                case 0xB001:
                case 0xB002:
                case 0xB003:
                case 0xB004:
                case 0xB005:
                    Map.Switch1kChrRom(data , address & 0x0007);
                    break;
                case 0xB006:
                    Map.Switch1kChrRom(data, 6);
                    break;
                case 0xB007:
                    Map.Switch1kChrRom(data, 7);
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
            stream.WriteByte((byte)(mapper32SwitchingMode ? 1 : 0));
        }
        public void LoadState(System.IO.Stream stream)
        {
            mapper32SwitchingMode = stream.ReadByte() == 1;
        }
    }
}

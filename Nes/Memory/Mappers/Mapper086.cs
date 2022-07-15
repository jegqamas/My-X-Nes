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

    class Mapper86 : IMapper
    {
        byte reg = 0xFF;
        byte cnt = 0;
        CPUMemory map;
        public Mapper86(CPUMemory mem)
        {
            map = mem;
        }
        public void Write(ushort Address, byte data)
        {
            if (Address == 0x6000)
            {
                map.Switch32kPrgRom(((data & 0x30) >> 4) * 8);

                map.Switch8kChrRom(((data & 0x03) | ((data & 0x40) >> 4)) * 8);
            }
            if (Address == 0x7000)
            {
                if ((reg & 0x10) == 0 && (data & 0x10) == 0x10 && cnt == 0)
                {
                    //DEBUGOUT( "WR:$%02X\n", data );
                    if ((data & 0x0F) == 0		// Strike
                     || (data & 0x0F) == 5)
                    {	// Foul
                        cnt = 60;		// ژں‚ج”­گ؛‚ً1•b’ِ‹ضژ~‚·‚é
                    }
                }
                reg = data;
            }
        }
        public void SetUpMapperDefaults()
        {
            map.Switch32kPrgRom(0);
            if (map.cartridge.IsVRAM)
                map.FillCHR(16);
            map.Switch8kChrRom(0);
        }
        public void TickScanlineTimer()
        {
            if (cnt > 0)
            {
                cnt--;
            }
        }
        public void TickCycleTimer(int cycles)
        {
        }
        public void SoftReset()
        { }
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
            stream.WriteByte(reg);
            stream.WriteByte(cnt);
        }
        public void LoadState(System.IO.Stream stream)
        {
            reg = (byte)stream.ReadByte();
            cnt = (byte)stream.ReadByte();
        }
    }
}

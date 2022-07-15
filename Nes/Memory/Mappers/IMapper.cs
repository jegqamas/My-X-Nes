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
    public interface IMapper
    {
        /// <summary>
        /// Write a value into this mapper
        /// </summary>
        /// <param name="address">The address</param>
        /// <param name="data">The value</param>
        void Write(ushort address, byte data);
        /// <summary>
        /// Read a value for the cartridge expansion area 0x4018 - 0x6000
        /// </summary>
        /// <param name="Address">The address</param>
        /// <returns>value at given address</returns>
        byte Read(ushort Address);
        /// <summary>
        /// Setup the mapper (reset)
        /// </summary>
        void SetUpMapperDefaults();
        /// <summary>
        /// Tick the timer that needs to tick in each scanline excapt the VBlank
        /// </summary>
        void TickScanlineTimer();
        /// <summary>
        /// Tick the timer that needs to tick in each cpu cycle
        /// </summary>
        /// <param name="cycles"># of cpu cycles</param>
        void TickCycleTimer(int cycles);
        /// <summary>
        /// Soft Reset the mapper
        /// </summary>
        void SoftReset();
        /// <summary>
        /// Get if this mapper accepts values written from addresses under 0x8000
        /// </summary>
        bool WriteUnder8000 { get; }
        /// <summary>
        /// Get if this mapper accepts values written from addresses under 0x6000
        /// </summary>
        bool WriteUnder6000 { get; }
        /// <summary>
        /// Get if the scanline timer should tick in vblank
        /// </summary>
        bool ScanlineTimerNotPauseAtVBLANK { get; }
        /// <summary>
        /// Save state of this mapper
        /// </summary>
        /// <param name="stream">The state file stream</param>
        void SaveState(System.IO.Stream stream);
        /// <summary>
        /// Load state from file for thi mapper
        /// </summary>
        /// <param name="stream">The state file stream</param>
        void LoadState(System.IO.Stream stream);
    }
}

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
using System.IO;
using MyNes.Nes.Output.Audio;
using MyNes.Nes.Output.Video;
using MyNes.Nes.Input;
using System.Threading;

namespace MyNes.Nes
{
    /// <summary>
    /// The main class of the emulator
    /// </summary>

    public class NES
    {
        CPU _Cpu;
        CPUMemory _Mem;
        PPU _Ppu;
        NesApu _Apu;
        PPUMemory _ppuMemory;
        Cartridge _cartridge;
        IMapper _MAPPER;

        Stream StateStream;
        public Stream SRAMstream;

        public bool ON = false;
        public bool PAUSE = false;
        public bool paused = false;
        public bool SoundEnabled = true;
        public bool AutoSaveSRAM = true;
        int PALaddCycle = 0;

        public ITimer Timer;
        double _currentFrameTime = 0;
        double _lastFrameTime = 0;
        public double FramePeriod = 0.01666666666666667;
        public ushort FPS = 0;

        bool StateSaveRequest = false;
        bool StateLoadRequest = false;
        string SaveStatePath = "";
        bool QuickState = false;
        public bool NoLimiter = false;

        /// <summary>
        /// Create a new nes
        /// </summary>
        /// <param name="Timer">Used to calculate limiting timings</param>
        public NES(ITimer Timer)
        {
            CONSOLE.WriteLine(this, "Initializing the NES engine..", DebugStatus.None);

            _Mem = new Nes.CPUMemory(this);
            _ppuMemory = new Nes.PPUMemory(this);
            _Ppu = new Nes.PPU(this);
            _Cpu = new Nes.CPU(this);
            _Apu = new Nes.NesApu(this);
            _cartridge = new Nes.Cartridge(this);

            this.Timer = Timer;

            CONSOLE.WriteLine(this, "NES engine initialized ok.", DebugStatus.Cool);
        }
        public void SetupVideo(TVFORMAT TV, PaletteFormat paletteFormat, IGraphicDevice GraphicDevice)
        {
            _Ppu.SetPallete(TV, paletteFormat);
            _Ppu.VIDEO = GraphicDevice;
        }
        public void SetupSound(IAudioDevice AudioDevice)
        {
            _Apu.Output = AudioDevice;
        }
        public void SetupControllers(IJoypad Joypad1, IJoypad Joypad2, IInputManager InputManager)
        {
            _Mem.Joypad1 = Joypad1;
            _Mem.Joypad2 = Joypad2;
            _Mem.InputManager = InputManager;
        }

        /// <summary>
        /// Call this after loading a rom 
        /// </summary>
        public void TurnOn()
        {
            _Cpu.Reset();
            ON = true;
        }
        public void RUN()
        {
            while (ON)
            {
                if (!PAUSE)
                {
                    paused = false;
                    int Cyc = _Cpu.Execute();
                    //1 cpu cycle = 3 ppu cycles = 1 apu's
                    //This is the best way I think to ensure
                    //the accurcay.
                    while (Cyc > 0)
                    {
                        //clock once for apu, more accuracy !!
                        _Apu.Execute();
                        //Clock mapper timer
                        _Mem.MAPPER.TickCycleTimer(1);
                        //clock 3 ppu cycles...
                        for (int i = 0; i < 3; i++)
                            _Ppu.RunPPU();
                        //In pal, ppu does 3.2 per 1 cpu cycle
                        //here, every 5 cpu cycles, the ppu
                        //will do 1 additional cycle
                        //0.2 * 5 = 1
                        if (_Ppu.TV == TVFORMAT.PAL)
                        {
                            PALaddCycle++;
                            if (PALaddCycle == 5)
                            {
                                _Ppu.RunPPU();
                                PALaddCycle = 0;
                            }
                        }
                        if (_Ppu.FrameDone)
                        {
                            if (SoundEnabled)
                                _Apu.UpdateBuffer();
                            _Ppu.FrameDone = false;
                            //Handle the speed
                            if (!NoLimiter)
                            {
                                double currentTime = Timer.GetCurrentTime();
                                _currentFrameTime = currentTime - _lastFrameTime;
                                if ((_currentFrameTime < FramePeriod))
                                {
                                    while (true)
                                    {
                                        if ((Timer.GetCurrentTime() - _lastFrameTime) >= FramePeriod)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _Apu.ClearBuffer();
                            }
                            _lastFrameTime = Timer.GetCurrentTime();
                            FPS++;
                            if (FPS >= 1000)
                                FPS = 0;

                        }
                        Cyc--;
                    }
                    _Apu.Play();//When paused
                }
                else
                {
                    paused = true;
                    _Apu.Stop();
                    if (StateSaveRequest)
                        SaveState();
                    if (StateLoadRequest)
                        LoadState();
                }
            }
        }
        /// <summary>
        /// Turn off the nes !!
        /// </summary>
        public void ShutDown()
        {
            PAUSE = true;
            _Ppu.VIDEO.Shutdown();
            _Apu.Shutdown();
            if (_Mem.cartridge.IsBatteryBacked & AutoSaveSRAM)
                SaveSRAM(_Mem.cartridge.SRAMFileName);
            if (SRAMstream != null)
                SRAMstream.Dispose();
            ON = false;
            CONSOLE.WriteLine(this, "SHUTDOWN", DebugStatus.Error);
            CONSOLE.WriteSeparateLine(this, DebugStatus.None);
        }
        public void SaveSRAM(string FilePath)
        {
            //If we have save RAM, try to save it
            try
            {
                SRAMstream.Position = 0;
                SRAMstream.Write(_Mem.SRAM[0], 0, 0x2000);
                CONSOLE.WriteLine(this, "SRAM saved !!", DebugStatus.Cool);
            }
            catch
            {
                CONSOLE.WriteLine(this, "Could not save S-RAM.", DebugStatus.Error);
            }
        }
        //Save/Load state
        public void SaveStateRequest(string FilePath, Stream stateStream, bool QuickState)
        {
            this.StateStream = stateStream;
            PAUSE = true;
            SaveStatePath = FilePath;
            StateSaveRequest = true; 
            this.QuickState = QuickState;
        }
        public void LoadStateRequest(string FilePath, Stream stateStream, bool QuickState)
        {
            this.StateStream = stateStream;
            PAUSE = true;
            SaveStatePath = FilePath;
            StateLoadRequest = true;
            this.QuickState = QuickState;
        }

        #region State
        void SaveState()
        {
            StateStream.WriteByte(0x4D);//M
            StateStream.WriteByte(0x53);//N
            StateStream.WriteByte(0x4E);//S
            //save crc
            StateStream.WriteByte((byte)((_cartridge.CRC32 & 0xFF000000) >> 24));
            StateStream.WriteByte((byte)((_cartridge.CRC32 & 0x00FF0000) >> 16));
            StateStream.WriteByte((byte)((_cartridge.CRC32 & 0x0000FF00) >> 8));
            StateStream.WriteByte((byte)((_cartridge.CRC32 & 0x000000FF)));

            _Cpu.SaveState(StateStream);
            _Mem.SaveState(StateStream);
            _cartridge.SaveState(StateStream);
            _MAPPER.SaveState(StateStream);
            _Ppu.SaveState(StateStream);
            _ppuMemory.SaveState(StateStream);
            _Apu.SaveState(StateStream);

            if (!QuickState)
                StateStream.Dispose();
            StateSaveRequest = false;
            PAUSE = false;
            CONSOLE.WriteLine(this, (QuickState ? "Quick " : "") + "State saved", DebugStatus.Notification);
        }
        void LoadState()
        {
            byte[] header = new byte[3];
            StateStream.Read(header, 0, 3);
            //check header
            if (header[0] != 0x4D | header[1] != 0x53 | header[2] != 0x4E)
            {
                StateLoadRequest = false;
                PAUSE = false;
                CONSOLE.WriteLine(this, "Unable to load state file, unknown file format", DebugStatus.Notification);
                CONSOLE.WriteLine(this, "Unable to load state file, unknown file format", DebugStatus.Error);
                StateStream.Dispose();
                return;
            }
            //check crc
            int crc32 = 0;
            crc32 = (int)(StateStream.ReadByte() << 24);
            crc32 |= (int)(StateStream.ReadByte() << 16);
            crc32 |= (int)(StateStream.ReadByte() << 8);
            crc32 |= StateStream.ReadByte();
            if (_cartridge.CRC32 != crc32)
            {
                StateLoadRequest = false;
                PAUSE = false;
                CONSOLE.WriteLine(this, "Unable to load state file, state file is not for this rom", DebugStatus.Notification);
                CONSOLE.WriteLine(this, "Unable to load state file, state file is not for this rom", DebugStatus.Error);
                return;
            }

            _Cpu.LoadState(StateStream);
            _Mem.LoadState(StateStream);
            _cartridge.LoadState(StateStream);
            _MAPPER.LoadState(StateStream);
            _Ppu.LoadState(StateStream);
            _ppuMemory.LoadState(StateStream);
            _Apu.LoadState(StateStream);

            if (!QuickState)
                StateStream.Dispose();
            StateLoadRequest = false;
            PAUSE = false;
            CONSOLE.WriteLine(this, (QuickState ? "Quick " : "") + "State loaded", DebugStatus.Notification);
        }
        #endregion
        /// <summary>
        /// Load nes rom file
        /// </summary>
        /// <param name="FileName">The full path of the rom file</param>
        /// <param name="ROMstream">The rom stream that will be used for reading, FileAccess must be Read</param>
        /// <param name="SRAMstream">The SRAM file stream that will be used for reading and writing, FileAccess must be ReadWrite</param>
        /// <returns>Load status</returns>
        public LoadRomStatus LoadRom(string FileName, Stream ROMstream, Stream SRAMstream)
        {
            this.SRAMstream = SRAMstream;
            return Cartridge.Load(FileName, ROMstream, SRAMstream, false);
        }
        //Properties
        /// <summary>
        /// The 6502 CPU
        /// </summary>
        public CPU CPU
        { get { return _Cpu; } set { _Cpu = value; } }
        /// <summary>
        /// The nes memory
        /// </summary>
        public CPUMemory Memory
        { get { return _Mem; } set { _Mem = value; } }
        /// <summary>
        /// Get the Picture Processing Unit class
        /// </summary>
        public PPU PPU
        { get { return _Ppu; } set { _Ppu = value; } }
        /// <summary>
        /// Get or set the APU (Audio Processing Unit)
        /// </summary>
        public NesApu APU
        { get { return _Apu; } set { _Apu = value; } }
        /// <summary>
        /// Get or set the ppu memory
        /// </summary>
        public PPUMemory PPUMemory
        { get { return _ppuMemory; } set { _ppuMemory = value; } }
        /// <summary>
        /// Get or set the cartridge of the nes
        /// </summary>
        public Cartridge Cartridge
        { get { return _cartridge; } set { _cartridge = value; } }
        /// <summary>
        /// Get or set the mapper of the nes
        /// </summary>
        public IMapper Mapper
        { get { return _MAPPER; } set { _MAPPER = value; } }
    }
    public enum TVFORMAT
    { PAL, NTSC }
}

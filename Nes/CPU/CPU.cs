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
    /// <summary>
    /// The 6502
    /// </summary>

    public class CPU
    {
        /// <summary>
        /// The 6502
        /// </summary>
        /// <param name="Nes">The nes device</param>
        public CPU(NES Nes)
        {
            _Nes = Nes;
        }
        //Registers
        public byte REG_A = 0;
        public byte REG_X = 0;
        public byte REG_Y = 0;
        public byte REG_S = 0xFF;
        public ushort REG_PC = 0;
        //Flags
        public bool Flag_N = false;
        public bool Flag_V = false;
        public bool Flag_B = false;
        public bool Flag_D = false;
        public bool Flag_I = true;
        public bool Flag_Z = false;
        public bool Flag_C = false;

        bool IRQRequest = false;
        public bool NMIRequest = false;

        public int ADDCycles = 0;
        int CycleCounter = 0;

        NES _Nes;
        byte OpCode = 0;

        /// <summary>
        /// Run the cpu looping
        /// </summary>
        public int Execute()
        {
            CycleCounter = ADDCycles;
            ADDCycles = 0;
            // Do opcode
            OpCode = _Nes.Memory[REG_PC];
            // We may not use both, but it's easier to grab them now
            byte arg1 = _Nes.Memory[(ushort)(REG_PC + 1)];
            byte arg2 = _Nes.Memory[(ushort)(REG_PC + 2)];
            byte M = 0xFF;//The value holder
            #region DO OPCODE
            switch (OpCode)
            {
                #region ADC
                case (0x61):
                    M = IndirectX(arg1);
                    ADC(M, 6, 2);
                    break;
                case (0x65):
                    M = ZeroPage(arg1);
                    ADC(M, 3, 2);
                    break;
                case (0x69):
                    M = arg1;
                    ADC(M, 2, 2);
                    break;
                case (0x6D):
                    M = Absolute(arg1, arg2);
                    ADC(M, 4, 3);
                    break;
                case (0x71):
                    M = IndirectY(arg1, true);
                    ADC(M, 5, 2);
                    break;
                case (0x75):
                    M = ZeroPageX(arg1);
                    ADC(M, 4, 2);
                    break;
                case (0x79):
                    M = AbsoluteY(arg1, arg2, true);
                    ADC(M, 4, 3);
                    break;
                case (0x7D):
                    M = AbsoluteX(arg1, arg2, true);
                    ADC(M, 4, 3);
                    break;
                #endregion
                #region AND
                case (0x21):
                    M = IndirectX(arg1);
                    AND(M, 6, 2);
                    break;
                case (0x25):
                    M = ZeroPage(arg1);
                    AND(M, 3, 2);
                    break;
                case (0x29):
                    M = arg1;
                    AND(M, 2, 2);
                    break;
                case (0x2D):
                    M = Absolute(arg1, arg2);
                    AND(M, 4, 3);
                    break;
                case (0x31):
                    M = IndirectY(arg1, true);
                    AND(M, 5, 2);
                    break;
                case (0x35):
                    M = ZeroPageX(arg1);
                    AND(M, 4, 2);
                    break;
                case (0x39):
                    M = AbsoluteY(arg1, arg2, true);
                    AND(M, 4, 3);
                    break;
                case (0x3D):
                    M = AbsoluteX(arg1, arg2, true);
                    AND(M, 4, 3);
                    break;
                #endregion
                #region ASL
                case (0x06):
                    M = ZeroPage(arg1);
                    ZeroPageWrite(arg1, ASL(M));
                    CycleCounter += 5;
                    REG_PC += 2;
                    break;
                case (0x0A):
                    M = REG_A;
                    REG_A = ASL(M);
                    CycleCounter += 2;
                    REG_PC += 1;
                    break;
                case (0x0E):
                    M = Absolute(arg1, arg2);
                    AbsoluteWrite(arg1, arg2, ASL(M));
                    CycleCounter += 6;
                    REG_PC += 3;
                    break;
                case (0x16):
                    M = ZeroPageX(arg1);
                    ZeroPageXWrite(arg1, ASL(M));
                    CycleCounter += 6;
                    REG_PC += 2;
                    break;
                case (0x1E):
                    M = AbsoluteX(arg1, arg2, false);
                    AbsoluteXWrite(arg1, arg2, ASL(M));
                    CycleCounter += 7;
                    REG_PC += 3;
                    break;
                #endregion
                #region BCC
                case (0x90): Branch(!Flag_C, arg1); break;
                #endregion
                #region BCS
                case (0xb0): Branch(Flag_C, arg1); break;
                #endregion
                #region BEQ
                case (0xf0): Branch(Flag_Z, arg1); break;
                #endregion
                #region BIT
                case (0x24):
                    M = ZeroPage(arg1);
                    BIT(M, 3, 2);
                    break;
                case (0x2c):
                    M = Absolute(arg1, arg2);
                    BIT(M, 4, 3);
                    break;
                #endregion
                #region BMI
                case (0x30): Branch(Flag_N, arg1); break;
                #endregion
                #region BNE
                case (0xd0): Branch(!Flag_Z, arg1); break;
                #endregion
                #region BPL
                case (0x10): Branch(!Flag_N, arg1); break;
                #endregion
                #region BRK
                case (0x00): BRK(); break;
                #endregion
                #region BVC
                case (0x50): Branch(!Flag_V, arg1); break;
                #endregion
                #region BVS
                case (0x70): Branch(Flag_V, arg1); break;
                #endregion
                #region CLC
                case (0x18): CLC(); break;
                #endregion
                #region CLD
                case (0xd8): CLD(); break;
                #endregion
                #region CLI
                case (0x58): CLI(); break;
                #endregion
                #region CLV
                case (0xb8): CLV(); break;
                #endregion
                #region CMP
                case (0xC1):
                    M = IndirectX(arg1);
                    CMP(M, 6, 2);
                    break;
                case (0xC5):
                    M = ZeroPage(arg1);
                    CMP(M, 3, 2);
                    break;
                case (0xC9):
                    M = arg1;
                    CMP(M, 2, 2);
                    break;
                case (0xCD):
                    M = Absolute(arg1, arg2);
                    CMP(M, 4, 3);
                    break;
                case (0xd1):
                    M = IndirectY(arg1, true);
                    CMP(M, 5, 2);
                    break;
                case (0xd5):
                    M = ZeroPageX(arg1);
                    CMP(M, 4, 2);
                    break;
                case (0xd9):
                    M = AbsoluteY(arg1, arg2, true);
                    CMP(M, 4, 3);
                    break;
                case (0xdd):
                    M = AbsoluteX(arg1, arg2, true);
                    CMP(M, 4, 3);
                    break;
                #endregion
                #region CPX
                case (0xE0):
                    M = arg1;
                    CPX(M, 2, 2);
                    break;
                case (0xE4):
                    M = ZeroPage(arg1);
                    CPX(M, 3, 2);
                    break;
                case (0xEC):
                    M = Absolute(arg1, arg2);
                    CPX(M, 4, 3);
                    break;
                #endregion
                #region CPY
                case (0xc0):
                    M = arg1;
                    CPY(M, 2, 2);
                    break;
                case (0xc4):
                    M = ZeroPage(arg1);
                    CPY(M, 3, 2);
                    break;
                case (0xcc):
                    M = Absolute(arg1, arg2);
                    CPY(M, 4, 3);
                    break;
                #endregion
                #region DEC
                case (0xc6):
                    M = ZeroPage(arg1);
                    ZeroPageWrite(arg1, DEC(M));
                    CycleCounter += 5;
                    REG_PC += 2;
                    break;
                case (0xce):
                    M = Absolute(arg1, arg2);
                    AbsoluteWrite(arg1, arg2, DEC(M));
                    CycleCounter += 6;
                    REG_PC += 3;
                    break;
                case (0xd6):
                    M = ZeroPageX(arg1);
                    ZeroPageXWrite(arg1, DEC(M));
                    CycleCounter += 6;
                    REG_PC += 2;
                    break;
                case (0xde):
                    M = AbsoluteX(arg1, arg2, false);
                    AbsoluteXWrite(arg1, arg2, DEC(M));
                    CycleCounter += 7;
                    REG_PC += 3;
                    break;
                #endregion
                #region DEX
                case (0xca): DEX(); break;
                #endregion
                #region DEY
                case (0x88): DEY(); break;
                #endregion
                #region EOR
                case (0x41):
                    M = IndirectX(arg1);
                    EOR(M, 6, 2);
                    break;
                case (0x45):
                    M = ZeroPage(arg1);
                    EOR(M, 3, 2);
                    break;
                case (0x49):
                    M = arg1;
                    EOR(M, 2, 2);
                    break;
                case (0x4d):
                    M = Absolute(arg1, arg2);
                    EOR(M, 4, 3);
                    break;
                case (0x51):
                    M = IndirectY(arg1, true);
                    EOR(M, 5, 2);
                    break;
                case (0x55):
                    M = ZeroPageX(arg1);
                    EOR(M, 4, 2);
                    break;
                case (0x59):
                    M = AbsoluteY(arg1, arg2, true);
                    EOR(M, 4, 3);
                    break;
                case (0x5d):
                    M = AbsoluteX(arg1, arg2, true);
                    EOR(M, 4, 3);
                    break;
                #endregion
                #region INC
                case (0xe6):
                    M = ZeroPage(arg1);
                    ZeroPageWrite(arg1, INC(M));
                    CycleCounter += 5;
                    REG_PC += 2;
                    break;
                case (0xee):
                    M = Absolute(arg1, arg2);
                    AbsoluteWrite(arg1, arg2, INC(M));
                    CycleCounter += 6;
                    REG_PC += 3;
                    break;
                case (0xf6):
                    M = ZeroPageX(arg1);
                    ZeroPageXWrite(arg1, INC(M));
                    CycleCounter += 6;
                    REG_PC += 2;
                    break;
                case (0xfe):
                    M = AbsoluteX(arg1, arg2, false);
                    AbsoluteXWrite(arg1, arg2, INC(M));
                    CycleCounter += 7;
                    REG_PC += 3;
                    break;
                #endregion
                #region INX
                case (0xe8): INX(); break;
                #endregion
                #region INY
                case (0xc8): INY(); break;
                #endregion
                #region JMP
                case (0x4c):
                    REG_PC = Read16((ushort)(REG_PC + 1));
                    CycleCounter += 3;
                    break;
                case (0x6c):
                    ushort myAddress = Read16((ushort)(REG_PC + 1));
                    if ((myAddress & 0x00FF) == 0x00FF)
                    {
                        REG_PC = _Nes.Memory[myAddress];
                        myAddress &= 0xFF00;
                        REG_PC |= (ushort)((_Nes.Memory[myAddress]) << 8);
                    }
                    else
                        REG_PC = Read16(myAddress);
                    CycleCounter += 5;
                    break;
                #endregion
                #region JSR
                case (0x20): JSR(arg1, arg2); break;
                #endregion
                #region LDA
                case (0xa1):
                    REG_A = IndirectX(arg1); LDA();
                    CycleCounter += 6; REG_PC += 2;
                    break;
                case (0xa5):
                    REG_A = ZeroPage(arg1);
                    CycleCounter += 3; REG_PC += 2;
                    LDA();
                    break;
                case (0xa9):
                    REG_A = arg1;
                    CycleCounter += 2; REG_PC += 2;
                    LDA(); break;
                case (0xad):
                    REG_A = Absolute(arg1, arg2);
                    CycleCounter += 4; REG_PC += 3;
                    LDA(); break;
                case (0xb1):
                    REG_A = IndirectY(arg1, true);
                    CycleCounter += 5; REG_PC += 2;
                    LDA();
                    break;
                case (0xb5):
                    REG_A = ZeroPageX(arg1);
                    CycleCounter += 4; REG_PC += 2;
                    LDA();
                    break;
                case (0xb9):
                    REG_A = AbsoluteY(arg1, arg2, true);
                    CycleCounter += 4; REG_PC += 3;
                    LDA(); break;
                case (0xbd):
                    REG_A = AbsoluteX(arg1, arg2, true);
                    CycleCounter += 4; REG_PC += 3;
                    LDA(); break;
                #endregion
                #region LDX
                case (0xa2):
                    REG_X = arg1;
                    CycleCounter += 2; REG_PC += 2;
                    LDX(); break;
                case (0xa6):
                    REG_X = ZeroPage(arg1);
                    CycleCounter += 3; REG_PC += 2;
                    LDX(); break;
                case (0xae):
                    REG_X = Absolute(arg1, arg2);
                    CycleCounter += 4; REG_PC += 3;
                    LDX(); break;
                case (0xb6):
                    REG_X = ZeroPageY(arg1);
                    CycleCounter += 4; REG_PC += 2;
                    LDX(); break;
                case (0xbe): REG_X = AbsoluteY(arg1, arg2, true);
                    CycleCounter += 4; REG_PC += 3;
                    LDX(); break;
                #endregion
                #region LDY
                case (0xa0):
                    REG_Y = arg1;
                    CycleCounter += 2; REG_PC += 2;
                    LDY(); break;
                case (0xa4):
                    REG_Y = ZeroPage(arg1);
                    CycleCounter += 3; REG_PC += 2;
                    LDY(); break;
                case (0xac):
                    REG_Y = Absolute(arg1, arg2);
                    CycleCounter += 4; REG_PC += 3;
                    LDY(); break;
                case (0xb4):
                    REG_Y = ZeroPageX(arg1);
                    CycleCounter += 4; REG_PC += 2;
                    LDY(); break;
                case (0xbc):
                    REG_Y = AbsoluteX(arg1, arg2, true);
                    CycleCounter += 4; REG_PC += 3;
                    LDY(); break;
                #endregion
                #region LSR
                case (0x46):
                    M = ZeroPage(arg1);
                    ZeroPageWrite(arg1, LSR(M));
                    CycleCounter += 5; REG_PC += 2;
                    break;
                case (0x4a):
                    M = REG_A;
                    REG_A = LSR(M);
                    CycleCounter += 2; REG_PC += 1; break;
                case (0x4e):
                    M = Absolute(arg1, arg2);
                    AbsoluteWrite(arg1, arg2, LSR(M));
                    CycleCounter += 6; REG_PC += 3;
                    break;
                case (0x56): M = ZeroPageX(arg1);
                    ZeroPageXWrite(arg1, LSR(M));
                    CycleCounter += 6; REG_PC += 2; break;
                case (0x5e):
                    M = AbsoluteX(arg1, arg2, false);
                    AbsoluteXWrite(arg1, arg2, LSR(M));
                    CycleCounter += 7; REG_PC += 3;
                    break;
                #endregion
                #region NOP
                case (0xEA): NOP(); break;
                #endregion
                #region ORA
                case (0x01):
                    M = IndirectX(arg1);
                    ORA(M, 6, 2);
                    break;
                case (0x05):
                    M = ZeroPage(arg1);
                    ORA(M, 3, 2);
                    break;
                case (0x09):
                    M = arg1;
                    ORA(M, 2, 2);
                    break;
                case (0x0d): M = Absolute(arg1, arg2);
                    ORA(M, 4, 3);
                    break;
                case (0x11):
                    M = IndirectY(arg1, true);
                    ORA(M, 5, 2);
                    break;
                case (0x15):
                    M = ZeroPageX(arg1);
                    ORA(M, 4, 2);
                    break;
                case (0x19):
                    M = AbsoluteY(arg1, arg2, true);
                    ORA(M, 4, 3);
                    break;
                case (0x1d):
                    M = AbsoluteX(arg1, arg2, true);
                    ORA(M, 4, 3);
                    break;
                #endregion
                #region PHA
                case (0x48): PHA(); break;
                #endregion
                #region PHP
                case (0x08): PHP(); break;
                #endregion
                #region PLA
                case (0x68): PLA(); break;
                #endregion
                #region PLP
                case (0x28): PLP(); break;
                #endregion
                #region ROL
                case (0x26):
                    M = ZeroPage(arg1);
                    ZeroPageWrite(arg1, ROL(M));
                    CycleCounter += 5; REG_PC += 2;
                    break;
                case (0x2a):
                    M = REG_A;
                    REG_A = ROL(M);
                    CycleCounter += 2; REG_PC += 1;
                    break;
                case (0x2e):
                    M = Absolute(arg1, arg2);
                    AbsoluteWrite(arg1, arg2, ROL(M));
                    CycleCounter += 6; REG_PC += 3; break;
                case (0x36):
                    M = ZeroPageX(arg1);
                    ZeroPageXWrite(arg1, ROL(M));
                    CycleCounter += 6; REG_PC += 2;
                    break;
                case (0x3e):
                    M = AbsoluteX(arg1, arg2, false);
                    AbsoluteXWrite(arg1, arg2, ROL(M));
                    CycleCounter += 7; REG_PC += 3; break;
                #endregion
                #region ROR
                case (0x66):
                    M = ZeroPage(arg1);
                    ZeroPageWrite(arg1, ROR(M));
                    CycleCounter += 5; REG_PC += 2; break;
                case (0x6a):
                    M = REG_A;
                    REG_A = ROR(M);
                    CycleCounter += 2; REG_PC += 1;
                    break;
                case (0x6e):
                    M = Absolute(arg1, arg2);
                    AbsoluteWrite(arg1, arg2, ROR(M));
                    CycleCounter += 6; REG_PC += 3; break;
                case (0x76):
                    M = ZeroPageX(arg1);
                    ZeroPageXWrite(arg1, ROR(M));
                    CycleCounter += 6; REG_PC += 2; break;
                case (0x7e):
                    M = AbsoluteX(arg1, arg2, false);
                    AbsoluteXWrite(arg1, arg2, ROR(M));
                    CycleCounter += 7; REG_PC += 3; break;
                #endregion
                #region RTI
                case (0x40): RTI(); break;
                #endregion
                #region RTS
                case (0x60): RTS(); break;
                #endregion
                #region SBC
                case (0xe1):
                    M = IndirectX(arg1);
                    SBC(M, 6, 2);
                    break;
                case (0xe5):
                    M = ZeroPage(arg1);
                    SBC(M, 3, 2);
                    break;
                case (0xe9):
                    M = arg1;
                    SBC(M, 2, 2);
                    break;
                case (0xed):
                    M = Absolute(arg1, arg2);
                    SBC(M, 4, 3);
                    break;
                case (0xf1):
                    M = IndirectY(arg1, true);
                    SBC(M, 5, 2);
                    break;
                case (0xf5):
                    M = ZeroPageX(arg1);
                    SBC(M, 4, 2);
                    break;
                case (0xf9):
                    M = AbsoluteY(arg1, arg2, true);
                    SBC(M, 4, 3);
                    break;
                case (0xfd):
                    M = AbsoluteX(arg1, arg2, true);
                    SBC(M, 4, 3);
                    break;
                #endregion
                #region SEC
                case (0x38): SEC(); break;
                #endregion
                #region SED
                case (0xf8): SED(); break;
                #endregion
                #region SEI
                case (0x78): SEI(); break;
                #endregion
                #region STA
                case (0x85): ZeroPageWrite(arg1, REG_A);
                    CycleCounter += 3; REG_PC += 2; break;
                case (0x95): ZeroPageXWrite(arg1, REG_A);
                    CycleCounter += 4; REG_PC += 2; break;
                case (0x8D): AbsoluteWrite(arg1, arg2, REG_A);
                    CycleCounter += 4; REG_PC += 3; break;
                case (0x9D): AbsoluteXWrite(arg1, arg2, REG_A);
                    CycleCounter += 5; REG_PC += 3; break;
                case (0x99): AbsoluteYWrite(arg1, arg2, REG_A);
                    CycleCounter += 5; REG_PC += 3; break;
                case (0x81): IndirectXWrite(arg1, REG_A);
                    CycleCounter += 6; REG_PC += 2; break;
                case (0x91): IndirectYWrite(arg1, REG_A);
                    CycleCounter += 6; REG_PC += 2; break;
                #endregion
                #region STX
                case (0x86): ZeroPageWrite(arg1, REG_X);
                    CycleCounter += 3; REG_PC += 2; break;
                case (0x96): ZeroPageYWrite(arg1, REG_X);
                    CycleCounter += 4; REG_PC += 2; break;
                case (0x8E): AbsoluteWrite(arg1, arg2, REG_X);
                    CycleCounter += 4; REG_PC += 3; break;
                #endregion
                #region STY
                case (0x84): ZeroPageWrite(arg1, REG_Y);
                    CycleCounter += 3; REG_PC += 2; break;
                case (0x94): ZeroPageXWrite(arg1, REG_Y);
                    CycleCounter += 4; REG_PC += 2; break;
                case (0x8C): AbsoluteWrite(arg1, arg2, REG_Y);
                    CycleCounter += 4; REG_PC += 3; break;
                #endregion
                #region TAX
                case (0xaa): TAX(); break;
                #endregion
                #region TAY
                case (0xa8): TAY(); break;
                #endregion
                #region TSX
                case (0xba): TSX(); break;
                #endregion
                #region TXA
                case (0x8a): TXA(); break;
                #endregion
                #region TXS
                case (0x9a): TXS(); break;
                #endregion
                #region TYA
                case (0x98): TYA(); break;
                #endregion
                /*Illegal Opcodes*/
                #region AAC
                case 0x0B:
                case 0x2B:
                    M = arg1;
                    AAC(M, 2, 2);
                    break;
                #endregion
                #region AAX
                case 0x87:
                    M = ZeroPage(arg1);
                    ZeroPageWrite(arg1, AAX(M));
                    REG_PC += 2;
                    CycleCounter += 3;
                    break;
                case 0x97:
                    M = ZeroPageY(arg1);
                    ZeroPageYWrite(arg1, AAX(M));
                    REG_PC += 2;
                    CycleCounter += 4;
                    break;
                case 0x83:
                    M = IndirectX(arg1);
                    IndirectXWrite(arg1, AAX(M));
                    REG_PC += 2;
                    CycleCounter += 6;
                    break;
                case 0x8F:
                    M = Absolute(arg1, arg2);
                    AbsoluteWrite(arg1, arg2, AAX(M));
                    REG_PC += 3;
                    CycleCounter += 4;
                    break;
                #endregion
                #region ARR
                case 0x6B:
                    M = arg1;
                    ARR(M);
                    break;
                #endregion
                #region ASR
                case 0x4B:
                    M = arg1;
                    ASR(M);
                    break;
                #endregion
                #region ATX
                case 0xAB:
                    ATX(arg1);
                    break;
                #endregion
                #region AXA
                case 0x9F:
                    M = AbsoluteY(arg1, arg2, false);
                    AbsoluteYWrite(arg1, arg2, AXA(M));
                    REG_PC += 3;
                    CycleCounter += 5;
                    break;
                case 0x93:
                    M = IndirectY(arg1, false);
                    IndirectYWrite(arg1, AXA(M));
                    REG_PC += 2;
                    CycleCounter += 6;
                    break;
                #endregion
                #region AXS
                case 0xCB: AXS(arg1); break;
                #endregion
                #region DCP
                case 0xC7:
                    M = ZeroPage(arg1);
                    DCP(M, 5, 2);
                    break;
                case 0xD7:
                    M = ZeroPageX(arg1);
                    DCP(M, 6, 2);
                    break;
                case 0xCF:
                    M = Absolute(arg1, arg2);
                    DCP(M, 6, 3);
                    break;
                case 0xDF:
                    M = AbsoluteX(arg1, arg2, true);
                    DCP(M, 7, 3);
                    break;
                case 0xDB:
                    M = AbsoluteY(arg1, arg2, true);
                    DCP(M, 7, 3);
                    break;
                case 0xC3:
                    M = IndirectX(arg1);
                    DCP(M, 8, 2);
                    break;
                case 0xD3:
                    M = IndirectY(arg1, true);
                    DCP(M, 8, 2);
                    break;
                #endregion
                #region DOP
                case 0x14:
                case 0x54:
                case 0x74:
                case 0xD4:
                case 0xF4:
                case 0x34: DOP(4, 2); break;
                case 0x04:
                case 0x64:
                case 0x44: DOP(3, 2); break;
                case 0x82:
                case 0x89:
                case 0xC2:
                case 0xE2:
                case 0x80: DOP(2, 2); break;
                #endregion
                #region ISC
                case 0xE7:
                    M = ZeroPage(arg1);
                    ISC(M, 5, 2);
                    break;
                case 0xF7:
                    M = ZeroPageX(arg1);
                    ISC(M, 6, 2);
                    break;
                case 0xEF:
                    M = Absolute(arg1, arg2);
                    ISC(M, 6, 3);
                    break;
                case 0xFF:
                    M = AbsoluteX(arg1, arg2, true);
                    ISC(M, 7, 3);
                    break;
                case 0xFB:
                    M = AbsoluteY(arg1, arg2, true);
                    ISC(M, 7, 3);
                    break;
                case 0xE3:
                    M = IndirectX(arg1);
                    ISC(M, 8, 2);
                    break;
                case 0xF3:
                    M = IndirectY(arg1, true);
                    ISC(M, 8, 2);
                    break;
                #endregion
                #region KIL
                case 0x02:
                case 0x12:
                case 0x22:
                case 0x32:
                case 0x42:
                case 0x52:
                case 0x62:
                case 0x72:
                case 0x92:
                case 0xB2:
                case 0xD2:
                case 0xF2: KIL(); break;
                #endregion
                #region LAR
                case 0xBB:
                    M = AbsoluteY(arg1, arg2, false);
                    LAR(M, 4, 2);
                    break;
                #endregion
                #region LAX
                case 0xA7:
                    M = ZeroPage(arg1);
                    LAX(M, 3, 2);
                    break;
                case 0xB7:
                    M = ZeroPageY(arg1);
                    LAX(M, 4, 2);
                    break;
                case 0xAF:
                    M = Absolute(arg1, arg2);
                    LAX(M, 4, 3);
                    break;
                case 0xBF:
                    M = AbsoluteY(arg1, arg2, true);
                    LAX(M, 4, 3);
                    break;
                case 0xA3:
                    M = IndirectX(arg1);
                    LAX(M, 6, 2);
                    break;
                case 0xB3:
                    M = IndirectY(arg1, true);
                    LAX(M, 5, 2);
                    break;
                #endregion
                #region NOP
                case 0x1A:
                case 0x3A:
                case 0x5A:
                case 0x7A:
                case 0xDA:
                case 0xFA: NOP(); break;
                #endregion
                #region RLA
                case 0x27:
                    M = ZeroPage(arg1);
                    RLA(M, 5, 2);
                    break;
                case 0x37:
                    M = ZeroPageX(arg1);
                    RLA(M, 6, 2);
                    break;
                case 0x2F:
                    M = Absolute(arg1, arg2);
                    RLA(M, 6, 3);
                    break;
                case 0x3F:
                    M = AbsoluteX(arg1, arg2, false);
                    RLA(M, 7, 3);
                    break;
                case 0x3B:
                    M = AbsoluteY(arg1, arg2, false);
                    RLA(M, 7, 3);
                    break;
                case 0x23:
                    M = IndirectX(arg1);
                    RLA(M, 8, 2);
                    break;
                case 0x33:
                    M = IndirectY(arg1, false);
                    RLA(M, 8, 2);
                    break;
                #endregion
                #region RRA
                case 0x67:
                    M = ZeroPage(arg1);
                    RRA(M, 5, 2);
                    break;
                case 0x77:
                    M = ZeroPageX(arg1);
                    RRA(M, 6, 2);
                    break;
                case 0x6F:
                    M = Absolute(arg1, arg2);
                    RRA(M, 6, 3);
                    break;
                case 0x7F:
                    M = AbsoluteX(arg1, arg2, false);
                    RRA(M, 7, 3);
                    break;
                case 0x7B:
                    M = AbsoluteY(arg1, arg2, false);
                    RRA(M, 7, 3);
                    break;
                case 0x63:
                    M = IndirectX(arg1);
                    RRA(M, 8, 2);
                    break;
                case 0x73:
                    M = IndirectY(arg1, false);
                    RRA(M, 8, 2);
                    break;
                #endregion
                #region SBC
                case 0xEB: M = arg1; SBC(M, 2, 2); break;
                #endregion
                #region SLO
                case 0x07:
                    M = ZeroPage(arg1);
                    SLO(M, 5, 2);
                    break;
                case 0x17:
                    M = ZeroPageX(arg1);
                    SLO(M, 6, 2);
                    break;
                case 0x0F:
                    M = Absolute(arg1, arg2);
                    SLO(M, 6, 3);
                    break;
                case 0x1F:
                    M = AbsoluteX(arg1, arg2, false);
                    SLO(M, 7, 3);
                    break;
                case 0x1B:
                    M = AbsoluteY(arg1, arg2, false);
                    SLO(M, 7, 3);
                    break;
                case 0x03:
                    M = IndirectX(arg1);
                    SLO(M, 8, 2);
                    break;
                case 0x13:
                    M = IndirectY(arg1, false);
                    SLO(M, 8, 2);
                    break;
                #endregion
                #region SRE
                case 0x47: SRE(ZeroPage(arg1), 5, 2); break;
                case 0x57: SRE(ZeroPageX(arg1), 6, 2); break;
                case 0x4F: SRE(Absolute(arg1, arg2), 6, 3); break;
                case 0x5F: SRE(AbsoluteX(arg1, arg2, false), 7, 3); break;
                case 0x5B: SRE(AbsoluteY(arg1, arg2, false), 7, 3); break;
                case 0x43: SRE(IndirectX(arg1), 8, 2); break;
                case 0x53: SRE(IndirectY(arg1, false), 8, 2); break;
                #endregion
                #region SXA
                case 0x9E:
                    AbsoluteYWrite(arg1, arg2, SXA(arg1));
                    break;
                #endregion
                #region SYA
                case 0x9C:
                    AbsoluteXWrite(arg1, arg2, SYA(arg1));
                    break;
                #endregion
                #region TOP
                case 0x0C: TOP(4, 3); break;
                case 0x1C:
                case 0x3C:
                case 0x5C:
                case 0x7C:
                case 0xDC:
                case 0xFC: AbsoluteX(arg1, arg2, true); TOP(4, 3); break;
                #endregion
                #region XAA
                case 0x8B: XAA(M, 2, 2); break;
                #endregion
                #region XAS
                case 0x9B:
                    M = AbsoluteY(arg1, arg2, false);
                    AbsoluteYWrite(arg1, arg2, XAS(M, arg1));
                    CycleCounter += 5;
                    REG_PC += 3;
                    break;
                #endregion
                default://Should not reach here
                    CONSOLE.WriteLine(this, "Unkown OPCODE : 0x" + string.Format("{0:X}", OpCode) + ", PC=0x" + string.Format("{0:X}", REG_PC), DebugStatus.Warning);
                    break;
            }
            #endregion

            // Handle interrupts
            if (NMIRequest)//NMI
            {
                Flag_B = false;
                Push16(REG_PC);
                PushStatus();
                Flag_I = true;
                REG_PC = Read16(0xFFFA);
                CycleCounter += 7;
                NMIRequest = false;
            }
            else if (!Flag_I & IRQRequest)//IRQ
            {
                Flag_B = false;
                Push16(REG_PC);
                PushStatus();
                Flag_I = true;
                REG_PC = Read16(0xFFFE);
                CycleCounter += 7;
                IRQRequest = false;
            }
            return CycleCounter;
        }
        /// <summary>
        /// RST
        /// </summary>
        public void Reset()
        {
            REG_S = 0xFD;
            REG_A = 0;
            REG_X = 0;
            REG_Y = 0;
            Flag_N = false;
            Flag_V = false;
            Flag_B = true;
            Flag_D = false;
            Flag_I = true;
            Flag_Z = false;
            Flag_C = false;
            //Reset memory
            _Nes.Memory[0x08] = 0xF7;
            _Nes.Memory[0x09] = 0xEF;
            _Nes.Memory[0x0A] = 0xDF;
            _Nes.Memory[0x0F] = 0xBF;

            REG_PC = Read16(0xFFFC);

            CONSOLE.WriteLine(this, "CPU RESET", DebugStatus.Warning);
        }
        /// <summary>
        /// Soft Reset
        /// </summary>
        public void SoftReset()
        {
            if (!_Nes.Memory.IsSoftResetTrigger)
            {
                //NMIRequest = false;
                Flag_I = true;
                REG_S -= 3;
                REG_PC = Read16(0xFFFC);
                _Nes.Memory.MAPPER.SoftReset();
                _Nes.APU.SoftReset();
            }
            else
            {
                Reset();
                _Nes.Memory.MAPPER.SoftReset();
                _Nes.Memory.MAPPER.SetUpMapperDefaults();
            }
            CONSOLE.WriteLine(this, "SOFT RESET", DebugStatus.Warning);
        }
        /// <summary>
        /// get two bytes into a correct address
        /// </summary>
        /// <param name="c">Byte A</param>
        /// <param name="d">Byte B</param>
        /// <returns>ushort A & B togather</returns>
        ushort MakeAddress(byte c, byte d)
        {
            return (ushort)((d << 8) | c);
        }
        ushort Read16(ushort Address)
        {
            return (ushort)(_Nes.Memory[Address] | (_Nes.Memory[(ushort)(Address + 1)] << 8));
        }
        /// <summary>
        /// Set an IRQ request
        /// </summary>
        /// <param name="SET"></param>
        public void SetIRQ(bool SET)
        {
            IRQRequest = SET;
        }
        #region Addressing Modes
        byte ZeroPage(ushort A)
        { return _Nes.Memory[A]; }
        byte ZeroPageX(ushort A)
        { return _Nes.Memory[(ushort)(0xFF & (A + REG_X))]; }
        byte ZeroPageY(ushort A)
        { return _Nes.Memory[(ushort)(0xFF & (A + REG_Y))]; }
        byte Absolute(byte A, byte B)
        { return _Nes.Memory[MakeAddress(A, B)]; }
        byte AbsoluteX(byte A, byte B, bool CheckPage)
        {
            if (CheckPage)
            {
                if ((MakeAddress(A, B) & 0xFF00) !=
                   ((MakeAddress(A, B) + REG_X) & 0xFF00))
                {
                    CycleCounter += 1;
                };
            }
            return _Nes.Memory[(ushort)(MakeAddress(A, B) + REG_X)];
        }
        byte AbsoluteY(byte A, byte B, bool CheckPage)
        {
            if (CheckPage)
            {
                if ((MakeAddress(A, B) & 0xFF00) !=
                   ((MakeAddress(A, B) + REG_Y) & 0xFF00))
                {
                    CycleCounter += 1;
                };
            }
            return _Nes.Memory[(ushort)(MakeAddress(A, B) + REG_Y)];
        }
        byte IndirectX(byte A)
        {
            byte l = _Nes.Memory[(ushort)(0xFF & (A + REG_X))];
            byte h = _Nes.Memory[(ushort)(0xFF & ((A + REG_X) + 1))];

            return _Nes.Memory[(ushort)(l | h << 8)];
        }
        byte IndirectY(byte A, bool CheckPage)
        {
            byte l = _Nes.Memory[(ushort)(0xFF & A)];
            byte h = _Nes.Memory[(ushort)(0xFF & (A + 1))];
            ushort addr = (ushort)((l | h << 8) + REG_Y);
            if (CheckPage)
            {
                if ((addr & 0xFF00) !=
                    ((l | h << 8) & 0xFF00))
                {
                    CycleCounter += 1;
                };
            }
            return _Nes.Memory[addr];
        }

        void ZeroPageWrite(ushort A, byte data)
        {
            _Nes.Memory[A] = data;
        }
        void ZeroPageXWrite(ushort A, byte data)
        {
            _Nes.Memory[(ushort)(0xff & (A + REG_X))] = data;
        }
        void ZeroPageYWrite(ushort A, byte data)
        {
            _Nes.Memory[(ushort)(0xff & (A + REG_Y))] = data;
        }
        void AbsoluteWrite(byte A, byte B, byte data)
        {
            _Nes.Memory[MakeAddress(A, B)] = data;
        }
        void AbsoluteXWrite(byte A, byte B, byte data)
        {
            _Nes.Memory[(ushort)(MakeAddress(A, B) + REG_X)] = data;
        }
        void AbsoluteYWrite(byte A, byte B, byte data)
        {
            _Nes.Memory[(ushort)(MakeAddress(A, B) + REG_Y)] = data;
        }
        void IndirectXWrite(byte A, byte data)
        {
            byte l = _Nes.Memory[(ushort)(0xFF & (A + REG_X))];
            byte h = _Nes.Memory[(ushort)(0xFF & ((A + REG_X) + 1))];
            _Nes.Memory[(ushort)(l | h << 8)] = data;
        }
        void IndirectYWrite(byte A, byte data)
        {
            byte l = _Nes.Memory[(ushort)(0xFF & A)];
            byte h = _Nes.Memory[(ushort)(0xFF & (A + 1))];
            ushort addr = (ushort)((l | h << 8) + REG_Y);
            _Nes.Memory[addr] = data;
        }
        #endregion
        #region OPCODES
        void ADC(byte M, int count, ushort bytes)
        {
            int carry_flag = Flag_C ? 1 : 0;
            uint valueholder32 = (uint)(REG_A + M + carry_flag);
            //Set flags
            Flag_V = (((valueholder32 ^ REG_A) & (valueholder32 ^ M)) & 0x80) != 0;
            Flag_C = ((valueholder32 >> 8) != 0);
            Flag_Z = ((valueholder32 & 0xFF) == 0);
            Flag_N = ((valueholder32 & 0x80) == 0x80);
            REG_A = (byte)(valueholder32 & 0xff);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        void AND(byte M, int count, ushort bytes)
        {
            REG_A = (byte)(REG_A & M);
            //Flags
            Flag_Z = (REG_A == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        byte ASL(byte M)
        {
            //Flags
            Flag_C = ((M & 0x80) == 0x80);
            M <<= 1;
            Flag_Z = (M == 0x0);
            Flag_N = ((M & 0x80) == 0x80);
            return M;
        }
        void Branch(bool COND, byte arg1)
        {
            REG_PC += 2;
            if (COND)
            {
                ushort adr = (ushort)(REG_PC + (sbyte)arg1);
                if ((adr & 0xFF00) != ((REG_PC) & 0xFF00))
                {
                    CycleCounter++;
                }
                CycleCounter++;
                REG_PC = adr;
            }
            CycleCounter += 2;
        }
        void BIT(byte M, int count, ushort bytes)
        {
            Flag_Z = ((REG_A & M) == 0x0);
            Flag_N = ((M & 0x80) == 0x80);
            Flag_V = ((M & 0x40) == 0x40);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        void BRK()
        {
            REG_PC += 2;
            Flag_B = true;
            Push16(REG_PC);
            PushStatus();
            Flag_I = true;
            REG_PC = Read16(0xFFFE);
            CycleCounter += 7;
        }
        void CLC()
        {
            Flag_C = false;
            REG_PC += 1;
            CycleCounter += 2;
        }
        void CLD()
        {
            Flag_D = false;
            REG_PC += 1;
            CycleCounter += 2;
        }
        void CLI()
        {
            Flag_I = false;
            REG_PC++;
            CycleCounter += 2;
        }
        void CLV()
        {
            Flag_V = false;
            REG_PC += 1;
            CycleCounter += 2;
        }
        void CMP(byte M, int count, ushort bytes)
        {
            //Flags
            Flag_C = (REG_A >= M);
            Flag_Z = (REG_A == M);
            M = (byte)(REG_A - M);
            Flag_N = ((M & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        void CPX(byte M, int count, ushort bytes)
        {
            Flag_C = (REG_X >= M);
            Flag_Z = (REG_X == M);
            M = (byte)(REG_X - M);
            Flag_N = ((M & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        void CPY(byte M, int count, ushort bytes)
        {
            Flag_C = (REG_Y >= M);
            Flag_Z = (REG_Y == M);
            M = (byte)(REG_Y - M);
            Flag_N = ((M & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        byte DEC(byte M)
        {
            M--;
            Flag_Z = (M == 0x0);
            Flag_N = ((M & 0x80) == 0x80);
            return M;
        }
        void DEX()
        {
            REG_X--;
            Flag_Z = (REG_X == 0x0);
            Flag_N = ((REG_X & 0x80) == 0x80);
            REG_PC++;
            CycleCounter += 2;
        }
        void DEY()
        {
            REG_Y--;
            Flag_Z = (REG_Y == 0x0);
            Flag_N = ((REG_Y & 0x80) == 0x80);
            REG_PC++;
            CycleCounter += 2;
        }
        void EOR(byte M, int count, ushort bytes)
        {
            REG_A = (byte)(REG_A ^ M);
            Flag_Z = (REG_A == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        byte INC(byte M)
        {
            M++;
            Flag_Z = ((M & 0xff) == 0x0);
            Flag_N = ((M & 0x80) == 0x80);
            return M;
        }
        void INX()
        {
            REG_X++;
            Flag_Z = ((REG_X & 0xff) == 0x0);
            Flag_N = ((REG_X & 0x80) == 0x80);
            REG_PC++;
            CycleCounter += 2;
        }
        void INY()
        {
            REG_Y++;
            Flag_Z = ((REG_Y & 0xff) == 0x0);
            Flag_N = ((REG_Y & 0x80) == 0x80);
            REG_PC++;
            CycleCounter += 2;
        }
        void JSR(byte arg1, byte arg2)
        {
            ushort addr = MakeAddress(arg1, arg2);
            REG_PC += 2;
            Push16((ushort)(REG_PC));
            REG_PC = addr;
            CycleCounter += 6;
        }
        void LDA()
        {
            Flag_Z = ((REG_A & 0xFF) == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
        }
        void LDX()
        {
            Flag_Z = (REG_X == 0);
            Flag_N = ((REG_X & 0x80) == 0x80);
        }
        void LDY()
        {
            Flag_Z = (REG_Y == 0);
            Flag_N = ((REG_Y & 0x80) == 0x80);
        }
        byte LSR(byte M)
        {
            Flag_C = ((M & 0x1) == 0x1);
            M = (byte)(M >> 1);
            Flag_Z = ((M & 0xff) == 0x0);
            Flag_N = ((M & 0x80) == 0x80);
            return M;
        }
        void NOP()
        {
            REG_PC++;
            CycleCounter += 2;
        }
        void ORA(byte M, int count, ushort bytes)
        {
            REG_A |= M;
            Flag_Z = ((REG_A & 0xff) == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        void PHA()
        {
            Push8(REG_A);
            REG_PC += 1;
            CycleCounter += 3;
        }
        void PHP()
        {
            Flag_B = true;
            PushStatus();
            REG_PC++;
            CycleCounter += 3;
        }
        void PLA()
        {
            REG_A = Pull8();
            Flag_Z = (REG_A == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            REG_PC += 1;
            CycleCounter += 4;
        }
        void PLP()
        {
            PullStatus();
            REG_PC += 1;
            CycleCounter += 4;
        }
        byte ROL(byte M)
        {
            byte bitholder = 0;
            if ((M & 0x80) == 0x80)
                bitholder = 1;
            else
                bitholder = 0;
            byte carry_flag = (byte)(Flag_C ? 1 : 0);
            M = (byte)(M << 1);
            M = (byte)(M | carry_flag);
            carry_flag = bitholder;
            Flag_C = (bitholder == 1);
            Flag_Z = ((M & 0xff) == 0x0);
            Flag_N = ((M & 0x80) == 0x80);
            return M;
        }
        byte ROR(byte M)
        {
            byte bitholder = 0;
            byte carry_flag = (byte)(Flag_C ? 1 : 0);
            if ((M & 0x1) == 0x1)
                bitholder = 1;
            else
                bitholder = 0;
            M = (byte)(M >> 1);

            if (carry_flag == 1)
                M = (byte)(M | 0x80);
            Flag_C = (bitholder == 1);
            Flag_Z = ((M & 0xff) == 0x0);
            Flag_N = ((M & 0x80) == 0x80);
            return M;
        }
        void RTI()
        {
            PullStatus();
            REG_PC = Pull16();
            CycleCounter += 6;
        }
        void RTS()
        {
            REG_PC = Pull16();
            CycleCounter += 6;
            REG_PC++;
        }
        void SBC(byte M, int count, ushort bytes)
        {
            int C = (!Flag_C) ? 1 : 0;
            ushort valueholder32 = (ushort)(REG_A - M - C);
            Flag_C = !((valueholder32 >> 8) != 0);
            Flag_Z = ((valueholder32 & 0xFF) == 0);
            Flag_V = ((REG_A ^ M) & (REG_A ^ valueholder32) & 0x80) != 0;
            Flag_N = ((valueholder32 & 0x80) == 0x80);
            REG_A = (byte)(valueholder32 & 0xFF);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        void SEC()
        {
            Flag_C = true;
            CycleCounter += 2;
            REG_PC += 1;
        }
        void SED()
        {
            Flag_D = true;
            CycleCounter += 2;
            REG_PC += 1;
        }
        void SEI()
        {
            Flag_I = true;
            CycleCounter += 2;
            REG_PC += 1;
        }
        void TAX()
        {
            REG_X = REG_A;
            Flag_Z = (REG_X == 0);
            Flag_N = ((REG_X & 0x80) == 0x80);
            REG_PC += 1;
            CycleCounter += 2;
        }
        void TAY()
        {
            REG_Y = REG_A;
            Flag_Z = (REG_Y == 0);
            Flag_N = ((REG_Y & 0x80) == 0x80);
            REG_PC += 1;
            CycleCounter += 2;
        }
        void TSX()
        {
            REG_X = REG_S;
            Flag_Z = (REG_X == 0);
            Flag_N = ((REG_X & 0x80) == 0x80);
            REG_PC += 1;
            CycleCounter += 2;
        }
        void TXA()
        {
            REG_A = REG_X;
            Flag_Z = (REG_A == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            REG_PC += 1;
            CycleCounter += 2;
        }
        void TXS()
        {
            REG_S = REG_X;
            REG_PC += 1;
            CycleCounter += 2;
        }
        void TYA()
        {
            REG_A = REG_Y;
            Flag_Z = (REG_A == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            REG_PC += 1;
            CycleCounter += 2;
        }
        /*Illegall Opcodes, not sure if all work*/
        void AAC(byte M, int count, ushort bytes)
        {
            REG_A &= M;
            Flag_C = ((REG_A & 0x80) == 0x80);
            Flag_Z = ((REG_A & 0xFF) == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        byte AAX(byte M)
        {
            //byte temp = (byte)((REG_X & REG_A) - M);
            //Flag_Z = (temp == 0);
            //Flag_N = ((temp & 0x80) == 0x80);
            return (byte)(REG_X & REG_A);
        }
        void ARR(byte M)
        {
            byte carry_flag = (byte)(Flag_C ? 1 : 0);
            REG_A = (byte)(((M & REG_A) >> 1) | (carry_flag << 7));
            Flag_Z = ((REG_A & 0xFF) == 0x0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            Flag_C = ((REG_A >> 6) & 0x1) == 0x1;
            Flag_V = ((REG_A >> 6 ^ REG_A >> 5) & 0x1) == 0x1;
            //Advance
            REG_PC += 2;
            CycleCounter += 2;
        }
        void ASR(byte M)
        {
            REG_A &= M;
            Flag_C = ((REG_A & 0x1) == 0x1);
            REG_A >>= 1;
            Flag_Z = ((REG_A & 0xFF) == 0x0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            REG_PC += 2;
            CycleCounter += 2;
        }
        void ATX(byte M)
        {
            REG_A &= M;
            REG_X = REG_A;

            Flag_Z = REG_A == 0;
            Flag_N = ((REG_A & 0x80) != 0);
            //Advance
            REG_PC += 2;
            CycleCounter += 2;
        }
        byte AXA(byte M)
        {
            M = (byte)((REG_A & REG_X) & 7);
            return M;
        }
        void AXS(byte M)
        {
            ushort tmp = (ushort)((REG_A & REG_X) - M);
            //Flag_C = (tmp <= 0xFF);
            Flag_C = (tmp >> 8) != 0;
            REG_X = (byte)(tmp & 0xFF);
            Flag_Z = ((REG_X & 0xFF) == 0);
            Flag_N = ((REG_X & 0x80) == 0x80);
            //Advance
            REG_PC += 2;
            CycleCounter += 2;
        }
        void DCP(byte M, int count, ushort bytes)
        {
            //M = (byte)((M - 1) & 0xFF);
            M--;
            CMP(M, count, bytes);
            //Flags

            //Flag_Z = (M == OldM);
            //M = (byte)(M - OldM);
            //Flag_N = ((M & 0x80) == 0x80);

            //Advance

        }
        void DOP(int count, ushort bytes)
        {
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        void ISC(byte M, int count, ushort bytes)
        {
            M++;
            SBC(M, count, bytes);
        }
        void KIL()
        {
            REG_PC++;
        }
        void LAR(byte M, int count, ushort bytes)
        {
            REG_S &= M;
            REG_X = REG_A = REG_S;
            Flag_Z = ((REG_X & 0xFF) == 0);
            Flag_N = ((REG_X & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        void LAX(byte M, int count, ushort bytes)
        {
            REG_X = REG_A = M;
            Flag_Z = ((REG_A & 0xFF) == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        void RLA(byte M, int count, ushort bytes)
        {
            byte d0 = (byte)(Flag_C ? 0x01 : 0x00);
            byte tmp = (byte)(M << 1);
            tmp |= d0;
            Flag_C = (tmp & 0x80) != 0;
            M = tmp;

            REG_A &= M;

            Flag_Z = ((REG_A & 0xFF) == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        void RRA(byte M, int count, ushort bytes)
        {
            byte d0 = (byte)(Flag_C ? 0x80 : 0x00);
            byte tmp = (byte)(M >> 1);
            tmp |= d0;
            Flag_C = (tmp & 0x1) != 0;
            M = tmp;
            ADC(M, count, bytes);
            // M >>= 1;
            // if (Flag_C)
            //     M |= 0x80;
            // Flag_C = (M & 1) != 0;
            // ADC(M, count, bytes);
        }
        void SLO(byte M, int count, ushort bytes)
        {
            Flag_C = (M & 0x80) != 0;
            M = (byte)((M << 1) & 0xFF);
            REG_A = (byte)(REG_A | M);
            Flag_Z = ((REG_A & 0xFF) == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        void SRE(byte M, int count, ushort bytes)
        {
            Flag_C = (M & 0x1) != 0;
            M >>= 1;
            REG_A = (byte)(REG_A ^ M);
            Flag_Z = ((REG_A & 0xFF) == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            //Advance
            CycleCounter += count;
            REG_PC += bytes;
        }
        byte SXA(byte M)
        {
            byte tmp = (byte)(REG_X & (M + 1));
            //Advance
            CycleCounter += 5;
            REG_PC += 3;
            return tmp;
        }
        byte SYA(byte M)
        {
            byte tmp = (byte)(REG_Y & (M + 1));
            //Advance
            CycleCounter += 5;
            REG_PC += 3;
            return tmp;
        }
        void TOP(int count, ushort bytes)
        {
            CycleCounter += count;
            REG_PC += bytes;
        }
        void XAA(byte M, int count, ushort bytes)
        {
            REG_A = (byte)(REG_X & M);
            Flag_Z = ((REG_A & 0xFF) == 0);
            Flag_N = ((REG_A & 0x80) == 0x80);
            CycleCounter += count;
            REG_PC += bytes;
        }
        byte XAS(byte M, byte arg1)
        {
            //REG_S = (byte)(REG_X & REG_A);
            //return (byte)(REG_S & (arg1 + 1));
            REG_S = (byte)(REG_A & REG_X & ((M >> 8) + 1));
            return REG_S;
        }
        #endregion
        #region (pull, Push ...)
        void Push8(byte data)
        {
            _Nes.Memory[(ushort)(0x100 + REG_S)] = data;
            REG_S--;
        }
        public void Push16(ushort data)
        {
            Push8((byte)((data & 0xFF00) >> 8));
            Push8((byte)(data & 0x00FF));
        }
        public void PushStatus()
        {
            byte statusdata = 0;

            if (Flag_N)
                statusdata = (byte)(statusdata | 0x80);

            if (Flag_V)
                statusdata = (byte)(statusdata | 0x40);

            statusdata = (byte)(statusdata | 0x20);

            if (Flag_B)
                statusdata = (byte)(statusdata | 0x10);

            if (Flag_D)
                statusdata = (byte)(statusdata | 0x08);

            if (Flag_I)
                statusdata = (byte)(statusdata | 0x04);

            if (Flag_Z)
                statusdata = (byte)(statusdata | 0x02);

            if (Flag_C)
                statusdata = (byte)(statusdata | 0x01);
            Push8(statusdata);
        }
        public byte StatusRegister()
        {
            byte statusdata = 0;

            if (Flag_N)
                statusdata = (byte)(statusdata | 0x80);

            if (Flag_V)
                statusdata = (byte)(statusdata | 0x40);

            statusdata = (byte)(statusdata | 0x20);

            if (Flag_B)
                statusdata = (byte)(statusdata | 0x10);

            if (Flag_D)
                statusdata = (byte)(statusdata | 0x08);

            if (Flag_I)
                statusdata = (byte)(statusdata | 0x04);

            if (Flag_Z)
                statusdata = (byte)(statusdata | 0x02);

            if (Flag_C)
                statusdata = (byte)(statusdata | 0x01);

            return statusdata;
        }
        byte Pull8()
        {
            REG_S++;
            return _Nes.Memory[(ushort)(0x100 + REG_S)];
        }
        ushort Pull16()
        {
            byte data1 = Pull8();
            byte data2 = Pull8();
            return MakeAddress(data1, data2);
        }
        void PullStatus()
        {
            byte statusdata = Pull8();
            Flag_N = ((statusdata & 0x80) == 0x80);
            Flag_V = ((statusdata & 0x40) == 0x40);
            Flag_B = ((statusdata & 0x10) == 0x10);
            Flag_D = ((statusdata & 0x08) == 0x08);
            Flag_I = ((statusdata & 0x04) == 0x04);
            Flag_Z = ((statusdata & 0x02) == 0x02);
            Flag_C = ((statusdata & 0x01) == 0x01);
        }
        #endregion

        public void SaveState(System.IO.Stream stream)
        {
            //registers
            stream.WriteByte(REG_A);
            stream.WriteByte(REG_X);
            stream.WriteByte(REG_Y);
            stream.WriteByte(REG_S);
            stream.WriteByte((byte)((REG_PC & 0xFF00) >> 8));
            stream.WriteByte((byte)((REG_PC & 0x00FF)));
            //status
            byte statusdata = 0;

            if (Flag_N)
                statusdata = (byte)(statusdata | 0x80);
            if (Flag_V)
                statusdata = (byte)(statusdata | 0x40);
            if (Flag_B)
                statusdata = (byte)(statusdata | 0x10);
            if (Flag_D)
                statusdata = (byte)(statusdata | 0x08);
            if (Flag_I)
                statusdata = (byte)(statusdata | 0x04);
            if (Flag_Z)
                statusdata = (byte)(statusdata | 0x02);
            if (Flag_C)
                statusdata = (byte)(statusdata | 0x01);
            stream.WriteByte(statusdata);
            //IRQ/NMI
            byte interrupts = 0;
            if (IRQRequest)
                interrupts = (byte)(interrupts | 0x01);
            if (NMIRequest)
                interrupts = (byte)(interrupts | 0x02);
            stream.WriteByte(interrupts);
            //opcode
            stream.WriteByte(OpCode);
        }
        public void LoadState(System.IO.Stream stream)
        {
            //registers
            REG_A = (byte)stream.ReadByte();
            REG_X = (byte)stream.ReadByte();
            REG_Y = (byte)stream.ReadByte();
            REG_S = (byte)stream.ReadByte();
            REG_PC = (ushort)(stream.ReadByte() << 8);
            REG_PC = (ushort)(REG_PC & 0xFF00 | (stream.ReadByte() & 0x00FF));
            //status
            byte statusdata = (byte)stream.ReadByte();
            Flag_N = ((statusdata & 0x80) == 0x80);
            Flag_V = ((statusdata & 0x40) == 0x40);
            Flag_B = ((statusdata & 0x10) == 0x10);
            Flag_D = ((statusdata & 0x08) == 0x08);
            Flag_I = ((statusdata & 0x04) == 0x04);
            Flag_Z = ((statusdata & 0x02) == 0x02);
            Flag_C = ((statusdata & 0x01) == 0x01);
            //IRQ/NMI
            byte interrupts = (byte)stream.ReadByte();
            IRQRequest = ((interrupts & 0x01) == 0x01);
            NMIRequest = ((interrupts & 0x02) == 0x02);
            //opcode
            OpCode = (byte)stream.ReadByte();
        }
    }
}
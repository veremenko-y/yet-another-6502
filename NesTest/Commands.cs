﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesTest
{
    public enum Command
    {
        AdcIndY = 0x71,
        AdcIndX = 0x61,
        AdcAbs = 0x6D,
        AdcAbsX = 0x7D,
        AdcAbsY = 0x79,
        AdcImm = 0x69,
        AdcZp = 0x65,
        AdcZpX = 0x75,
        AndIndY = 0x31,
        AndIndX = 0x21,
        AndAbs = 0x2D,
        AndAbsX = 0x3D,
        AndAbsY = 0x39,
        AndImm = 0x29,
        AndZp = 0x25,
        AndZpX = 0x35,
        AslAbs = 0x0E,
        AslAbsX = 0x1E,
        AslAcc = 0x0A,
        AslZp = 0x06,
        AslZpX = 0x16,
        Bcc = 0x90,
        Bcs = 0xB0,
        Beq = 0xF0,
        BitAbs = 0x2C,
        BitZp = 0x24,
        Bmi = 0x30,
        Bne = 0xD0,
        Bpl = 0x10,
        Brk = 0x00,
        Bvc = 0x50,
        Bvs = 0x70,
        Clc = 0x18,
        Cld = 0xD8,
        Cli = 0x58,
        Clv = 0xB8,
        CmpIndY = 0xD1,
        CmpIndX = 0xC1,
        CmpAbs = 0xCD,
        CmpAbsX = 0xDD,
        CmpAbsY = 0xD9,
        CmpImm = 0xC9,
        CmpZp = 0xC5,
        CmpZpX = 0xD5,
        CpxAbs = 0xEC,
        CpxImm = 0xE0,
        CpxZp = 0xE4,
        CpyAbs = 0xCC,
        CpyImm = 0xC0,
        CpyZp = 0xC4,
        DecAbs = 0xCE,
        DecAbsX = 0xDE,
        DecZp = 0xC6,
        DecZpX = 0xD6,
        Dex = 0xCA,
        Dey = 0x88,
        EorIndY = 0x51,
        EorIndX = 0x41,
        EorAbs = 0x4D,
        EorAbsX = 0x5D,
        EorAbsY = 0x59,
        EorImm = 0x49,
        EorZp = 0x45,
        EorZpX = 0x55,
        IncAbs = 0xEE,
        IncAbsX = 0xFE,
        IncZp = 0xE6,
        IncZpX = 0xF6,
        Inx = 0xE8,
        Iny = 0xC8,
        JmpAbs = 0x4C,
        JmpInd = 0x6C,
        Jsr = 0x20,
        LdaIndY = 0xB1,
        LdaIndX = 0xA1,
        LdaAbs = 0xAD,
        LdaAbsX = 0xBD,
        LdaAbsY = 0xB9,
        LdaImm = 0xA9,
        LdaZp = 0xA5,
        LdaZpX = 0xB5,
        LdxAbs = 0xAE,
        LdxAbsY = 0xBE,
        LdxImm = 0xA2,
        LdxZp = 0xA6,
        LdxZpY = 0xB6,
        LdyAbs = 0xAC,
        LdyAbsX = 0xBC,
        LdyImm = 0xA0,
        LdyZp = 0xA4,
        LdyZpX = 0xB4,
        LsrAbs = 0x4E,
        LsrAbsX = 0x5E,
        LsrAcc = 0x4A,
        LsrZp = 0x46,
        LsrZpX = 0x56,
        Nop = 0xEA,
        OraIndY = 0x11,
        OraIndX = 0x01,
        OraAbs = 0x0D,
        OraAbsX = 0x1D,
        OraAbsY = 0x19,
        OraImm = 0x09,
        OraZp = 0x05,
        OraZpX = 0x15,
        Pha = 0x48,
        Php = 0x08,
        Pla = 0x68,
        Plp = 0x28,
        RolAbs = 0x2E,
        RolAbsX = 0x3E,
        RolAcc = 0x2A,
        RolZp = 0x26,
        RolZpX = 0x36,
        RorAbs = 0x6E,
        RorAbsX = 0x7E,
        RorAcc = 0x6A,
        RorZp = 0x66,
        RorZpX = 0x76,
        Rti = 0x40,
        Rts = 0x60,
        SbcIndY = 0xF1,
        SbcIndX = 0xE1,
        SbcAbs = 0xED,
        SbcAbsX = 0xFD,
        SbcAbsY = 0xF9,
        SbcImm = 0xE9,
        SbcZp = 0xE5,
        SbcZpX = 0xF5,
        Sec = 0x38,
        Sed = 0xF8,
        Sei = 0x78,
        StaIndY = 0x91,
        StaIndX = 0x81,
        StaAbs = 0x8D,
        StaAbsX = 0x9D,
        StaAbsY = 0x99,
        StaZp = 0x85,
        StaZpX = 0x95,
        StxAbs = 0x8E,
        StxZp = 0x86,
        StxZpY = 0x96,
        StyAbs = 0x8C,
        StyZp = 0x84,
        StyZpX = 0x94,
        Tax = 0xAA,
        Tay = 0xA8,
        Tsx = 0xBA,
        Txa = 0x8A,
        Txs = 0x9A,
        Tya = 0x98
    }
}

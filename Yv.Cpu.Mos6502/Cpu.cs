using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NesTest
{
    public delegate void IoEvent(ref IoEventArgs e);

    public partial class Cpu
    {
        public Registers regs;
        public int clock;
        public Command command;

        private List<IoMapping> ioMapping = new List<IoMapping>();

        public Cpu()
        {
            clock = 0;
            regs = new Registers
            {
                Status = Status.Na
            };
        }

        public void RegisterIo(ushort addr, IoEvent handler)
        {
            ioMapping.Add(new IoMapping(addr, addr, handler));
        }

        public void RegisterIo(ushort addrStart, ushort addrEnd, IoEvent handler)
        {
            if (addrStart > addrEnd)
                throw new InvalidOperationException();
            ioMapping.Add(new IoMapping(addrStart, addrEnd, handler));
        }

        public void Step()
        {
            command = (Command)NextByte();
            Execute(command);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte NextByte()
        {
            byte cell = GetByte(regs.PcLow, regs.PcHigh);
            regs.PcAdd(1);
            return cell;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected byte GetByte(byte low, byte high)
        {
            return GetByte((ushort)(low + (high << 8)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected byte GetByte(ushort address)
        {
            for (var i = 0; i < ioMapping.Count; i++)
            {
                var mapping = ioMapping[i];
                if (!mapping.IsInRange(address))
                    continue;
                var e = IoEventArgs.Read(address);
                mapping.Handler(ref e);
                if (e.ReadSuccess)
                {
                    return e.Value;
                }
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetAddr(byte low, byte high)
        {
            return (ushort)(low + (high << 8));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetByte(byte low, byte high, byte val)
        {
            SetByte(GetAddr(low, high), val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetByte(ushort address, byte value)
        {
            for (var i = 0; i < ioMapping.Count; i++)
            {
                var mapping = ioMapping[i];
                if (!mapping.IsInRange(address))
                    continue;
                var e = IoEventArgs.Write(address, value);
                mapping.Handler(ref e);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Push(byte val)
        {
            SetByte(regs.Sp--, 0x01, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte Pop()
        {
            return GetByte(++regs.Sp, 0x01);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetRelAddr(byte op)
        {
            ushort rt = GetAddr(regs.PcLow, regs.PcHigh);
            rt = (ushort)(rt + (sbyte)op);
            return rt;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetAbsAddr()
        {
            return GetAddr(NextByte(), NextByte());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetAbsXAddr()
        {
            ushort rt = (ushort)(NextByte() + (NextByte() << 8) + regs.Xr);
            return GetAddr((byte)rt, (byte)(rt >> 8));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetAbsYAddr()
        {
            ushort rt = (ushort)(NextByte() + (NextByte() << 8) + regs.Yr);
            return GetAddr((byte)rt, (byte)(rt >> 8));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetPreIndXAddr()
        {
            ushort rt = (ushort)(NextByte() + regs.Xr);
            rt = (ushort)(rt & 0x00FF); // wraparound;
            return GetAddr(
                GetByte((byte)rt, (byte)(rt >> 8)),
                GetByte((byte)(rt + 1), (byte)((rt + 1) >> 8)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetPostIndYAddr()
        {
            byte op = NextByte();
            ushort rt = (ushort)(GetByte(op, 0x00) +
                                  (GetByte((byte)(op + 1), 0x00) << 8) +
                                  regs.Yr);
            return GetAddr((byte)rt, (byte)(rt >> 8));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetZpAddr()
        {
            return GetAddr(NextByte(), 0x00);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetZpXAddr()
        {
            return GetAddr((byte)(NextByte() + regs.Xr), 0x00);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetZpYAddr()
        {
            return GetAddr((byte)(NextByte() + regs.Yr), 0x00);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Execute(Command instruction)
        {
            switch (instruction)
            {
                case Command.AdcIndY:
                    Add(GetByte(GetPostIndYAddr()));
                    break;
                case Command.AdcIndX:
                    Add(GetByte(GetPreIndXAddr()));
                    break;
                case Command.AdcAbs:
                    Add(GetByte(GetAbsAddr()));
                    break;
                case Command.AdcAbsX:
                    Add(GetByte(GetAbsXAddr()));
                    break;
                case Command.AdcAbsY:
                    Add(GetByte(GetAbsYAddr()));
                    break;
                case Command.AdcImm:
                    Add(NextByte());
                    break;
                case Command.AdcZp:
                    Add(GetByte(GetZpAddr()));
                    break;
                case Command.AdcZpX:
                    Add(GetByte(GetZpXAddr()));
                    break;
                case Command.AndIndY:
                    And(GetByte(GetPostIndYAddr()));
                    break;
                case Command.AndIndX:
                    And(GetByte(GetPreIndXAddr()));
                    break;
                case Command.AndAbs:
                    And(GetByte(GetAbsAddr()));
                    break;
                case Command.AndAbsX:
                    And(GetByte(GetAbsXAddr()));
                    break;
                case Command.AndAbsY:
                    And(GetByte(GetAbsYAddr()));
                    break;
                case Command.AndImm:
                    And(NextByte());
                    break;
                case Command.AndZp:
                    And(GetByte(GetZpAddr()));
                    break;
                case Command.AndZpX:
                    And(GetByte(GetZpXAddr()));
                    break;
                case Command.AslAbs:
                    Asl(GetAbsAddr());
                    break;
                case Command.AslAbsX:
                    Asl(GetAbsXAddr());
                    break;
                case Command.AslAcc:
                    Asl();
                    break;
                case Command.AslZp:
                    Asl(GetZpAddr());
                    break;
                case Command.AslZpX:
                    Asl(GetZpXAddr());
                    break;
                case Command.Bcc:
                    Bcc(NextByte());
                    break;
                case Command.Bcs:
                    Bcs(NextByte());
                    break;
                case Command.Beq:
                    Beq(NextByte());
                    break;
                case Command.BitAbs:
                    Bit(GetByte(GetAbsAddr()));
                    break;
                case Command.BitZp:
                    Bit(GetByte(GetZpAddr()));
                    break;
                case Command.Bmi:
                    Bmi(NextByte());
                    break;
                case Command.Bne:
                    Bne(NextByte());
                    break;
                case Command.Bpl:
                    Bpl(NextByte());
                    break;
                case Command.Brk:
                    Brk();
                    break;
                case Command.Bvc:
                    Bvc(NextByte());
                    break;
                case Command.Bvs:
                    Bvs(NextByte());
                    break;
                case Command.Clc:
                    regs.Reset(Status.Carry);
                    break;
                case Command.Cld:
                    regs.Reset(Status.Decimal);
                    break;
                case Command.Cli:
                    regs.Reset(Status.Interrupt);
                    break;
                case Command.Clv:
                    regs.Reset(Status.VOverflow);
                    break;
                case Command.CmpIndY:
                    Cmp(GetByte(GetPostIndYAddr()));
                    break;
                case Command.CmpIndX:
                    Cmp(GetByte(GetPreIndXAddr()));
                    break;
                case Command.CmpAbs:
                    Cmp(GetByte(GetAbsAddr()));
                    break;
                case Command.CmpAbsX:
                    Cmp(GetByte(GetAbsXAddr()));
                    break;
                case Command.CmpAbsY:
                    Cmp(GetByte(GetAbsYAddr()));
                    break;
                case Command.CmpImm:
                    Cmp(NextByte());
                    break;
                case Command.CmpZp:
                    Cmp(GetByte(GetZpAddr()));
                    break;
                case Command.CmpZpX:
                    Cmp(GetByte(GetZpXAddr()));
                    break;
                case Command.CpxAbs:
                    Cpx(GetByte(GetAbsAddr()));
                    break;
                case Command.CpxImm:
                    Cpx(NextByte());
                    break;
                case Command.CpxZp:
                    Cpx(GetByte(GetZpAddr()));
                    break;
                case Command.CpyAbs:
                    Cpy(GetByte(GetAbsAddr()));
                    break;
                case Command.CpyImm:
                    Cpy(NextByte());
                    break;
                case Command.CpyZp:
                    Cpy(GetByte(GetZpAddr()));
                    break;
                case Command.DecAbs:
                    Dec(GetAbsAddr());
                    break;
                case Command.DecAbsX:
                    Dec(GetAbsXAddr());
                    break;
                case Command.DecZp:
                    Dec(GetZpAddr());
                    break;
                case Command.DecZpX:
                    Dec(GetZpXAddr());
                    break;
                case Command.Dex:
                    Dex();
                    break;
                case Command.Dey:
                    Dey();
                    break;
                case Command.EorIndY:
                    Eor(GetByte(GetPostIndYAddr()));
                    break;
                case Command.EorIndX:
                    Eor(GetByte(GetPreIndXAddr()));
                    break;
                case Command.EorAbs:
                    Eor(GetByte(GetAbsAddr()));
                    break;
                case Command.EorAbsX:
                    Eor(GetByte(GetAbsXAddr()));
                    break;
                case Command.EorAbsY:
                    Eor(GetByte(GetAbsYAddr()));
                    break;
                case Command.EorImm:
                    Eor(NextByte());
                    break;
                case Command.EorZp:
                    Eor(GetByte(GetZpAddr()));
                    break;
                case Command.EorZpX:
                    Eor(GetByte(GetZpXAddr()));
                    break;
                case Command.IncAbs:
                    Inc(GetAbsAddr());
                    break;
                case Command.IncAbsX:
                    Inc(GetAbsXAddr());
                    break;
                case Command.IncZp:
                    Inc(GetZpAddr());
                    break;
                case Command.IncZpX:
                    Inc(GetZpXAddr());
                    break;
                case Command.Inx:
                    Inx();
                    break;
                case Command.Iny:
                    Iny();
                    break;
                case Command.JmpAbs:
                    Jmp(GetAbsAddr());
                    break;
                case Command.JmpInd:
                    var jmpAddr = GetAbsAddr();
                    Jmp(GetAddr(GetByte(jmpAddr), GetByte((ushort)(jmpAddr + 1))));
                    break;
                case Command.Jsr:
                    Jsr(GetAbsAddr());
                    break;
                case Command.LdaIndY:
                    Lda(GetByte(GetPostIndYAddr()));
                    break;
                case Command.LdaIndX:
                    Lda(GetByte(GetPreIndXAddr()));
                    break;
                case Command.LdaAbs:
                    Lda(GetByte(GetAbsAddr()));
                    break;
                case Command.LdaAbsX:
                    Lda(GetByte(GetAbsXAddr()));
                    break;
                case Command.LdaAbsY:
                    Lda(GetByte(GetAbsYAddr()));
                    break;
                case Command.LdaImm:
                    Lda(NextByte());
                    break;
                case Command.LdaZp:
                    Lda(GetByte(GetZpAddr()));
                    break;
                case Command.LdaZpX:
                    Lda(GetByte(GetZpXAddr()));
                    break;
                case Command.LdxAbs:
                    Ldx(GetByte(GetAbsAddr()));
                    break;
                case Command.LdxAbsY:
                    Ldx(GetByte(GetAbsYAddr()));
                    break;
                case Command.LdxImm:
                    Ldx(NextByte());
                    break;
                case Command.LdxZp:
                    Ldx(GetByte(GetZpAddr()));
                    break;
                case Command.LdxZpY:
                    Ldx(GetByte(GetZpYAddr()));
                    break;
                case Command.LdyAbs:
                    Ldy(GetByte(GetAbsAddr()));
                    break;
                case Command.LdyAbsX:
                    Ldy(GetByte(GetAbsXAddr()));
                    break;
                case Command.LdyImm:
                    Ldy(NextByte());
                    break;
                case Command.LdyZp:
                    Ldy(GetByte(GetZpAddr()));
                    break;
                case Command.LdyZpX:
                    Ldy(GetByte(GetZpXAddr()));
                    break;
                case Command.LsrAbs:
                    Lsr(GetAbsAddr());
                    break;
                case Command.LsrAbsX:
                    Lsr(GetAbsXAddr());
                    break;
                case Command.LsrAcc:
                    Lsr();
                    break;
                case Command.LsrZp:
                    Lsr(GetZpAddr());
                    break;
                case Command.LsrZpX:
                    Lsr(GetZpXAddr());
                    break;
                case Command.Nop:
                    clock += 2;
                    break;
                case Command.OraIndY:
                    Ora(GetByte(GetPostIndYAddr()));
                    break;
                case Command.OraIndX:
                    Ora(GetByte(GetPreIndXAddr()));
                    break;
                case Command.OraAbs:
                    Ora(GetByte(GetAbsAddr()));
                    break;
                case Command.OraAbsX:
                    Ora(GetByte(GetAbsXAddr()));
                    break;
                case Command.OraAbsY:
                    Ora(GetByte(GetAbsYAddr()));
                    break;
                case Command.OraImm:
                    Ora(NextByte());
                    break;
                case Command.OraZp:
                    Ora(GetByte(GetZpAddr()));
                    break;
                case Command.OraZpX:
                    Ora(GetByte(GetZpXAddr()));
                    break;
                case Command.Pha:
                    Pha();
                    break;
                case Command.Php:
                    Php();
                    break;
                case Command.Pla:
                    Pla();
                    break;
                case Command.Plp:
                    regs.Status = (Status)Pop();
                    break;
                case Command.RolAbs:
                    Rol(GetAbsAddr());
                    break;
                case Command.RolAbsX:
                    Rol(GetAbsXAddr());
                    break;
                case Command.RolAcc:
                    Rol();
                    break;
                case Command.RolZp:
                    Rol(GetZpAddr());
                    break;
                case Command.RolZpX:
                    Rol(GetZpXAddr());
                    break;
                case Command.RorAbs:
                    Ror(GetAbsAddr());
                    break;
                case Command.RorAbsX:
                    Ror(GetAbsXAddr());
                    break;
                case Command.RorAcc:
                    Ror();
                    break;
                case Command.RorZp:
                    Ror(GetZpAddr());
                    break;
                case Command.RorZpX:
                    Ror(GetZpXAddr());
                    break;
                case Command.Rti:
                    Rti();
                    break;
                case Command.Rts:
                    Rts();
                    break;
                case Command.SbcIndY:
                    Sbc(GetByte(GetPostIndYAddr()));
                    break;
                case Command.SbcIndX:
                    Sbc(GetByte(GetPreIndXAddr()));
                    break;
                case Command.SbcAbs:
                    Sbc(GetByte(GetAbsAddr()));
                    break;
                case Command.SbcAbsX:
                    Sbc(GetByte(GetAbsXAddr()));
                    break;
                case Command.SbcAbsY:
                    Sbc(GetByte(GetAbsYAddr()));
                    break;
                case Command.SbcImm:
                    Sbc(NextByte());
                    break;
                case Command.SbcZp:
                    Sbc(GetByte(GetZpAddr()));
                    break;
                case Command.SbcZpX:
                    Sbc(GetByte(GetZpXAddr()));
                    break;
                case Command.Sec:
                    regs.SetCarry(true);
                    break;
                case Command.Sed:
                    regs.Set(Status.Decimal);
                    break;
                case Command.Sei:
                    regs.SetInterrupt(true);
                    break;
                case Command.StaIndY:
                    Sta(GetPostIndYAddr());
                    break;
                case Command.StaIndX:
                    Sta(GetPreIndXAddr());
                    break;
                case Command.StaAbs:
                    Sta(GetAbsAddr());
                    break;
                case Command.StaAbsX:
                    Sta(GetAbsXAddr());
                    break;
                case Command.StaAbsY:
                    Sta(GetAbsYAddr());
                    break;
                case Command.StaZp:
                    Sta(GetZpAddr());
                    break;
                case Command.StaZpX:
                    Sta(GetZpXAddr());
                    break;
                case Command.StxAbs:
                    Stx(GetAbsAddr());
                    break;
                case Command.StxZp:
                    Stx(GetZpAddr());
                    break;
                case Command.StxZpY:
                    Stx(GetZpYAddr());
                    break;
                case Command.StyAbs:
                    Sty(GetAbsAddr());
                    break;
                case Command.StyZp:
                    Sty(GetZpAddr());
                    break;
                case Command.StyZpX:
                    Sty(GetZpXAddr());
                    break;
                case Command.Tax:
                    Tax();
                    break;
                case Command.Tay:
                    Tay();
                    break;
                case Command.Tsx:
                    Tsx();
                    break;
                case Command.Txa:
                    Txa();
                    break;
                case Command.Txs:
                    Txs();
                    break;
                case Command.Tya:
                    Tya();
                    break;
            }
        }
    }
}

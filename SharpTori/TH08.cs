﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTori
{
    /// <summary>
    /// Class for handling TH08 ver 1.00d
    /// </summary>
    public class TH08 : THBase
    {
        private byte _menuValue;
        private byte _difficulty, _mainShot;
        private byte _stage;
        private uint _score;
        private byte _continue;
        private int _missCount;
        private int _bombCount;
        private int _deathbombCount;
        private int _spellCapturedCount;

        public TH08(IntPtr handle) : base(handle)
        {

        }

        public override void Reset()
        {

        }

        public override bool IsInGame()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x017CE8B0 }, ref _menuValue, sizeof(byte)))
                Console.WriteLine("Failed to read memory of menu value.");
            return _menuValue == 2;
        }

        public byte GetDifficulty()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0160F538 }, ref _difficulty, sizeof(byte)))
                Console.WriteLine("Failed to read memory of difficulty.");
            return _difficulty;
        }

        public byte GetMainShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0164D0B1 }, ref _mainShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of mainshot.");
            return _mainShot;
        }

        public byte GetStage()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004E4850 }, ref _stage, sizeof(byte)))
                Console.WriteLine("Failed to read memory of stage.");
            return _stage;
        }

        public uint GetInternalScore()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0160F510, 0x0 }, ref _score, sizeof(uint)))
                Console.WriteLine("Failed to read memory of score.");
            return _score;
        }

        public byte GetContinue()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004D77DC }, ref _continue, sizeof(byte)))
                Console.WriteLine("Failed to read memory of continue.");
            return _continue;
        }

        public ulong GetScore()
        {
            ulong cont = GetContinue();
            return (ulong)GetInternalScore() * 10 + (cont > 9 ? 9 : cont);
        }

        public int GetMissCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0164CFA4 }, ref _missCount, sizeof(int)))
                Console.WriteLine("Failed to read memory of miss count.");
            return _missCount;
        }

        public int GetBombCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0164CFA8 }, ref _bombCount, sizeof(int)))
                Console.WriteLine("Failed to read memory of bomb count.");
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0164CFAC }, ref _deathbombCount, sizeof(int)))
                Console.WriteLine("Failed to read memory of deathbomb count.");
            return _bombCount + _deathbombCount;
        }

        public int GetBorderBreakCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0160F510, 0x1C }, ref _spellCapturedCount, sizeof(int)))
                Console.WriteLine("Failed to read memory of number of spells captured.");
            return _spellCapturedCount;
        }
    }
}
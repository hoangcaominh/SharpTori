using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTori
{
    /// <summary>
    /// Class for handling TH07 ver 1.00b
    /// </summary>
    public class TH07 : THBase
    {
        private THState<uint> _pGuiImplState;
        private byte _difficulty, _mainShot, _subShot;
        private uint _score;
        private byte _continue;
        private float _missCount;
        private float _bombCount;
        private THState<byte> _cherryPlusState;
        private byte _borderState;
        private int _borderBreakCount;

        public TH07(IntPtr handle) : base(handle)
        {
            _pGuiImplState = new THState<uint>();
            _cherryPlusState = new THState<byte>();
        }

        public override void Reset()
        {
            _borderBreakCount = 0;
        }

        public override bool IsNewGame()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0049FBF8 }, ref _pGuiImplState.State, sizeof(uint)))
                Console.WriteLine("Failed to read memory of gui implementation pointer.");
            // A new gui implementation instance is allocated
            return _pGuiImplState.Trigger((prev, curr) => prev != curr && curr != 0);
        }

        public byte GetDifficulty()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x00626280 }, ref _difficulty, sizeof(byte)))
                Console.WriteLine("Failed to read memory of difficulty.");
            return _difficulty;
        }

        public byte GetMainShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0062F645 }, ref _mainShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of mainshot.");
            return _mainShot;
        }

        public byte GetSubShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0062F646 }, ref _subShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of subshot.");
            return _subShot;
        }

        public uint GetInternalScore()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x00626278, 0x0 }, ref _score, sizeof(uint)))
                Console.WriteLine("Failed to read memory of score.");
            return _score;
        }

        public byte GetContinue()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x00626278, 0x20 }, ref _continue, sizeof(byte)))
                Console.WriteLine("Failed to read memory of continue.");
            return _continue;
        }

        public ulong GetScore()
        {
            return (ulong)GetInternalScore() * 10 + GetContinue();
        }

        public int GetMissCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x00626278, 0x50 }, ref _missCount, sizeof(float)))
                Console.WriteLine("Failed to read memory of miss count.");
            return (int)_missCount;
        }

        public int GetBombCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x00626278, 0x6C }, ref _bombCount, sizeof(float)))
                Console.WriteLine("Failed to read memory of bomb count.");
            return (int)_bombCount;
        }

        public int GetBorderBreakCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004BFEE5 }, ref _cherryPlusState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of cherry+ state.");
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x01346218 }, ref _borderState, sizeof(byte)))
                Console.WriteLine("Failed to read memory of border state.");

            // if cherry+ state is 0 and border state is 1, increment border break
            if (_cherryPlusState.Trigger((prev, curr) => prev != curr && curr == 0) && _borderState == 1)
                _borderBreakCount++;
            _cherryPlusState.Update();

            return _borderBreakCount;
        }
    }
}

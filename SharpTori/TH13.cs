using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTori
{
    /// <summary>
    /// Class for handling TH13 ver 1.00c
    /// </summary>
    public class TH13 : THBase
    {
        private THState<uint> _pGuiState;
        private byte _difficulty, _mainShot;
        private uint _score;
        private byte _continue;
        private THState<byte> _playerState;
        private int _missCount;
        private THState<byte> _bombState;
        private int _bombCount;
        private THState<byte> _tranceState;
        private int _tranceCount;

        public TH13(IntPtr handle) : base(handle)
        {
            _pGuiState = new THState<uint>();
            _playerState = new THState<byte>();
            _bombState = new THState<byte>();
            _tranceState = new THState<byte>();
        }

        public override void Reset()
        {
            _missCount = 0;
            _bombCount = 0;
            _tranceCount = 0;
        }

        public override bool IsNewGame()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004C2190 }, ref _pGuiState.State, sizeof(uint)))
                Console.WriteLine("Failed to read memory of gui pointer.");
            // A new gui instance is allocated
            return _pGuiState.Trigger((prev, curr) => prev != curr && curr != 0);
        }

        public byte GetDifficulty()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004BE7C4 }, ref _difficulty, sizeof(byte)))
                Console.WriteLine("Failed to read memory of difficulty.");
            return _difficulty;
        }

        public byte GetMainShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004BE7B8 }, ref _mainShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of mainshot.");
            return _mainShot;
        }

        public uint GetInternalScore()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004BE7C0 }, ref _score, sizeof(uint)))
                Console.WriteLine("Failed to read memory of score.");
            return _score;
        }

        public byte GetContinue()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004BE7C8 }, ref _continue, sizeof(byte)))
                Console.WriteLine("Failed to read memory of continue.");
            return _continue;
        }

        public ulong GetScore()
        {
            return (ulong)GetInternalScore() * 10 + GetContinue();
        }

        public int GetMissCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004C22C4, 0x65C }, ref _playerState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of player state.");

            // if player state changes form 4 to 2, increase miss count by 1
            if (_playerState.Trigger((prev, curr) => prev != curr && curr == 2))
                _missCount++;
            _playerState.Update();

            return _missCount;
        }

        public int GetBombCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004C2170, 0x40 }, ref _bombState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of bomb state.");

            // if bomb state changes to 1, increase bomb count by 1
            if (_bombState.Trigger((prev, curr) => prev != curr && curr == 1))
                _bombCount++;
            _bombState.Update();

            return _bombCount;
        }

        public int GetTranceCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004C22C4, 0x65C }, ref _playerState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of player state.");
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004BE831 }, ref _tranceState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of trance state.");

            // If trance is over, check if the player dies
            if (_tranceState.Trigger((prev, curr) => prev != curr && curr == 0) && _playerState.State != 2)
                _tranceCount++;
            _tranceState.Update();

            return _tranceCount;
        }
    }
}

using System;

namespace SharpTori
{
    /// <summary>
    /// Class for handling TH11 ver 1.00a
    /// </summary>
    public class TH11 : THBase
    {
        private uint _pGui;
        private THState<uint> _pGuiState;
        private byte _difficulty, _mainShot, _subShot;
        private uint _score;
        private byte _continue;
        private THState<byte> _playerState;
        private int _missCount;
        private THState<byte> _bombState;
        private int _bombCount;

        public TH11(IntPtr handle) : base(handle)
        {
            _pGuiState = new THState<uint>();
            _playerState = new THState<byte>();
            _bombState = new THState<byte>();
        }

        public override void Reset()
        {
            _missCount = 0;
            _bombCount = 0;
        }

        public override bool IsInGame()
        {
            return GetGuiPointer() != 0;
        }

        public override bool IsNewGame()
        {
            _pGuiState.State = GetGuiPointer();

            // A new gui instance is allocated
            bool result = _pGuiState.Trigger((prev, curr) => prev != curr && curr != 0);
            _pGuiState.Update();

            return result;
        }

        public byte GetDifficulty()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A5720 }, ref _difficulty, sizeof(byte)))
                Console.WriteLine("Failed to read memory of difficulty.");
            return _difficulty;
        }

        public byte GetMainShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A5710 }, ref _mainShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of mainshot.");
            return _mainShot;
        }

        public byte GetSubShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A5714 }, ref _subShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of subshot.");
            return _subShot;
        }

        public uint GetInternalScore()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A56E4 }, ref _score, sizeof(uint)))
                Console.WriteLine("Failed to read memory of score.");
            return _score;
        }

        public byte GetContinue()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A573C }, ref _continue, sizeof(byte)))
                Console.WriteLine("Failed to read memory of continue.");
            return _continue;
        }

        public ulong GetScore()
        {
            return (ulong)GetInternalScore() * 10 + GetContinue();
        }

        public int GetMissCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A8EB4, 0x928 }, ref _playerState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of player state.");

            // if player state changes form 4 to 2, increase miss count by 1
            if (_playerState.Trigger((prev, curr) => prev != curr && curr == 2))
                _missCount++;
            _playerState.Update();

            return _missCount;
        }

        public int GetBombCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A8D64, 0x3C }, ref _bombState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of bomb state.");

            // if bomb state changes to 1, increase bomb count by 1
            if (_bombState.Trigger((prev, curr) => prev != curr && curr == 1))
                _bombCount++;
            _bombState.Update();

            return _bombCount;
        }

        private uint GetGuiPointer()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A8D84 }, ref _pGui, sizeof(uint)))
                Console.WriteLine("Failed to read memory of gui pointer.");
            return _pGui;
        }
    }
}

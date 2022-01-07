using System;

namespace SharpTori
{
    /// <summary>
    /// Class for handling TH16 ver 1.00a
    /// </summary>
    public class TH16 : THBase
    {
        private THState<uint> _pGuiState;
        private byte _difficulty, _mainShot, _subShot;
        private uint _score;
        private byte _continue;
        private THState<byte> _playerState;
        private int _missCount;
        private THState<byte> _bombState;
        private int _bombCount;
        private THState<byte> _releaseState;
        private int _releaseCount;

        public TH16(IntPtr handle) : base(handle)
        {
            _pGuiState = new THState<uint>();
            _playerState = new THState<byte>();
            _bombState = new THState<byte>();
            _releaseState = new THState<byte>();
        }

        public override void Reset()
        {
            _missCount = 0;
            _bombCount = 0;
            _releaseCount = 0;
        }

        public override bool IsNewGame()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A6DCC }, ref _pGuiState.State, sizeof(uint)))
                Console.WriteLine("Failed to read memory of gui pointer.");

            // A new gui instance is allocated
            bool result = _pGuiState.Trigger((prev, curr) => prev != curr && curr != 0);
            _pGuiState.Update();

            return result;
        }

        public byte GetDifficulty()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A57B4 }, ref _difficulty, sizeof(byte)))
                Console.WriteLine("Failed to read memory of difficulty.");
            return _difficulty;
        }

        public byte GetMainShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A57A4 }, ref _mainShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of mainshot.");
            return _mainShot;
        }

        public byte GetSubShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A57AC }, ref _subShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of subshot.");
            return _subShot;
        }

        public uint GetInternalScore()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A57B0 }, ref _score, sizeof(uint)))
                Console.WriteLine("Failed to read memory of score.");
            return _score;
        }

        public byte GetContinue()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A57B8 }, ref _continue, sizeof(byte)))
                Console.WriteLine("Failed to read memory of continue.");
            return _continue;
        }

        public ulong GetScore()
        {
            return (ulong)GetInternalScore() * 10 + GetContinue();
        }

        public int GetMissCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A6EF8, 0x165A8 }, ref _playerState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of player state.");

            // if player state changes form 4 to 2, increase miss count by 1
            if (_playerState.Trigger((prev, curr) => prev != curr && curr == 2))
                _missCount++;
            _playerState.Update();

            return _missCount;
        }

        public int GetBombCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A6DA8, 0x30 }, ref _bombState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of bomb state.");

            // if bomb state changes to 1, increase bomb count by 1
            if (_bombState.Trigger((prev, curr) => prev != curr && curr == 1))
                _bombCount++;
            _bombState.Update();

            return _bombCount;
        }

        public int GetReleaseCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004A6DA4, 0x30 }, ref _releaseState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of release state.");

            // similar to bomb state
            if (_releaseState.Trigger((prev, curr) => prev != curr && curr == 1))
                _releaseCount++;
            _releaseState.Update();

            return _releaseCount;
        }
    }
}

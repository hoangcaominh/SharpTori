using System;

namespace SharpTori
{
    /// <summary>
    /// Class for handling TH128 ver 1.00a
    /// </summary>
    public class TH128 : THBase
    {
        public struct MedalCount
        {
            public int Gold, Silver, Bronze;
        }

        private uint _pGui;
        private THState<uint> _pGuiState;
        private byte _difficulty, _stage;
        private uint _score;
        private byte _continue;
        private THState<byte> _playerState;
        private int _missCount;
        private THState<byte> _bombState;
        private int _bombCount;
        private THState<byte> _medalState;
        private MedalCount _medalCount;

        public TH128(IntPtr handle) : base(handle)
        {
            _pGuiState = new THState<uint>();
            _playerState = new THState<byte>();
            _bombState = new THState<byte>();
            _medalState = new THState<byte>();
        }

        public override bool IsInGame()
        {
            return GetGuiPointer() != 0;
        }

        public override void Reset()
        {
            _missCount = 0;
            _bombCount = 0;
            _medalCount = new MedalCount { Gold = 0, Silver = 0, Bronze = 0 };
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
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B4D0C }, ref _difficulty, sizeof(byte)))
                Console.WriteLine("Failed to read memory of difficulty.");
            return _difficulty;
        }

        public byte GetStage()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B4D14 }, ref _stage, sizeof(byte)))
                Console.WriteLine("Failed to read memory of stage.");
            return _stage;
        }

        public uint GetInternalScore()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B4CC4 }, ref _score, sizeof(uint)))
                Console.WriteLine("Failed to read memory of score.");
            return _score;
        }

        public byte GetContinue()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B4D28 }, ref _continue, sizeof(byte)))
                Console.WriteLine("Failed to read memory of continue.");
            return _continue;
        }

        public ulong GetScore()
        {
            return (ulong)GetInternalScore() * 10 + GetContinue();
        }

        public int GetMissCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B8A80, 0xF78 }, ref _playerState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of player state.");

            // if player state changes form 4 to 2, increase miss count by 1
            if (_playerState.Trigger((prev, curr) => prev != curr && curr == 2))
                _missCount++;
            _playerState.Update();

            return _missCount;
        }

        public int GetBombCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B892C, 0x40 }, ref _bombState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of bomb state.");

            // if bomb state changes to 1, increase bomb count by 1
            if (_bombState.Trigger((prev, curr) => prev != curr && curr == 1))
                _bombCount++;
            _bombState.Update();

            return _bombCount;
        }

        public MedalCount GetMedalCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B8934, 0x7C }, ref _medalState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of medal state.");

            // if the MSB changes to 0 -> spell card ends, count medals
            if (_medalState.Trigger((prev, curr) => (prev & 128) > 0 && (curr & 128) == 0))
            {
                // 3rd bit is the gold medal check
                if ((_medalState.State & 4) > 0)
                    _medalCount.Gold++;
                // 2nd bit is the silver medal check
                else if ((_medalState.State & 2) > 0)
                    _medalCount.Silver++;
                else
                    _medalCount.Bronze++;
            }
            _medalState.Update();

            return _medalCount;
        }

        private uint GetGuiPointer()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B8950 }, ref _pGui, sizeof(uint)))
                Console.WriteLine("Failed to read memory of gui pointer.");
            return _pGui;
        }
    }
}

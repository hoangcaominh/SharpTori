using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTori
{
    /// <summary>
    /// Class for handling TH17 ver 1.00b
    /// </summary>
    public class TH17 : THBase
    {
        public struct HyperCount
        {
            public int Wolf, Otter, Eagle, Neutral, Break;
        }

        private uint _pMenu;
        private byte _difficulty, _mainShot, _subShot;
        private uint _score;
        private byte _continue;
        private THState<byte> _playerState;
        private int _missCount;
        private THState<byte> _bombState;
        private int _bombCount;
        private byte _hyperType;
        private ushort _hyperState;
        private THState<byte> _hyperActive, _hyperBreak;
        private HyperCount _hyperCount;

        public TH17(IntPtr handle) : base(handle)
        {
            _playerState = new THState<byte>();
            _bombState = new THState<byte>();
            _hyperActive = new THState<byte>();
            _hyperBreak = new THState<byte>();
        }

        public override void Reset()
        {
            _missCount = 0;
            _bombCount = 0;
            _hyperCount = new HyperCount { Wolf = 0, Otter = 0, Eagle = 0, Neutral = 0, Break = 0 };
        }

        public override bool IsNewGame()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B77F0 }, ref _pMenu, sizeof(uint)))
                Console.WriteLine("Failed to read memory of menu pointer.");
            return _pMenu == 0;
        }

        public byte GetDifficulty()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B5A00 }, ref _difficulty, sizeof(byte)))
                Console.WriteLine("Failed to read memory of difficulty.");
            return _difficulty;
        }

        public byte GetMainShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B59F4 }, ref _mainShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of mainshot.");
            return _mainShot;
        }

        public byte GetSubShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B59F8 }, ref _subShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of subshot.");
            return _subShot;
        }

        public uint GetInternalScore()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B59FC }, ref _score, sizeof(uint)))
                Console.WriteLine("Failed to read memory of score.");
            return _score;
        }

        public byte GetContinue()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B5A04 }, ref _continue, sizeof(byte)))
                Console.WriteLine("Failed to read memory of continue.");
            return _continue;
        }

        public ulong GetScore()
        {
            return (ulong)GetInternalScore() * 10 + GetContinue();
        }

        public int GetMissCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B77D0, 0x18DB0 }, ref _playerState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of player state.");

            // if player state changes form 4 to 2, increase miss count by 1
            if (_playerState.Trigger((prev, curr) => prev != curr && curr == 2))
                _missCount++;
            _playerState.Update();

            return _missCount;
        }

        public int GetBombCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B7688, 0x30 }, ref _bombState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of bomb state.");

            // if bomb state changes to 1, increase bomb count by 1
            if (_bombState.Trigger((prev, curr) => prev != curr && curr == 1))
                _bombCount++;
            _bombState.Update();

            return _bombCount;
        }

        public HyperCount GetHyperCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B5ABC }, ref _hyperType, sizeof(byte)))
                Console.WriteLine("Failed to read memory of hyper type.");
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B5AC4 }, ref _hyperState, sizeof(byte)))
                Console.WriteLine("Failed to read memory of hyper state.");

            _hyperActive.State = (byte)(_hyperState & (1 << 1));
            _hyperBreak.State = (byte)(_hyperState & (1 << 2));

            // if bit 1 of hyper state is set, the player is in hyper mode
            // if bit 2 of hyper state is set, the player breaks the hyper
            if (_hyperActive.Trigger((prev, curr) => prev != curr && curr != 0))
                if (_hyperType == 1)
                    _hyperCount.Wolf++;
                else if (_hyperType == 2)
                    _hyperCount.Otter++;
                else if (_hyperType == 3)
                    _hyperCount.Eagle++;
                else if (_hyperType == 4)
                    _hyperCount.Neutral++;
            if (_hyperBreak.Trigger((prev, curr) => prev != curr && curr != 0))
                _hyperCount.Break++;

            _hyperActive.Update();
            _hyperBreak.Update();

            return _hyperCount;
        }
    }
}

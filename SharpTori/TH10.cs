using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTori
{
    /// <summary>
    /// Class for handling TH10 ver 1.00a
    /// </summary>
    public class TH10 : THBase
    {
        private uint _pMenu;
        private byte _difficulty, _mainShot, _subShot;
        private uint _score;
        private byte _continue;
        private THState<byte> _playerState;
        private int _missCount;
        private THState<byte> _bombState;
        private int _bombCount;

        public TH10(IntPtr handle) : base(handle)
        {
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
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0047784C }, ref _pMenu, sizeof(uint)))
                Console.WriteLine("Failed to read memory of menu pointer.");
            return _pMenu == 0;
        }

        public byte GetDifficulty()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x00474C74 }, ref _difficulty, sizeof(byte)))
                Console.WriteLine("Failed to read memory of difficulty.");
            return _difficulty;
        }

        public byte GetMainShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x00474C68 }, ref _mainShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of mainshot.");
            return _mainShot;
        }

        public byte GetSubShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x00474C6C }, ref _subShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of subshot.");
            return _subShot;
        }

        public uint GetInternalScore()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x00474C44 }, ref _score, sizeof(uint)))
                Console.WriteLine("Failed to read memory of score.");
            return _score;
        }

        public byte GetContinue()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x00474C90 }, ref _continue, sizeof(byte)))
                Console.WriteLine("Failed to read memory of continue.");
            return _continue;
        }

        public ulong GetScore()
        {
            return (ulong)GetInternalScore() * 10 + GetContinue();
        }

        public int GetMissCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x00477834, 0x458 }, ref _playerState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of player state.");

            // if player state changes form 4 to 2, increase miss count by 1
            if (_playerState.Trigger((prev, curr) => prev != curr && curr == 2))
                _missCount++;
            _playerState.Update();

            return _missCount;
        }

        public int GetBombCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004776EC, 0x28 }, ref _bombState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of bomb state.");

            // if bomb state changes to 1, increase bomb count by 1
            if (_bombState.Trigger((prev, curr) => prev != curr && curr == 1))
                _bombCount++;
            _bombState.Update();

            return _bombCount;
        }
    }
}

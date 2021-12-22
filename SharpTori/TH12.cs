using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTori
{
    /// <summary>
    /// Class for handling TH12 ver 1.00b
    /// </summary>
    public class TH12 : THBase
    {
        public struct UFOCount
        {
            public int Red, Blue, Green, Rainbow;
        }

        private byte _pMenu;
        private byte _difficulty, _mainShot, _subShot;
        private uint _score;
        private byte _continue;
        private THState<byte> _playerState;
        private int _missCount;
        private THState<byte> _bombState;
        private int _bombCount;
        private byte[] _vaults = new byte[3];
        private THState<uint> _vaultCount;
        private UFOCount _ufoCount;

        public TH12(IntPtr handle) : base(handle)
        {
            _playerState = new THState<byte>();
            _bombState = new THState<byte>();
            _vaultCount = new THState<uint>();
        }

        public override void Reset()
        {
            _missCount = 0;
            _bombCount = 0;
            _ufoCount = new UFOCount { Red = 0, Blue = 0, Green = 0, Rainbow = 0 };
        }

        public override bool IsInGame()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B4530 }, ref _pMenu, sizeof(uint)))
                Console.WriteLine("Failed to read memory of menu pointer.");
            return _pMenu == 0;
        }

        public byte GetDifficulty()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B0CA8 }, ref _difficulty, sizeof(byte)))
                Console.WriteLine("Failed to read memory of difficulty.");
            return _difficulty;
        }

        public byte GetMainShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B0C90 }, ref _mainShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of mainshot.");
            return _mainShot;
        }

        public byte GetSubShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B0C94 }, ref _subShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of subshot.");
            return _subShot;
        }
        
        public uint GetInternalScore()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B0C44 }, ref _score, sizeof(uint)))
                Console.WriteLine("Failed to read memory of score.");
            return _score;
        }

        public byte GetContinue()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B0CC4 }, ref _continue, sizeof(byte)))
                Console.WriteLine("Failed to read memory of continue.");
            return _continue;
        }

        public ulong GetScore()
        {
            return (ulong)GetInternalScore() * 10 + GetContinue();
        }

        public int GetMissCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B4514, 0xA28 }, ref _playerState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of player state.");

            // if player state changes form 4 to 2, increase miss count by 1
            if (_playerState.Trigger((prev, curr) => prev != curr && curr == 2))
                _missCount++;
            _playerState.Update();

            return _missCount;
        }

        public int GetBombCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B43C4, 0x3C }, ref _bombState.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of bomb state.");

            // if bomb state changes to 1, increase bomb count by 1
            if (_playerState.Trigger((prev, curr) => prev != curr && curr == 1))
                _bombCount++;
            _bombState.Update();

            return _bombCount;
        }

        public UFOCount GetUFOCount()
        {
            for (uint i = 0; i < _vaults.Length; i++)
            {
                if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B0C4C + i * sizeof(uint) }, ref _vaults[i], sizeof(byte)))
                    Console.WriteLine("Failed to read memory of vault {0}.", i);
            }
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x004B0C58 }, ref _vaultCount.State, sizeof(uint)))
                Console.WriteLine("Failed to read memory of vault count.");
            if (_vaultCount.Trigger((prev, curr) => prev != curr && curr == 3))
            {
                if (_vaults[0] == 1 && _vaults[1] == 1)
                    _ufoCount.Red++;
                else if (_vaults[0] == 2 && _vaults[1] == 2)
                    _ufoCount.Blue++;
                else if (_vaults[0] == 3 && _vaults[1] == 3)
                    _ufoCount.Green++;
                else
                    _ufoCount.Rainbow++;
            }
            _vaultCount.Update();

            return _ufoCount;
        }
    }
}

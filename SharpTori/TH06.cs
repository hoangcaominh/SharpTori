using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTori
{
    /// <summary>
    /// Class for handling TH06 ver 1.02h
    /// </summary>
    public class TH06 : THBase
    {
        private byte _isMenu;
        private byte _menuValue;
        private byte _difficulty, _mainShot, _subShot;
        private uint _score;
        private THState<byte> _deathbombWindow;
        private int _missCount;
        private int _bombCount;
        private readonly int[] _invalidMenu = new int[] { 1, 2, 10, 16 };

        public TH06(IntPtr handle) : base(handle)
        {
            _deathbombWindow = new THState<byte>();
        }

        public override void Reset()
        {
            _missCount = 0;
        }

        public override bool IsInGame()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x006DC8F8 }, ref _isMenu, sizeof(byte)))
                Console.WriteLine("Failed to read memory of menu state.");
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x006DC8B0 }, ref _menuValue, sizeof(byte)))
                Console.WriteLine("Failed to read memory of menu value.");
            return _isMenu == 0 && !_invalidMenu.Contains(_menuValue);
        }

        public byte GetDifficulty()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0069BCB0 }, ref _difficulty, sizeof(byte)))
                Console.WriteLine("Failed to read memory of difficulty.");
            return _difficulty;
        }

        public byte GetMainShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0069D4BD }, ref _mainShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of mainshot.");
            return _mainShot;
        }

        public byte GetSubShot()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0069D4BE }, ref _subShot, sizeof(byte)))
                Console.WriteLine("Failed to read memory of subshot.");
            return _subShot;
        }

        public uint GetScore()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0069BCA0 }, ref _score, sizeof(uint)))
                Console.WriteLine("Failed to read memory of score.");
            return _score;
        }

        public int GetMissCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x006CB000 }, ref _deathbombWindow.State, sizeof(byte)))
                Console.WriteLine("Failed to read memory of deathbomb window.");

            // for some reasons when the player dies the deathbomb window gets set to 0
            if (_deathbombWindow.Trigger((prev, curr) => prev != curr && curr == 0))
                _missCount++;
            _deathbombWindow.Update();

            return _missCount;
        }

        public int GetBombCount()
        {
            if (!MemoryReader.ReadMemory(Handle, new uint[] { 0x0069BCC4 }, ref _bombCount, sizeof(int)))
                Console.WriteLine("Failed to read memory of bomb count.");
            return _bombCount;
        }
    }
}

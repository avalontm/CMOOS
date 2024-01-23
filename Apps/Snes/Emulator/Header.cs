namespace SNES.Emulator
{
    public class Header
    {
        public string Name { set; get; }
        public int Type { set; get; }
        public int Speed { set; get; }
        public int Chips { set; get; }
        public int RomSize { set; get; }
        public int RamSize { set; get; }
    }
}
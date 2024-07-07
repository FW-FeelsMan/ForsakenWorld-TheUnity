namespace ForsakenWorld
{
    public class Character
    {
        public int CharacterId { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public int GenderId { get; set; }
        public int RaceId { get; set; }
        public int Level { get; set; }
        public string Appearance { get; set; }
        public int Currency { get; set; }
        public int GoldCurrency { get; set; }
        public int CharacterStatus { get; set; }

        public string ClassName { get; set; }
        public string GenderName { get; set; }
        public string RaceName { get; set; }

        public static string GetClassName(int classId)
        {
            return classId switch
            {
                1 => "Warrior",
                2 => "Cleric",
                3 => "Assassin",
                4 => "Jagernaut",
                5 => "Vampire",
                6 => "Witch",
                7 => "Ripper",
                8 => "Caster",
                9 => "Defender",
                10 => "Torturer",
                11 => "Archer",
                12 => "Mage",
                13 => "Paladin",
                14 => "Crosshear",
                15 => "Bard",              
                16 => "Dark Knight",
                17 => "Necromancer",
                _ => "Unknown Class"
            };
        }

        public static string GetGenderName(int genderId)
        {
            return genderId switch
            {
                1 => "Male",
                2 => "Female",
                _ => "Unknown Gender"
            };
        }

        public static string GetRaceName(int raceId)
        {
            return raceId switch
            {
                1 => "Demon",
                2 => "Dworf",
                3 => "Elf",
                4 => "Frangor",
                5 => "Human",
                6 => "Likan",
                7 => "Vesperian",
                _ => "Unknown Race"
            };
        }
    }
}

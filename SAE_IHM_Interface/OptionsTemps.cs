namespace SAE_IHM_Interface
{
    public static class OptionsTemps
    {
        private static readonly int[] _valeurs = { 10, 20, 30, 45, 60, 90, 0 };

        public static int Secondes(int index)
        {
            if (index < 0) index = 0;
            if (index >= _valeurs.Length) index = _valeurs.Length - 1;
            return _valeurs[index];
        }

        public static string Format(int secondes)
        {
            if (secondes <= 0) return "∞ (illimité)";
            if (secondes % 60 == 0) return $"{secondes} s ({secondes / 60} min)";
            return $"{secondes} s";
        }

        public static string FormatDepuisIndex(int index) => Format(Secondes(index));
    }
}

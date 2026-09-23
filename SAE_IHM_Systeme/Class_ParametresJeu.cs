using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace SAE_IHM_Systeme
{
    public class ParametresJeu : INotifyPropertyChanged
    {
        private double _taille = 100;
        int _formeIndexJ1 = 0;
        int _couleurIndexJ1 = 0;
        int _formeIndexJ2 = 1;
        int _couleurIndexJ2 = 1;

        public double Taille
        {
            get => _taille;
            set { _taille = value; OnPropertyChanged(nameof(Taille)); }
        }

        public int FormeIndex1
        {
            get => _formeIndexJ1;
            set { _formeIndexJ1 = value; OnPropertyChanged(nameof(FormeIndex1)); }
        }

        public int FormeIndex2
        {
            get => _formeIndexJ2;
            set { _formeIndexJ2 = value; OnPropertyChanged(nameof(FormeIndex2)); }
        }

        public int CouleurIndex1
        {
            get => _couleurIndexJ1;
            set { _couleurIndexJ1 = value; OnPropertyChanged(nameof(CouleurIndex1)); }
        }
        public int CouleurIndex2
        {
            get => _couleurIndexJ2;
            set { _couleurIndexJ2 = value; OnPropertyChanged(nameof(CouleurIndex2)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string nomPropriete)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nomPropriete));
        }
    }
}

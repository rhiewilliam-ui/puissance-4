using SAE_IHM_Systeme;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SAE_IHM_Interface
{
    public partial class FenetreParametresPartie : Window
    {
        int MaxLargeur = 10;
        int MinLargeur = 4;
        int MaxLongueur = 10;
        int MinLongueur = 4;
        int MaxNbJetons = 7;
        int MinNbJetons = 4;
        int MaxManches = 9;
        int MinManches = 3;

        private ParametresJeu _parametresJeu;
        private bool _estIA;
        private int _difficulteIA;
        private int _tempsJ1;
        private int _tempsJ2;

        public FenetreParametresPartie(ParametresJeu parametresJeu, bool estIA = false,
                                       int difficulteIA = 4, int tempsJ1 = 0, int tempsJ2 = 0)
        {
            InitializeComponent();
            _parametresJeu = parametresJeu;
            _estIA = estIA;
            _difficulteIA = difficulteIA;
            _tempsJ1 = tempsJ1;
            _tempsJ2 = tempsJ2;
            MAJ_MaxJetons();
        }

        private void MAJ_MaxJetons()
        {
            if (int.TryParse(SpinnerLongueur.Text, out int longueur) &&
                int.TryParse(SpinnerLargeur.Text, out int largeur))
            {
                if (longueur > largeur)
                    MaxNbJetons = longueur;
                else
                    MaxNbJetons = largeur;

                if (int.TryParse(SpinnerJetons.Text, out int jetons))
                    SpinnerJetons.Text = Math.Clamp(jetons, MinNbJetons, MaxNbJetons).ToString();
            }
        }
        private void SpinnerJetons_Up_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SpinnerJetons.Text, out int val) && val < MaxNbJetons)
                SpinnerJetons.Text = (val + 1).ToString();

        }
        private void SpinnerJetons_Down_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SpinnerJetons.Text, out int val) && val > MinNbJetons)
                SpinnerJetons.Text = (val - 1).ToString();

        }

        private void SpinnerLongueur_Up_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SpinnerLongueur.Text, out int val) && val < MaxLongueur)
            {
                SpinnerLongueur.Text = (val + 1).ToString();
                MAJ_MaxJetons();
            }
        }
        private void SpinnerLongueur_Down_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SpinnerLongueur.Text, out int val) && val > MinLongueur)
            {
                SpinnerLongueur.Text = (val - 1).ToString();
                MAJ_MaxJetons();
            }
        }

        private void SpinnerLargeur_Up_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SpinnerLargeur.Text, out int val) && val < MaxLargeur)
            {
                SpinnerLargeur.Text = (val + 1).ToString();
                MAJ_MaxJetons();
            }
        }
        private void SpinnerLargeur_Down_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SpinnerLargeur.Text, out int val) && val > MinLargeur)
            {
                SpinnerLargeur.Text = (val - 1).ToString();
                MAJ_MaxJetons();
            }
        }

        private void SpinnerManche_Up_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SpinnerManche.Text, out int val) && val < MaxManches)
                SpinnerManche.Text = (val + 2).ToString();
        }
        private void SpinnerManche_Down_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SpinnerManche.Text, out int val) && val > MinManches)
                SpinnerManche.Text = (val - 2).ToString();
        }

        private void CheckModeChallenge_Changed(object sender, RoutedEventArgs e)
        {
            bool active = CheckModeChallenge.IsChecked == true;

            SpinnerManche.IsEnabled = active;
            BtnMancheUp.IsEnabled = active;
            BtnMancheDown.IsEnabled = active;

            BorderManche.Opacity = active ? 1.0 : 0.4;
            BorderManche.BorderBrush = active ? Brushes.Black : Brushes.Gray;
        }

        private void Bouton_Valider_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(SpinnerLongueur.Text, out int longueur);
            int.TryParse(SpinnerLargeur.Text, out int largeur);
            int.TryParse(SpinnerJetons.Text, out int jetons);
            int.TryParse(SpinnerManche.Text, out int manches);
            bool modeChallenge = CheckModeChallenge.IsChecked == true;

            FenetreJeu f = new FenetreJeu(
                _parametresJeu,
                _estIA,
                longueur, largeur,
                jetons,
                modeChallenge, manches,
                _difficulteIA,
                _tempsJ1, _tempsJ2
            );
            f.Owner = this;
            f.Show();
            this.Hide();
        }

        private void Bouton_retour_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }
    }
}

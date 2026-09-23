using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SAE_IHM_Systeme;

namespace SAE_IHM_Interface
{
    public partial class FenetreParametresJeu : Window
    {
        private ParametresJeu _parametres;

        private readonly List<string> _formes = new List<string>
        {
            "Cercle",
            "CercleCible",
            "Hexagone"
        };

        private readonly List<Color> _couleurs = new List<Color>
        {
            Colors.Black, Colors.Red, Colors.Blue,
            Colors.Green, Colors.Orange, Colors.Purple,
            Colors.Brown, Colors.Yellow
        };

        private readonly int[] _paliersTaille =
        {
            10, 20, 30, 40, 50, 60, 70, 80, 90, 100,
            110, 120, 130, 140, 150, 160, 170, 180, 190, 200
        };

        public string FormeSelectionnee1 => _formes[_parametres.FormeIndex1];
        public Color CouleurSelectionnee1 => _couleurs[_parametres.CouleurIndex1];

        public string FormeSelectionnee2 => _formes[_parametres.FormeIndex2];
        public Color CouleurSelectionnee2 => _couleurs[_parametres.CouleurIndex2];

        public FenetreParametresJeu(ParametresJeu parametresJeu)
        {
            InitializeComponent();
            _parametres = parametresJeu;
            this.DataContext = _parametres;
            this.Loaded += FenetreParametresJeu_Loaded;
        }

        private void FenetreParametresJeu_Loaded(object sender, RoutedEventArgs e)
        {
            ActualiserFormes();
            ActualiserCouleurs();
            AppliquerZoom();
        }

        private void SliderTaille_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LabelTaille != null)
                LabelTaille.Text = $"{(int)SliderTaille.Value}%";
            AppliquerZoom();
        }

        private void AppliquerZoom()
        {
            if (ZoomApercu == null) return;
            double facteur = SliderTaille.Value / 100.0;
            ZoomApercu.ScaleX = facteur;
            ZoomApercu.ScaleY = facteur;
        }

        private void Bouton_moins_taille_Click(object sender, RoutedEventArgs e)
        {
            int valeur = (int)SliderTaille.Value;
            for (int i = _paliersTaille.Length - 1; i >= 0; i--)
            {
                if (_paliersTaille[i] < valeur)
                {
                    SliderTaille.Value = _paliersTaille[i];
                    return;
                }
            }
            SliderTaille.Value = _paliersTaille[0];
        }

        private void Bouton_plus_taille_Click(object sender, RoutedEventArgs e)
        {
            int valeur = (int)SliderTaille.Value;
            foreach (int palier in _paliersTaille)
            {
                if (palier > valeur)
                {
                    SliderTaille.Value = palier;
                    return;
                }
            }
            SliderTaille.Value = _paliersTaille[^1];
        }

        private void ActualiserFormes()
        {
            int gaucheJ1 = (_parametres.FormeIndex1 - 1 + _formes.Count) % _formes.Count;
            int droiteJ1 = (_parametres.FormeIndex1 + 1) % _formes.Count;
            DessinerForme(FormeGauche_J1, _formes[gaucheJ1], 55, Brushes.Transparent,
                new SolidColorBrush(Color.FromRgb(150, 150, 150)), 1.5);
            DessinerForme(FormeCentre_J1, _formes[_parametres.FormeIndex1], 75, Brushes.Transparent,
                Brushes.Black, 2.5);
            DessinerForme(FormeDroite_J1, _formes[droiteJ1], 55, Brushes.Transparent,
                new SolidColorBrush(Color.FromRgb(150, 150, 150)), 1.5);

            int gaucheJ2 = (_parametres.FormeIndex2 - 1 + _formes.Count) % _formes.Count;
            int droiteJ2 = (_parametres.FormeIndex2 + 1) % _formes.Count;
            DessinerForme(FormeGauche_J2, _formes[gaucheJ2], 55, Brushes.Transparent,
                new SolidColorBrush(Color.FromRgb(150, 150, 150)), 1.5);
            DessinerForme(FormeCentre_J2, _formes[_parametres.FormeIndex2], 75, Brushes.Transparent,
                Brushes.Black, 2.5);
            DessinerForme(FormeDroite_J2, _formes[droiteJ2], 55, Brushes.Transparent,
                new SolidColorBrush(Color.FromRgb(150, 150, 150)), 1.5);
        }

        private void DessinerForme(Canvas canvas, string forme, double taille,
            Brush remplissage, Brush contour, double epaisseur)
        {
            canvas.Children.Clear();
            canvas.Width = taille;
            canvas.Height = taille;

            switch (forme)
            {
                case "Cercle":
                    var cercle = new Ellipse
                    {
                        Width = taille - epaisseur * 2,
                        Height = taille - epaisseur * 2,
                        Fill = remplissage,
                        Stroke = contour,
                        StrokeThickness = epaisseur
                    };
                    Canvas.SetLeft(cercle, epaisseur); Canvas.SetTop(cercle, epaisseur);
                    canvas.Children.Add(cercle);
                    return;

                case "CercleCible":
                    var ext = new Ellipse
                    {
                        Width = taille - epaisseur * 2,
                        Height = taille - epaisseur * 2,
                        Fill = remplissage,
                        Stroke = contour,
                        StrokeThickness = epaisseur
                    };
                    Canvas.SetLeft(ext, epaisseur); Canvas.SetTop(ext, epaisseur);
                    canvas.Children.Add(ext);
                    double r2 = (taille - epaisseur * 2) * 0.45;
                    var inner = new Ellipse
                    {
                        Width = r2,
                        Height = r2,
                        Fill = remplissage,
                        Stroke = contour,
                        StrokeThickness = epaisseur * 0.8
                    };
                    Canvas.SetLeft(inner, (taille - r2) / 2); Canvas.SetTop(inner, (taille - r2) / 2);
                    canvas.Children.Add(inner);
                    return;

                case "Hexagone":
                    canvas.Children.Add(CreerHexagone(taille, remplissage, contour, epaisseur));
                    return;
            }
        }

        private Polygon CreerHexagone(double taille, Brush remplissage, Brush contour, double epaisseur)
        {
            var hex = new Polygon { Fill = remplissage, Stroke = contour, StrokeThickness = epaisseur };
            double cx = taille / 2, cy = taille / 2;
            double r = taille / 2 - epaisseur;
            double step = 2 * Math.PI / 6, start = Math.PI / 2;
            for (int i = 0; i < 6; i++)
            {
                double angle = start + i * step;
                hex.Points.Add(new Point(cx + r * Math.Cos(angle), cy + r * Math.Sin(angle)));
            }
            return hex;
        }

        private void Bouton_forme_gauche_J1_Click(object sender, RoutedEventArgs e)
        {
            _parametres.FormeIndex1 = (_parametres.FormeIndex1 - 1 + _formes.Count) % _formes.Count;
            ActualiserFormes();
            ActualiserCouleurs();
        }
        private void Bouton_forme_droite_J1_Click(object sender, RoutedEventArgs e)
        {
            _parametres.FormeIndex1 = (_parametres.FormeIndex1 + 1) % _formes.Count;
            ActualiserFormes();
            ActualiserCouleurs();
        }

        private void Bouton_forme_gauche_J2_Click(object sender, RoutedEventArgs e)
        {
            _parametres.FormeIndex2 = (_parametres.FormeIndex2 - 1 + _formes.Count) % _formes.Count;
            ActualiserFormes();
            ActualiserCouleurs();
        }
        private void Bouton_forme_droite_J2_Click(object sender, RoutedEventArgs e)
        {
            _parametres.FormeIndex2 = (_parametres.FormeIndex2 + 1) % _formes.Count;
            ActualiserFormes();
            ActualiserCouleurs();
        }

        private bool CouleurDisponible(int index, int joueur)
        {
            if (joueur == 1) return index != _parametres.CouleurIndex2;
            if (joueur == 2) return index != _parametres.CouleurIndex1;
            return true;
        }

        private void Bouton_couleur_gauche_J1_Click(object sender, RoutedEventArgs e)
        {
            int next = (_parametres.CouleurIndex1 - 1 + _couleurs.Count) % _couleurs.Count;
            while (!CouleurDisponible(next, 1))
                next = (next - 1 + _couleurs.Count) % _couleurs.Count;
            _parametres.CouleurIndex1 = next;
            ActualiserCouleurs();
        }
        private void Bouton_couleur_droite_J1_Click(object sender, RoutedEventArgs e)
        {
            int next = (_parametres.CouleurIndex1 + 1) % _couleurs.Count;
            while (!CouleurDisponible(next, 1))
                next = (next + 1) % _couleurs.Count;
            _parametres.CouleurIndex1 = next;
            ActualiserCouleurs();
        }

        private void Bouton_couleur_gauche_J2_Click(object sender, RoutedEventArgs e)
        {
            int next = (_parametres.CouleurIndex2 - 1 + _couleurs.Count) % _couleurs.Count;
            while (!CouleurDisponible(next, 2))
                next = (next - 1 + _couleurs.Count) % _couleurs.Count;
            _parametres.CouleurIndex2 = next;
            ActualiserCouleurs();
        }
        private void Bouton_couleur_droite_J2_Click(object sender, RoutedEventArgs e)
        {
            int next = (_parametres.CouleurIndex2 + 1) % _couleurs.Count;
            while (!CouleurDisponible(next, 2))
                next = (next + 1) % _couleurs.Count;
            _parametres.CouleurIndex2 = next;
            ActualiserCouleurs();
        }

        private void ActualiserCouleurs()
        {
            int gaucheJ1 = (_parametres.CouleurIndex1 - 1 + _couleurs.Count) % _couleurs.Count;
            int droiteJ1 = (_parametres.CouleurIndex1 + 1) % _couleurs.Count;
            AppliquerCouleurForme(CouleurGauche_J1, _couleurs[gaucheJ1], false, _parametres.FormeIndex1);
            AppliquerCouleurForme(CouleurCentre_J1, _couleurs[_parametres.CouleurIndex1], true, _parametres.FormeIndex1);
            AppliquerCouleurForme(CouleurDroite_J1, _couleurs[droiteJ1], false, _parametres.FormeIndex1);

            int gaucheJ2 = (_parametres.CouleurIndex2 - 1 + _couleurs.Count) % _couleurs.Count;
            int droiteJ2 = (_parametres.CouleurIndex2 + 1) % _couleurs.Count;
            AppliquerCouleurForme(CouleurGauche_J2, _couleurs[gaucheJ2], false, _parametres.FormeIndex2);
            AppliquerCouleurForme(CouleurCentre_J2, _couleurs[_parametres.CouleurIndex2], true, _parametres.FormeIndex2);
            AppliquerCouleurForme(CouleurDroite_J2, _couleurs[droiteJ2], false, _parametres.FormeIndex2);
        }

        private void AppliquerCouleurForme(Canvas canvas, Color couleur, bool estCentre, int formeIndex)
        {
            canvas.Children.Clear();
            double taille = estCentre ? 65 : 45;
            double epaisseur = estCentre ? 2.5 : 1.5;
            Brush contour = new SolidColorBrush(couleur);
            Brush remplissage = new SolidColorBrush(Color.FromArgb(40, couleur.R, couleur.G, couleur.B));

            switch (_formes[formeIndex])
            {
                case "Cercle":
                    var cercle = new Ellipse
                    {
                        Width = taille - epaisseur * 2,
                        Height = taille - epaisseur * 2,
                        Fill = remplissage,
                        Stroke = contour,
                        StrokeThickness = epaisseur
                    };
                    Canvas.SetLeft(cercle, epaisseur); Canvas.SetTop(cercle, epaisseur);
                    canvas.Children.Add(cercle);
                    break;

                case "CercleCible":
                    var ext = new Ellipse
                    {
                        Width = taille - epaisseur * 2,
                        Height = taille - epaisseur * 2,
                        Fill = remplissage,
                        Stroke = contour,
                        StrokeThickness = epaisseur
                    };
                    Canvas.SetLeft(ext, epaisseur); Canvas.SetTop(ext, epaisseur);
                    canvas.Children.Add(ext);
                    double r2 = (taille - epaisseur * 2) * 0.45;
                    var inner = new Ellipse
                    {
                        Width = r2,
                        Height = r2,
                        Fill = remplissage,
                        Stroke = contour,
                        StrokeThickness = epaisseur * 0.8
                    };
                    Canvas.SetLeft(inner, (taille - r2) / 2); Canvas.SetTop(inner, (taille - r2) / 2);
                    canvas.Children.Add(inner);
                    break;

                case "Hexagone":
                    canvas.Children.Add(CreerHexagone(taille, remplissage, contour, epaisseur));
                    break;
            }
            canvas.Opacity = estCentre ? 1.0 : 0.4;
        }

        private void Bouton_retour_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        private void Bouton_Valider_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }
    }
}

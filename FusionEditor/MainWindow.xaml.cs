using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FusionEditor {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private List<string> _entitymap; 

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            LoadEntityMap();
        }

        private void LoadEntityMap() {
            _entitymap = new List<string>();
            StreamReader file = new StreamReader(FusionEngine.GameManager.GetContentManager().RootDirectory + "//entity_map.dat");
            string line;

            while ((line = file.ReadLine()) != null) {
                if (line.StartsWith(";") || line.StartsWith("[")) continue;

                _entitymap.Add(line.Trim());
            }

            file.Close();

            foreach (String actor in _entitymap) {
                actors.Items.Add(actor);
            }

            foreach (string animation in Enum.GetNames(typeof(FusionEngine.Animation.State))) {
                animations.Items.Add(animation);
            }
        }

        private void actors_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            animations.SelectedIndex = 0;
        }

        private void animations_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            frames.SelectedIndex = 0;
        }

        private void scrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { 
            int step = (int)e.NewValue - (int)e.OldValue;

            frames.SelectedIndex += step;
            if (frames.SelectedIndex < 0)frames.SelectedIndex = 0;
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e) {

        }
    }
}

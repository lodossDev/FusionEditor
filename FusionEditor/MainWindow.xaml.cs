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
        private SortedDictionary<String, SortedDictionary<String, List<String>>> actorsMap; 

        public MainWindow() {
            InitializeComponent();
            actorsMap = new SortedDictionary<string, SortedDictionary<string, List<string>>>();
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            initActorsMap(null, null, FusionEngine.System.contentManager.RootDirectory + "//Sprites//Actors//");

            foreach (String actor in actorsMap.Keys) {
                actors.Items.Add(actor);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

        }

        private void actors_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            animations.Items.Clear();

            foreach (String animation in actorsMap[(string)actors.SelectedValue].Keys) {
                animations.Items.Add(animation);
            }
        }

        private void animations_SelectionChanged(object sender, SelectionChangedEventArgs e){
            frames.Items.Clear();

            foreach (String frame in actorsMap[(string)actors.SelectedValue][(string)animations.SelectedValue]) {
                frames.Items.Add(frame);
            }
        }

        private void initActorsMap(String actor, String animation, String dir) {
            List<string> files = Directory.GetFiles(dir).ToList();

            if (files.Count != 0 && actor != null && animation != null) {
                for (int i = 0; i < files.Count; i++) {
                    actorsMap[actor][animation].Add(Convert.ToString(i + 1));
                }
            } else {
                foreach (String path in Directory.GetDirectories(dir)) {
                    String folderName = System.IO.Path.GetFileName(path);

                    if (actor != null) {
                        if (actorsMap[actor].ContainsKey(folderName) == false) {
                            actorsMap[actor].Add(folderName, new List<string>());
                            initActorsMap(actor, folderName, path);
                        }
                    } else if (actorsMap.ContainsKey(folderName) == false) {
                        actorsMap.Add(folderName, new SortedDictionary<string, List<string>>());
                        initActorsMap(folderName, null, path);
                    }
                }
            }
        }
        

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            //Debug.WriteLine(slider.Value);
            //characterView.WI
        }
    }
}

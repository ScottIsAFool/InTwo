using System.Collections.Generic;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using InTwo.Controls.Settings;

namespace InTwo.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the SettingsViewModel class.
        /// </summary>
        public SettingsViewModel()
        {
            SettingsControls = new List<Group<UserControl>>
            {
                new Group<UserControl>("GAME DEFAULTS", new List<UserControl> {new GameDefaults()}),
                new Group<UserControl>("PROMPTS", new List<UserControl> {new Prompts()}),
                new Group<UserControl>("TILES", new List<UserControl> {new Tiles()})
            };
        }

        public List<Group<UserControl>> SettingsControls { get; set; }
    }
}
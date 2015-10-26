using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.TmThal;

namespace Mufasa.Pages.Settings
{
    class ParametersViewModel : NotifyPropertyChanged
    {
        public ParametersViewModel()
        {
            tmMethodsDict = new Dictionary<string, Thermodynamics.p3_tm_method_type>();
            tmMethodsDict.Add(Breslauer, Thermodynamics.p3_tm_method_type.p3_breslauer_auto);
            tmMethodsDict.Add(SantaLucia, Thermodynamics.p3_tm_method_type.p3_santalucia_auto);

            saltCorrMethodsDict = new Dictionary<string, Thermodynamics.p3_salt_correction_type>();
            saltCorrMethodsDict.Add(Schildkraut, Thermodynamics.p3_salt_correction_type.p3_schildkraut);
            saltCorrMethodsDict.Add(SantaLucia, Thermodynamics.p3_salt_correction_type.p3_santalucia);
            saltCorrMethodsDict.Add(Owczarzy, Thermodynamics.p3_salt_correction_type.p3_owczarzy);

            this.SelectedTmMethod = SantaLucia;
            this.SelectedSaltCorrMethod = SantaLucia;
        }

        private const string SantaLucia = "SantaLucia";
        private const string Breslauer = "Breslauer";
        private const string Schildkraut = "Schildkraut";
        private const string Owczarzy = "Owczarzy";
        private string selectedTmMethod;
        private string selectedSaltCorrMethod;

        private Dictionary<string, Thermodynamics.p3_tm_method_type> tmMethodsDict;
        private Dictionary<string, Thermodynamics.p3_salt_correction_type> saltCorrMethodsDict;

        /// <summary>
        /// Available tm calculation methods.
        /// </summary>
        public string[] TmMethods
        {
            get { return new string[] { Breslauer, SantaLucia }; }

        }

        /// <summary>
        /// Available salt correction methods.
        /// </summary>
        public string[] SaltCorrMethods
        {
            get { return new string[] { Schildkraut, SantaLucia, Owczarzy }; }

        }

        /// <value>
        /// Selected tm calculation method.
        /// </value>
        public string SelectedTmMethod
        {
            get { return this.selectedTmMethod; }
            set
            {
                if (this.selectedTmMethod != value)
                {
                    this.selectedTmMethod = value;
                    OnPropertyChanged("SelectedTmMethod");

                    Design.Designer.Settings.TmThalSettings.TmMethod = this.tmMethodsDict[this.selectedTmMethod];
                }
            }
        }

        /// <value>
        /// Selected salt correction method.
        /// </value>
        public string SelectedSaltCorrMethod
        {
            get { return this.selectedSaltCorrMethod; }
            set
            {
                if (this.selectedSaltCorrMethod != value)
                {
                    this.selectedSaltCorrMethod = value;
                    OnPropertyChanged("SelectedSaltCorrMethod");

                    Design.Designer.Settings.TmThalSettings.SaltCorrectionMethod = this.saltCorrMethodsDict[this.selectedSaltCorrMethod];
                }
            }
        }
    }
}

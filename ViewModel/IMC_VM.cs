using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Maui_1.ViewModel
{
    public partial class IMC_VM : ObservableObject
    {

        [RelayCommand]
        public async Task CalcularIMC(object param)
        {
            if (NotBusy)
            {
                await Task.Run(() =>
                {
                    Busy = true;
                    double tximc = Peso / (Altura * Altura);
                    Func<double, String> func = (tx) =>  tx switch {
                        < 18.5 => "Abaixo do peso",
                        < 24.5 => "Peso normal",
                        < 29.9 => "Sobrepeso",
                        < 34.9 => "Obesidade grau I",
                        < 39.9 => "Obesidade grau II (Severa)",
                        _ => "Obesidade grau III (Morbida)"
                    };
                    Imc = $"tx:{tximc.ToString("0.00")} \n {func(tximc)}";
                    Busy = false;
                });
            }
        }

        public IMC_VM()
        {
            Altura = 1.85;
            Peso = 97;
        }


        [ObservableProperty]
        private double _altura;

        [ObservableProperty]
        private double _peso;

        [ObservableProperty]
        private bool _busy;

        [ObservableProperty]
        private string _imc;

        public bool NotBusy => !Busy;
    }
}

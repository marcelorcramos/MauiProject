using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Maui_1.ViewModel
{
    public partial class PessoaVM : ObservableObject
    {
        [ObservableProperty]
        private Pessoa? _pessoacur = new();

        public ObservableCollection<Pessoa> Pessoas { get; set; } = new();

        [ObservableProperty]
        private bool? _ocupado=false;

        [ObservableProperty]
        private String? msg;

        private readonly DataContext _dataContext;

        public PessoaVM(DataContext datacontext)
        {
            _dataContext = datacontext;
            Task.Run(async () => await GetPessoas());
        }

        public async Task Execute(Func<Task> operacao, String? msg = null)
        {
            try
            {
                if (Ocupado.HasValue && !Ocupado.Value)
                {
                    Ocupado = true;
                    try
                    {
                        Msg = msg ?? "Executando";
                        await operacao.Invoke();
                    }
                    finally
                    {
                        Ocupado = false;
                        Msg = "Concluído";
                    }
                }
            }
            catch (Exception erro)
            {

                await Shell.Current.DisplayAlert("Aviso", erro.Message, "OK");
            }
        }

        [RelayCommand]

        private async Task GetPessoas()
        {
            await Execute(async() =>{ 
                var lstpessoas = await _dataContext.GetAllAsync();
                    if(lstpessoas.Any())
                    {
                    if (Pessoas != null && Pessoas.Any()) Pessoas.Clear();
                    foreach (var pess in lstpessoas) Pessoas.Add(pess);
                    } 
            }, "Criando Observable Collection");
        }

        [RelayCommand]
        private Task SetPessoasCur(Pessoa? nova=null)
        {
            Pessoacur = nova ?? new();
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task<bool> SalvarPessoa()
        {
            Pessoacur = Pessoacur ?? new();
            bool ok = false;
            (bool pessoavalida, String? msg) = Pessoacur.Valida();
            if (!pessoavalida)
            {
                await Shell.Current.DisplayAlert("Aviso", msg, "OK");
                return false;
            }

            await Execute(async () => {
                if (Pessoacur.Id == 0)
                {
                    ok = await _dataContext.InsertAsyncPessoa(Pessoacur);
                    if (ok) Pessoas.Add(Pessoacur);
                }
                else
                {
                    ok = await _dataContext.UpdateAsyncPessoa(Pessoacur);
                    if (ok)
                    {
                        var index = Pessoas.IndexOf(Pessoacur);
                        Pessoa? copia = Pessoacur?.Clone();
                        if (copia != null)
                        {
                            Pessoas.RemoveAt(index);
                            Pessoas.Insert(index, copia);
                        }
                    }
                }
            });

            await SetPessoasCur(new());
            return ok;
        }

        [RelayCommand]
        private async Task<bool> DeletePessoa(Pessoa? morta)
        {
            bool ok = false;
            await Execute(async() =>
            {
                ok = await _dataContext.DeleteAsyncPessoa(morta);
                if (ok)
                { 
                    Pessoas.Remove(morta);
                }
            
            }, "Apagar Registo");
            return ok;
        }
    }
}
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

        [ObservableProperty]
        private bool isSortedByNameAscending = true;

        [ObservableProperty]
        private bool isSortedByNameDescending = false;

        [ObservableProperty]
        private string _selectedSortOption = "Nome (A-Z)";



        public List<string> SortOptions { get; } = new List<string>
        {
            "Nome (A-Z)",
            "Nome (Z-A)",
            "Mais antigos",
            "Mais recentes"
        };

        public string SortStatusMessage =>
        SelectedSortOption switch
        {
            "Nome (A-Z)" => "Ordenado por: Nome (A-Z)",
            "Nome (Z-A)" => "Ordenado por: Nome (Z-A)",
            "Mais antigos" => "Ordenado por: Mais antigos",
            "Mais recentes" => "Ordenado por: Mais recentes",
            _ => "Ordenado por: Nome (A-Z)"
        };

        private readonly DataContext _dataContext;


        public PessoaVM(DataContext datacontext)
        {
            _dataContext = datacontext;
            Task.Run(async () =>
            {
                await GetPessoas();
                SortByName();
            });
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
        private async Task Ligar(Pessoa pessoa)
        {
            if (pessoa == null || string.IsNullOrWhiteSpace(pessoa.Telefone))
            {
                await Shell.Current.DisplayAlert("Aviso", "Número de telefone inválido", "OK");
                return;
            }

            bool confirmar = await Shell.Current.DisplayAlert("Discar",
                $"Deseja discar para {pessoa.Nome}?\n{pessoa.Telefone}",
                "Sim", "Não");

            if (confirmar)
            {
                try
                {
                    if (PhoneDialer.Default.IsSupported)
                    {
                        PhoneDialer.Default.Open(pessoa.Telefone);
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Aviso",
                            "A funcionalidade de discagem não é suportada neste dispositivo",
                            "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Erro",
                        $"Não foi possível discar: {ex.Message}",
                        "OK");
                }
            }
        }

        [RelayCommand]
        private void SortByName()
        {
            if (Pessoas == null || !Pessoas.Any()) return;

            isSortedByNameAscending = !isSortedByNameAscending;

            var sortedList = isSortedByNameAscending
                ? Pessoas.OrderBy(p => p.Nome).ToList()
                : Pessoas.OrderByDescending(p => p.Nome).ToList();

            Pessoas.Clear();
            foreach (var pessoa in sortedList)
            {
                Pessoas.Add(pessoa);
            }
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
                    if (ok)
                    {
                        Pessoas.Add(Pessoacur);
                        // Aplica a ordenação atual após adicionar
                        Sort();
                    }
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
                            // Aplica a ordenação atual após atualizar
                            Sort();
                        }
                    }
                }
            });

            await SetPessoasCur(new());
            return ok;
        }

        [RelayCommand]
        private async Task ShowSortOptions()
        {
            string action = await Shell.Current.DisplayActionSheet("Ordenar por:", "Cancelar", null,
                "Nome (A-Z)",
                "Nome (Z-A)",
                "Mais antigos",
                "Mais recentes");

            if (action != "Cancelar" && !string.IsNullOrEmpty(action))
            {
                SelectedSortOption = action;
                Sort();
            }
        }

        [RelayCommand]
        private void Sort()
        {
            switch (SelectedSortOption)
            {
                case "Nome (A-Z)":
                    Pessoas = new ObservableCollection<Pessoa>(Pessoas.OrderBy(p => p.Nome));
                    IsSortedByNameAscending = true;
                    IsSortedByNameDescending = false;
                    break;
                case "Nome (Z-A)":
                    Pessoas = new ObservableCollection<Pessoa>(Pessoas.OrderByDescending(p => p.Nome));
                    IsSortedByNameAscending = false;
                    IsSortedByNameDescending = true;
                    break;
                case "Mais antigos":
                    Pessoas = new ObservableCollection<Pessoa>(Pessoas.OrderBy(p => p.Id));
                    break;
                case "Mais recentes":
                    Pessoas = new ObservableCollection<Pessoa>(Pessoas.OrderByDescending(p => p.Id));
                    break;
            }
            OnPropertyChanged(nameof(Pessoas));
            OnPropertyChanged(nameof(SortStatusMessage));
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
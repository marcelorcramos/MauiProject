using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SQLite;


namespace Maui_1.Model
{
    public class DataContext:IAsyncDisposable
    {
        private const string DATABASENAME = "Agenda.db3";
        private readonly string caminho = Path.Combine(FileSystem.AppDataDirectory, DATABASENAME);
        private readonly SQLiteAsyncConnection? _cnn;
        public SQLiteAsyncConnection Cnn => _cnn ?? new SQLiteAsyncConnection(caminho, SQLiteOpenFlags.Create
            | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);

        public async Task CreateTableIfNotExists()
        {
            await Cnn.CreateTableAsync<Pessoa>();
        }

        public async Task <List<Pessoa>>GetAllAsync()
        {
            await CreateTableIfNotExists();
            return await Cnn.Table<Pessoa>().ToListAsync();
        }

        public async Task<List<Pessoa>> GetFilteredAsync(Expression<Func<Pessoa,bool>>expressao)
        {
            await CreateTableIfNotExists();
            return await Cnn.Table<Pessoa>().Where(expressao).ToListAsync();
        }

        public async Task <Pessoa> GetPessoa(object pk)
        {
            await CreateTableIfNotExists();
            return await Cnn.GetAsync<Pessoa>(pk);
        }

        public async Task<Pessoa> GetFilteredPessoa(Expression<Func<Pessoa,bool>> expressao)
        {
            await CreateTableIfNotExists();
            return await Cnn.GetAsync<Pessoa>(expressao);
        }

        public async Task<bool> InsertAsyncPessoa(Pessoa nova) 
        {
            await CreateTableIfNotExists();
            return await Cnn.InsertAsync(nova) > 0;
        }

        public async Task<bool> DeleteAsyncPessoa(Pessoa morta)
        {
            await CreateTableIfNotExists();
            return await Cnn.DeleteAsync(morta) > 0;
        }

        public async Task<bool> DeleteAsyncPessoaByID(object pk)
        {
            await CreateTableIfNotExists();
            Pessoa? morta = await Cnn.GetAsync<Pessoa>(pk);
            return await Cnn.DeleteAsync(morta) > 0;
        }

        public async Task<bool> UpdateAsyncPessoa(Pessoa editada)
        {
            await CreateTableIfNotExists();
            return await Cnn.UpdateAsync(editada) > 0;
        }

        public async Task<bool> UpdateAsyncPessoaById(object pk)
        {
            await CreateTableIfNotExists();
            Pessoa esta = await Cnn.GetAsync<Pessoa>(pk);
            return await Cnn.UpdateAsync(esta) > 0;

        }



        public ValueTask DisposeAsync()
        {
            Cnn.CloseAsync();
            return ValueTask.CompletedTask;
        }
    }
}

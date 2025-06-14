using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SQLite;

namespace Maui_1.Model
{
    public class Pessoa
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string? Nome {  get; set; }

        public string? Telefone { get; set; }

        public Pessoa? Clone() => MemberwiseClone() as Pessoa;

        public (bool ok, string? msg) Valida()
        {
            if (String.IsNullOrEmpty(Nome) || String.IsNullOrEmpty(Telefone)) return (false,"Campos Vazios");
            string pattern = @"\b\d{2}\s?\d{7}\b";
            if (!Regex.IsMatch(Telefone, pattern)) return (false,"Telefone Invalido");
            if (Nome.Length <= 2) return (false,"Nome tamanho maior do que 2");
            return (true,null);
        }
       
    }
}

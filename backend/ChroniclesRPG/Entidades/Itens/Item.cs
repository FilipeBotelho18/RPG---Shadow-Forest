namespace ChroniclesRPG.Entidades.Itens
{
    public abstract class Item
    {
        private string? _nome;       // Sempre inicializado pelo construtor
        private string? _descricao;  // Sempre inicializado pelo construtor
        private int _valor;

        public string Nome { 
            get { return _nome ?? string.Empty; } 
            set { _nome = value; } 
        }
        
        public string Descricao { 
            get { return _descricao ?? string.Empty; } 
            set { _descricao = value; } 
        }
        
        public int Valor { 
            get { return _valor; } 
            set { _valor = value; } 
        }

        protected Item(string nome, string descricao, int valor){
            Nome = nome;
            Descricao = descricao;
            Valor = valor;
        }
    }
}
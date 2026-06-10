namespace ChroniclesRPG.Entidades.Itens
{
    public enum TipoArma{
        LaminasCurtas,   // Adagas, Foices, Facas
        LaminasLongas,   // Espadas Curtas, Longas, Cimitarras, Rapieiras
        LaminasPesadas,  // Espada Grande, Alabarda, Glaive
        Machados,        // Machadinha, Machado de Batalha, Machado Grande
        Impacto,         // Porrete, Maça, Martelo, Malho, Mangual
        Hastes,          // Lança, Tridente, Azagaia, Bordão, Lança Longa
        Arcos,           // Arco Curto e Arco Longo
        Bestas,          // Leve, Pesada e de Mão
        Arremesso,       // Dardo, Funda, Rede (armas de projétil simples)
        Cajado           // Cajado
    }

    public enum TipoDano{
        Cortante,
        Concussão,
        Perfurante,
    }

    public class Arma : Item{

        public TipoArma Tipo { get; set; }
        public TipoDano TipoDano { get; set; }
        public string DadoDeDano { get; set; } // Ex: "1d8"
        public bool UsaDestreza { get; set; } // Para adagas ou arcos

        public Arma(string nome, string descricao, TipoArma tipo, TipoDano tipoDano, int valor, string dadoDeDano, bool usaDestreza = false) 
            : base(nome, descricao, valor){
                
            Tipo = tipo;
            TipoDano = tipoDano;
            DadoDeDano = dadoDeDano;
            UsaDestreza = usaDestreza;
        }
    }
}
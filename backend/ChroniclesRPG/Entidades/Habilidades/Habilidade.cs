namespace ChroniclesRPG.Entidades.Habilidades{
    public enum TipoHabilidade
    {
        AtaqueFisico,       // Usa a arma equipada
        TruqueMagico,       // Magia nível 0 (infinito, não gasta slot)
        Magia,              // Gasta slot de magia
        CaracteristicaClasse // Smite, Fúria (pode gastar recursos próprios)
    }

    public enum TipoAcao 
    { 
        AcaoPrincipal,  // ex: Ataque Básico
        AcaoBonus,      // ex: Retomar o Fôlego
        Reacao          // ex: Aparar
    }

    public abstract class Habilidade
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public TipoAcao TempoDeConjuracao { get; set; }
        public TipoHabilidade Tipo { get; set; }
        
        // Se for magia, qual o nível do slot que ela consome? (0 para truques/ataques)
        public int CustoDeSlot { get; set; } 

        protected Habilidade(string nome, string descricao, TipoAcao tempo, TipoHabilidade tipo, int custoSlot = 0)
        {
            Nome = nome;
            Descricao = descricao;
            TempoDeConjuracao = tempo;
            Tipo = tipo;
            CustoDeSlot = custoSlot;
        }

        // alvo é null para habilidades que afetam apenas o próprio usuário (curas, buffs)
        public abstract int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null);
    }
}
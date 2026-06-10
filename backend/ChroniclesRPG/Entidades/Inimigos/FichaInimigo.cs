using ChroniclesRPG.Entidades.Habilidades;

namespace ChroniclesRPG.Entidades.Inimigos
{
    /// <summary>
    /// Representa um inimigo no jogo. Simplificado em relação à FichaPersonagem:
    /// não possui classe RPG, slots de magia nem inventário. Utiliza um único
    /// AtaqueInimigo configurado na FabricaDeInimigos.
    /// </summary>
    public class FichaInimigo
    {
        // ==========================================
        // IDENTIFICAÇÃO
        // ==========================================
        public string Nome { get; set; }
        public string Descricao { get; set; }

        /// <summary>Nível de Desafio (ex: "1/8", "1/4", "1", "2", "5")</summary>
        public string NivelDeDesafio { get; set; }

        /// <summary>Categoria do inimigo para exibição ("Comum", "Mini-Boss", "Boss Final")</summary>
        public string Categoria { get; set; }

        // ==========================================
        // ATRIBUTOS BASE
        // ==========================================
        public int Forca        { get; set; }
        public int Destreza     { get; set; }
        public int Constituicao { get; set; }

        // ==========================================
        // MODIFICADORES (calculados a partir dos atributos)
        // ==========================================
        public int ModificadorForca    => (Forca        - 10) / 2;
        public int ModificadorDestreza => (Destreza     - 10) / 2;

        // ==========================================
        // STATUS DE COMBATE
        // ==========================================
        public int Iniciativa      => ModificadorDestreza;
        public int ClasseArmadura  { get; set; }
        public int HpMaximo        { get; set; }
        public int HpAtual         { get; set; }
        public int MargemCritico   { get; set; } = 20;

        // ==========================================
        // ATAQUE ÚNICO DO INIMIGO
        // ==========================================

        /// <summary>
        /// Habilidade de ataque do inimigo — cada inimigo tem apenas um ataque
        /// para simplificar a implementação.
        /// </summary>
        public AtaqueInimigo Ataque { get; set; }

        // ==========================================
        // CONSTRUTOR
        // ==========================================
        public FichaInimigo(
            string nome,
            string descricao,
            string categoria,
            string nivelDeDesafio,
            int forca,
            int destreza,
            int constituicao,
            int classArmadura,
            int hpMaximo,
            AtaqueInimigo ataque)
        {
            Nome            = nome;
            Descricao       = descricao;
            Categoria       = categoria;
            NivelDeDesafio  = nivelDeDesafio;
            Forca           = forca;
            Destreza        = destreza;
            Constituicao    = constituicao;
            ClasseArmadura  = classArmadura;
            HpMaximo        = hpMaximo;
            HpAtual         = hpMaximo;
            Ataque          = ataque;
        }

        // ==========================================
        // MÉTODOS DE COMBATE
        // ==========================================

        /// <summary>
        /// Aplica dano ao inimigo. Retorna o dano efetivamente sofrido.
        /// </summary>
        public int ReceberDano(int dano)
        {
            dano = Math.Max(1, dano);
            HpAtual -= dano;
            return dano;
        }

        /// <summary>
        /// Realiza o ataque do inimigo contra o alvo (FichaPersonagem do jogador).
        /// Adaptado para usar FichaPersonagem como alvo, reutilizando o sistema de
        /// ReceberDano já existente.
        /// </summary>
        public int ExecutarAtaque(FichaPersonagem alvo)
        {
            return Ataque.ExecutarContraPersonagem(this, alvo);
        }

        // ==========================================
        // EXIBIÇÃO
        // ==========================================
        public void ExibirStatus()
        {
            Console.WriteLine();
            Console.WriteLine($"  ========================================");
            Console.WriteLine($"  INIMIGO: {Nome}");
            Console.WriteLine($"  ========================================");
            Console.WriteLine($"  Categoria    : {Categoria,-15} ND : {NivelDeDesafio}");
            Console.WriteLine($"  ----------------------------------------");
            Console.WriteLine($"  COMBATE");
            Console.WriteLine($"  HP : {HpAtual}/{HpMaximo}   CA: {ClasseArmadura}   Iniciativa: {(ModificadorDestreza >= 0 ? $"+{ModificadorDestreza}" : $"{ModificadorDestreza}")}");
            Console.WriteLine($"  ----------------------------------------");
            Console.WriteLine($"  ATAQUE: {Ataque.Nome}");
            Console.WriteLine($"  {Ataque.Descricao}");
            Console.WriteLine($"  ========================================");
            Console.WriteLine();
        }
    }
}

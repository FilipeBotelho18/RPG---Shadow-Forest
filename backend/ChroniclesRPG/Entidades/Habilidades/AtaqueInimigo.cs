using ChroniclesRPG.Funcoes;
using ChroniclesRPG.Entidades.Inimigos;

namespace ChroniclesRPG.Entidades.Habilidades
{
    /// <summary>
    /// Representa o único ataque de um inimigo. Simplificado para conter apenas
    /// nome, dado de dano, bônus de acerto e bônus de dano fixos — sem dependência
    /// de IClasseRPG ou armas do catálogo.
    /// </summary>
    public class AtaqueInimigo
    {
        public string Nome        { get; set; }
        public string Descricao   { get; set; }

        /// <summary>Dado de dano em notação padrão (ex: "1d4", "1d8", "2d6").</summary>
        public string DadoDeDano  { get; set; }

        /// <summary>Bônus somado ao d20 de acerto (simula FOR/DEX + proficiência).</summary>
        public int BonusDeAcerto  { get; set; }

        /// <summary>Bônus fixo somado ao dano (ex: modificador de FOR do monstro).</summary>
        public int BonusDeDano    { get; set; }

        /// <summary>Descrição do tipo de dano para exibição (ex: "cortante", "perfurante").</summary>
        public string TipoDano    { get; set; }

        // ==========================================
        // CONSTRUTOR
        // ==========================================
        public AtaqueInimigo(
            string nome,
            string descricao,
            string dadoDeDano,
            int bonusDeAcerto,
            int bonusDeDano,
            string tipoDano)
        {
            Nome         = nome;
            Descricao    = descricao;
            DadoDeDano   = dadoDeDano;
            BonusDeAcerto = bonusDeAcerto;
            BonusDeDano  = bonusDeDano;
            TipoDano     = tipoDano;
        }

        // ==========================================
        // EXECUÇÃO
        // ==========================================

        /// <summary>
        /// Executa o ataque do inimigo contra um FichaPersonagem do jogador.
        /// Segue as mesmas regras de d20 + bônus vs CA da FichaPersonagem.
        /// Retorna o dano efetivo causado (0 se errou).
        /// </summary>
        public int ExecutarContraPersonagem(FichaInimigo atacante, FichaPersonagem alvo)
        {
            int rolagem = Dados.RolarD20();
            int totalDeAtaque = rolagem + BonusDeAcerto;

            Console.WriteLine($"  {atacante.Nome} usa {Nome} contra {alvo.Nome}!");
            Console.WriteLine($"    Rolagem de acerto: 1d20({rolagem}) + Bônus({BonusDeAcerto:+#;-#;+0}) = {totalDeAtaque} vs CA {alvo.ClasseArmadura}");

            // Erro crítico
            if (rolagem == 1)
            {
                Console.WriteLine($"    ERRO CRÍTICO! O ataque falha completamente.");
                return 0;
            }

            // Acerto crítico
            if (rolagem >= atacante.MargemCritico)
            {
                int dano1 = Dados.Rolar(DadoDeDano);
                int dano2 = Dados.Rolar(DadoDeDano);
                int danoTotal = dano1 + dano2 + BonusDeDano;
                int danoEfetivo = alvo.ReceberDano(danoTotal);

                Console.WriteLine($"    ACERTO CRÍTICO! Dano: {danoTotal} ({TipoDano})");
                Console.WriteLine($"    HP de {alvo.Nome}: {alvo.HpAtual}/{alvo.HpMaximo}");
                return danoEfetivo;
            }

            // Acerto normal
            if (totalDeAtaque >= alvo.ClasseArmadura)
            {
                int dano = Math.Max(1, Dados.Rolar(DadoDeDano) + BonusDeDano);
                int danoEfetivo = alvo.ReceberDano(dano);

                Console.WriteLine($"    ACERTOU! Dano: {dano} ({TipoDano})");
                Console.WriteLine($"    HP de {alvo.Nome}: {alvo.HpAtual}/{alvo.HpMaximo}");
                return danoEfetivo;
            }

            // Errou
            Console.WriteLine($"    ERROU! O ataque não penetrou a defesa de {alvo.Nome}.");
            return 0;
        }

        /// <summary>
        /// Executa o ataque do inimigo contra outro inimigo (para combates inimigo vs inimigo, se necessário).
        /// </summary>
        public int ExecutarContraInimigo(FichaInimigo atacante, FichaInimigo alvo)
        {
            int rolagem = Dados.RolarD20();
            int totalDeAtaque = rolagem + BonusDeAcerto;

            Console.WriteLine($"  {atacante.Nome} usa {Nome} contra {alvo.Nome}!");
            Console.WriteLine($"    Rolagem de acerto: 1d20({rolagem}) + Bônus({BonusDeAcerto:+#;-#;+0}) = {totalDeAtaque} vs CA {alvo.ClasseArmadura}");

            if (rolagem == 1)
            {
                Console.WriteLine($"    ERRO CRÍTICO! O ataque falha completamente.");
                return 0;
            }

            if (rolagem >= atacante.MargemCritico)
            {
                int dano1 = Dados.Rolar(DadoDeDano);
                int dano2 = Dados.Rolar(DadoDeDano);
                int danoTotal = dano1 + dano2 + BonusDeDano;
                int danoEfetivo = alvo.ReceberDano(danoTotal);

                Console.WriteLine($"    ACERTO CRÍTICO! Dano: {danoTotal} ({TipoDano})");
                Console.WriteLine($"    HP de {alvo.Nome}: {alvo.HpAtual}/{alvo.HpMaximo}");
                return danoEfetivo;
            }

            if (totalDeAtaque >= alvo.ClasseArmadura)
            {
                int dano = Math.Max(1, Dados.Rolar(DadoDeDano) + BonusDeDano);
                int danoEfetivo = alvo.ReceberDano(dano);

                Console.WriteLine($"    ACERTOU! Dano: {dano} ({TipoDano})");
                Console.WriteLine($"    HP de {alvo.Nome}: {alvo.HpAtual}/{alvo.HpMaximo}");
                return danoEfetivo;
            }

            Console.WriteLine($"    ERROU! O ataque não penetrou a defesa de {alvo.Nome}.");
            return 0;
        }
    }
}

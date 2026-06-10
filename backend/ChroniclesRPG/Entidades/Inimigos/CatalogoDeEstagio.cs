namespace ChroniclesRPG.Entidades.Inimigos
{
    /// <summary>
    /// Catálogo de estágios da campanha. Mapeia cada um dos 20 estágios para
    /// o inimigo correspondente, seguindo a distribuição de dificuldade definida
    /// no documento de design.
    /// 
    /// Estrutura da campanha:
    ///   Estágios  1–6  : Monstros Iniciais  (ND 1/8 e 1/4)  — introdução ao combate
    ///   Estágio   7    : Mini-Boss 1 — Ogro Esmagador (ND 2)
    ///   Estágios  8–13 : Monstros Intermediários (ND 1/2 e 1)
    ///   Estágio   14   : Mini-Boss 2 — Árvore Desperta (ND 2)
    ///   Estágios 15–18 : Combates de Alta Intensidade (rotação dos inimigos mais difíceis)
    ///   Estágio   19   : Mini-Boss 3 — Manticora Faminta (ND 3)
    ///   Estágio   20   : Boss Final — Lich Primordial Enfraquecido (ND 5)
    /// </summary>
    public static class CatalogoDeEstagio
    {
        /// <summary>
        /// Retorna uma nova instância fresca do inimigo do estágio informado.
        /// Cada chamada gera uma instância nova para garantir HP zerado.
        /// </summary>
        /// <param name="estagio">Número do estágio (1 a 20).</param>
        public static FichaInimigo ObterInimigo(int estagio)
        {
            return estagio switch
            {
                // ── ESTÁGIOS 1–6: Monstros Iniciais ──────────────────────────────
                // Dois inimigos de ND 1/8 e quatro de ND 1/4, alternando para
                // que o jogador experiencie diferentes perfis de combate.
                1  => FabricaDeInimigos.CriarTwigBlight(),
                2  => FabricaDeInimigos.CriarTwigBlight(),
                3  => FabricaDeInimigos.CriarLoboCorrempido(),
                4  => FabricaDeInimigos.CriarEsqueletoAventureiro(),
                5  => FabricaDeInimigos.CriarZumbiPutrefato(),
                6  => FabricaDeInimigos.CriarEsqueletoAventureiro(),

                // ── ESTÁGIO 7: Mini-Boss 1 ────────────────────────────────────────
                7  => FabricaDeInimigos.CriarOgroEsmagador(),

                // ── ESTÁGIOS 8–13: Monstros Intermediários ────────────────────────
                // Alternância entre ND 1/2 (Orc) e ND 1 (Bugbear) em intensidade
                // crescente — o jogador aprende a maximizar dano por turno.
                8  => FabricaDeInimigos.CriarOrcGarraDeSangue(),
                9  => FabricaDeInimigos.CriarOrcGarraDeSangue(),
                10 => FabricaDeInimigos.CriarBugbear(),
                11 => FabricaDeInimigos.CriarLoboCorrempido(),    // volta com dificuldade relativa maior
                12 => FabricaDeInimigos.CriarBugbear(),
                13 => FabricaDeInimigos.CriarOrcGarraDeSangue(),

                // ── ESTÁGIO 14: Mini-Boss 2 ───────────────────────────────────────
                14 => FabricaDeInimigos.CriarArvoreDespertaMaligna(),

                // ── ESTÁGIOS 15–18: Alta Intensidade ─────────────────────────────
                // Rotação dos inimigos mais agressivos antes do boss final.
                // A dificuldade escala pelo peso mecânico exigido, não pela variedade.
                15 => FabricaDeInimigos.CriarBugbear(),
                16 => FabricaDeInimigos.CriarOrcGarraDeSangue(),
                17 => FabricaDeInimigos.CriarBugbear(),
                18 => FabricaDeInimigos.CriarOrcGarraDeSangue(),

                // ── ESTÁGIO 19: Mini-Boss 3 ───────────────────────────────────────
                19 => FabricaDeInimigos.CriarManticoraFaminta(),

                // ── ESTÁGIO 20: Boss Final ────────────────────────────────────────
                20 => FabricaDeInimigos.CriarLichPrimordialEnfraquecido(),

                // Fora do intervalo
                _ => throw new ArgumentOutOfRangeException(nameof(estagio),
                        $"Estágio inválido: {estagio}. Deve ser entre 1 e 20.")
            };
        }

        /// <summary>
        /// Retorna uma descrição resumida de todos os 20 estágios para exibição em menus.
        /// </summary>
        public static void ExibirMapaDaCampanha()
        {
            Console.WriteLine();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              MAPA DA CAMPANHA — 20 ESTÁGIOS                     ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════════╣");

            for (int i = 1; i <= 20; i++)
            {
                var inimigo = ObterInimigo(i);
                string prefixo = inimigo.Categoria switch
                {
                    "Mini-Boss"  => "⚔ ",
                    "Boss Final" => "☠ ",
                    _            => "  "
                };
                Console.WriteLine($"║ {prefixo}Estágio {i,2}: [{inimigo.NivelDeDesafio,-3} ND] {inimigo.Nome,-40}║");
            }

            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
        }
    }
}

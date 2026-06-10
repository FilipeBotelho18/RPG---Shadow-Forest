using ChroniclesRPG.Entidades.Habilidades;

namespace ChroniclesRPG.Entidades.Inimigos
{
    /// <summary>
    /// Fábrica estática que cria instâncias dos 10 inimigos do jogo.
    /// 
    /// Todos os valores de HP, CA, bônus de acerto e dados de dano são baseados no
    /// System Reference Document 5.1 (SRD 5.1) do D&D 5e, simplificados para o
    /// formato 1x1 físico do projeto.
    /// 
    /// Cada inimigo possui apenas UM ataque para simplificar a implementação.
    /// </summary>
    public static class FabricaDeInimigos
    {
        // ──────────────────────────────────────────────────────────────────────────
        // COMUNS — ND 1/8
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Twig Blight (Arbusto Errante) — ND 1/8
        /// 
        /// Inimigo Fraco: focado em ataques básicos com galhos. Ideal para
        /// introduzir as mecânicas de acerto e turno inicial do jogador.
        /// HP: 4 | CA: 8 | Ataque: +3 vs CA | Dano: 1d4+1 (perfurante)
        /// </summary>
        public static FichaInimigo CriarTwigBlight() =>
            new FichaInimigo(
                nome:           "Arbusto Errante (Twig Blight)",
                descricao:      "Um arbusto animado por magia sombria. Seus galhos finos arranham com precisão traiçoeira.",
                categoria:      "Comum",
                nivelDeDesafio: "1/8",
                forca:          6,
                destreza:       13,
                constituicao:   12,
                classArmadura:  8,
                hpMaximo:       4,
                ataque: new AtaqueInimigo(
                    nome:         "Arranhão de Galho",
                    descricao:    "O arbusto açoita com seus galhos afiados.",
                    dadoDeDano:   "1d4",
                    bonusDeAcerto: 3,
                    bonusDeDano:  1,
                    tipoDano:     "perfurante"
                )
            );

        // ──────────────────────────────────────────────────────────────────────────
        // COMUNS — ND 1/4
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Lobo Corrompido — ND 1/4
        /// 
        /// Inimigo Veloz: alta taxa de acerto com mordidas rápidas. Testa a
        /// importância da Classe de Armadura (CA) do jogador.
        /// HP: 11 | CA: 13 | Ataque: +4 vs CA | Dano: 1d6+2 (perfurante)
        /// </summary>
        public static FichaInimigo CriarLoboCorrempido() =>
            new FichaInimigo(
                nome:           "Lobo Corrompido",
                descricao:      "Um lobo corrompido por miasma das trevas. Seus olhos brilham de vermelho e sua mordida é rápida e certeira.",
                categoria:      "Comum",
                nivelDeDesafio: "1/4",
                forca:          12,
                destreza:       15,
                constituicao:   12,
                classArmadura:  13,
                hpMaximo:       11,
                ataque: new AtaqueInimigo(
                    nome:         "Mordida Veloz",
                    descricao:    "O lobo avança e crava seus dentes com agilidade sobrenatural.",
                    dadoDeDano:   "1d6",
                    bonusDeAcerto: 4,
                    bonusDeDano:  2,
                    tipoDano:     "perfurante"
                )
            );

        /// <summary>
        /// Esqueleto de Aventureiro — ND 1/4
        /// 
        /// Inimigo Padrão: ataque físico equilibrado com espada enferrujada.
        /// Introduz combates de troca de golpes mais longos.
        /// HP: 13 | CA: 13 | Ataque: +4 vs CA | Dano: 1d6+2 (cortante)
        /// </summary>
        public static FichaInimigo CriarEsqueletoAventureiro() =>
            new FichaInimigo(
                nome:           "Esqueleto de Aventureiro",
                descricao:      "Os restos reanimados de um aventureiro que falhou em sua missão. Empunha sua espada enferrujada com memória muscular de além-túmulo.",
                categoria:      "Comum",
                nivelDeDesafio: "1/4",
                forca:          10,
                destreza:       14,
                constituicao:   15,
                classArmadura:  13,
                hpMaximo:       13,
                ataque: new AtaqueInimigo(
                    nome:         "Espada Enferrujada",
                    descricao:    "O esqueleto desfere um golpe lento mas calculado com sua espada corroída.",
                    dadoDeDano:   "1d6",
                    bonusDeAcerto: 4,
                    bonusDeDano:  2,
                    tipoDano:     "cortante"
                )
            );

        /// <summary>
        /// Zumbi Putrefato — ND 1/4
        /// 
        /// Tanque Lento: alta quantidade de HP e ataques lentos. Ensina o
        /// jogador a gerenciar recursos em lutas prolongadas.
        /// HP: 22 | CA: 8 | Ataque: +3 vs CA | Dano: 1d6+1 (concussão)
        /// </summary>
        public static FichaInimigo CriarZumbiPutrefato() =>
            new FichaInimigo(
                nome:           "Zumbi Putrefato",
                descricao:      "Uma massa de carne em decomposição animada pela necromancia. Lento, mas incansável e difícil de derrubar.",
                categoria:      "Comum",
                nivelDeDesafio: "1/4",
                forca:          13,
                destreza:       6,
                constituicao:   16,
                classArmadura:  8,
                hpMaximo:       22,
                ataque: new AtaqueInimigo(
                    nome:         "Soco Putrefato",
                    descricao:    "O zumbi avança lentamente e desfere um soco pesado com seus braços podres.",
                    dadoDeDano:   "1d6",
                    bonusDeAcerto: 3,
                    bonusDeDano:  1,
                    tipoDano:     "concussão"
                )
            );

        // ──────────────────────────────────────────────────────────────────────────
        // COMUNS — ND 1/2
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Orc da Garra de Sangue — ND 1/2
        /// 
        /// Agressor Moderado: foco em dano físico constante com machado. Testa
        /// se o dano do jogador supera a durabilidade do inimigo.
        /// HP: 15 | CA: 13 | Ataque: +5 vs CA | Dano: 1d8+3 (cortante)
        /// </summary>
        public static FichaInimigo CriarOrcGarraDeSangue() =>
            new FichaInimigo(
                nome:           "Orc da Garra de Sangue",
                descricao:      "Um orc guerreiro do clã Garra de Sangue. Feroz e resistente, empunha seu machado com brutalidade implacável.",
                categoria:      "Comum",
                nivelDeDesafio: "1/2",
                forca:          16,
                destreza:       12,
                constituicao:   16,
                classArmadura:  13,
                hpMaximo:       15,
                ataque: new AtaqueInimigo(
                    nome:         "Golpe do Machado",
                    descricao:    "O orc balança seu machado pesado em um arco brutal.",
                    dadoDeDano:   "1d8",
                    bonusDeAcerto: 5,
                    bonusDeDano:  3,
                    tipoDano:     "cortante"
                )
            );

        // ──────────────────────────────────────────────────────────────────────────
        // COMUNS — ND 1
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Bugbear — ND 1
        /// 
        /// Agressor de Impacto: causa muito dano físico em um único golpe com
        /// clava pesada, forçando o uso imediato de curas.
        /// HP: 27 | CA: 14 | Ataque: +4 vs CA | Dano: 2d8+2 (concussão)
        /// </summary>
        public static FichaInimigo CriarBugbear() =>
            new FichaInimigo(
                nome:           "Bugbear",
                descricao:      "Um goblinoide colossal de pelos eriçados. Sua clava pesada desfere golpes devastadores capazes de entorpecer qualquer adversário.",
                categoria:      "Comum",
                nivelDeDesafio: "1",
                forca:          15,
                destreza:       14,
                constituicao:   13,
                classArmadura:  14,
                hpMaximo:       27,
                ataque: new AtaqueInimigo(
                    nome:         "Clava Pesada",
                    descricao:    "O Bugbear ergue sua clava enorme e a desce com força brutal sobre o inimigo.",
                    dadoDeDano:   "2d8",
                    bonusDeAcerto: 4,
                    bonusDeDano:  2,
                    tipoDano:     "concussão"
                )
            );

        // ──────────────────────────────────────────────────────────────────────────
        // MINI-BOSSES — ND 2
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Ogro Esmagador — ND 2
        /// 
        /// Bruto Pesado: golpes esmagadores drenam rapidamente o HP. Exige uso
        /// coordenado de ações principais e bônus (poções, curas).
        /// HP: 59 | CA: 11 | Ataque: +6 vs CA | Dano: 2d8+4 (concussão)
        /// </summary>
        public static FichaInimigo CriarOgroEsmagador() =>
            new FichaInimigo(
                nome:           "Ogro Esmagador",
                descricao:      "Uma criatura de pele grossa e força descomunal. Seu porrete improvisado esmaga armaduras e ossos com igual facilidade.",
                categoria:      "Mini-Boss",
                nivelDeDesafio: "2",
                forca:          19,
                destreza:       8,
                constituicao:   16,
                classArmadura:  11,
                hpMaximo:       59,
                ataque: new AtaqueInimigo(
                    nome:         "Porretada Esmagadora",
                    descricao:    "O Ogro ergue seu enorme porrete e o desce com toda sua força brutal.",
                    dadoDeDano:   "2d8",
                    bonusDeAcerto: 6,
                    bonusDeDano:  4,
                    tipoDano:     "concussão"
                )
            );

        /// <summary>
        /// Árvore Desperta Maligna — ND 2
        /// 
        /// Alta Resiliência: grande defesa física e HP massivo. O desafio é não
        /// esgotar as habilidades antes de zerar a vida do monstro.
        /// HP: 72 | CA: 13 | Ataque: +5 vs CA | Dano: 1d10+3 (concussão)
        /// </summary>
        public static FichaInimigo CriarArvoreDespertaMaligna() =>
            new FichaInimigo(
                nome:           "Árvore Desperta Maligna",
                descricao:      "Uma árvore antiga corrompida por energia das trevas. Sua casca é mais dura que pedra e seus ramos massivos podem esmagar qualquer guerreiro.",
                categoria:      "Mini-Boss",
                nivelDeDesafio: "2",
                forca:          19,
                destreza:       6,
                constituicao:   15,
                classArmadura:  13,
                hpMaximo:       72,
                ataque: new AtaqueInimigo(
                    nome:         "Açoite de Ramo",
                    descricao:    "A árvore balança um de seus ramos massivos com força arrasadora.",
                    dadoDeDano:   "1d10",
                    bonusDeAcerto: 5,
                    bonusDeDano:  3,
                    tipoDano:     "concussão"
                )
            );

        // ──────────────────────────────────────────────────────────────────────────
        // MINI-BOSS — ND 3
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Manticora Faminta — ND 3
        /// 
        /// Implacável: ataques físicos constantes de alto dano. Exige planejamento
        /// estratégico perfeito turno a turno.
        /// HP: 68 | CA: 14 | Ataque: +5 vs CA | Dano: 2d6+3 (cortante)
        /// </summary>
        public static FichaInimigo CriarManticoraFaminta() =>
            new FichaInimigo(
                nome:           "Manticora Faminta",
                descricao:      "Um predador alado com corpo de leão, cabeça humana e cauda de escorpião. Reduzida à sua forma mais primitiva pela fome insaciável, usa suas garras e presas sem parar.",
                categoria:      "Mini-Boss",
                nivelDeDesafio: "3",
                forca:          17,
                destreza:       16,
                constituicao:   17,
                classArmadura:  14,
                hpMaximo:       68,
                ataque: new AtaqueInimigo(
                    nome:         "Garra Rasgante",
                    descricao:    "A Manticora ataca com suas garras poderosas em uma investida feroz.",
                    dadoDeDano:   "2d6",
                    bonusDeAcerto: 5,
                    bonusDeDano:  3,
                    tipoDano:     "cortante"
                )
            );

        // ──────────────────────────────────────────────────────────────────────────
        // BOSS FINAL — ND 5
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Lich Primordial (Enfraquecido) — ND 5
        /// 
        /// Ameaça Suprema: enfraquecido pelo selo quebrado, perdeu suas magias e
        /// utiliza ataques físicos devastadores infundidos com miasma pelo cajado.
        /// Exige domínio total das habilidades do nível máximo.
        /// HP: 135 | CA: 17 | Ataque: +9 vs CA | Dano: 2d10+5 (concussão + necrótico)
        /// </summary>
        public static FichaInimigo CriarLichPrimordialEnfraquecido() =>
            new FichaInimigo(
                nome:           "Lich Primordial (Enfraquecido)",
                descricao:      "O arqui-feiticeiro não-morto que aterroriza a região. Com seu selo mágico parcialmente quebrado, perdeu o acesso às suas magias devastadoras — mas os golpes de seu cajado carregam miasma suficiente para matar qualquer aventureiro descuidado.",
                categoria:      "Boss Final",
                nivelDeDesafio: "5",
                forca:          16,
                destreza:       16,
                constituicao:   20,
                classArmadura:  17,
                hpMaximo:       135,
                ataque: new AtaqueInimigo(
                    nome:         "Cajadada do Miasma",
                    descricao:    "O Lich desfere um golpe devastador com seu cajado, canalizando o miasma das trevas através do impacto.",
                    dadoDeDano:   "2d10",
                    bonusDeAcerto: 9,
                    bonusDeDano:  5,
                    tipoDano:     "concussão"
                )
            );
    }
}

using ChroniclesRPG.Entidades;
using ChroniclesRPG.Entidades.Itens;
using ChroniclesRPG.Entidades.Classes;

namespace ChroniclesRPG{

    public static class ScriptInicial{

        // ==========================================
        // CATÁLOGOS DE ITENS DO MUNDO
        // ==========================================

        // Listas públicas que armazenam todos os itens disponíveis no jogo
        public static List<Armadura>  Armaduras  { get; private set; } = new List<Armadura>();
        public static List<Arma>      Armas      { get; private set; } = new List<Arma>();
        public static List<Consumivel> Consumiveis { get; private set; } = new List<Consumivel>();

        // Popula os catálogos de itens. Deve ser chamado uma única vez no início do jogo
        public static void CarregarDados(){
            Armaduras.Clear();
            Armas.Clear();
            Consumiveis.Clear();
            
            // ==========================================
            // INSTANCIA DE ARMADURAS
            // ==========================================

            // Armaduras Leves
            Armaduras.Add(new Armadura("Armadura Acolchoada",  "Armadura leve de tecido acolchoado.",  TipoArmadura.Leve, 5,    1));
            Armaduras.Add(new Armadura("Armadura de Couro",    "Armadura de couro animal.",            TipoArmadura.Leve, 10,   1));
            Armaduras.Add(new Armadura("Armadura de Couro Batido", "Armadura de couro endurecido.",    TipoArmadura.Leve, 45,   2));

            // Armaduras Médias
            Armaduras.Add(new Armadura("Gibão",          "Armadura média de couro batido.",            TipoArmadura.Media, 10,  3));
            Armaduras.Add(new Armadura("Camisão de Malha", "Armadura média de malha de metal.",        TipoArmadura.Media, 30,  4));
            Armaduras.Add(new Armadura("Brunea",         "Armadura média de metal forjado.",           TipoArmadura.Media, 50,  5));
            Armaduras.Add(new Armadura("Peitoral",       "Armadura média de metal.",                   TipoArmadura.Media, 400, 6));
            Armaduras.Add(new Armadura("Meia Armadura",  "Armadura média de metal.",                   TipoArmadura.Media, 750, 7));

            // Armaduras Pesadas
            Armaduras.Add(new Armadura("Cota de Anéis",   "Armadura pesada de anéis entrelaçados.",   TipoArmadura.Pesada, 30,   6));
            Armaduras.Add(new Armadura("Cota de Malha",   "Armadura pesada de malha de metal.",       TipoArmadura.Pesada, 75,   8));
            Armaduras.Add(new Armadura("Cota de Talas",   "Armadura pesada de talas metálicas.",      TipoArmadura.Pesada, 200,  9));
            Armaduras.Add(new Armadura("Armadura Completa","Armadura pesada de metal integral.",       TipoArmadura.Pesada, 1500, 10));

            // ==========================================
            // INSTANCIA DE ARMAS
            // ==========================================

            // --- Lâminas Curtas ---
            Armas.Add(new Arma("Adaga",       "Pequena e fácil de esconder.",               TipoArma.LaminasCurtas, TipoDano.Perfurante, 2,  "1d4", true)); // Acuidade
            Armas.Add(new Arma("Foice Curta", "Lâmina curva para colheita ou combate.",     TipoArma.LaminasCurtas, TipoDano.Cortante,   1,  "1d4"));

            // --- Lâminas Longas ---
            Armas.Add(new Arma("Espada Curta", "Lâmina curta e perfurante.",                TipoArma.LaminasLongas, TipoDano.Perfurante, 10, "1d6", true)); // Acuidade
            Armas.Add(new Arma("Espada Longa", "Uma espada de lâmina reta afiada.",         TipoArma.LaminasLongas, TipoDano.Cortante,   15, "1d8"));
            Armas.Add(new Arma("Cimitarra",    "Lâmina curva e ágil.",                      TipoArma.LaminasLongas, TipoDano.Cortante,   25, "1d6", true)); // Acuidade
            Armas.Add(new Arma("Rapieira",     "Lâmina fina e elegante.",                   TipoArma.LaminasLongas, TipoDano.Perfurante, 25, "1d8", true)); // Acuidade

            // --- Lâminas Pesadas ---
            Armas.Add(new Arma("Espada Grande", "Lâmina enorme de duas mãos.",              TipoArma.LaminasPesadas, TipoDano.Cortante, 50, "2d6"));
            Armas.Add(new Arma("Alabarda",      "Haste longa com lâmina de machado.",       TipoArma.LaminasPesadas, TipoDano.Cortante, 20, "1d10"));
            Armas.Add(new Arma("Glaive",        "Lâmina larga em uma haste.",               TipoArma.LaminasPesadas, TipoDano.Cortante, 20, "1d10"));

            // --- Machados ---
            Armas.Add(new Arma("Machadinha",       "Machado pequeno de mão.",               TipoArma.Machados, TipoDano.Cortante, 5,  "1d6"));
            Armas.Add(new Arma("Machado de Batalha","Machado versátil de metal.",           TipoArma.Machados, TipoDano.Cortante, 10, "1d8"));
            Armas.Add(new Arma("Machado Grande",    "Machado pesado de guerra.",            TipoArma.Machados, TipoDano.Cortante, 30, "1d12"));

            // --- Impacto ---
            Armas.Add(new Arma("Porrete",          "Um porrete simples de madeira.",        TipoArma.Impacto, TipoDano.Concussão, 1,  "1d4"));
            Armas.Add(new Arma("Maça",             "Cabo de madeira com cabeça de metal.",  TipoArma.Impacto, TipoDano.Concussão, 5,  "1d6"));
            Armas.Add(new Arma("Martelo Leve",     "Martelo pequeno para arremesso.",       TipoArma.Impacto, TipoDano.Concussão, 2,  "1d4"));
            Armas.Add(new Arma("Martelo de Guerra","Martelo pesado para quebrar armaduras.",TipoArma.Impacto, TipoDano.Concussão, 15, "1d8"));
            Armas.Add(new Arma("Malho",            "Martelo enorme de duas mãos.",          TipoArma.Impacto, TipoDano.Concussão, 10, "2d6"));

            // --- Hastes ---
            Armas.Add(new Arma("Bordão",      "Um cajado de madeira resistente.",           TipoArma.Hastes, TipoDano.Concussão,  1, "1d6"));
            Armas.Add(new Arma("Clava Grande","Um pedaço de madeira pesado.",               TipoArma.Hastes, TipoDano.Concussão,  1, "1d8"));
            Armas.Add(new Arma("Lança",       "Haste de madeira com ponta de metal.",       TipoArma.Hastes, TipoDano.Perfurante, 1, "1d6"));
            Armas.Add(new Arma("Tridente",    "Lança de três pontas.",                      TipoArma.Hastes, TipoDano.Perfurante, 5, "1d6"));

            // --- Arcos ---
            Armas.Add(new Arma("Arco Curto", "Arco compacto de madeira.",                  TipoArma.Arcos, TipoDano.Perfurante, 25, "1d6", true));
            Armas.Add(new Arma("Arco Longo", "Arco grande para longas distâncias.",         TipoArma.Arcos, TipoDano.Perfurante, 50, "1d8", true));

            // --- Bestas ---
            Armas.Add(new Arma("Besta Leve",  "Besta mecânica simples.",                   TipoArma.Bestas, TipoDano.Perfurante, 25, "1d8",  true));
            Armas.Add(new Arma("Besta Pesada","Besta mecânica potente.",                    TipoArma.Bestas, TipoDano.Perfurante, 50, "1d10", true));

            // --- Arremesso ---
            Armas.Add(new Arma("Dardo",    "Pequena lâmina de arremesso.",                  TipoArma.Arremesso, TipoDano.Perfurante, 1, "1d4", true));
            Armas.Add(new Arma("Funda",    "Tira de couro para lançar pedras.",             TipoArma.Arremesso, TipoDano.Concussão,  1, "1d4", true));
            Armas.Add(new Arma("Azagaia",  "Uma lança leve de arremesso.",                  TipoArma.Arremesso, TipoDano.Perfurante, 1, "1d6"));

            // --- Cajados ---
            Armas.Add(new Arma("Cajado Simples", "Um cajado de madeira resistente.",        TipoArma.Cajado, TipoDano.Concussão, 1, "1d6"));
            Armas.Add(new Arma("Cajado de Mago", "Um cajado encantado.",                    TipoArma.Cajado, TipoDano.Concussão, 1, "1d6"));
            Armas.Add(new Arma("Bastão",         "Um bastão de madeira resistente.",        TipoArma.Cajado, TipoDano.Concussão, 1, "1d6"));

            // ==========================================
            // INSTANCIA DE CONSUMÍVEIS
            // ==========================================

            Consumiveis.Add(new Consumivel("Poção de Cura",       "Um líquido vermelho borbulhante.", 10, 8));
            Consumiveis.Add(new Consumivel("Poção de Cura Maior", "Uma poção de cura mais potente.",  50, 20));
        }
    }
}

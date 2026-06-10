using ChroniclesRPG.Entidades.Itens;
using ChroniclesRPG.Entidades.Classes;
using ChroniclesRPG.Entidades.Habilidades;

namespace ChroniclesRPG.Entidades{
    
    public class FichaPersonagem{
        // ==========================================
        // INFORMAÇÕES BÁSICAS
        // ==========================================
        public string Nome { get; set; }
        public IClasseRPG Classe { get; private set; }
        public int Nivel { get; set; } = 1;
        public int XP { get; set; } = 0;
        public int Ouro { get; set; } = 0;
        public string? Raca { get; set; }  // Pode ser definida após a criação da ficha
        public string? Lore { get; set; }  // Pode ser definida após a criação da ficha

        // ==========================================
        // ATRIBUTOS BASE (Página em branco)
        // ==========================================
        public int Forca { get; set; }
        public int Destreza { get; set; }
        public int Constituicao { get; set; }
        public int Inteligencia { get; set; }
        public int Sabedoria { get; set; }
        public int Carisma { get; set; }

        // ==========================================
        // MODIFICADORES 
        // ==========================================
        public int ModificadorForca => (Forca - 10) / 2;
        public int ModificadorDestreza => (Destreza - 10) / 2;
        public int ModificadorConstituicao => (Constituicao - 10) / 2;
        public int ModificadorInteligencia => (Inteligencia - 10) / 2;
        public int ModificadorSabedoria => (Sabedoria - 10) / 2;
        public int ModificadorCarisma => (Carisma - 10) / 2;

        // ==========================================
        // STATUS DE COMBATE 
        // ==========================================
        public int Iniciativa => ModificadorDestreza; 
        public int ClasseArmadura { get; set; }
        public int HpMaximo { get; set; }
        public int HpAtual { get; set; }

        // ==========================================
        // ITENS 
        // ==========================================
        public Armadura? ArmaduraVestida { get; set; }  // Null até o personagem equipar uma armadura
        public Arma? ArmaEquipada { get; set; }          // Null até o personagem equipar uma arma
        public List<Item> Inventario { get; set; } = new List<Item>();

        // ==========================================
        // HABILIDADES
        // ==========================================
        public List<Habilidade> HabilidadesConhecidas { get; set; } = new List<Habilidade>();
        public Dictionary<int, int> SlotsDeMagia { get; set; } = new Dictionary<int, int>();

        // Recursos que já eram usados pela interface web
        public int CuraPelasMaosRestante { get; set; }
        public bool TemAtaqueExtra { get; set; }

        // Recursos e efeitos trazidos da implementação de combate
        public int ReservaCuraPelasMaos
        {
            get => CuraPelasMaosRestante;
            set => CuraPelasMaosRestante = value;
        }
        public int UsosSurtoDeAcao { get; set; } = 0;
        public int AcoesExtras { get; set; } = 0;
        public int NumeroDeAtaques { get; set; } = 1;
        public int UsosCanalizarDivindade { get; set; } = 0;
        public int OndaVitalidadeTurnosRestantes { get; set; } = 0;
        public bool TemBencao { get; set; } = false;
        public bool TemEscudoDaFe { get; set; } = false;
        public bool ProximoAtaqueTrovejante { get; set; } = false;
        public bool TemArmaMagica { get; set; } = false;
        public int HpEscudoArcano { get; set; } = 0;
        public int HpMaxEscudoArcano { get; set; } = 0;
        public bool TemVantagemProximoAtaque { get; set; } = false;
        public bool TemResistenciaFisica { get; set; } = false;
        public int MargemCritico { get; set; } = 20;

        public void ReceberSlotsDeMagia(int nivelSlot, int quantidade){
            if (SlotsDeMagia.ContainsKey(nivelSlot)){
                SlotsDeMagia[nivelSlot] += quantidade;
            }
            else{
                SlotsDeMagia.Add(nivelSlot, quantidade);
            }
        }

        public bool GastarSlotDeMagia(int nivelDesejado)
        {
            if (SlotsDeMagia.ContainsKey(nivelDesejado) && SlotsDeMagia[nivelDesejado] > 0)
            {
                SlotsDeMagia[nivelDesejado]--;
                return true;
            }

            foreach (var slot in SlotsDeMagia.Keys.OrderBy(k => k))
            {
                if (slot > nivelDesejado && SlotsDeMagia[slot] > 0)
                {
                    SlotsDeMagia[slot]--;
                    return true;
                }
            }

            return false;
        }

        public int ReceberDano(int dano, bool ehDanoFisico = true)
        {
            if (ehDanoFisico && TemResistenciaFisica)
            {
                dano = Math.Max(1, dano / 2);
            }

            if (HpEscudoArcano > 0)
            {
                int bloqueado = Math.Min(HpEscudoArcano, dano);
                int excedente = dano - bloqueado;
                HpEscudoArcano -= bloqueado;

                if (HpEscudoArcano == 0 && Classe.NomeDaClasse == "Mago" && ClasseArmadura > 10 + ModificadorDestreza)
                {
                    // Proteção Arcana concedia +2 CA enquanto o escudo estava ativo.
                    ClasseArmadura -= 2;
                }

                HpAtual = Math.Max(0, HpAtual - excedente);
                return excedente;
            }

            HpAtual = Math.Max(0, HpAtual - dano);
            return dano;
        }

        // ==========================================
        // CONSTRUTOR
        // ==========================================
        public FichaPersonagem(string nome, IClasseRPG classeEscolhida){
            Nome = nome;
            Classe = classeEscolhida;
            
            // 1. A ficha está "zerada". O método abaixo será o responsável 
            // por preencher todos os atributos da ficha (Força, Destreza, etc.)
            Classe.AplicarBonusIniciais(this);

            // 2. O HP só é calculado APÓS a classe preencher a Constituição
            HpMaximo = Classe.VidaInicial + ModificadorConstituicao;
            HpAtual = HpMaximo;

            // 3. CA base sem armadura é 10 + o modificador de Destreza
            ClasseArmadura = 10 + ModificadorDestreza;

            // 4. Aplica habilidades do nível 1
            Classe.AplicarHabilidadesDeNivel(this, Nivel);
        }

        // ==========================================
        // MÉTODOS DE EQUIPAMENTO
        // ==========================================

        // Equipa uma armadura verificando proficiência e recalculando a CA
        public void EquiparArmadura(Armadura novaArmadura){
            // Verifica se o tipo da armadura nova está na lista de proficiências da classe
            if (!Classe.ProficienciasArmadura.Contains(novaArmadura.Tipo)){
                Console.WriteLine($"  [ERRO] {Nome} não tem proficiência para usar {novaArmadura.Nome}!");
                return;
            }

            ArmaduraVestida = novaArmadura;

            // Recalcula a CA com base no tipo da armadura:
            // - Leve  → CA = 10 + bônus da armadura + modificador de Destreza completo
            // - Média → CA = 10 + bônus da armadura + modificador de Destreza (máx +2)
            // - Pesada→ CA = 10 + bônus da armadura (sem bônus de Destreza)
            ClasseArmadura = novaArmadura.Tipo switch{
                TipoArmadura.Leve   => 10 + novaArmadura.BonusDefesa + ModificadorDestreza,
                TipoArmadura.Media  => 10 + novaArmadura.BonusDefesa + Math.Min(ModificadorDestreza, 2),
                TipoArmadura.Pesada => 10 + novaArmadura.BonusDefesa,
                _                   => ClasseArmadura
            };

            Console.WriteLine($"  {Nome} equipou {novaArmadura.Nome}! (CA ajustada para {ClasseArmadura})");
        }

        // ==========================================
        // MÉTODOS DE NÍVEIS
        // ==========================================

        public void GanharXP(int quantidade){
            XP += quantidade;
            Console.WriteLine($"  {Nome} ganhou {quantidade} XP! Total: {XP} XP");
        }

        public void SubirNivel() {

            while (XP >= Nivel * 100) {
                int XPParaProximoNivel = Nivel * 100;
                
                XP -= XPParaProximoNivel;
                Nivel++; 
                
                Console.WriteLine($"{Nome} subiu para o nível {Nivel}!");
                
                HpMaximo += Math.Max(1, Classe.CalcularVida() + ModificadorConstituicao);
                HpAtual = HpMaximo;

                Classe.AplicarHabilidadesDeNivel(this, Nivel);
            }
        }

        // Equipa uma arma verificando a proficiência da classe do personagem
        public void EquiparArma(Arma novaArma){
            // Verifica se o tipo da arma está na lista de proficiências da classe
            if (!Classe.ProficienciasArmas.Contains(novaArma.Tipo))
            {
                Console.WriteLine($"  [ERRO] {Nome} não tem proficiência para usar {novaArma.Nome}!");
                return;
            }

            ArmaEquipada = novaArma;
            Console.WriteLine($"  {Nome} equipou {novaArma.Nome}! ({novaArma.DadoDeDano} de dano {novaArma.TipoDano})");
        }

        // ==========================================
        // MÉTODO DE EXIBIÇÃO
        // ==========================================

        // Imprime no console um resumo completo da ficha do personagem
        public void ExibirStatus(){
            // Formata o modificador com sinal de + ou - para legibilidade
            // Exemplo: +3, -1, +0
            string Mod(int m) => m >= 0 ? $"+{m}" : $"{m}";

            Console.WriteLine();
            Console.WriteLine($"  ========================================");
            Console.WriteLine($"  FICHA: {Nome}");
            Console.WriteLine($"  ========================================");
            Console.WriteLine($"  Classe : {Classe.NomeDaClasse,-15} Nível : {Nivel}");
            Console.WriteLine($"  Raça   : {(Raca ?? "Indefinida"),-15} XP    : {XP}");
            Console.WriteLine($"  Ouro   : {Ouro} moedas");
            Console.WriteLine($"  ----------------------------------------");
            Console.WriteLine($"  ATRIBUTOS");
            Console.WriteLine($"  FOR: {Forca,2} ({Mod(ModificadorForca)})   DEX: {Destreza,2} ({Mod(ModificadorDestreza)})");
            Console.WriteLine($"  CON: {Constituicao,2} ({Mod(ModificadorConstituicao)})   INT: {Inteligencia,2} ({Mod(ModificadorInteligencia)})");
            Console.WriteLine($"  SAB: {Sabedoria,2} ({Mod(ModificadorSabedoria)})   CAR: {Carisma,2} ({Mod(ModificadorCarisma)})");
            Console.WriteLine($"  ----------------------------------------");
            Console.WriteLine($"  COMBATE");
            Console.WriteLine($"  HP : {HpAtual}/{HpMaximo}   CA: {ClasseArmadura}   Iniciativa: {Mod(Iniciativa)}");
            Console.WriteLine($"  ----------------------------------------");
            Console.WriteLine($"  EQUIPAMENTO");
            Console.WriteLine($"  Arma    : {(ArmaEquipada   != null ? $"{ArmaEquipada.Nome} ({ArmaEquipada.DadoDeDano} {ArmaEquipada.TipoDano})" : "Nenhuma")}");
            Console.WriteLine($"  Armadura: {(ArmaduraVestida != null ? $"{ArmaduraVestida.Nome} (+{ArmaduraVestida.BonusDefesa} CA)" : "Nenhuma")}");
            Console.WriteLine($"  ----------------------------------------");
            Console.WriteLine($"  INVENTÁRIO ({Inventario.Count} item(ns))");

            // Exibe cada item do inventário ou uma mensagem caso esteja vazio
            if (Inventario.Count == 0)
                Console.WriteLine($"  (vazio)");
            else
                foreach (var item in Inventario)
                    Console.WriteLine($"  - {item.Nome} (val: {item.Valor})");

            Console.WriteLine($"  ========================================");
            Console.WriteLine();
        }
    }
}

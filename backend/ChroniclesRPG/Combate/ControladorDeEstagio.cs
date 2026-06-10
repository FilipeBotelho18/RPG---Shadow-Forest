using ChroniclesRPG.Entidades;
using ChroniclesRPG.Entidades.Classes;
using ChroniclesRPG.Entidades.Habilidades;
using ChroniclesRPG.Entidades.Inimigos;

namespace ChroniclesRPG.Combate
{
    public enum ResultadoEstagio { Vitoria, Derrota }

    /// <summary>
    /// Gerencia a progressão completa da campanha de 20 estágios.
    ///
    /// Responsabilidades:
    ///   - Sequenciar os 20 estágios via CatalogoDeEstagio (seleção automática de inimigos)
    ///   - Executar o loop de combate turno a turno (jogador interativo / inimigo automático)
    ///   - Gerenciar a economia de ações do jogador (Ação Principal, Bônus, Surto de Ação)
    ///   - Processar efeitos contínuos (Onda de Vitalidade, buffs, escudo arcano)
    ///   - Conceder XP e disparar subida de nível após vitórias
    ///   - Oferecer recuperação parcial de HP e slots entre estágios
    ///
    /// Integração com FichaInimigo:
    ///   As Habilidades do jogador (AtaqueBasico, DestruicaoDivina, etc.) assinam
    ///   FichaPersonagem como parâmetro de alvo. Para compatibilidade, um proxy de
    ///   FichaPersonagem é criado a partir dos dados do FichaInimigo usando ClasseVazia,
    ///   e o HP é sincronizado de volta ao FichaInimigo após cada ação do jogador.
    /// </summary>
    public class ControladorDeEstagio
    {
        // ==========================================
        // ESTADO DA CAMPANHA
        // ==========================================
        private readonly FichaPersonagem _jogador;
        public int  EstagioAtual       { get; private set; }
        public bool CampanhaEmAndamento { get; private set; } = false;
        private const int ESTAGIO_MAXIMO = 20;

        // Proxy FichaPersonagem para o inimigo atual (recriado a cada estágio)
        private FichaPersonagem? _proxyInimigo;

        // Recursos iniciais do jogador — capturados ao iniciar a campanha para
        // restauração entre estágios.
        private Dictionary<int, int> _slotsMaximos        = new();
        private int _surtoDeAcaoMax                       = 0;
        private int _canalizarDivindadeMax                = 0;
        private int _reservaCuraPelasMaosMax              = 0; // Paladino — rastreado para restauração

        // ==========================================
        // CONSTANTES DE EXPERIÊNCIA
        // ==========================================
        // XP calibrado para o jogador chegar ao nível 5 antes do Estágio 20.
        // Progressão alvo:
        //   Nível 2 → após estágio ~4   (precisa de 100 XP)
        //   Nível 3 → após estágio ~7   (precisa de 300 XP total)
        //   Nível 4 → após estágio ~14  (precisa de 600 XP total)
        //   Nível 5 → após estágio ~19  (precisa de 1000 XP total)
        private static int XpPorEstagio(FichaInimigo inimigo) => inimigo.Categoria switch
        {
            "Mini-Boss"  => 150,
            "Boss Final" => 500,
            _            => 50
        };

        // ==========================================
        // CONSTRUTOR
        // ==========================================
        public ControladorDeEstagio(FichaPersonagem jogador, int estagioInicial = 1)
        {
            _jogador     = jogador;
            EstagioAtual = Math.Clamp(estagioInicial, 1, ESTAGIO_MAXIMO);
        }

        // ==========================================
        // LOOP PRINCIPAL DA CAMPANHA
        // ==========================================

        public void IniciarCampanha()
        {
            CampanhaEmAndamento = true;

            // Captura os recursos iniciais para poder restaurá-los entre estágios
            _slotsMaximos                = new Dictionary<int, int>(_jogador.SlotsDeMagia);
            _surtoDeAcaoMax              = _jogador.UsosSurtoDeAcao;
            _canalizarDivindadeMax       = _jogador.UsosCanalizarDivindade;
            _reservaCuraPelasMaosMax     = _jogador.ReservaCuraPelasMaos;

            ExibirCabecalhoCampanha();

            while (CampanhaEmAndamento && EstagioAtual <= ESTAGIO_MAXIMO)
            {
                ExibirTransicaoDeEstagio(EstagioAtual);
                Console.WriteLine("\n  Pressione qualquer tecla para entrar em combate...");
                Console.ReadKey(true);

                var resultado = ExecutarEstagio(EstagioAtual);

                if (resultado == ResultadoEstagio.Derrota)
                {
                    ExibirGameOver();
                    CampanhaEmAndamento = false;
                    return;
                }

                // Vitória — concede XP e sobe de nível se necessário
                var inimigoDefeated = CatalogoDeEstagio.ObterInimigo(EstagioAtual);
                int xp = XpPorEstagio(inimigoDefeated);
                ExibirVitoriaEstagio(EstagioAtual, xp);
                _jogador.GanharXP(xp);
                _jogador.SubirNivel();

                // Atualiza o máximo de reserva se o personagem subiu de nível
                // (Paladino ganha mais reserva a cada nível)
                if (_jogador.ReservaCuraPelasMaos > _reservaCuraPelasMaosMax)
                    _reservaCuraPelasMaosMax = _jogador.ReservaCuraPelasMaos;

                if (EstagioAtual == ESTAGIO_MAXIMO)
                {
                    ExibirVitoriaFinal();
                    CampanhaEmAndamento = false;
                    return;
                }

                // Após derrotar um boss → descanso longo completo
                // Após estágio comum → recuperação parcial
                bool foiBoss = inimigoDefeated.Categoria is "Mini-Boss" or "Boss Final";
                if (foiBoss)
                    DescansoLongo();
                else
                    RecuperarEntreEstagios();

                EstagioAtual++;
            }
        }

        // ==========================================
        // EXECUÇÃO DE UM ESTÁGIO (LOOP DE COMBATE)
        // ==========================================

        private ResultadoEstagio ExecutarEstagio(int estagio)
        {
            // 1. Seleciona e instancia o inimigo do estágio
            var inimigo = CatalogoDeEstagio.ObterInimigo(estagio);

            // 2. Cria o proxy FichaPersonagem para o inimigo (compatibilidade com Habilidades)
            _proxyInimigo = CriarProxyInimigo(inimigo);

            // 3. Determina ordem de iniciativa (empate → jogador vai primeiro)
            bool jogadorVaiPrimeiro = _jogador.Iniciativa >= inimigo.Iniciativa;
            string quemInicia = jogadorVaiPrimeiro ? _jogador.Nome : inimigo.Nome;
            Console.WriteLine($"\n  Iniciativa: {quemInicia} age primeiro!");

            // 4. Loop de rodadas
            int rodada = 0;
            while (_jogador.HpAtual > 0 && inimigo.HpAtual > 0)
            {
                rodada++;
                ExibirInicioRodada(rodada, _jogador, inimigo);

                // Efeitos de início do turno do jogador (Onda de Vitalidade, Escudo Arcano)
                AplicarEfeitosInicioTurno(_jogador);

                if (jogadorVaiPrimeiro)
                {
                    ExecutarTurnoJogador(_jogador, inimigo);
                    SincronizarProxyComInimigo(inimigo);
                    if (inimigo.HpAtual <= 0) break;

                    ExecutarTurnoInimigo(inimigo, _jogador);
                }
                else
                {
                    ExecutarTurnoInimigo(inimigo, _jogador);
                    if (_jogador.HpAtual <= 0) break;

                    ExecutarTurnoJogador(_jogador, inimigo);
                    SincronizarProxyComInimigo(inimigo);
                }

                // Efeitos de fim do turno (expira buffs de duração 1 turno)
                AplicarEfitosFimTurno(_jogador);

                ExibirResumoRodada(rodada, _jogador, inimigo);
            }

            return _jogador.HpAtual > 0 ? ResultadoEstagio.Vitoria : ResultadoEstagio.Derrota;
        }

        // ==========================================
        // TURNO DO JOGADOR (INTERATIVO)
        // ==========================================

        private void ExecutarTurnoJogador(FichaPersonagem jogador, FichaInimigo inimigo)
        {
            Console.WriteLine($"\n  ┌─ TURNO DE {jogador.Nome.ToUpper()} ─────────────────────────────");

            bool acaoPrincipalUsada = false;
            bool acaoBonusUsada     = false;
            int  acoesExtras        = 0; // concedidas pelo Surto de Ação

            while (true)
            {
                bool podePrincipal = !acaoPrincipalUsada || acoesExtras > 0;
                bool podeBonus     = !acaoBonusUsada;

                if (!podePrincipal && !podeBonus)
                    break;

                var opcoes = MontarOpcoes(jogador, acaoPrincipalUsada, acaoBonusUsada, acoesExtras);

                if (opcoes.Count == 0)
                {
                    Console.WriteLine("  (Nenhuma ação disponível — turno encerrado)");
                    break;
                }

                ExibirMenuAcoes(jogador, inimigo, acaoPrincipalUsada, acaoBonusUsada, acoesExtras, opcoes);

                Console.Write("\n  Escolha: ");
                string? entrada = Console.ReadLine();

                if (!int.TryParse(entrada, out int escolha) || escolha < 0 || escolha > opcoes.Count)
                {
                    Console.WriteLine("  [!] Opção inválida. Tente novamente.");
                    continue;
                }

                if (escolha == 0)
                    break; // Encerrar turno manualmente

                var (habilidade, tipoAcao) = opcoes[escolha - 1];

                // Determina o alvo correto para a habilidade
                FichaPersonagem? alvo = DeterminarAlvo(habilidade, jogador);

                // Executa
                habilidade.Executar(jogador, alvo);

                // Registra consumo da ação
                if (tipoAcao == TipoAcao.AcaoPrincipal)
                {
                    if (!acaoPrincipalUsada)
                        acaoPrincipalUsada = true;
                    else
                        acoesExtras--; // consumiu uma ação extra do Surto
                }
                else if (tipoAcao == TipoAcao.AcaoBonus)
                {
                    acaoBonusUsada = true;

                    // Surto de Ação: concede 1 ação principal extra
                    if (habilidade.Nome == "Surto de Ação")
                    {
                        acoesExtras = 1;
                        jogador.UsosSurtoDeAcao--;
                        Console.WriteLine($"  [Surto de Ação] {jogador.Nome} ganha 1 Ação Extra!");
                    }
                }

                // Sincroniza HP do proxy → FichaInimigo após cada ação
                SincronizarProxyComInimigo(inimigo);

                if (inimigo.HpAtual <= 0)
                {
                    Console.WriteLine($"\n  ✖ {inimigo.Nome} foi derrotado!");
                    break;
                }
            }

            Console.WriteLine($"  └─────────────────────────────────────────────────────");
        }

        // ==========================================
        // TURNO DO INIMIGO (AUTOMÁTICO)
        // ==========================================

        private void ExecutarTurnoInimigo(FichaInimigo inimigo, FichaPersonagem jogador)
        {
            Console.WriteLine($"\n  ┌─ TURNO DE {inimigo.Nome.ToUpper()} ─────────────────────────");
            inimigo.ExecutarAtaque(jogador);
            Console.WriteLine($"  └─────────────────────────────────────────────────────");
        }

        // ==========================================
        // CONSTRUÇÃO DO MENU DE AÇÕES
        // ==========================================

        /// <summary>
        /// Monta a lista de ações disponíveis para o jogador no turno atual,
        /// filtrando habilidades que não podem ser usadas (sem slots, sem usos, HP cheio, etc.).
        /// </summary>
        private List<(Habilidade hab, TipoAcao tipo)> MontarOpcoes(
            FichaPersonagem jogador,
            bool acaoPrincipalUsada,
            bool acaoBonusUsada,
            int  acoesExtras)
        {
            var opcoes = new List<(Habilidade, TipoAcao)>();

            bool podePrincipal = !acaoPrincipalUsada || acoesExtras > 0;
            bool podeBonus     = !acaoBonusUsada;

            foreach (var hab in jogador.HabilidadesConhecidas)
            {
                // Filtra habilidades puramente passivas — elas não aparecem no menu
                if (EhPassiva(hab))
                    continue;

                if (hab.TempoDeConjuracao == TipoAcao.AcaoPrincipal && podePrincipal)
                {
                    if (!TemRecursosParaUsar(hab, jogador))
                        continue;
                    opcoes.Add((hab, TipoAcao.AcaoPrincipal));
                }
                else if (hab.TempoDeConjuracao == TipoAcao.AcaoBonus && podeBonus)
                {
                    if (!TemRecursosParaUsar(hab, jogador))
                        continue;
                    opcoes.Add((hab, TipoAcao.AcaoBonus));
                }
                // TipoAcao.Reacao é ignorado no fluxo de turno (tratado fora do menu)
            }

            return opcoes;
        }

        private void ExibirMenuAcoes(
            FichaPersonagem jogador,
            FichaInimigo    inimigo,
            bool acaoPrincipalUsada,
            bool acaoBonusUsada,
            int  acoesExtras,
            List<(Habilidade hab, TipoAcao tipo)> opcoes)
        {
            Console.WriteLine();
            ExibirBarrasHP(jogador, inimigo);

            // Exibe slots de magia se existirem
            if (jogador.SlotsDeMagia.Count > 0)
            {
                Console.Write("  Slots de Magia: ");
                foreach (var kv in jogador.SlotsDeMagia.OrderBy(k => k.Key))
                    Console.Write($"Nv.{kv.Key}:{kv.Value}  ");
                Console.WriteLine();
            }

            Console.WriteLine();

            // ─── AÇÃO PRINCIPAL ────────────────────────────────────────────────
            string statusPrincipal = acoesExtras > 0 ? $"AÇÃO EXTRA ({acoesExtras} restante(s))" :
                                     acaoPrincipalUsada ? "USADA ✗" : "DISPONÍVEL ✓";
            Console.WriteLine($"  ┌─ AÇÃO PRINCIPAL [{statusPrincipal}] ───────────────────────");

            int idx = 1;
            foreach (var (hab, tipo) in opcoes.Where(o => o.tipo == TipoAcao.AcaoPrincipal))
            {
                string custo   = hab.CustoDeSlot > 0 ? $" [Slot Nv.{hab.CustoDeSlot}]" : "";
                string recurso = hab.Nome == "Cura pelas Mãos" ? $" (Reserva: {jogador.ReservaCuraPelasMaos} HP)" : "";
                Console.WriteLine($"  │ {idx,2}. {hab.Nome,-26}{custo}{recurso}");
                Console.WriteLine($"  │     {hab.Descricao}");
                idx++;
            }

            // ─── AÇÃO BÔNUS ────────────────────────────────────────────────────
            string statusBonus = acaoBonusUsada ? "USADA ✗" : "DISPONÍVEL ✓";
            Console.WriteLine($"  ├─ AÇÃO BÔNUS [{statusBonus}] ──────────────────────────────");

            foreach (var (hab, tipo) in opcoes.Where(o => o.tipo == TipoAcao.AcaoBonus))
            {
                string custo   = hab.CustoDeSlot > 0 ? $" [Slot Nv.{hab.CustoDeSlot}]" : "";
                string recurso = "";
                if (hab.Nome == "Surto de Ação")     recurso = $" ({jogador.UsosSurtoDeAcao} uso(s))";
                if (hab.Nome == "Cura pelas Mãos")    recurso = $" (Reserva: {jogador.ReservaCuraPelasMaos} HP)";
                if (hab.Nome == "Onda de Vitalidade") recurso = $" ({jogador.UsosCanalizarDivindade} uso(s))";

                Console.WriteLine($"  │ {idx,2}. {hab.Nome,-26}{custo}{recurso}");
                Console.WriteLine($"  │     {hab.Descricao}");
                idx++;
            }

            Console.WriteLine($"  └──────────────────────────────────────────────────────");
            Console.WriteLine($"   0. Encerrar turno");
        }

        // ==========================================
        // HELPERS DE HABILIDADES
        // ==========================================

        /// <summary>
        /// Retorna true para habilidades puramente passivas que nunca devem aparecer
        /// no menu de ações do jogador.
        /// </summary>
        private static bool EhPassiva(Habilidade hab) =>
            hab.Nome == "Ataque Extra"              ||
            hab.Nome == "Arquétipo Marcial (Campeão)" ||
            hab.Nome == "Proteção Projetada";

        /// <summary>
        /// Verifica se o jogador tem os recursos necessários para usar a habilidade.
        /// </summary>
        private static bool TemRecursosParaUsar(Habilidade hab, FichaPersonagem jogador)
        {
            // Habilidades com custo de slot exigem slot disponível
            if (hab.CustoDeSlot > 0)
            {
                bool temSlot = jogador.SlotsDeMagia
                    .Any(kv => kv.Key >= hab.CustoDeSlot && kv.Value > 0);
                if (!temSlot) return false;
            }

            // Filtros por recurso específico
            if (hab.Nome == "Surto de Ação"     && jogador.UsosSurtoDeAcao    <= 0) return false;
            if (hab.Nome == "Cura pelas Mãos"    && jogador.ReservaCuraPelasMaos <= 0) return false;
            if (hab.Nome == "Onda de Vitalidade" && jogador.UsosCanalizarDivindade <= 0) return false;

            // Retomar o Fôlego é inútil com HP cheio
            if (hab.Nome == "Retomar o Fôlego" && jogador.HpAtual >= jogador.HpMaximo) return false;

            return true;
        }

        /// <summary>
        /// Determina o alvo correto (FichaPersonagem) para uma habilidade em combate 1x1.
        ///
        /// Regra:
        ///   - Habilidades de ataque (AtaqueFisico, truques/magias ofensivas) → proxy do inimigo
        ///   - Cura pelas Mãos → self (o jogador é seu próprio aliado)
        ///   - Escudo da Fé → null (a implementação usa self quando alvo é null)
        ///   - Resto → null (habilidades de self-buff leem/escrevem em 'usuario' diretamente)
        /// </summary>
        private FichaPersonagem? DeterminarAlvo(Habilidade hab, FichaPersonagem jogador)
        {
            // Ataque físico (AtaqueBasico, etc.)
            if (hab.Tipo == TipoHabilidade.AtaqueFisico)
                return _proxyInimigo;

            // Magias/truques ofensivos que precisam de alvo
            if (hab.Nome is "Raio de Fogo"       or
                            "Raio de Gelo"        or
                            "Destruição Divina")
                return _proxyInimigo;

            // Cura pelas Mãos: em 1x1, o alvo é o próprio jogador
            if (hab.Nome == "Cura pelas Mãos")
                return jogador;

            // Escudo da Fé sem alvo → aplica em 'usuario' (ver MagiaEscudoDaFe.cs)
            // Demais habilidades de buff/cura → sem alvo (atuam sobre 'usuario')
            return null;
        }

        // ==========================================
        // PROXY E SINCRONIZAÇÃO
        // ==========================================

        /// <summary>
        /// Cria uma FichaPersonagem com os atributos brutos do FichaInimigo.
        /// Usada como alvo nas Habilidades do jogador.
        /// </summary>
        private static FichaPersonagem CriarProxyInimigo(FichaInimigo inimigo)
        {
            // ClasseVazia → AplicarBonusIniciais é no-op, não altera os atributos
            var proxy = new FichaPersonagem(inimigo.Nome, new ClasseVazia());

            // Define manualmente os atributos do inimigo
            proxy.Forca          = inimigo.Forca;
            proxy.Destreza       = inimigo.Destreza;
            proxy.Constituicao   = inimigo.Constituicao;
            proxy.HpMaximo       = inimigo.HpMaximo;
            proxy.HpAtual        = inimigo.HpAtual;
            proxy.ClasseArmadura = inimigo.ClasseArmadura;
            proxy.MargemCritico  = inimigo.MargemCritico;

            return proxy;
        }

        /// <summary>
        /// Após cada execução de habilidade do jogador, copia o HP do proxy
        /// de volta para o FichaInimigo real (fonte de verdade do HP do inimigo).
        /// </summary>
        private void SincronizarProxyComInimigo(FichaInimigo inimigo)
        {
            if (_proxyInimigo != null)
                inimigo.HpAtual = _proxyInimigo.HpAtual;
        }

        // ==========================================
        // EFEITOS CONTÍNUOS (BUFFS / DEBUFFS)
        // ==========================================

        private void AplicarEfeitosInicioTurno(FichaPersonagem jogador)
        {
            // Escudo Arcano — exibe status
            if (jogador.HpEscudoArcano > 0)
                Console.WriteLine($"  │ [Escudo Arcano] HP: {jogador.HpEscudoArcano}/{jogador.HpMaxEscudoArcano}");

            // Onda de Vitalidade — cura progressiva (ModCarisma por turno)
            if (jogador.OndaVitalidadeTurnosRestantes > 0)
            {
                int cura    = Math.Max(1, jogador.ModificadorCarisma);
                int hpAntes = jogador.HpAtual;
                jogador.HpAtual = Math.Min(jogador.HpAtual + cura, jogador.HpMaximo);
                int curado  = jogador.HpAtual - hpAntes;
                jogador.OndaVitalidadeTurnosRestantes--;

                Console.WriteLine($"  │ [Onda de Vitalidade] Cura +{curado} HP! " +
                                  $"(HP: {jogador.HpAtual}/{jogador.HpMaximo} | " +
                                  $"{jogador.OndaVitalidadeTurnosRestantes} turno(s) restante(s))");
            }
        }

        private void AplicarEfitosFimTurno(FichaPersonagem jogador)
        {
            if (jogador.TemResistenciaFisica)
            {
                jogador.TemResistenciaFisica = false;
                Console.WriteLine($"  │ [Proteção contra Lâminas] Resistência física expirou.");
            }

            if (jogador.TemVantagemProximoAtaque)
            {
                jogador.TemVantagemProximoAtaque = false;
                Console.WriteLine($"  │ [Ataque Certeiro] Vantagem expirou sem ser usada.");
            }
        }

        // ==========================================
        // RECUPERAÇÃO ENTRE ESTÁGIOS
        // ==========================================

        /// <summary>
        /// Recuperação parcial — descanso curto após estágios comuns.
        /// Restaura metade do HP faltante e todos os slots de magia.
        /// </summary>
        private void RecuperarEntreEstagios()
        {
            Console.WriteLine("\n  ┌─ DESCANSO RÁPIDO ─────────────────────────────────────");

            // Cura: metade do HP faltante (arredondado para cima)
            int hpFaltando = _jogador.HpMaximo - _jogador.HpAtual;
            int cura       = (hpFaltando + 1) / 2;
            if (cura > 0)
            {
                _jogador.HpAtual = Math.Min(_jogador.HpAtual + cura, _jogador.HpMaximo);
                Console.WriteLine($"  │ HP Recuperado: +{cura} (HP: {_jogador.HpAtual}/{_jogador.HpMaximo})");
            }
            else
            {
                Console.WriteLine($"  │ HP: {_jogador.HpAtual}/{_jogador.HpMaximo} (cheio)");
            }

            AplicarRestauracaoDeRecursos(parcial: true);

            Console.WriteLine($"  └─────────────────────────────────────────────────────\n");

            Console.WriteLine("  Pressione qualquer tecla para continuar...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Descanso longo completo — restaura HP máximo, todos os slots e recursos.
        /// Acionado automaticamente após derrotar um Mini-Boss ou Boss.
        /// </summary>
        private void DescansoLongo()
        {
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║             ✦  DESCANSO LONGO  ✦                    ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("  Depois de derrotar um grande inimigo, você encontra");
            Console.WriteLine("  um lugar seguro para descansar. O silêncio do local");
            Console.WriteLine("  contrasta com a batalha que acabou de travar.");
            Console.WriteLine();
            Console.WriteLine("  Você limpa suas feridas, recupera o fôlego e sente");
            Console.WriteLine("  suas forças retornando completamente.");
            Console.WriteLine("  Agora está revigorado e pronto para o próximo desafio.");
            Console.WriteLine();
            Console.WriteLine("  ┌─ RECUPERAÇÃO COMPLETA ────────────────────────────────");

            // HP máximo
            int hpRestaurado = _jogador.HpMaximo - _jogador.HpAtual;
            _jogador.HpAtual = _jogador.HpMaximo;
            Console.WriteLine($"  │ ♥  HP totalmente restaurado! (+{hpRestaurado}) [{_jogador.HpMaximo}/{_jogador.HpMaximo}]");

            AplicarRestauracaoDeRecursos(parcial: false);

            Console.WriteLine($"  └─────────────────────────────────────────────────────\n");

            Console.WriteLine("  Pressione qualquer tecla para continuar...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Restaura os recursos de magia, habilidades especiais e limpa buffs.
        /// parcial=true → restaura apenas slots que existiam na abertura da campanha.
        /// parcial=false → restaura todos os slots atuais ao máximo.
        /// </summary>
        private void AplicarRestauracaoDeRecursos(bool parcial)
        {
            // Atualiza snapshots caso o personagem tenha novos slots (subiu de nível)
            foreach (var kv in _jogador.SlotsDeMagia)
                if (!_slotsMaximos.ContainsKey(kv.Key))
                    _slotsMaximos[kv.Key] = kv.Value;

            // Restaura slots de magia
            bool slotsRestaurados = false;
            var referencia = parcial ? _slotsMaximos : _jogador.SlotsDeMagia
                                           .ToDictionary(k => k.Key, v => v.Value + v.Value); // full
            foreach (var kv in _slotsMaximos)
            {
                if (_jogador.SlotsDeMagia.ContainsKey(kv.Key) && _jogador.SlotsDeMagia[kv.Key] < kv.Value)
                {
                    _jogador.SlotsDeMagia[kv.Key] = kv.Value;
                    slotsRestaurados = true;
                }
            }
            // Em descanso longo, restaura também slots adquiridos por level-up que não estavam no snapshot
            if (!parcial)
            {
                foreach (var kv in _jogador.SlotsDeMagia.ToList())
                {
                    if (!_slotsMaximos.ContainsKey(kv.Key))
                    {
                        // Slot novo (ganho por level-up): já está com o valor correto
                        slotsRestaurados = true;
                    }
                }
                // Atualiza snapshot para refletir nível atual
                _slotsMaximos = new Dictionary<int, int>(_jogador.SlotsDeMagia);
            }
            if (slotsRestaurados)
                Console.WriteLine($"  │ ✦  Slots de Magia restaurados!");

            // Restaura Surto de Ação
            if (_jogador.UsosSurtoDeAcao < _surtoDeAcaoMax)
            {
                _jogador.UsosSurtoDeAcao = _surtoDeAcaoMax;
                Console.WriteLine($"  │ ✦  Surto de Ação restaurado! ({_surtoDeAcaoMax} uso(s))");
            }

            // Restaura Canalizar Divindade
            if (_jogador.UsosCanalizarDivindade < _canalizarDivindadeMax)
            {
                _jogador.UsosCanalizarDivindade = _canalizarDivindadeMax;
                Console.WriteLine($"  │ ✦  Canalizar Divindade restaurado! ({_canalizarDivindadeMax} uso(s))");
            }

            // Restaura Cura pelas Mãos (Paladino)
            if (_reservaCuraPelasMaosMax > 0 && _jogador.ReservaCuraPelasMaos < _reservaCuraPelasMaosMax)
            {
                _jogador.ReservaCuraPelasMaos = _reservaCuraPelasMaosMax;
                Console.WriteLine($"  │ ✦  Cura pelas Mãos restaurada! (Reserva: {_reservaCuraPelasMaosMax} HP)");
            }

            // Limpa buffs temporários
            _jogador.TemBencao                    = false;
            _jogador.TemEscudoDaFe                = false;
            _jogador.TemArmaMagica                = false;
            _jogador.ProximoAtaqueTrovejante      = false;
            _jogador.TemResistenciaFisica         = false;
            _jogador.TemVantagemProximoAtaque     = false;
            _jogador.OndaVitalidadeTurnosRestantes = 0;
            _jogador.HpEscudoArcano               = 0;
        }

        // ==========================================
        // EXIBIÇÃO
        // ==========================================

        private void ExibirCabecalhoCampanha()
        {
            Console.WriteLine();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         CHRONICLES RPG — MODO CAMPANHA                   ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine($"║  Herói  : {_jogador.Nome,-45}║");
            Console.WriteLine($"║  Classe : {_jogador.Classe.NomeDaClasse,-45}║");
            Console.WriteLine($"║  Nível  : {_jogador.Nivel,-45}║");
            Console.WriteLine($"║  HP     : {_jogador.HpAtual}/{_jogador.HpMaximo,-43}║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine($"║  Estágio inicial: {EstagioAtual,-39}║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

            CatalogoDeEstagio.ExibirMapaDaCampanha();

            Console.WriteLine("  Pressione qualquer tecla para iniciar...");
            Console.ReadKey(true);
        }

        private void ExibirTransicaoDeEstagio(int estagio)
        {
            var inimigo = CatalogoDeEstagio.ObterInimigo(estagio);

            // Cena dramática para Mini-Bosses e Boss Final
            if (inimigo.Categoria == "Mini-Boss" || inimigo.Categoria == "Boss Final")
                ExibirCenaChefe(inimigo, estagio);

            string titulo = inimigo.Categoria switch
            {
                "Boss Final" => "╔══╡ ☠  CONFRONTO FINAL  ☠ ╞══════════════════════════╗",
                "Mini-Boss"  => "╔══╡ ⚔  MINI-BOSS  ⚔ ╞════════════════════════════════╗",
                _            => "╔══════════════════════════════════════════════════════════╗"
            };

            Console.WriteLine();
            Console.WriteLine(titulo);
            Console.WriteLine($"║  Estágio {estagio,2} de {ESTAGIO_MAXIMO}                                          ║");
            Console.WriteLine($"╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine($"║  Inimigo  : {inimigo.Nome,-43}║");
            Console.WriteLine($"║  Categoria: {inimigo.Categoria,-43}║");
            Console.WriteLine($"║  ND       : {inimigo.NivelDeDesafio,-43}║");
            Console.WriteLine($"║  HP       : {inimigo.HpMaximo,-43}║");
            Console.WriteLine($"║  CA       : {inimigo.ClasseArmadura,-43}║");
            Console.WriteLine($"║  Ataque   : {inimigo.Ataque.Nome,-43}║");
            Console.WriteLine($"╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine($"║  {inimigo.Descricao.PadRight(54)}║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        }

        /// <summary>
        /// Exibe uma cena narrativa dramática antes do combate contra um chefe.
        /// Inclui anúncio do sistema e uma fala característica do inimigo.
        /// </summary>
        private void ExibirCenaChefe(FichaInimigo inimigo, int estagio)
        {
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════════════╗");

            if (inimigo.Categoria == "Boss Final")
            {
                Console.WriteLine("  ║         ☠  UM GRANDE INIMIGO SURGE  ☠              ║");
                Console.WriteLine("  ╠══════════════════════════════════════════════════════╣");
                Console.WriteLine("  ║  Uma presença sombria e gélida preenche o salão.    ║");
                Console.WriteLine("  ║  O Lich Primordial se ergue de seu trono de ossos   ║");
                Console.WriteLine("  ║  e te observa com olhos que ardem em chamas roxas.  ║");
                Console.WriteLine("  ║  Ele te impede de prosseguir.                       ║");
            }
            else
            {
                Console.WriteLine("  ║        ⚔  UM GRANDE INIMIGO TE BLOQUEIA  ⚔         ║");
                Console.WriteLine("  ╠══════════════════════════════════════════════════════╣");
                string anuncio = inimigo.Nome switch
                {
                    var n when n.Contains("Ogro")    => "  ║  O chão treme. Uma sombra enorme bloqueia a passagem.║",
                    var n when n.Contains("Árvore")  => "  ║  As raízes arrancam o chão. A floresta desperta.    ║",
                    var n when n.Contains("Mantic")  => "  ║  Um rugido estremece as paredes. Asas rasgam o ar.  ║",
                    _                               => $"  ║  {inimigo.Nome,-50}║"
                };
                Console.WriteLine(anuncio);
                Console.WriteLine("  ║  Este inimigo te impede de prosseguir.              ║");
            }

            Console.WriteLine("  ╚══════════════════════════════════════════════════════╝");

            // Fala do chefe
            Console.WriteLine();
            string fala = inimigo.Nome switch
            {
                var n when n.Contains("Ogro")    =>
                    $"  {inimigo.Nome}: \"VOCÊ... PEQUENOOO! OGRO VAI ESMAGAR!\"",
                var n when n.Contains("Árvore")  =>
                    $"  {inimigo.Nome}: *As raízes rangem e as folhas sussurram em uma língua antiga...*",
                var n when n.Contains("Mantic")  =>
                    $"  {inimigo.Nome}: \"Carne fresca... Finalmente algo digno de ser devorado!\"",
                var n when n.Contains("Lich")    =>
                    $"  {inimigo.Nome}: \"Audacioso mortal... Você ousou chegar até mim?"
                    + $"\n  Que seus ossos sirvam à minha coleção por toda a eternidade.\"",
                _                               =>
                    $"  {inimigo.Nome}: \"Não passarás!\""
            };
            Console.WriteLine(fala);
            Console.WriteLine();
        }

        private void ExibirInicioRodada(int rodada, FichaPersonagem jogador, FichaInimigo inimigo)
        {
            Console.WriteLine($"\n{'─',60}");
            Console.WriteLine($"  RODADA {rodada}");
            Console.WriteLine($"{'─',60}");
        }

        /// <summary>Exibe barras de HP visuais para os dois combatentes.</summary>
        private void ExibirBarrasHP(FichaPersonagem jogador, FichaInimigo inimigo)
        {
            const int barLen = 24;

            double pctJ   = (double)Math.Max(0, jogador.HpAtual) / jogador.HpMaximo;
            int    barsJ  = (int)(pctJ * barLen);
            string barraJ = new string('█', barsJ) + new string('░', barLen - barsJ);

            double pctI   = (double)Math.Max(0, inimigo.HpAtual) / inimigo.HpMaximo;
            int    barsI  = (int)(pctI * barLen);
            string barraI = new string('█', barsI) + new string('░', barLen - barsI);

            Console.WriteLine($"  {jogador.Nome,-20} [{barraJ}] {Math.Max(0, jogador.HpAtual)}/{jogador.HpMaximo} HP");
            Console.WriteLine($"  {inimigo.Nome,-20} [{barraI}] {Math.Max(0, inimigo.HpAtual)}/{inimigo.HpMaximo} HP");
        }

        private void ExibirResumoRodada(int rodada, FichaPersonagem jogador, FichaInimigo inimigo)
        {
            Console.WriteLine($"\n  {'─',55}");
            Console.WriteLine($"  Fim da Rodada {rodada}:");
            string escudo = jogador.HpEscudoArcano > 0 ? $" | Escudo: {jogador.HpEscudoArcano}" : "";
            Console.WriteLine($"    {jogador.Nome,-25} HP: {Math.Max(0, jogador.HpAtual),3}/{jogador.HpMaximo,-3}{escudo}");
            Console.WriteLine($"    {inimigo.Nome,-25} HP: {Math.Max(0, inimigo.HpAtual),3}/{inimigo.HpMaximo,-3}");
            Console.WriteLine($"  {'─',55}");
        }

        private void ExibirVitoriaEstagio(int estagio, int xp)
        {
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════════════╗");
            Console.WriteLine($"  ║  ✔ ESTÁGIO {estagio,2} CONCLUÍDO!                              ║");
            Console.WriteLine($"  ║  XP Ganho: {xp,-42}║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════╝");
        }

        private void ExibirGameOver()
        {
            string linha2 = $"  {_jogador.Nome} foi derrotado no Estágio {EstagioAtual}.";
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║                  ✖  GAME OVER  ✖                    ║");
            Console.WriteLine($"  ║  {linha2.PadRight(52)}║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════╝");
        }

        private void ExibirVitoriaFinal()
        {
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║       ★ ★ ★  CAMPANHA CONCLUÍDA!  ★ ★ ★            ║");
            Console.WriteLine("  ╠══════════════════════════════════════════════════════╣");
            Console.WriteLine($"  ║  {_jogador.Nome,-50}║");
            Console.WriteLine("  ║  derrotou o Lich Primordial e salvou o reino!        ║");
            Console.WriteLine($"  ║  Nível final: {_jogador.Nivel,-38}║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════╝");
        }
    }
}

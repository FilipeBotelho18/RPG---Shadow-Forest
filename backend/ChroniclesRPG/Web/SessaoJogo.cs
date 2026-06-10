using ChroniclesRPG;
using ChroniclesRPG.Entidades;
using ChroniclesRPG.Entidades.Classes;
using ChroniclesRPG.Entidades.Itens;
using ChroniclesRPG.Funcoes;
using ChroniclesRPG.Sistemas;
using System.Text.Json;

namespace ChroniclesRPG.Web
{
    public class SessaoJogo
    {
        private readonly string _caminhoJornada;
        private readonly string _caminhoInimigos;
        private readonly string _caminhoSave;
        private readonly List<string> _log = new();
        private readonly List<string> _caminhosEscolhidos = new();

        private FichaPersonagem _jogador = null!;
        private List<EtapaJornada> _etapas = new();
        private int _indiceEtapa;
        private int _inimigosDerrotados;
        private EventoJornada? _eventoAtual;
        private Inimigo? _inimigoAtual;
        private string _fase = "criacao";
        private string _cenario = "guilda";
        private FeedbackCombate _feedback = new("neutro", "Escolha seu heroi");
        private bool _acaoPrincipalUsada;
        private bool _acaoBonusUsada;
        private bool _surtoDisponivel;
        private int _vantagemJogador;
        private int _desvantagemInimigo;
        private int _paralisiaInimigo;

        public SessaoJogo(string caminhoJornada, string caminhoInimigos)
        {
            _caminhoJornada = caminhoJornada;
            _caminhoInimigos = caminhoInimigos;
            _caminhoSave = Path.Combine(Path.GetDirectoryName(caminhoJornada) ?? ".", "savegame.json");

            if (!CarregarJogo())
            {
                NovoJogo();
            }
        }

        public void NovoJogo()
        {
            ScriptInicial.CarregarDados();
            _etapas = Jornada.CarregarEtapas(_caminhoJornada, _caminhoInimigos);
            _indiceEtapa = 0;
            _eventoAtual = null;
            _inimigoAtual = null;
            _inimigosDerrotados = 0;
            _fase = "criacao";
            _cenario = "guilda";
            _log.Clear();
            _caminhosEscolhidos.Clear();
            _feedback = new FeedbackCombate("neutro", "Escolha seu heroi");
            _acaoPrincipalUsada = false;
            _acaoBonusUsada = false;
            _surtoDisponivel = false;
            _vantagemJogador = 0;
            _desvantagemInimigo = 0;
            _paralisiaInimigo = 0;

            _jogador = new FichaPersonagem("Aventureiro", new Paladino())
            {
                Raca = "Humano",
                Ouro = 35
            };

            Registrar("A guilda Pluma de Prata recebe o chamado da Floresta de Letes.");
            SalvarJogo();
        }

        public void CriarPersonagem(string classe, string? nome)
        {
            ScriptInicial.CarregarDados();
            _etapas = Jornada.CarregarEtapas(_caminhoJornada, _caminhoInimigos);
            _indiceEtapa = 0;
            _eventoAtual = null;
            _inimigoAtual = null;
            _inimigosDerrotados = 0;
            _fase = "caminho";
            _cenario = "guilda";
            _log.Clear();
            _caminhosEscolhidos.Clear();
            _feedback = new FeedbackCombate("jornada", "Jornada iniciada");

            var classeEscolhida = CriarClasse(classe);
            string nomeHeroi = string.IsNullOrWhiteSpace(nome) ? NomePadrao(classeEscolhida.NomeDaClasse) : nome.Trim();
            _jogador = new FichaPersonagem(nomeHeroi, classeEscolhida)
            {
                Raca = "Humano",
                Ouro = OuroInicialPorClasse(classeEscolhida.NomeDaClasse)
            };

            EquiparInicial(classeEscolhida.NomeDaClasse);
            Registrar($"{_jogador.Nome}, {_jogador.Classe.NomeDaClasse}, parte da guilda Pluma de Prata.");
            Registrar($"Recurso inicial da classe: {_jogador.Ouro} ouro.");
            Registrar("O Bosque dos Lamentos aguarda alem da trilha principal.");
            SalvarJogo();
        }

        public void EscolherCaminho(string caminho)
        {
            if (_fase != "caminho" || JogoTerminou)
            {
                return;
            }

            var etapa = EtapaAtual;
            _eventoAtual = etapa.Eventos[CaminhoJornada.Meio];
            _cenario = CriarCenario(_eventoAtual);
            _caminhosEscolhidos.Add($"{etapa.Numero}. {_eventoAtual.Titulo}");
            _feedback = new FeedbackCombate(_eventoAtual.Boss ? "boss" : "jornada", _eventoAtual.Titulo);

            Registrar($"Etapa {etapa.Numero}: {etapa.Nome}");
            Registrar(_eventoAtual.Titulo);
            Registrar(_eventoAtual.Descricao);

            if (_eventoAtual.Tipo.Equals("cutscene", StringComparison.OrdinalIgnoreCase))
            {
                _fase = "cutscene";
                SalvarJogo();
                return;
            }

            if (_eventoAtual.Tipo.Equals("descanso", StringComparison.OrdinalIgnoreCase))
            {
                _fase = "descanso";
                SalvarJogo();
                return;
            }

            if (_eventoAtual.Tipo.Equals("loja", StringComparison.OrdinalIgnoreCase))
            {
                _fase = "loja";
                SalvarJogo();
                return;
            }

            AplicarRecompensas(_eventoAtual);
            _inimigoAtual = _eventoAtual.Inimigo;
            if (_inimigoAtual != null)
            {
                _fase = "combate";
                IniciarTurnoJogador();
                _surtoDisponivel = _jogador.Classe.NomeDaClasse == "Guerreiro";
                Registrar($"Combate iniciado contra {_inimigoAtual.Nome}.");
                SalvarJogo();
                return;
            }

            AvancarEtapa();
            SalvarJogo();
        }

        public void ExecutarEvento(string acao, string? item)
        {
            if (JogoTerminou)
            {
                return;
            }

            string acaoNormalizada = acao.ToLowerInvariant();

            if (_fase == "descanso" && acaoNormalizada == "descansar")
            {
                Descansar();
                AvancarEtapa();
            }
            else if (_fase == "loja" && acaoNormalizada == "comprar")
            {
                Comprar(item ?? string.Empty);
            }
            else if ((_fase == "loja" || _fase == "cutscene") && acaoNormalizada == "continuar")
            {
                AvancarEtapa();
            }

            SalvarJogo();
        }

        public void ExecutarAcao(string acao)
        {
            if (_fase != "combate" || _inimigoAtual == null || JogoTerminou)
            {
                return;
            }

            string acaoNormalizada = acao.ToLowerInvariant();

            if (acaoNormalizada == "encerrar")
            {
                EncerrarTurnoJogador();
                SalvarJogo();
                return;
            }

            bool acaoBonus = acaoNormalizada is "cura" or "pocao" or "folego" or "foco" or "surto"
                or "escudo_fe" or "trovejante" or "arma_magica";
            if (acaoBonus && _acaoBonusUsada)
            {
                Registrar("A acao bonus deste turno ja foi usada.");
                return;
            }

            if (!acaoBonus && _acaoPrincipalUsada)
            {
                Registrar("A acao principal deste turno ja foi usada.");
                return;
            }

            if (acaoBonus)
            {
                ExecutarAcaoBonus(acaoNormalizada);
                _acaoBonusUsada = true;
            }
            else
            {
                ExecutarAcaoPrincipal(acaoNormalizada);
                _acaoPrincipalUsada = true;
            }

            if (_inimigoAtual is { EstaVivo: false })
            {
                ConcluirCombate();
            }
            else if (_jogador.HpAtual <= 0)
            {
                DerrotarJogador();
            }

            SalvarJogo();
        }

        public RespostaJogo CriarResposta()
        {
            return new RespostaJogo(
                _fase,
                _indiceEtapa + 1,
                _etapas.Count,
                JogoTerminou && _jogador.HpAtual > 0,
                JogoTerminou && _jogador.HpAtual <= 0,
                CriarResumoJogador(),
                CriarResumoEtapa(),
                CriarOpcoesCaminho(),
                CriarResumoInimigo(),
                _cenario,
                _feedback,
                CriarResumoFinal(),
                CriarResumoTurno(),
                CriarResumoEvento(),
                CriarLoja(),
                CriarClasses(),
                _log.TakeLast(14).ToList());
        }

        private EtapaJornada EtapaAtual => _etapas[Math.Clamp(_indiceEtapa, 0, _etapas.Count - 1)];
        private bool JogoTerminou => _fase == "fim" || _indiceEtapa >= _etapas.Count;

        private void ExecutarAcaoPrincipal(string acao)
        {
            switch (_jogador.Classe.NomeDaClasse)
            {
                case "Mago":
                    ExecutarAcaoMago(acao);
                    break;
                case "Guerreiro":
                    ExecutarAcaoGuerreiro(acao);
                    break;
                default:
                    ExecutarAcaoPaladino(acao);
                    break;
            }
        }

        private void ExecutarAcaoPaladino(string acao)
        {
            switch (acao)
            {
                case "bencao":
                    if (GastarSlot(1))
                    {
                        _jogador.TemBencao = true;
                        Registrar($"{_jogador.Nome} conjura Bencao. O proximo ataque recebe +1d4.");
                        _feedback = new FeedbackCombate("status", "Bencao");
                    }
                    return;
                case "ajuda":
                    if (GastarSlot(2))
                    {
                        int aumento = 5;
                        _jogador.HpMaximo += aumento;
                        _jogador.HpAtual = Math.Min(_jogador.HpMaximo, _jogador.HpAtual + aumento);
                        Registrar($"Ajuda fortalece {_jogador.Nome}: +{aumento} HP maximo.");
                        _feedback = new FeedbackCombate("cura", $"+{aumento} HP");
                    }
                    return;
                case "restauracao":
                    if (GastarSlot(2))
                    {
                        _desvantagemInimigo = 0;
                        int cura = Math.Min(8, _jogador.HpMaximo - _jogador.HpAtual);
                        _jogador.HpAtual += cura;
                        Registrar($"Restauracao Menor remove efeitos negativos e recupera {cura} HP.");
                        _feedback = new FeedbackCombate("cura", $"+{cura} HP");
                    }
                    return;
                default:
                    AtacarInimigo(usarSmite: acao == "smite", dadoExtra: null, nomeAcao: acao == "smite" ? "Destruicao Divina" : "Ataque");
                    return;
            }
        }

        private void ExecutarAcaoGuerreiro(string acao)
        {
            if (acao == "golpe")
            {
                AtacarInimigo(usarSmite: false, dadoExtra: "1d6", nomeAcao: "Golpe preciso");
                _desvantagemInimigo = 1;
                Registrar("O inimigo fica em desvantagem no proximo ataque.");
                return;
            }

            AtacarInimigo(usarSmite: false, dadoExtra: null, nomeAcao: "Ataque");
        }

        private void ExecutarAcaoMago(string acao)
        {
            if (acao == "raio_gelo")
            {
                AtacarMagico("Raio de Gelo", _jogador.Nivel >= 5 ? "2d8" : "1d8");
                _desvantagemInimigo = 1;
                return;
            }

            if (acao == "raio_fogo")
            {
                AtacarMagico("Raio de Fogo", _jogador.Nivel >= 5 ? "2d10" : "1d10");
                return;
            }

            if (acao == "protecao_laminas")
            {
                _jogador.TemResistenciaFisica = true;
                Registrar($"{_jogador.Nome} usa Protecao contra Laminas. Dano fisico recebido sera reduzido neste turno.");
                _feedback = new FeedbackCombate("status", "Resistencia");
                return;
            }

            if (acao == "protecao_arcana")
            {
                int escudo = Math.Max(1, _jogador.Nivel * 2 + _jogador.ModificadorInteligencia);
                _jogador.HpEscudoArcano = escudo;
                _jogador.HpMaxEscudoArcano = escudo;
                _jogador.ClasseArmadura += 2;
                Registrar($"Protecao Arcana cria um escudo de {escudo} HP e concede +2 CA enquanto durar.");
                _feedback = new FeedbackCombate("status", "Escudo Arcano");
                return;
            }

            if (acao == "misil")
            {
                if (!GastarSlot(1))
                {
                    return;
                }

                int dano = Dados.Rolar("3d4") + 3;
                AplicarDanoInimigo(dano, "Misseis Magicos");
                _feedback = new FeedbackCombate("magia", $"-{dano} HP");
                return;
            }

            if (acao == "paralisar")
            {
                if (!GastarSlot(2) && !GastarSlot(1))
                {
                    return;
                }

                _paralisiaInimigo = 1;
                Registrar($"{_jogador.Nome} prende o inimigo com energia arcana.");
                _feedback = new FeedbackCombate("status", "Paralisado");
                return;
            }

            AtacarMagico("Rajada Arcana", "1d10");
        }

        private void ExecutarAcaoBonus(string acao)
        {
            switch (acao)
            {
                case "cura":
                    CurarPelasMaos(5);
                    break;
                case "pocao":
                    UsarPocao();
                    break;
                case "folego":
                    RetomarFolego();
                    break;
                case "foco":
                    _vantagemJogador = 1;
                    Registrar($"{_jogador.Nome} assume postura de foco. O proximo ataque tera vantagem.");
                    _feedback = new FeedbackCombate("status", "Vantagem");
                    break;
                case "surto":
                    if (_surtoDisponivel && _jogador.Classe.NomeDaClasse == "Guerreiro")
                    {
                        _surtoDisponivel = false;
                        _acaoPrincipalUsada = false;
                        Registrar("Surto de Acao: o Guerreiro recupera a acao principal neste turno.");
                        _feedback = new FeedbackCombate("status", "Surto");
                    }
                    else
                    {
                        Registrar("Surto de Acao indisponivel.");
                    }
                    break;
                case "escudo_fe":
                    if (_jogador.Classe.NomeDaClasse == "Paladino" && GastarSlot(1))
                    {
                        if (!_jogador.TemEscudoDaFe)
                        {
                            _jogador.TemEscudoDaFe = true;
                            _jogador.ClasseArmadura += 2;
                        }
                        Registrar("Escudo da Fe ativo: +2 CA.");
                        _feedback = new FeedbackCombate("status", "+2 CA");
                    }
                    break;
                case "trovejante":
                    if (_jogador.Classe.NomeDaClasse == "Paladino" && GastarSlot(1))
                    {
                        _jogador.ProximoAtaqueTrovejante = true;
                        Registrar("Destruicao Trovejante preparada para o proximo ataque.");
                        _feedback = new FeedbackCombate("status", "Trovejante");
                    }
                    break;
                case "arma_magica":
                    if (_jogador.Classe.NomeDaClasse == "Paladino" && GastarSlot(2))
                    {
                        _jogador.TemArmaMagica = true;
                        Registrar("Arma Magica ativa: +1 no ataque e no dano.");
                        _feedback = new FeedbackCombate("status", "Arma +1");
                    }
                    break;
            }
        }

        private void EncerrarTurnoJogador()
        {
            if (_inimigoAtual == null)
            {
                return;
            }

            if (!_acaoPrincipalUsada)
            {
                Registrar("Use uma acao principal antes de encerrar o turno.");
                return;
            }

            if (_paralisiaInimigo > 0)
            {
                _paralisiaInimigo--;
                Registrar($"{_inimigoAtual.Nome} esta paralisado e perde o turno.");
                _feedback = new FeedbackCombate("defesa", "Inimigo paralisado");
            }
            else
            {
                AtacarJogador();
            }

            if (_jogador.HpAtual <= 0)
            {
                DerrotarJogador();
                return;
            }

            IniciarTurnoJogador();
        }

        private void IniciarTurnoJogador()
        {
            _acaoPrincipalUsada = false;
            _acaoBonusUsada = false;
        }

        private void ConcluirCombate()
        {
            if (_inimigoAtual == null)
            {
                return;
            }

            Registrar($"{_inimigoAtual.Nome} foi derrotado.");
            _inimigosDerrotados++;
            _jogador.GanharXP(_inimigoAtual.XPConcedido);
            _jogador.SubirNivel();
            _inimigoAtual = null;
            _paralisiaInimigo = 0;
            _desvantagemInimigo = 0;
            _vantagemJogador = 0;
            _acaoPrincipalUsada = false;
            _acaoBonusUsada = false;
            _surtoDisponivel = false;
            AvancarEtapa();
        }

        private void DerrotarJogador()
        {
            _fase = "fim";
            Registrar("Derrota. O miasma ainda domina a floresta.");
        }

        private void AplicarRecompensas(EventoJornada eventoJornada)
        {
            if (eventoJornada.Cura > 0)
            {
                int hpAntes = _jogador.HpAtual;
                _jogador.HpAtual = Math.Min(_jogador.HpMaximo, _jogador.HpAtual + eventoJornada.Cura);
                Registrar($"HP recuperado: {_jogador.HpAtual - hpAntes}.");
            }

            if (eventoJornada.Ouro > 0)
            {
                _jogador.Ouro += eventoJornada.Ouro;
                Registrar($"Ouro recebido: {eventoJornada.Ouro}.");
            }

            if (eventoJornada.ConcedePocao)
            {
                var pocao = ScriptInicial.Consumiveis.First();
                _jogador.Inventario.Add(pocao);
                Registrar($"Item recebido: {pocao.Nome}.");
            }

            if (eventoJornada.XP > 0)
            {
                _jogador.GanharXP(eventoJornada.XP);
                _jogador.SubirNivel();
                Registrar($"XP recebido: {eventoJornada.XP}.");
            }
        }

        private void AvancarEtapa()
        {
            _indiceEtapa++;

            if (_indiceEtapa >= _etapas.Count)
            {
                _fase = "fim";
                _cenario = "lich";
                Registrar("Vitoria! O mal antigo e selado, e a Floresta de Letes respira novamente.");
            }
            else
            {
                _fase = "caminho";
                _eventoAtual = null;
            }
        }

        private void AtacarInimigo(bool usarSmite, string? dadoExtra, string nomeAcao)
        {
            if (_jogador.ArmaEquipada == null || _inimigoAtual == null)
            {
                return;
            }

            int rolagem = RolarD20Jogador(out string rolagemTexto);
            int modificador = _jogador.ArmaEquipada.UsaDestreza ? _jogador.ModificadorDestreza : _jogador.ModificadorForca;
            int bonusBencao = _jogador.TemBencao ? Dados.Rolar("1d4") : 0;
            int bonusMagico = _jogador.TemArmaMagica ? 1 : 0;
            int bonusAtaque = modificador + BonusProficiencia(_jogador.Nivel) + bonusBencao + bonusMagico;
            int totalAtaque = rolagem + bonusAtaque;

            Registrar($"{_jogador.Nome} usa {nomeAcao}: {rolagemTexto} + {bonusAtaque} = {totalAtaque} vs CA {_inimigoAtual.ClasseArmadura}.");

            if (rolagem == 1)
            {
                Registrar("Erro critico.");
                _feedback = new FeedbackCombate("erro", "Erro critico");
                return;
            }

            if (rolagem != 20 && totalAtaque < _inimigoAtual.ClasseArmadura)
            {
                Registrar("O ataque errou.");
                _feedback = new FeedbackCombate("erro", "Errou");
                return;
            }

            int dano = Dados.Rolar(_jogador.ArmaEquipada.DadoDeDano) + modificador + bonusMagico;
            if (!string.IsNullOrWhiteSpace(dadoExtra))
            {
                dano += Dados.Rolar(dadoExtra);
            }

            if (rolagem == 20)
            {
                dano += Dados.Rolar(_jogador.ArmaEquipada.DadoDeDano);
                Registrar("Acerto critico!");
                _feedback = new FeedbackCombate("critico", "Critico");
            }

            if (usarSmite && _jogador.Classe.NomeDaClasse == "Paladino" && GastarSlot(1))
            {
                int danoSmite = Dados.Rolar("2d8");
                dano += danoSmite;
                Registrar($"Destruicao Divina adicionou {danoSmite} de dano radiante.");
                _feedback = new FeedbackCombate("smite", $"Smite +{danoSmite}");
            }

            if (_jogador.ProximoAtaqueTrovejante)
            {
                int danoTrovejante = Dados.Rolar("2d6");
                dano += danoTrovejante;
                _jogador.ProximoAtaqueTrovejante = false;
                Registrar($"Destruicao Trovejante adicionou {danoTrovejante} de dano.");
            }

            AplicarDanoInimigo(Math.Max(1, dano), nomeAcao);
        }

        private void AtacarMagico(string nomeAcao, string dadoDano)
        {
            if (_inimigoAtual == null)
            {
                return;
            }

            int rolagem = RolarD20Jogador(out string rolagemTexto);
            int bonusAtaque = _jogador.ModificadorInteligencia + BonusProficiencia(_jogador.Nivel);
            int totalAtaque = rolagem + bonusAtaque;
            Registrar($"{_jogador.Nome} conjura {nomeAcao}: {rolagemTexto} + {bonusAtaque} = {totalAtaque} vs CA {_inimigoAtual.ClasseArmadura}.");

            if (rolagem == 1 || (rolagem != 20 && totalAtaque < _inimigoAtual.ClasseArmadura))
            {
                Registrar("A magia errou.");
                _feedback = new FeedbackCombate("erro", "Errou");
                return;
            }

            int dano = Dados.Rolar(dadoDano) + _jogador.ModificadorInteligencia;
            if (rolagem == 20)
            {
                dano += Dados.Rolar(dadoDano);
            }

            AplicarDanoInimigo(Math.Max(1, dano), nomeAcao);
        }

        private void AplicarDanoInimigo(int dano, string fonte)
        {
            if (_inimigoAtual == null)
            {
                return;
            }

            _inimigoAtual.HpAtual = Math.Max(0, _inimigoAtual.HpAtual - dano);
            Registrar($"{fonte} causa {dano} de dano em {_inimigoAtual.Nome}.");
            if (_feedback.Tipo != "critico" && _feedback.Tipo != "smite")
            {
                _feedback = new FeedbackCombate("dano", $"-{dano} HP");
            }
        }

        private void AtacarJogador()
        {
            if (_inimigoAtual == null)
            {
                return;
            }

            int rolagem = RolarD20Inimigo(out string rolagemTexto);
            int totalAtaque = rolagem + _inimigoAtual.BonusAtaque;
            Registrar($"{_inimigoAtual.Nome} ataca: {rolagemTexto} + {_inimigoAtual.BonusAtaque} = {totalAtaque} vs CA {_jogador.ClasseArmadura}.");

            if (rolagem == 1 || (rolagem != 20 && totalAtaque < _jogador.ClasseArmadura))
            {
                Registrar($"{_inimigoAtual.Nome} errou.");
                _feedback = new FeedbackCombate("defesa", "Inimigo errou");
                return;
            }

            int dano = Dados.Rolar(_inimigoAtual.DadoDeDano) + _inimigoAtual.BonusDano;
            if (rolagem == 20)
            {
                dano += Dados.Rolar(_inimigoAtual.DadoDeDano);
                Registrar("O inimigo conseguiu um critico.");
            }

            dano = Math.Max(1, dano);
            int danoEfetivo = _jogador.ReceberDano(dano);
            Registrar($"{_jogador.Nome} sofreu {danoEfetivo} de dano.");
            _feedback = new FeedbackCombate("sofreu-dano", $"-{danoEfetivo} HP");
        }

        private int RolarD20Jogador(out string texto)
        {
            if (_vantagemJogador > 0)
            {
                int a = Dados.RolarD20();
                int b = Dados.RolarD20();
                _vantagemJogador--;
                texto = $"d20({a}/{b}, vantagem)";
                return Math.Max(a, b);
            }

            int rolagem = Dados.RolarD20();
            texto = $"d20({rolagem})";
            return rolagem;
        }

        private int RolarD20Inimigo(out string texto)
        {
            if (_desvantagemInimigo > 0)
            {
                int a = Dados.RolarD20();
                int b = Dados.RolarD20();
                _desvantagemInimigo--;
                texto = $"d20({a}/{b}, desvantagem)";
                return Math.Min(a, b);
            }

            int rolagem = Dados.RolarD20();
            texto = $"d20({rolagem})";
            return rolagem;
        }

        private void CurarPelasMaos(int quantidade)
        {
            if (_jogador.Classe.NomeDaClasse != "Paladino")
            {
                Registrar("Apenas Paladinos possuem Cura pelas Maos.");
                return;
            }

            if (_jogador.CuraPelasMaosRestante <= 0)
            {
                Registrar("A reserva de Cura pelas Maos acabou.");
                return;
            }

            int cura = Math.Min(quantidade, _jogador.CuraPelasMaosRestante);
            cura = Math.Min(cura, _jogador.HpMaximo - _jogador.HpAtual);
            _jogador.CuraPelasMaosRestante -= cura;
            _jogador.HpAtual += cura;
            Registrar($"{_jogador.Nome} recuperou {cura} HP.");
            _feedback = new FeedbackCombate("cura", $"+{cura} HP");
        }

        private void RetomarFolego()
        {
            if (_jogador.Classe.NomeDaClasse != "Guerreiro")
            {
                Registrar("Apenas Guerreiros podem Retomar Folego.");
                return;
            }

            int cura = Dados.Rolar("1d10") + _jogador.Nivel;
            int hpAntes = _jogador.HpAtual;
            _jogador.HpAtual = Math.Min(_jogador.HpMaximo, _jogador.HpAtual + cura);
            Registrar($"{_jogador.Nome} retoma o folego e recupera {_jogador.HpAtual - hpAntes} HP.");
            _feedback = new FeedbackCombate("cura", $"+{_jogador.HpAtual - hpAntes} HP");
        }

        private void UsarPocao()
        {
            var pocao = _jogador.Inventario.OfType<Consumivel>().FirstOrDefault();
            if (pocao == null)
            {
                Registrar("Voce nao tem pocoes.");
                return;
            }

            int hpAntes = _jogador.HpAtual;
            pocao.Usar(_jogador);
            Registrar($"Pocao usada. HP recuperado: {_jogador.HpAtual - hpAntes}.");
            _feedback = new FeedbackCombate("cura", $"+{_jogador.HpAtual - hpAntes} HP");
        }

        private bool GastarSlot(int nivelSlot)
        {
            if (!_jogador.SlotsDeMagia.TryGetValue(nivelSlot, out int quantidade) || quantidade <= 0)
            {
                Registrar("Sem slots de magia suficientes.");
                return false;
            }

            _jogador.SlotsDeMagia[nivelSlot] = quantidade - 1;
            return true;
        }

        private void Descansar()
        {
            _jogador.HpAtual = _jogador.HpMaximo;
            _jogador.CuraPelasMaosRestante = _jogador.Classe.NomeDaClasse == "Paladino" ? _jogador.Nivel * 5 : 0;
            RestaurarSlotsPorNivel();
            Registrar($"{_jogador.Nome} descansa e recupera HP e recursos.");
            _feedback = new FeedbackCombate("cura", "Descanso completo");
        }

        private void Comprar(string itemId)
        {
            var item = CriarLoja().FirstOrDefault(entrada => entrada.Id == itemId);
            if (item == null)
            {
                Registrar("Item nao encontrado.");
                return;
            }

            if (_jogador.Ouro < item.Preco)
            {
                Registrar("Ouro insuficiente.");
                _feedback = new FeedbackCombate("erro", "Sem ouro");
                return;
            }

            _jogador.Ouro -= item.Preco;
            if (item.Tipo == "consumivel")
            {
                var consumivel = item.Id == "pocao-maior"
                    ? ScriptInicial.Consumiveis.First(consumivel => consumivel.Nome.Contains("Maior", StringComparison.OrdinalIgnoreCase))
                    : ScriptInicial.Consumiveis.First();
                _jogador.Inventario.Add(consumivel);
            }
            else if (item.Tipo == "arma")
            {
                var arma = ScriptInicial.Armas.First(arma => arma.Nome == item.Nome);
                _jogador.EquiparArma(arma);
            }
            else if (item.Tipo == "armadura")
            {
                var armadura = ScriptInicial.Armaduras.First(armadura => armadura.Nome == item.Nome);
                _jogador.EquiparArmadura(armadura);
            }

            Registrar($"{item.Nome} comprado.");
            _feedback = new FeedbackCombate("jornada", "Compra feita");
        }

        private void EquiparInicial(string classe)
        {
            if (classe == "Mago")
            {
                _jogador.EquiparArma(ScriptInicial.Armas.First(a => a.Nome == "Cajado de Mago"));
            }
            else if (classe == "Guerreiro")
            {
                _jogador.EquiparArmadura(ScriptInicial.Armaduras.First(a => a.Nome == "Cota de Malha"));
                _jogador.EquiparArma(ScriptInicial.Armas.First(a => a.Nome == "Espada Grande"));
            }
            else
            {
                _jogador.EquiparArmadura(ScriptInicial.Armaduras.First(a => a.Nome == "Cota de Malha"));
                _jogador.EquiparArma(ScriptInicial.Armas.First(a => a.Nome == "Espada Longa"));
                _jogador.CuraPelasMaosRestante = _jogador.Nivel * 5;
            }

            _jogador.Inventario.Add(ScriptInicial.Consumiveis.First());
        }

        private void RestaurarSlotsPorNivel()
        {
            _jogador.SlotsDeMagia.Clear();
            string classe = _jogador.Classe.NomeDaClasse;
            if (classe == "Paladino")
            {
                if (_jogador.Nivel >= 2) _jogador.SlotsDeMagia[1] = _jogador.Nivel >= 5 ? 4 : _jogador.Nivel >= 3 ? 3 : 2;
                if (_jogador.Nivel >= 5) _jogador.SlotsDeMagia[2] = 2;
            }
            else if (classe == "Mago")
            {
                _jogador.SlotsDeMagia[1] = _jogador.Nivel >= 3 ? 4 : _jogador.Nivel >= 2 ? 3 : 2;
                if (_jogador.Nivel >= 3) _jogador.SlotsDeMagia[2] = _jogador.Nivel >= 5 ? 3 : 2;
                if (_jogador.Nivel >= 5) _jogador.SlotsDeMagia[3] = 1;
            }
        }

        private JogadorResumo CriarResumoJogador()
        {
            int pocoes = _jogador.Inventario.OfType<Consumivel>().Count();
            int slotsNivel1 = _jogador.SlotsDeMagia.TryGetValue(1, out int qtd1) ? qtd1 : 0;
            int slotsNivel2 = _jogador.SlotsDeMagia.TryGetValue(2, out int qtd2) ? qtd2 : 0;
            int slotsNivel3 = _jogador.SlotsDeMagia.TryGetValue(3, out int qtd3) ? qtd3 : 0;
            var atributos = new AtributosResumo(_jogador.Forca, _jogador.Destreza, _jogador.Constituicao, _jogador.Inteligencia, _jogador.Sabedoria, _jogador.Carisma);
            var inventario = _jogador.Inventario.Select(item => item.Nome).ToList();

            return new JogadorResumo(
                _jogador.Nome,
                _jogador.Classe.NomeDaClasse,
                _jogador.Nivel,
                _jogador.XP,
                _jogador.HpAtual,
                _jogador.HpMaximo,
                _jogador.ClasseArmadura,
                _jogador.Ouro,
                _jogador.CuraPelasMaosRestante,
                slotsNivel1,
                slotsNivel2,
                slotsNivel3,
                pocoes,
                _jogador.ArmaEquipada?.Nome ?? "Nenhuma",
                _jogador.ArmaduraVestida?.Nome ?? "Nenhuma",
                atributos,
                inventario);
        }

        private EtapaResumo CriarResumoEtapa()
        {
            if (JogoTerminou)
            {
                return new EtapaResumo(0, "Fim da Jornada");
            }

            var etapa = EtapaAtual;
            return new EtapaResumo(etapa.Numero, etapa.Nome);
        }

        private InimigoResumo? CriarResumoInimigo()
        {
            if (_inimigoAtual == null)
            {
                return null;
            }

            bool boss = _eventoAtual?.Boss == true || _inimigoAtual.Nome.Contains("Lich", StringComparison.OrdinalIgnoreCase);
            return new InimigoResumo(_inimigoAtual.Nome, _inimigoAtual.HpAtual, _inimigoAtual.HpMaximo, _inimigoAtual.ClasseArmadura, boss);
        }

        private ResumoFinal? CriarResumoFinal()
        {
            if (!JogoTerminou)
            {
                return null;
            }

            string resultado = _jogador.HpAtual > 0
                ? "A floresta e purificada, e a Pluma de Prata ganha uma nova lenda."
                : $"{_jogador.Nome} caiu antes de conter o miasma do Bosque dos Lamentos.";

            return new ResumoFinal(
                resultado,
                _jogador.Nivel,
                _inimigosDerrotados,
                _jogador.Ouro,
                _caminhosEscolhidos.ToList());
        }

        private TurnoResumo CriarResumoTurno()
        {
            return new TurnoResumo(_acaoPrincipalUsada, _acaoBonusUsada, _surtoDisponivel, _vantagemJogador > 0, _desvantagemInimigo > 0, _paralisiaInimigo > 0);
        }

        private EventoResumo? CriarResumoEvento()
        {
            if (_eventoAtual == null || _fase is "caminho" or "combate" or "fim" or "criacao")
            {
                return null;
            }

            return new EventoResumo(_eventoAtual.Tipo, _eventoAtual.Titulo, _eventoAtual.Descricao, _eventoAtual.Cena);
        }

        private List<CaminhoResumo> CriarOpcoesCaminho()
        {
            if (JogoTerminou || _fase != "caminho")
            {
                return new List<CaminhoResumo>();
            }

            var etapa = EtapaAtual;
            var evento = etapa.Eventos[CaminhoJornada.Meio];
            string natureza = evento.Tipo switch
            {
                "descanso" => "Zona segura",
                "loja" => "Mercador",
                "cutscene" => "Historia",
                _ => evento.Boss ? "Chefe" : "Combate 1x1"
            };

            return new List<CaminhoResumo>
            {
                CriarCaminhoResumo("avancar", "Iniciar", natureza, evento)
            };
        }

        private static CaminhoResumo CriarCaminhoResumo(string id, string nome, string natureza, EventoJornada eventoJornada)
        {
            string risco = eventoJornada.Inimigo == null ? "Sem combate" : $"Combate: {eventoJornada.Inimigo.Nome}";
            string recompensa = CriarTextoRecompensa(eventoJornada);
            return new CaminhoResumo(id, nome, natureza, eventoJornada.Titulo, eventoJornada.Descricao, risco, recompensa, eventoJornada.Boss);
        }

        private static string CriarTextoRecompensa(EventoJornada eventoJornada)
        {
            var partes = new List<string>();
            if (eventoJornada.XP > 0) partes.Add($"+{eventoJornada.XP} XP");
            if (eventoJornada.Cura > 0) partes.Add($"+{eventoJornada.Cura} HP");
            if (eventoJornada.Ouro > 0) partes.Add($"+{eventoJornada.Ouro} ouro");
            if (eventoJornada.ConcedePocao) partes.Add("+1 pocao");
            if (eventoJornada.Tipo == "descanso") partes.Add("recupera recursos");
            if (eventoJornada.Tipo == "loja") partes.Add("compra itens");
            return partes.Count == 0 ? "Sem recompensa imediata" : string.Join(" | ", partes);
        }

        private List<LojaItemResumo> CriarLoja()
        {
            return new List<LojaItemResumo>
            {
                new("pocao", "Pocao de Cura", "consumivel", 10, "Recupera HP em combate."),
                new("pocao-maior", "Pocao de Cura Maior", "consumivel", 50, "Recupera bastante HP."),
                new("arma-paladino", "Martelo de Guerra", "arma", 45, "Boa arma de uma mao para Paladino."),
                new("arma-guerreiro", "Machado Grande", "arma", 60, "Dano pesado para Guerreiro."),
                new("arma-mago", "Cajado de Mago", "arma", 25, "Foco arcano simples."),
                new("armadura", "Cota de Talas", "armadura", 120, "Aumenta a CA de classes marciais.")
            };
        }

        private static List<ClasseResumo> CriarClasses()
        {
            return new List<ClasseResumo>
            {
                new("guerreiro", "Guerreiro", "O Mestre de Armas", "Alta resistencia, Retomar Folego, Golpe Preciso e Surto de Acao.", "assets/guerreiro.png"),
                new("mago", "Mago", "O Erudito Arcano", "Baixo HP, magias de dano automatico e controle com paralisia.", "assets/mago.png"),
                new("paladino", "Paladino", "O Baluarte", "Armadura alta, Cura pelas Maos e Destruicao Divina.", "assets/paladino.png")
            };
        }

        private static IClasseRPG CriarClasse(string? classe)
        {
            return (classe ?? "paladino").ToLowerInvariant() switch
            {
                "guerreiro" => new Guerreiro(),
                "mago" => new Mago(),
                _ => new Paladino()
            };
        }

        private static string NomePadrao(string classe)
        {
            return classe switch
            {
                "Guerreiro" => "Kael Varn",
                "Mago" => "Eldrin",
                _ => "Sir Alden"
            };
        }

        private static int OuroInicialPorClasse(string classe)
        {
            return classe switch
            {
                "Guerreiro" => 30,
                "Mago" => 45,
                _ => 35
            };
        }

        private static int BonusProficiencia(int nivel)
        {
            return nivel >= 5 ? 3 : 2;
        }

        private static string CriarCenario(EventoJornada evento)
        {
            if (!string.IsNullOrWhiteSpace(evento.Cena))
            {
                return evento.Cena;
            }

            var inimigo = evento.Inimigo;
            if (evento.Tipo == "loja") return "loja";
            if (evento.Tipo == "descanso") return "cidade";
            if (evento.Tipo == "cutscene") return "guilda";
            if (inimigo?.Nome.Contains("Lich", StringComparison.OrdinalIgnoreCase) == true) return "lich";
            if (inimigo?.Nome.Contains("Manticora", StringComparison.OrdinalIgnoreCase) == true) return "pedras";
            if (inimigo?.Nome.Contains("Zumbi", StringComparison.OrdinalIgnoreCase) == true) return "ruinas";
            if (inimigo?.Nome.Contains("Arvore", StringComparison.OrdinalIgnoreCase) == true) return "raizes";
            if (inimigo?.Nome.Contains("Ogro", StringComparison.OrdinalIgnoreCase) == true) return "pedras";
            return "bosque";
        }

        private void Registrar(string mensagem)
        {
            _log.Add(mensagem);
        }

        private void SalvarJogo()
        {
            var save = new SaveGameDto
            {
                Fase = _fase,
                IndiceEtapa = _indiceEtapa,
                InimigosDerrotados = _inimigosDerrotados,
                Cenario = _cenario,
                Feedback = _feedback,
                Log = _log.ToList(),
                CaminhosEscolhidos = _caminhosEscolhidos.ToList(),
                AcaoPrincipalUsada = _acaoPrincipalUsada,
                AcaoBonusUsada = _acaoBonusUsada,
                SurtoDisponivel = _surtoDisponivel,
                VantagemJogador = _vantagemJogador,
                DesvantagemInimigo = _desvantagemInimigo,
                ParalisiaInimigo = _paralisiaInimigo,
                Jogador = new JogadorSaveDto
                {
                    Nome = _jogador.Nome,
                    Classe = _jogador.Classe.NomeDaClasse,
                    Nivel = _jogador.Nivel,
                    XP = _jogador.XP,
                    Ouro = _jogador.Ouro,
                    Raca = _jogador.Raca,
                    Lore = _jogador.Lore,
                    Forca = _jogador.Forca,
                    Destreza = _jogador.Destreza,
                    Constituicao = _jogador.Constituicao,
                    Inteligencia = _jogador.Inteligencia,
                    Sabedoria = _jogador.Sabedoria,
                    Carisma = _jogador.Carisma,
                    ClasseArmadura = _jogador.ClasseArmadura,
                    HpMaximo = _jogador.HpMaximo,
                    HpAtual = _jogador.HpAtual,
                    CuraPelasMaosRestante = _jogador.CuraPelasMaosRestante,
                    TemAtaqueExtra = _jogador.TemAtaqueExtra,
                    ArmaEquipada = _jogador.ArmaEquipada?.Nome,
                    ArmaduraVestida = _jogador.ArmaduraVestida?.Nome,
                    Inventario = _jogador.Inventario.Select(item => item.Nome).ToList(),
                    SlotsDeMagia = new Dictionary<int, int>(_jogador.SlotsDeMagia)
                },
                InimigoAtual = _inimigoAtual == null ? null : new InimigoSaveDto
                {
                    Nome = _inimigoAtual.Nome,
                    ClasseArmadura = _inimigoAtual.ClasseArmadura,
                    HpMaximo = _inimigoAtual.HpMaximo,
                    HpAtual = _inimigoAtual.HpAtual,
                    BonusAtaque = _inimigoAtual.BonusAtaque,
                    DadoDeDano = _inimigoAtual.DadoDeDano,
                    BonusDano = _inimigoAtual.BonusDano,
                    XPConcedido = _inimigoAtual.XPConcedido
                }
            };

            var opcoes = new JsonSerializerOptions { WriteIndented = true };
            Directory.CreateDirectory(Path.GetDirectoryName(_caminhoSave) ?? ".");
            File.WriteAllText(_caminhoSave, JsonSerializer.Serialize(save, opcoes));
        }

        private bool CarregarJogo()
        {
            if (!File.Exists(_caminhoSave))
            {
                return false;
            }

            try
            {
                ScriptInicial.CarregarDados();
                _etapas = Jornada.CarregarEtapas(_caminhoJornada, _caminhoInimigos);
                var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var save = JsonSerializer.Deserialize<SaveGameDto>(File.ReadAllText(_caminhoSave), opcoes);
                if (save?.Jogador == null)
                {
                    return false;
                }

                _fase = save.Fase;
                _indiceEtapa = Math.Clamp(save.IndiceEtapa, 0, _etapas.Count);
                _inimigosDerrotados = save.InimigosDerrotados;
                _cenario = string.IsNullOrWhiteSpace(save.Cenario) ? "bosque" : save.Cenario;
                _feedback = save.Feedback ?? new FeedbackCombate("neutro", "Jogo carregado");
                _acaoPrincipalUsada = save.AcaoPrincipalUsada;
                _acaoBonusUsada = save.AcaoBonusUsada;
                _surtoDisponivel = save.SurtoDisponivel;
                _vantagemJogador = save.VantagemJogador;
                _desvantagemInimigo = save.DesvantagemInimigo;
                _paralisiaInimigo = save.ParalisiaInimigo;

                _log.Clear();
                _log.AddRange(save.Log);
                _caminhosEscolhidos.Clear();
                _caminhosEscolhidos.AddRange(save.CaminhosEscolhidos);

                _jogador = new FichaPersonagem(save.Jogador.Nome, CriarClasse(save.Jogador.Classe))
                {
                    Nivel = save.Jogador.Nivel,
                    XP = save.Jogador.XP,
                    Ouro = save.Jogador.Ouro,
                    Raca = save.Jogador.Raca,
                    Lore = save.Jogador.Lore,
                    Forca = save.Jogador.Forca,
                    Destreza = save.Jogador.Destreza,
                    Constituicao = save.Jogador.Constituicao,
                    Inteligencia = save.Jogador.Inteligencia,
                    Sabedoria = save.Jogador.Sabedoria,
                    Carisma = save.Jogador.Carisma,
                    ClasseArmadura = save.Jogador.ClasseArmadura,
                    HpMaximo = save.Jogador.HpMaximo,
                    HpAtual = save.Jogador.HpAtual,
                    CuraPelasMaosRestante = save.Jogador.CuraPelasMaosRestante,
                    TemAtaqueExtra = save.Jogador.TemAtaqueExtra,
                    SlotsDeMagia = new Dictionary<int, int>(save.Jogador.SlotsDeMagia)
                };

                _jogador.Inventario.Clear();
                foreach (string nomeItem in save.Jogador.Inventario)
                {
                    var item = ScriptInicial.Consumiveis.FirstOrDefault(consumivel => consumivel.Nome == nomeItem)
                        ?? (Item?)ScriptInicial.Armas.FirstOrDefault(arma => arma.Nome == nomeItem)
                        ?? ScriptInicial.Armaduras.FirstOrDefault(armadura => armadura.Nome == nomeItem);

                    if (item != null)
                    {
                        _jogador.Inventario.Add(item);
                    }
                }

                if (!string.IsNullOrWhiteSpace(save.Jogador.ArmaEquipada))
                {
                    _jogador.ArmaEquipada = ScriptInicial.Armas.FirstOrDefault(item => item.Nome == save.Jogador.ArmaEquipada);
                }

                if (!string.IsNullOrWhiteSpace(save.Jogador.ArmaduraVestida))
                {
                    _jogador.ArmaduraVestida = ScriptInicial.Armaduras.FirstOrDefault(item => item.Nome == save.Jogador.ArmaduraVestida);
                }

                _inimigoAtual = save.InimigoAtual == null ? null : new Inimigo(
                    save.InimigoAtual.Nome,
                    save.InimigoAtual.ClasseArmadura,
                    save.InimigoAtual.HpMaximo,
                    save.InimigoAtual.BonusAtaque,
                    save.InimigoAtual.DadoDeDano,
                    save.InimigoAtual.BonusDano,
                    save.InimigoAtual.XPConcedido)
                {
                    HpAtual = save.InimigoAtual.HpAtual
                };

                _eventoAtual = _indiceEtapa < _etapas.Count ? _etapas[_indiceEtapa].Eventos[CaminhoJornada.Meio] : null;
                Registrar("Partida carregada de Dados/savegame.json.");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public sealed record RespostaJogo(string Fase, int EtapaAtual, int TotalEtapas, bool Vitoria, bool Derrota, JogadorResumo Jogador, EtapaResumo Etapa, List<CaminhoResumo> Caminhos, InimigoResumo? Inimigo, string Cenario, FeedbackCombate Feedback, ResumoFinal? ResumoFinal, TurnoResumo Turno, EventoResumo? Evento, List<LojaItemResumo> Loja, List<ClasseResumo> Classes, List<string> Log);
    public sealed record JogadorResumo(string Nome, string Classe, int Nivel, int XP, int HpAtual, int HpMaximo, int ClasseArmadura, int Ouro, int CuraPelasMaos, int SlotsNivel1, int SlotsNivel2, int SlotsNivel3, int Pocoes, string Arma, string Armadura, AtributosResumo Atributos, List<string> Inventario);
    public sealed record AtributosResumo(int Forca, int Destreza, int Constituicao, int Inteligencia, int Sabedoria, int Carisma);
    public sealed record EtapaResumo(int Numero, string Nome);
    public sealed record CaminhoResumo(string Id, string Nome, string Natureza, string Titulo, string Descricao, string Risco, string Recompensa, bool Boss);
    public sealed record InimigoResumo(string Nome, int HpAtual, int HpMaximo, int ClasseArmadura, bool Boss);
    public sealed record FeedbackCombate(string Tipo, string Texto);
    public sealed record ResumoFinal(string Resultado, int NivelFinal, int InimigosDerrotados, int OuroFinal, List<string> CaminhosEscolhidos);
    public sealed record TurnoResumo(bool AcaoPrincipalUsada, bool AcaoBonusUsada, bool SurtoDisponivel, bool VantagemJogador, bool DesvantagemInimigo, bool ParalisiaInimigo);
    public sealed record EventoResumo(string Tipo, string Titulo, string Descricao, string Cena);
    public sealed record LojaItemResumo(string Id, string Nome, string Tipo, int Preco, string Descricao);
    public sealed record ClasseResumo(string Id, string Nome, string Arquetipo, string Descricao, string Imagem);

    public sealed class SaveGameDto
    {
        public string Fase { get; set; } = "criacao";
        public int IndiceEtapa { get; set; }
        public int InimigosDerrotados { get; set; }
        public string Cenario { get; set; } = "guilda";
        public FeedbackCombate? Feedback { get; set; }
        public List<string> Log { get; set; } = new();
        public List<string> CaminhosEscolhidos { get; set; } = new();
        public bool AcaoPrincipalUsada { get; set; }
        public bool AcaoBonusUsada { get; set; }
        public bool SurtoDisponivel { get; set; }
        public int VantagemJogador { get; set; }
        public int DesvantagemInimigo { get; set; }
        public int ParalisiaInimigo { get; set; }
        public JogadorSaveDto? Jogador { get; set; }
        public InimigoSaveDto? InimigoAtual { get; set; }
    }

    public sealed class JogadorSaveDto
    {
        public string Nome { get; set; } = "Sir Alden";
        public string Classe { get; set; } = "Paladino";
        public int Nivel { get; set; } = 1;
        public int XP { get; set; }
        public int Ouro { get; set; }
        public string? Raca { get; set; }
        public string? Lore { get; set; }
        public int Forca { get; set; }
        public int Destreza { get; set; }
        public int Constituicao { get; set; }
        public int Inteligencia { get; set; }
        public int Sabedoria { get; set; }
        public int Carisma { get; set; }
        public int ClasseArmadura { get; set; }
        public int HpMaximo { get; set; }
        public int HpAtual { get; set; }
        public int CuraPelasMaosRestante { get; set; }
        public bool TemAtaqueExtra { get; set; }
        public string? ArmaEquipada { get; set; }
        public string? ArmaduraVestida { get; set; }
        public List<string> Inventario { get; set; } = new();
        public Dictionary<int, int> SlotsDeMagia { get; set; } = new();
    }

    public sealed class InimigoSaveDto
    {
        public string Nome { get; set; } = string.Empty;
        public int ClasseArmadura { get; set; }
        public int HpMaximo { get; set; }
        public int HpAtual { get; set; }
        public int BonusAtaque { get; set; }
        public string DadoDeDano { get; set; } = "1d4";
        public int BonusDano { get; set; }
        public int XPConcedido { get; set; }
    }
}

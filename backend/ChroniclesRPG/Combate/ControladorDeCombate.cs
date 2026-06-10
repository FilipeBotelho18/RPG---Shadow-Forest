using ChroniclesRPG.Entidades;
using ChroniclesRPG.Entidades.Habilidades;

namespace ChroniclesRPG.Combate{

    /// <summary>
    /// Gerencia o combate rodada a rodada:
    ///   - Ordem de iniciativa
    ///   - Economia de ações por turno (AcaoPrincipal / AcaoBonus / Reação)
    ///   - Efeitos por duração (buffs que expiram ao fim do turno)
    ///   - Eliminação de combatentes derrotados
    /// </summary>
    public class ControladorDeCombate{

        // ==========================================
        // ESTADO DO COMBATE
        // ==========================================
        public List<FichaPersonagem> Participantes { get; private set; } = new();
        public int RodadaAtual { get; private set; } = 0;
        public bool CombateEmAndamento { get; private set; } = false;

        // Mapa de estado de turno atual por personagem
        private Dictionary<FichaPersonagem, EstadoTurno> _estadosTurno = new();

        // ==========================================
        // INICIALIZAÇÃO
        // ==========================================

        public void IniciarCombate(params FichaPersonagem[] participantes){
            // Ordena por iniciativa (Mod DEX), desempate por nome
            Participantes = participantes
                .OrderByDescending(p => p.Iniciativa)
                .ThenBy(p => p.Nome)
                .ToList();

            CombateEmAndamento = true;
            RodadaAtual = 0;

            Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║               COMBATE INICIADO!                          ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Ordem de Iniciativa:                                    ║");
            for (int i = 0; i < Participantes.Count; i++){
                var p = Participantes[i];
                Console.WriteLine($"║  {i + 1}. {p.Nome,-20} (Iniciativa: +{p.Iniciativa})               ║");
            }
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");
        }

        // ==========================================
        // FLUXO DE RODADA
        // ==========================================

        /// <summary>
        /// Inicia uma nova rodada. Deve ser chamado antes de processar os turnos.
        /// </summary>
        public void IniciarRodada(){
            RodadaAtual++;
            Console.WriteLine($"\n{'─',60}");
            Console.WriteLine($"  RODADA {RodadaAtual}");
            Console.WriteLine($"{'─',60}");

            // Prepara o estado de turno para todos
            _estadosTurno.Clear();
            foreach (var p in Participantes){
                var estado = new EstadoTurno{
                    AcoesExtrasRestantes = p.AcoesExtras
                };
                _estadosTurno[p] = estado;
            }
        }

        /// <summary>
        /// Processa o início do turno de um personagem (efeitos contínuos).
        /// </summary>
        public void ProcessarInicioTurno(FichaPersonagem personagem){
            Console.WriteLine($"\n  ┌─ TURNO DE {personagem.Nome.ToUpper()} ─────────────");

            // Escudo Arcano: exibir status se ativo
            if (personagem.HpEscudoArcano > 0){
                Console.WriteLine($"  │ [Escudo Arcano] HP: {personagem.HpEscudoArcano}/{personagem.HpMaxEscudoArcano}");
            }

            // Onda de Vitalidade: cura progressiva do Paladino (ModCarisma por turno)
            if (personagem.OndaVitalidadeTurnosRestantes > 0){
                int cura = Math.Max(1, personagem.ModificadorCarisma);
                int hpAntes = personagem.HpAtual;
                personagem.HpAtual = Math.Min(personagem.HpAtual + cura, personagem.HpMaximo);
                int curado = personagem.HpAtual - hpAntes;
                personagem.OndaVitalidadeTurnosRestantes--;

                Console.WriteLine($"  │ [Onda de Vitalidade] Cura +{curado} HP! " +
                                  $"(HP: {personagem.HpAtual}/{personagem.HpMaximo} | " +
                                  $"{personagem.OndaVitalidadeTurnosRestantes} turnos restantes)");
            }
        }

        /// <summary>
        /// Executa uma habilidade do personagem, validando a economia de ações.
        /// Retorna true se a ação foi executada com sucesso.
        /// </summary>
        public bool Agir(FichaPersonagem usuario, Habilidade? habilidade, FichaPersonagem? alvo = null){
            if (habilidade == null){
                Console.WriteLine($"  [ERRO] Habilidade não encontrada para {usuario.Nome}!");
                return false;
            }
            if (!_estadosTurno.TryGetValue(usuario, out var estado)){
                Console.WriteLine($"  [ERRO] {usuario.Nome} não está participando do combate ou o turno não foi iniciado.");
                return false;
            }

            // Validação de economia de ações
            switch (habilidade.TempoDeConjuracao){
                case TipoAcao.AcaoPrincipal:
                    if (!estado.AcaoPrincipalDisponivel && estado.AcoesExtrasRestantes <= 0){
                        Console.WriteLine($"  ⚠ {usuario.Nome} já usou sua Ação Principal este turno!");
                        return false;
                    }
                    if (!estado.AcaoPrincipalDisponivel){
                        // Consome a ação extra (doada pelo Surto de Ação)
                        estado.AcoesExtrasRestantes--;
                        Console.WriteLine($"  [Ação Extra] {usuario.Nome} usa sua Ação Extra! ({estado.AcoesExtrasRestantes} restantes)");
                    } else {
                        estado.UsarAcaoPrincipal();
                    }
                    break;

                case TipoAcao.AcaoBonus:
                    if (!estado.AcaoBonusDisponivel){
                        Console.WriteLine($"  ⚠ {usuario.Nome} já usou sua Ação Bônus este turno!");
                        return false;
                    }
                    estado.UsarAcaoBonus();
                    // Se for o Surto de Ação: sincroniza as AcoesExtras com o EstadoTurno
                    if (habilidade.Nome == "Surto de Ação"){
                        habilidade.Executar(usuario, alvo);
                        estado.AcoesExtrasRestantes = usuario.AcoesExtras; // sincroniza
                        usuario.AcoesExtras = 0; // limpa para não acumular
                        return true;
                    }
                    break;

                case TipoAcao.Reacao:
                    if (!estado.ReacaoDisponivel){
                        Console.WriteLine($"  ⚠ {usuario.Nome} já usou sua Reação este turno!");
                        return false;
                    }
                    estado.UsarReacao();
                    break;
            }

            // Ativa vantagem se o Ataque Certeiro estiver ativo
            if (habilidade.Tipo == TipoHabilidade.AtaqueFisico && usuario.TemVantagemProximoAtaque){
                Console.WriteLine($"  [Ataque Certeiro] {usuario.Nome} rola com Vantagem!");
                usuario.TemVantagemProximoAtaque = false;
            }

            habilidade.Executar(usuario, alvo);
            return true;
        }

        /// <summary>
        /// Processa o fim do turno de um personagem (expira buffs de 1 turno).
        /// </summary>
        public void ProcessarFimTurno(FichaPersonagem personagem){
            // Expira ProtecaoContraLaminas (dura até o fim do PRÓXIMO turno)
            // Flag fica ativa pelo turno em que foi lançada + 1 turno do inimigo
            // Solução simples: expira no fim do turno do personagem que tem a resistência
            if (personagem.TemResistenciaFisica){
                personagem.TemResistenciaFisica = false;
                Console.WriteLine($"  │ [Proteção contra Lâminas] Resistência física expirou.");
            }

            // Expira vantagem de Ataque Certeiro se não foi usada
            if (personagem.TemVantagemProximoAtaque){
                personagem.TemVantagemProximoAtaque = false;
                Console.WriteLine($"  │ [Ataque Certeiro] Vantagem expirou sem ser usada.");
            }

            Console.WriteLine($"  └─────────────────────────────────────────────────────");
        }

        /// <summary>
        /// Verifica e remove combatentes derrotados da ordem de iniciativa.
        /// Retorna true se o combate ainda continua.
        /// </summary>
        public bool VerificarMortos(){
            var derrotados = Participantes.Where(p => p.HpAtual <= 0).ToList();
            foreach (var d in derrotados){
                Console.WriteLine($"\n  ✖ {d.Nome} foi derrotado e cai no chão!");
                Participantes.Remove(d);
            }

            if (Participantes.Count == 0){
                Console.WriteLine("\n  Todos os combatentes foram derrotados. Empate!");
                CombateEmAndamento = false;
            }

            return CombateEmAndamento;
        }

        /// <summary>
        /// Exibe um resumo rápido do HP de todos os participantes.
        /// </summary>
        public void ExibirResumo(){
            Console.WriteLine($"\n  {'─',55}");
            Console.WriteLine($"  Status ao final da Rodada {RodadaAtual}:");
            foreach (var p in Participantes){
                string escudo = p.HpEscudoArcano > 0 ? $" | Escudo: {p.HpEscudoArcano}" : "";
                Console.WriteLine($"    {p.Nome,-25} HP: {p.HpAtual,3}/{p.HpMaximo,-3}{escudo}");
            }
            Console.WriteLine($"  {'─',55}");
        }
    }
}

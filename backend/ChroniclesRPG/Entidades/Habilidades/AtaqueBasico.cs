using ChroniclesRPG.Funcoes;
using ChroniclesRPG.Entidades.Itens;

namespace ChroniclesRPG.Entidades.Habilidades{
    public class AtaqueBasico : Habilidade{
        public AtaqueBasico() 
            : base("Ataque Básico", "Realiza um ataque com a arma equipada.", TipoAcao.AcaoPrincipal, TipoHabilidade.AtaqueFisico) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){

            // --- Pré-condições ---
            if (usuario.ArmaEquipada == null){
                Console.WriteLine($"  {usuario.Nome} não tem uma arma equipada para atacar!");
                return 0;
            }
            if (alvo == null){
                Console.WriteLine($"  Ataque Básico requer um alvo!");
                return 0;
            }

            int danoTotalTurno = 0;

            for (int i = 1; i <= usuario.NumeroDeAtaques; i++){
                if (usuario.NumeroDeAtaques > 1){
                    Console.WriteLine($"\n  --- Ataque {i} de {usuario.NumeroDeAtaques} ---");
                }

                // ==========================================
                // PASSO 1: ESCOLHER O ATRIBUTO DE ATAQUE
                // ==========================================
                bool ehArmaDeAlcance = usuario.ArmaEquipada.Tipo == TipoArma.Arcos 
                                    || usuario.ArmaEquipada.Tipo == TipoArma.Bestas
                                    || usuario.ArmaEquipada.Tipo == TipoArma.Arremesso;

                int modificadorDeAtaque;

                if (ehArmaDeAlcance){
                    modificadorDeAtaque = usuario.ModificadorDestreza;
                } else if (usuario.ArmaEquipada.UsaDestreza){
                    modificadorDeAtaque = Math.Max(usuario.ModificadorForca, usuario.ModificadorDestreza);
                } else {
                    modificadorDeAtaque = usuario.ModificadorForca;
                }

                // ==========================================
                // PASSO 2: ROLAR O D20 DE ACERTO
                // ==========================================
                int rolagem = Dados.RolarD20();

                // Aplica bônus de buffs
                int bonusAtaqueMagico = usuario.TemArmaMagica ? 1 : 0;
                int bonusBencao = usuario.TemBencao ? Dados.Rolar("1d4") : 0;
                
                int totalDeAtaque = rolagem + modificadorDeAtaque + bonusAtaqueMagico + bonusBencao;

                string nomeAtributo = ehArmaDeAlcance || usuario.ArmaEquipada.UsaDestreza && usuario.ModificadorDestreza > usuario.ModificadorForca 
                    ? "DEX" : "FOR";

                Console.WriteLine($"  {usuario.Nome} ataca {alvo.Nome} com {usuario.ArmaEquipada.Nome}!");
                string msgAtaque = $"    Rolagem de acerto: 1d20({rolagem}) + {nomeAtributo}({modificadorDeAtaque:+#;-#;+0})";
                if (bonusAtaqueMagico > 0) msgAtaque += $" + Arma Mágica(+1)";
                if (bonusBencao > 0) msgAtaque += $" + Bênção(+{bonusBencao})";
                msgAtaque += $" = {totalDeAtaque} vs CA {alvo.ClasseArmadura}";
                Console.WriteLine(msgAtaque);

                // ==========================================
                // PASSO 3: COMPARAR COM A CA DO ALVO
                // ==========================================
                if (rolagem >= usuario.MargemCritico){
                    int dano1 = Dados.Rolar(usuario.ArmaEquipada.DadoDeDano);
                    int dano2 = Dados.Rolar(usuario.ArmaEquipada.DadoDeDano);
                    int danoBaseTotal = dano1 + dano2 + modificadorDeAtaque + bonusAtaqueMagico;
                    
                    int danoTrovejante = 0;
                    if (usuario.ProximoAtaqueTrovejante) {
                        danoTrovejante = Dados.Rolar("2d6") + Dados.Rolar("2d6"); // Crítico dobra também
                        usuario.ProximoAtaqueTrovejante = false;
                    }

                    int danoTotal = danoBaseTotal + danoTrovejante;
                    int danoEfetivo = alvo.ReceberDano(danoTotal);

                    string msgDano = $"    ACERTO CRÍTICO! Dano da Arma: {danoBaseTotal} ({usuario.ArmaEquipada.TipoDano})";
                    if (danoTrovejante > 0) msgDano += $" | Dano Trovejante: {danoTrovejante}";
                    
                    Console.WriteLine(msgDano);
                    Console.WriteLine($"    HP de {alvo.Nome}: {alvo.HpAtual}/{alvo.HpMaximo}");
                    danoTotalTurno += danoEfetivo;
                }
                else if (rolagem == 1){
                    Console.WriteLine($"    ERRO CRÍTICO! O ataque falha completamente.");
                    // O ataque trovejante não gasta no erro crítico, fica pro próximo ataque.
                }
                else if (totalDeAtaque >= alvo.ClasseArmadura){
                    int dano = Dados.Rolar(usuario.ArmaEquipada.DadoDeDano) + modificadorDeAtaque + bonusAtaqueMagico;
                    dano = Math.Max(1, dano);

                    int danoTrovejante = 0;
                    if (usuario.ProximoAtaqueTrovejante) {
                        danoTrovejante = Dados.Rolar("2d6");
                        usuario.ProximoAtaqueTrovejante = false;
                    }

                    int danoTotal = dano + danoTrovejante;
                    int danoEfetivo = alvo.ReceberDano(danoTotal);

                    string msgDano = $"    ACERTOU! Dano da Arma: {dano} ({usuario.ArmaEquipada.TipoDano})";
                    if (danoTrovejante > 0) msgDano += $" | Dano Trovejante: {danoTrovejante}";

                    Console.WriteLine(msgDano);
                    Console.WriteLine($"    HP de {alvo.Nome}: {alvo.HpAtual}/{alvo.HpMaximo}");
                    danoTotalTurno += danoEfetivo;
                } else {
                    Console.WriteLine($"    ERROU! O ataque não penetrou a defesa do alvo.");
                }

                // Interrompe os ataques se o alvo já foi derrotado
                if (alvo.HpAtual <= 0){
                    Console.WriteLine($"\n  O {alvo.Nome} foi derrotado!");
                    break;
                }
            }

            return danoTotalTurno;
        }
    }
}
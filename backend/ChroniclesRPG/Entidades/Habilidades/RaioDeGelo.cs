using ChroniclesRPG.Funcoes;

namespace ChroniclesRPG.Entidades.Habilidades{
    // Truque (custo 0) — Ataque mágico à distância
    // Dano: 1d8 de frio (escala para 2d8 no nível 5)
    public class RaioDeGelo : Habilidade{
        public RaioDeGelo() 
            : base("Raio de Gelo", "Truque: Um raio de luz glacial atinge o alvo. 1d8 de dano de frio (2d8 no nível 5).", TipoAcao.AcaoPrincipal, TipoHabilidade.TruqueMagico) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            if (alvo == null){
                Console.WriteLine($"  Raio de Gelo requer um alvo!");
                return 0;
            }

            // Rolagem de ataque com Inteligência (modificador do conjurador)
            int rolagem = Dados.RolarD20();
            int totalAtaque = rolagem + usuario.ModificadorInteligencia;

            Console.WriteLine($"  {usuario.Nome} dispara um Raio de Gelo em {alvo.Nome}!");
            Console.WriteLine($"    Rolagem de acerto: 1d20({rolagem}) + INT({usuario.ModificadorInteligencia:+#;-#;+0}) = {totalAtaque} vs CA {alvo.ClasseArmadura}");

            if (rolagem == 1){
                Console.WriteLine($"    ERRO CRÍTICO! O raio erra completamente.");
                return 0;
            }

            if (totalAtaque >= alvo.ClasseArmadura || rolagem == 20){
                // Escala de dano: 2d8 no nível 5+, 1d8 antes
                string dadoDeDano = usuario.Nivel >= 5 ? "2d8" : "1d8";
                int dano = Dados.Rolar(dadoDeDano);

                alvo.ReceberDano(dano);
                Console.WriteLine($"    ACERTOU! Raio de Gelo causa {dano} de dano de Frio! [dado: {dadoDeDano}]");
                Console.WriteLine($"    HP de {alvo.Nome}: {alvo.HpAtual}/{alvo.HpMaximo}");
                return dano;
            } else {
                Console.WriteLine($"    ERROU! O raio passou pelo alvo.");
                return 0;
            }
        }
    }
}

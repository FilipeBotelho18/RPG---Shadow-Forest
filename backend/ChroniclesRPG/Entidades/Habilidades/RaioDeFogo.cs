using ChroniclesRPG.Funcoes;

namespace ChroniclesRPG.Entidades.Habilidades{
    // Truque desbloqueado no nível 6
    // Dano: 1d10 de fogo (escala para 2d10 no nível 5)
    public class RaioDeFogo : Habilidade{
        public RaioDeFogo() 
            : base("Raio de Fogo", "Truque (Nível 6): Um cisco incandescente atinge o alvo. 1d10 de dano de fogo (2d10 no nível 5).", TipoAcao.AcaoPrincipal, TipoHabilidade.TruqueMagico) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            if (alvo == null){
                Console.WriteLine($"  Raio de Fogo requer um alvo!");
                return 0;
            }

            int rolagem = Dados.RolarD20();
            int totalAtaque = rolagem + usuario.ModificadorInteligencia;

            Console.WriteLine($"  {usuario.Nome} arremessa um Raio de Fogo em {alvo.Nome}!");
            Console.WriteLine($"    Rolagem de acerto: 1d20({rolagem}) + INT({usuario.ModificadorInteligencia:+#;-#;+0}) = {totalAtaque} vs CA {alvo.ClasseArmadura}");

            if (rolagem == 1){
                Console.WriteLine($"    ERRO CRÍTICO! As chamas se apagam antes de atingir.");
                return 0;
            }

            if (totalAtaque >= alvo.ClasseArmadura || rolagem == 20){
                string dadoDeDano = usuario.Nivel >= 5 ? "2d10" : "1d10";
                int dano = Dados.Rolar(dadoDeDano);

                alvo.ReceberDano(dano);
                Console.WriteLine($"    ACERTOU! Raio de Fogo causa {dano} de dano de Fogo! [dado: {dadoDeDano}]");
                Console.WriteLine($"    HP de {alvo.Nome}: {alvo.HpAtual}/{alvo.HpMaximo}");
                return dano;
            } else {
                Console.WriteLine($"    ERROU! O cisco de fogo se apagou.");
                return 0;
            }
        }
    }
}

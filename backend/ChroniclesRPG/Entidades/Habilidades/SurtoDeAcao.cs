using System;

namespace ChroniclesRPG.Entidades.Habilidades{
    public class SurtoDeAcao : Habilidade{
        public SurtoDeAcao() : base("Surto de Ação", "Permite realizar uma ação adicional no seu turno.", TipoAcao.AcaoBonus, TipoHabilidade.CaracteristicaClasse){
        }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            if (usuario.UsosSurtoDeAcao <= 0){
                Console.WriteLine($"{usuario.Nome} não tem mais usos de Surto de Ação disponíveis. É necessário um descanso.");
                return 0;
            }

            usuario.UsosSurtoDeAcao--;
            usuario.AcoesExtras++;

            Console.WriteLine($"{usuario.Nome} usou Surto de Ação! Ganhou uma ação adicional no turno atual.");
            return 1;
        }
    }
}

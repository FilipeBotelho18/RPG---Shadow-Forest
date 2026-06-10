using System;

namespace ChroniclesRPG.Entidades.Habilidades{
    public class AtaqueExtra : Habilidade{
        public AtaqueExtra() : base("Ataque Extra", "Passiva: Você pode atacar duas vezes, em vez de uma, ao usar a ação de Ataque no seu turno.", TipoAcao.AcaoPrincipal, TipoHabilidade.CaracteristicaClasse){
        }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            Console.WriteLine("Esta é uma habilidade passiva (Ataque Extra) e ativa automaticamente ao usar o Ataque Básico.");
            return 0;
        }
    }
}
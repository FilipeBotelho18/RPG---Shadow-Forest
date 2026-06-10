using System;

namespace ChroniclesRPG.Entidades.Habilidades{
    public class ArquetipoMarcial : Habilidade{
        public ArquetipoMarcial() : base("Arquétipo Marcial (Campeão)", "Passiva: Seus ataques com armas causam acerto crítico com resultados de 19 a 20.", TipoAcao.AcaoPrincipal, TipoHabilidade.CaracteristicaClasse){
        }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            usuario.MargemCritico = 19; // Crítico agora ocorre com 19 ou 20
            Console.WriteLine("Esta é uma habilidade passiva (Crítico Aprimorado) e está sempre ativa.");
            return 0;
        }
    }
}
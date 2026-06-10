using ChroniclesRPG.Funcoes;

namespace ChroniclesRPG.Entidades.Habilidades
{
    public class RetomarFolego : Habilidade
    {
        public RetomarFolego() 
            : base("Retomar o Fôlego", "Recupera energia vital durante o combate.", TipoAcao.AcaoBonus, TipoHabilidade.CaracteristicaClasse) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null)
        {
            // A regra do D&D: Cura 1d10 + Nível do Guerreiro
            int cura = Dados.Rolar("1d10") + usuario.Nivel;
            
            usuario.HpAtual += cura;
            if (usuario.HpAtual > usuario.HpMaximo) 
                usuario.HpAtual = usuario.HpMaximo;

            Console.WriteLine($"{usuario.Nome} usou Retomar o Fôlego e recuperou {cura} de HP!");
            return cura;
        }
    }
}
using ChroniclesRPG.Entidades.Habilidades;
using ChroniclesRPG.Entidades.Itens;
using ChroniclesRPG.Funcoes;

namespace ChroniclesRPG.Entidades.Classes
{
    public class Guerreiro : IClasseRPG
    {
        public string NomeDaClasse => "Guerreiro";
        public int VidaInicial => 10;
        public string DadoDeVida => "1d10";

        public List<TipoArmadura> ProficienciasArmadura => new()
        {
            TipoArmadura.Leve,
            TipoArmadura.Media,
            TipoArmadura.Pesada
        };

        public List<TipoArma> ProficienciasArmas => new()
        {
            TipoArma.LaminasCurtas,
            TipoArma.LaminasLongas,
            TipoArma.LaminasPesadas,
            TipoArma.Machados,
            TipoArma.Impacto,
            TipoArma.Hastes,
            TipoArma.Arcos,
            TipoArma.Bestas,
            TipoArma.Arremesso
        };

        public int CalcularVida()
        {
            return Dados.Rolar(DadoDeVida);
        }

        public void AplicarBonusIniciais(FichaPersonagem ficha)
        {
            ficha.Forca = 16;
            ficha.Constituicao = 15;
            ficha.Destreza = 13;
            ficha.Sabedoria = 12;
            ficha.Inteligencia = 10;
            ficha.Carisma = 8;
        }

        public void AplicarHabilidadesDeNivel(FichaPersonagem ficha, int nivel)
        {
            switch (nivel)
            {
                case 1:
                    ficha.HabilidadesConhecidas.Add(new AtaqueBasico());
                    ficha.HabilidadesConhecidas.Add(new RetomarFolego());
                    break;
                case 2:
                    ficha.HabilidadesConhecidas.Add(new SurtoDeAcao());
                    ficha.UsosSurtoDeAcao = 1;
                    break;
                case 3:
                    ficha.HabilidadesConhecidas.Add(new ArquetipoMarcial());
                    ficha.MargemCritico = 19;
                    break;
                case 4:
                    ficha.Forca += 2;
                    break;
                case 5:
                    ficha.HabilidadesConhecidas.Add(new AtaqueExtra());
                    ficha.TemAtaqueExtra = true;
                    ficha.NumeroDeAtaques = 2;
                    break;
                case 6:
                    ficha.Forca += 2;
                    break;
            }
        }
    }
}

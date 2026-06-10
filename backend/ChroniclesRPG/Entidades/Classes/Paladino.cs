using ChroniclesRPG.Entidades.Habilidades;
using ChroniclesRPG.Entidades.Itens;
using ChroniclesRPG.Funcoes;

namespace ChroniclesRPG.Entidades.Classes
{
    public class Paladino : IClasseRPG
    {
        public string NomeDaClasse => "Paladino";
        public string DadoDeVida => "1d10";
        public int VidaInicial => 10;

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
            TipoArma.Arremesso
        };

        public void AplicarBonusIniciais(FichaPersonagem ficha)
        {
            ficha.Forca = 16;
            ficha.Constituicao = 15;
            ficha.Carisma = 14;
            ficha.Sabedoria = 12;
            ficha.Destreza = 10;
            ficha.Inteligencia = 8;
            ficha.CuraPelasMaosRestante = ficha.Nivel * 5;
        }

        public int CalcularVida()
        {
            return Dados.Rolar(DadoDeVida);
        }

        public void AplicarHabilidadesDeNivel(FichaPersonagem ficha, int nivel)
        {
            ficha.CuraPelasMaosRestante = ficha.Nivel * 5;
            ficha.ReservaCuraPelasMaos = ficha.Nivel * 5;

            switch (nivel)
            {
                case 1:
                    ficha.HabilidadesConhecidas.Add(new AtaqueBasico());
                    ficha.HabilidadesConhecidas.Add(new CuraPelasMaos());
                    break;
                case 2:
                    ficha.HabilidadesConhecidas.Add(new DestruicaoDivina());
                    ficha.ReceberSlotsDeMagia(1, 2);
                    break;
                case 3:
                    ficha.UsosCanalizarDivindade = 1;
                    ficha.HabilidadesConhecidas.Add(new OndaDeVitalidade());
                    ficha.HabilidadesConhecidas.Add(new MagiaBencao());
                    ficha.HabilidadesConhecidas.Add(new MagiaEscudoDaFe());
                    ficha.HabilidadesConhecidas.Add(new MagiaDestruicaoTrovejante());
                    ficha.ReceberSlotsDeMagia(1, 1);
                    break;
                case 4:
                    ficha.Forca += 2;
                    break;
                case 5:
                    ficha.HabilidadesConhecidas.Add(new AtaqueExtra());
                    ficha.TemAtaqueExtra = true;
                    ficha.NumeroDeAtaques = 2;
                    ficha.HabilidadesConhecidas.Add(new MagiaAjuda());
                    ficha.HabilidadesConhecidas.Add(new MagiaRestauracaoMenor());
                    ficha.ReceberSlotsDeMagia(1, 1);
                    ficha.ReceberSlotsDeMagia(2, 2);
                    break;
                case 6:
                    ficha.HabilidadesConhecidas.Add(new AuraDeProtecao());
                    ficha.HabilidadesConhecidas.Add(new MagiaArmaMagica());
                    break;
            }
        }
    }
}

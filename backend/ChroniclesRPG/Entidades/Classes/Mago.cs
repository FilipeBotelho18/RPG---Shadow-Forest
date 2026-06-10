using ChroniclesRPG.Entidades.Habilidades;
using ChroniclesRPG.Entidades.Itens;
using ChroniclesRPG.Funcoes;

namespace ChroniclesRPG.Entidades.Classes
{
    public class Mago : IClasseRPG
    {
        public string NomeDaClasse => "Mago";
        public string DadoDeVida => "1d6";
        public int VidaInicial => 8;

        public List<TipoArmadura> ProficienciasArmadura => new()
        {
            TipoArmadura.Leve
        };

        public List<TipoArma> ProficienciasArmas => new()
        {
            TipoArma.Cajado,
            TipoArma.LaminasCurtas,
            TipoArma.Arremesso,
            TipoArma.Bestas,
            TipoArma.Hastes
        };

        public void AplicarBonusIniciais(FichaPersonagem ficha)
        {
            ficha.Inteligencia = 16;
            ficha.Destreza = 14;
            ficha.Constituicao = 14;
            ficha.Sabedoria = 12;
            ficha.Carisma = 10;
            ficha.Forca = 8;
        }

        public int CalcularVida()
        {
            return Dados.Rolar(DadoDeVida);
        }

        public void AplicarHabilidadesDeNivel(FichaPersonagem ficha, int nivel)
        {
            switch (nivel)
            {
                case 1:
                    ficha.HabilidadesConhecidas.Add(new AtaqueBasico());
                    ficha.HabilidadesConhecidas.Add(new AtaqueCerteiro());
                    ficha.HabilidadesConhecidas.Add(new ProtecaoContraLaminas());
                    ficha.HabilidadesConhecidas.Add(new RaioDeGelo());
                    ficha.ReceberSlotsDeMagia(1, 2);
                    break;
                case 2:
                    ficha.HabilidadesConhecidas.Add(new ProtecaoArcana());
                    ficha.ReceberSlotsDeMagia(1, 1);
                    break;
                case 3:
                    ficha.ReceberSlotsDeMagia(1, 1);
                    ficha.ReceberSlotsDeMagia(2, 2);
                    break;
                case 4:
                    ficha.Inteligencia += 2;
                    ficha.ReceberSlotsDeMagia(2, 1);
                    break;
                case 5:
                    ficha.ReceberSlotsDeMagia(3, 2);
                    break;
                case 6:
                    ficha.HabilidadesConhecidas.Add(new ProtecaoProjetada());
                    ficha.HabilidadesConhecidas.Add(new RaioDeFogo());
                    ficha.ReceberSlotsDeMagia(3, 1);
                    break;
            }
        }
    }
}

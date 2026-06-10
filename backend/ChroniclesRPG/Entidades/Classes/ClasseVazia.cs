using ChroniclesRPG.Entidades;
using ChroniclesRPG.Entidades.Itens;

namespace ChroniclesRPG.Entidades.Classes
{
    /// <summary>
    /// Implementação nula de IClasseRPG usada pelo ControladorDeEstagio para criar
    /// FichaPersonagem proxies que representam inimigos no sistema de habilidades.
    ///
    /// O sistema de Habilidades (AtaqueBasico, DestruicaoDivina, etc.) espera uma
    /// FichaPersonagem como alvo — esta classe permite criar uma ficha "vazia" a
    /// partir dos atributos brutos do FichaInimigo sem aplicar nenhum bônus de classe.
    /// </summary>
    internal class ClasseVazia : IClasseRPG
    {
        public string NomeDaClasse              => "—";
        public int    VidaInicial               => 0;
        public string DadoDeVida                => "1d4";
        public List<TipoArmadura> ProficienciasArmadura => new();
        public List<TipoArma>    ProficienciasArmas     => new();

        public int CalcularVida() => 0;

        /// <summary>Intencional: nenhum bônus aplicado — os atributos são definidos manualmente pelo proxy.</summary>
        public void AplicarBonusIniciais(FichaPersonagem ficha) { }

        /// <summary>Intencional: nenhuma habilidade concedida — o proxy não usa o sistema de habilidades do jogador.</summary>
        public void AplicarHabilidadesDeNivel(FichaPersonagem ficha, int nivel) { }
    }
}

namespace ChroniclesRPG.Combate{

    /// <summary>
    /// Representa a economia de ações disponíveis para um personagem durante UM turno.
    /// Criado no início de cada turno e descartado no final — ações não se acumulam.
    /// </summary>
    public class EstadoTurno{
        public bool AcaoPrincipalDisponivel { get; set; } = true;
        public bool AcaoBonusDisponivel     { get; set; } = true;
        public bool ReacaoDisponivel        { get; set; } = true;
        public int  AcoesExtrasRestantes    { get; set; } = 0; // Surto de Ação do Guerreiro

        public void UsarAcaoPrincipal(){
            AcaoPrincipalDisponivel = false;
        }
        public void UsarAcaoBonus(){
            AcaoBonusDisponivel = false;
        }
        public void UsarReacao(){
            ReacaoDisponivel = false;
        }
    }
}

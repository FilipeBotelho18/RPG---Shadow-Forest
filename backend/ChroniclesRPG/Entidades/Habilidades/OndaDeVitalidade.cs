namespace ChroniclesRPG.Entidades.Habilidades{
    public class OndaDeVitalidade : Habilidade{
        public OndaDeVitalidade() 
            : base("Onda de Vitalidade", "Canaliza a divindade para curar aliados ao longo do tempo (3 turnos).", TipoAcao.AcaoBonus, TipoHabilidade.CaracteristicaClasse) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            if (usuario.UsosCanalizarDivindade <= 0){
                Console.WriteLine($"  {usuario.Nome} não possui mais usos de Canalizar Divindade!");
                return 0;
            }

            usuario.UsosCanalizarDivindade--;
            
            // Ativa o buff de cura progressiva (a engine/loop de combate deverá descontar isso a cada turno e curar)
            usuario.OndaVitalidadeTurnosRestantes = 3;

            Console.WriteLine($"  {usuario.Nome} levanta seu símbolo sagrado! Uma Onda de Vitalidade é conjurada e irá curar nos próximos 3 turnos.");
            return 1;
        }
    }
}

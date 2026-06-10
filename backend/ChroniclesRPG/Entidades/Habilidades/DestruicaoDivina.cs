using ChroniclesRPG.Funcoes;
using ChroniclesRPG.Entidades.Itens;

namespace ChroniclesRPG.Entidades.Habilidades{
    public class DestruicaoDivina : Habilidade{
        public DestruicaoDivina() 
            : base("Destruição Divina", "Realiza um ataque e consome um espaço de magia para adicionar dano radiante extra.", TipoAcao.AcaoPrincipal, TipoHabilidade.Magia, 1) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            if (usuario.ArmaEquipada == null){
                Console.WriteLine($"  {usuario.Nome} não tem uma arma equipada para atacar!");
                return 0;
            }
            if (alvo == null){
                Console.WriteLine($"  Destruição Divina requer um alvo!");
                return 0;
            }

            // Tenta consumir o slot
            if (!usuario.GastarSlotDeMagia(1)){
                Console.WriteLine($"  {usuario.Nome} não possui mais espaços de magia disponíveis para usar Destruição Divina!");
                return 0;
            }

            Console.WriteLine($"  {usuario.Nome} canaliza poder sagrado em sua arma!");

            // Executa a lógica de ataque (Simplificada aqui para a habilidade)
            bool ehArmaDeAlcance = usuario.ArmaEquipada.Tipo == TipoArma.Arcos 
                                || usuario.ArmaEquipada.Tipo == TipoArma.Bestas
                                || usuario.ArmaEquipada.Tipo == TipoArma.Arremesso;

            if (ehArmaDeAlcance){
                Console.WriteLine("  Destruição Divina requer um ataque corpo a corpo!");
                return 0;
            }

            int modificadorDeAtaque = usuario.ArmaEquipada.UsaDestreza 
                ? Math.Max(usuario.ModificadorForca, usuario.ModificadorDestreza) 
                : usuario.ModificadorForca;

            int rolagem = Dados.RolarD20();
            int totalDeAtaque = rolagem + modificadorDeAtaque;

            if (rolagem >= usuario.MargemCritico){
                int danoBase = Dados.Rolar(usuario.ArmaEquipada.DadoDeDano) + Dados.Rolar(usuario.ArmaEquipada.DadoDeDano);
                int danoRadiante = Dados.Rolar("2d8") + Dados.Rolar("2d8"); // Crítico dobra os dados do Smite também
                int danoTotal = danoBase + danoRadiante + modificadorDeAtaque;

                alvo.ReceberDano(danoTotal);
                Console.WriteLine($"    ACERTO CRÍTICO SAGRADO! Dano da Arma: {danoBase}+{modificadorDeAtaque} | Dano Radiante: {danoRadiante} | Total: {danoTotal}");
                return danoTotal;
            }
            else if (rolagem == 1){
                Console.WriteLine($"    ERRO CRÍTICO! O ataque falha completamente e o poder divino se dissipa.");
                return 0;
            }
            else if (totalDeAtaque >= alvo.ClasseArmadura){
                int danoBase = Dados.Rolar(usuario.ArmaEquipada.DadoDeDano);
                int danoRadiante = Dados.Rolar("2d8"); 
                int danoTotal = danoBase + danoRadiante + modificadorDeAtaque;

                alvo.ReceberDano(danoTotal);
                Console.WriteLine($"    ACERTOU! Dano da Arma: {danoBase}+{modificadorDeAtaque} | Dano Radiante: {danoRadiante} | Total: {danoTotal}");
                return danoTotal;
            } else {
                Console.WriteLine($"    ERROU! O ataque não penetrou a defesa do alvo.");
                return 0;
            }
        }
    }
}

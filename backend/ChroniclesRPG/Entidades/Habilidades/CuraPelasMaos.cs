using System;

namespace ChroniclesRPG.Entidades.Habilidades{
    public class CuraPelasMaos: Habilidade{
        
        public CuraPelasMaos(): base("Cura pelas Mãos", "Cura o alvo usando uma reserva de pontos (5 HP por nível do conjurador).", TipoAcao.AcaoPrincipal, TipoHabilidade.CaracteristicaClasse){
            
        }
        
        // Método padrão da interface (vai curar tudo o que for possível se chamado sem parâmetros extras)
        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            if (alvo == null) return 0;
            
            // Se invocado genericamente, tenta curar toda a vida faltante do alvo ou gasta o resto da reserva.
            int vidaFaltante = alvo.HpMaximo - alvo.HpAtual;
            int cura = Math.Min(vidaFaltante, usuario.ReservaCuraPelasMaos);
            
            return ExecutarComCura(usuario, alvo, cura);
        }

        // Método específico dessa habilidade onde o sistema pode informar a quantidade de cura escolhida
        public int ExecutarComCura(FichaPersonagem usuario, FichaPersonagem alvo, int cura)
        {
            if (usuario.ReservaCuraPelasMaos <= 0)
            {
                Console.WriteLine("Você não tem mais pontos na sua reserva de Cura pelas Mãos!");
                return 0;
            }
            
            if (alvo.HpAtual == alvo.HpMaximo)
            {
                Console.WriteLine("O alvo já está com a vida cheia.");
                return 0;
            }

            if (cura > usuario.ReservaCuraPelasMaos)
            {
                Console.WriteLine($"Quantidade insuficiente na reserva. Você tem apenas {usuario.ReservaCuraPelasMaos} pontos.");
                return 0;
            }
            
            // Subtrai da reserva do Paladino
            usuario.ReservaCuraPelasMaos -= cura;
            
            // Aplica a cura no alvo e garante que não passe da vida máxima
            alvo.HpAtual += cura;
            if (alvo.HpAtual > alvo.HpMaximo)
                alvo.HpAtual = alvo.HpMaximo;

            Console.WriteLine($"{usuario.Nome} curou {cura} HP de {alvo.Nome}. Reserva restante: {usuario.ReservaCuraPelasMaos}");
            return cura;   
        }
    }
}

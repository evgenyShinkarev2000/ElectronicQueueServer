using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ElectronicQueueServer.SocketsManager
{
    public class TicketMenager
    { 
        // TODO добавить очищение _validTickets от устаревших билетов
        private readonly HashSet<string> _validTickets = new HashSet<string>(4);
        private readonly RandomNumberGenerator _generator = RandomNumberGenerator.Create();

        /// <summary>
        /// generate and save ticket to set
        /// </summary>
        public string GenerateTicket()
        {
            var bytes = new byte[16];
            _generator.GetNonZeroBytes(bytes);
            var ticket = Convert.ToBase64String(bytes);
            _validTickets.Add(ticket);

            return ticket;
        }

        /// <summary>
        /// also remove valid ticket from set
        /// </summary>
        public bool isTicketValid(string ticket)
        {
            if (_validTickets.Contains(ticket))
            {
                _validTickets.Remove(ticket);
                return true;
            }

            return false;
        }
    }
}

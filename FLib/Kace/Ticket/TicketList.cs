using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;

namespace FLib.Kace.Ticket
{
    public class TicketList
    {
        public int Count { get; set; }
        public string Warning { get; set; }

        public List<Ticket> Tickets { get; set; }
        private ObservableCollection<Ticket> obsTickets = null;

        public DataTable GetTable()
        {

            DataTable table = new DataTable("Tickets");
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("title", typeof(string));
            table.Columns.Add("time", typeof(int));
            table.Columns.Add("resolution", typeof(string));
            table.Columns.Add("owner", typeof(string));
            table.Columns.Add("submitter", typeof(string));

            if ((Tickets == null) || (Tickets.Count == 0)) return table;

            foreach (Ticket ticket in Tickets)
            {
                table.Rows.Add(ticket.id, ticket.title, ticket.custom_1, ticket.resolution
                    , ticket.owner.full_name
                    , ticket.submitter.full_name
                    );
            }
            return table;
        }


        /// <summary>
        /// return the ticket list as observable collections
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Ticket> GetObsCollection()
        {
            obsTickets = new ObservableCollection<Ticket>();
            if ((Tickets == null) || (Tickets.Count == 0)) return obsTickets;

            foreach (Ticket ticket in Tickets)
            {
                obsTickets.CollectionChanged += ObsTickets_CollectionChanged;
                obsTickets.Add(ticket);
            }
            return obsTickets;
        }

        private void ObsTickets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        public int getTicketIdByIndex(int index)
        {
            try
            {
                return Tickets[index].id;
            } catch (Exception ex) { return -1; }
            
        }

        public void updateTicket(Ticket ticket)
        {
            if ((ticket == null) || (ticket.id == 0)) return;
            int id = ticket.id;
            int index = Tickets.FindIndex(x => x.id == id);
            Tickets[index] = ticket;
            obsTickets[index] = ticket;

        }
    }
}

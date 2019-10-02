using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom
{
    class Contacts
    {
        public int id { get; set; }
        public string fio { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
         
        public Contacts(int id, string fio, string phone, string email)
        {
            this.id = id;
            this.fio = fio;
            this.phone = phone;
            this.email = email;
        }
    }
}



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
	
    class StaffList
    {
        public int id { get; set; }
        public string dep{ get; set; }
        public string des_dep{ get; set; }
        public string post { get; set; }
        public int count { get; set; }
        public double vacant { get; set; }
        public string all_salary { get; set; }//зарплата
        public int access { get; set; }//форма допуска

        public StaffList( int id, string des_dep, string dep, string post, int count, double vacant, string all_salary, int access)
        {
            this.id = id;
            this.dep= dep;
            this.des_dep = des_dep;
            this.post = post;
            this.count = count;
            this.vacant = vacant;
            this.all_salary = all_salary;
            this.access = access;
        }
    }
}

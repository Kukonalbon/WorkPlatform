using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.BusinessFacade;
using Project.Entities;

namespace ServerManage
{
    public class PersonManager
    {
        private static PersonManager instance;
        public static PersonManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PersonManager();
                }
                return instance;
            }
        }

        public Sys_PersonFacade PersonFacade;

        public List<Sys_Person> Persons
        {
            get
            {
                return PersonFacade.GetSys_Persons().ToList();
            }
        }

        protected PersonManager()
        {
            PersonFacade = new Sys_PersonFacade();
            InitManger();
        }

        public void InitManger()
        {

        }

        public Sys_Person Login(string dacname, string password)
        {
            return PersonFacade.FindSys_Person(dacname, password);
        }

        public void GetUserModules(int keyid)
        {
            var table = PersonFacade.GetUserModules(1, 0);
        }

        public Sys_Person GetPerson(int keyid)
        {
            return PersonFacade.FindSys_Person(keyid);
        }

        public List<Sys_Person> GetAllPersons()
        {
            return Persons;
        }
    }
}

using LemonCat.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LemonCat.Models.DAO
{
    public class SuggestDAO
    {
        private static SuggestDAO instance;
        LemonCatEntities db = null;
        public SuggestDAO()
        {
            db = new LemonCatEntities();
        }

        public static SuggestDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new SuggestDAO();
                return SuggestDAO.instance;
            }
            private set
            {
                SuggestDAO.instance = value;
            }
        }
        public int CountDBSetByUserID(int id)
        {
            return db.DATASETs.Where(n => n.MaTK == id).Count();
        }
        public List<DATASET> GetModelByUserID(int id)
        {
            return db.DATASETs.Where(n => n.MaTK == id).ToList();
        }

    }
}
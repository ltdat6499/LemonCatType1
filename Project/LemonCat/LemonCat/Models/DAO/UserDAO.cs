using LemonCat.Common;
using LemonCat.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using System.Data;

namespace Model.DAO
{
    public class UserDAO
    {
        private static UserDAO instance;
        LemonCatEntities db = null;
        
        public UserDAO()
        {
            db = new LemonCatEntities();
        }

        public static UserDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new UserDAO();
                return UserDAO.instance;
            }
            private set
            {
                UserDAO.instance = value;
            }
        }
        public bool StaffLogin(string username, string password)
        {
            int count = db.TAIKHOANs.Where(n => n.TenTK == username && n.MatKhau == password && n.ChucVu == 6).Count();
            if (count > 0)
            {
                return true;
            }
            return false;
        }
        public string GetPositionNameByID(int id)
        {
            return db.CHUCVUs.Find(id).TenChucVu;
        }
        public bool SQLIsNotValidAccount(string username, string email, string password)
        {
            var res = db.Database.SqlQuery<Boolean>("SELECT dbo.IsExistUser(@username, @email, @password)", 
                new SqlParameter("@username", username), new SqlParameter("@email", email), 
                new SqlParameter("@password", password)).FirstOrDefault();
            return res;
        }
        public int SQLUpdateInfoAccount(int id, string email, string phone, string cmnd, string fname, string lname, string diachi)
        {
            int result = db.UpdateUser(id, fname, lname, email, phone, cmnd, diachi);
            db = new LemonCatEntities();
            return result;
        }
        public bool IsExistEmail(string email)
        {
            var user = db.TAIKHOANs.SingleOrDefault(n=>n.Email == email);
            if (user != null)
                return true;
            return false;
        }
        public bool IsExistUserName(string username)
        {
            var user = db.TAIKHOANs.SingleOrDefault(n => n.TenTK == username);
            if (user != null)
                return true;
            return false;
        }
        public int CreateFirstAdmin(string username, string password, string email)
        {
            //TAIKHOAN newuser = new TAIKHOAN();
            //newuser.TenTK = username;
            //newuser.RealPass = password;
            //newuser.MatKhau = Encryptor.MD5Hash(password);
            //newuser.Email = email;
            //newuser.ChucVu = 4;
            //newuser.KichHoat = true;
            //newuser.AnhDaiDien = "/Avata/profile_av.jpg";
            //newuser.Money = 0;
            //db.TAIKHOANs.Add(newuser);
            //db.SaveChanges();
            //return newuser.MaTK;
            return -1;
        }
        public int CreateFirstStaff(string username, string password, string email)
        {
            TAIKHOAN newuser = new TAIKHOAN();
            newuser.TenTK = username;
            newuser.RealPass = password;
            newuser.MatKhau = Encryptor.MD5Hash(password);
            newuser.Email = email;
            newuser.ChucVu = 6;
            newuser.KichHoat = true;
            newuser.AnhDaiDien = "/Avata/profile_av.jpg";
            newuser.Money = 0;
            db.TAIKHOANs.Add(newuser);
            db.SaveChanges();
            return newuser.MaTK;
        }
        public int CreateFirstManager(string username, string password, string email)
        {
            TAIKHOAN newuser = new TAIKHOAN();
            newuser.TenTK = username;
            newuser.RealPass = password;
            newuser.MatKhau = Encryptor.MD5Hash(password);
            newuser.Email = email;
            newuser.ChucVu = 5;
            newuser.KichHoat = true;
            newuser.AnhDaiDien = "/Avata/profile_av.jpg";
            newuser.Money = 0;
            db.TAIKHOANs.Add(newuser);
            db.SaveChanges();
            return newuser.MaTK;
        }
        public int CreateFirstUser(string username, string password, string email)
        {
            TAIKHOAN newuser = new TAIKHOAN();
            newuser.LastName = username;
            newuser.TenTK = username;
            newuser.RealPass = password;
            newuser.MatKhau = Encryptor.MD5Hash(password);
            newuser.Email = email;
            newuser.ChucVu = 1;
            newuser.KichHoat = true;
            newuser.AnhDaiDien = "/Avata/profile_av.jpg";
            newuser.Money = 0;
            newuser.GioiTinh = true;
            db.TAIKHOANs.Add(newuser);
            db.SaveChanges();
            return newuser.MaTK;
        }
        public int getIDbyUserName(string userName)
        {
            return db.TAIKHOANs.FirstOrDefault(n => n.TenTK == userName).MaTK;
        }

        public List<CHUCVU> GetAllPosition()
        {
            return db.CHUCVUs.ToList();
        }

        public int GetPositionByID(int maTK)
        {
            return (int)db.TAIKHOANs.Find(maTK).ChucVu;
        }

        public string[] GetAllNameOfPositionByIndex()
        {
            var list = db.CHUCVUs.ToList();
            string[] result = new string[200];
            // This null at index = 0 and != null at index > 0
            result[0] = "";
            for (int i = 1; i <= list.Count; i++)
            {
                result[i] = list[i - 1].TenChucVu;
            }
            return result;
        }
        public string GetRealPasswordByEmail(string email)
        {
            return db.TAIKHOANs.SingleOrDefault(n => n.Email == email).RealPass;
        }
        public TAIKHOAN GetByUserName(string username)
        {
            return db.TAIKHOANs.SingleOrDefault(n => n.TenTK == username);
        }
        private bool IsNotExistUser(TAIKHOAN entity)
        {
            int emailExist = 0, userNameExist = 0, cmndExist = 0, phoneExist = 0;
            emailExist = db.TAIKHOANs.Count(n => n.Email == entity.Email);
            userNameExist = db.TAIKHOANs.Count(n => n.TenTK == entity.TenTK);
            cmndExist = db.TAIKHOANs.Count(n => n.CMND == entity.CMND);
            phoneExist = db.TAIKHOANs.Count(n => n.Phone == entity.Phone);
            if (emailExist == 0 && userNameExist == 0 && cmndExist == 0 && phoneExist == 0)
                return true;
            return false;
        }
        public int Insert(TAIKHOAN entity)
        {
            entity.RealPass = entity.MatKhau;
            entity.Money = 0;
            entity.MatKhau = Encryptor.MD5Hash(entity.MatKhau);
            if (IsNotExistUser(entity))
            {
                db.TAIKHOANs.Add(entity);
                db.SaveChanges();
                return entity.MaTK;
            }
            else
            {
                return -1;
            }
            
        }

        public string GetEmailByID(int account)
        {
            return db.TAIKHOANs.Find(account).Email;
        }

        public string GetPhoneByID(int account)
        {
            return db.TAIKHOANs.Find(account).Phone;
        }

        public string IsLocked(string username)
        {
            int result = db.TAIKHOANs.Count(n => n.TenTK == username && n.KichHoat == false && n.ChucVu == 4);
            if (result == 1)
                return db.TAIKHOANs.FirstOrDefault(n => n.TenTK == username).AnhDaiDien;
            return "";
        }
        public bool Login(string username, string password)
        {
            int result = db.TAIKHOANs.Count(n => n.TenTK == username && n.MatKhau == password && n.KichHoat == true && n.ChucVu == 4);
            if (result > 0)
            {
                return true;
            }
            else
                return false;
        }
        public bool ManagerLogin(string username, string password)
        {
            int result = db.TAIKHOANs.Count(n => n.TenTK == username && n.MatKhau == password && n.KichHoat == true && n.ChucVu == 5);
            if (result > 0)
            {
                return true;
            }
            else
                return false;
        }
        public List<TAIKHOAN> GetAllUser()
        {
            return db.TAIKHOANs.ToList();
        }
        public int Update(TAIKHOAN entity)
        {
            if (IsNotExistUser(entity.TenTK, entity.Email, entity.Phone, entity.CMND))
            {
                TAIKHOAN tk = GetByUserName(entity.TenTK);
                tk.Email = entity.Email;
                tk.Phone = entity.Phone;
                tk.CMND = entity.CMND;
                tk.FirstName = entity.FirstName;
                tk.LastName = entity.LastName;
                tk.RealPass = entity.MatKhau;
                tk.MatKhau = Encryptor.MD5Hash(entity.MatKhau);
                tk.ChucVu = entity.ChucVu;
                tk.DiaChi = entity.DiaChi;
                if (entity.AnhDaiDien != null)
                    tk.AnhDaiDien = entity.AnhDaiDien;
                tk.KichHoat = entity.KichHoat;
                tk.GioiTinh = entity.GioiTinh;
                db.SaveChanges();
                return tk.MaTK;
            }
            else
            {
                return -1;
            }
        }
        public bool Delete(int id)
        {
            var user = db.TAIKHOANs.Find(id);
            //var filePath = Server.MapPath("~/Images/" + filename);
            //if (File.Exists(filePath))
            //{
            //    File.Delete(filePath);
            //}

            //Delete all Comment Score


            //Delete all Review Score


            //Delete all Child Comment

            //Delete all Comment

            //Delete all Review


            db.TAIKHOANs.Remove(user);
            db.SaveChanges();
            return true;      
        }

        public bool ChangeStatus(long id)
        {
            var user = db.TAIKHOANs.SingleOrDefault(n => n.MaTK == id);
            user.KichHoat = !user.KichHoat;
            db.SaveChanges();
            return (bool)user.KichHoat;
        }

        private bool IsNotExistUser(string tentk, string email, string phone, string cmnd)
        {
            int emailExist = 0, cmndExist = 0, phoneExist = 0;
            emailExist = db.TAIKHOANs.Count(n => n.Email == email && n.TenTK != tentk);
            cmndExist = db.TAIKHOANs.Count(n => n.CMND == cmnd && n.TenTK != tentk);
            phoneExist = db.TAIKHOANs.Count(n => n.Phone == phone && n.TenTK != tentk);
            if (emailExist == 0 && cmndExist == 0 && phoneExist == 0)
                return true;
            return false;
        }

        public bool ClientIsLocked(string username)
        {
            var res = db.Database.SqlQuery<Boolean>("SELECT dbo.ClientCheckLock(@username)", new SqlParameter("@username", username)).FirstOrDefault();
            return res;
        }

        public TAIKHOAN GetByID(int id)
        {
            return db.TAIKHOANs.SingleOrDefault(n => n.MaTK == id);
        }
        public int GetPositionByName(string name)
        {
            return db.CHUCVUs.SingleOrDefault(n => n.TenChucVu == name).MaChucVu;
        }
        public string GetAvataByID(int id)
        {
            return db.TAIKHOANs.Find(id).AnhDaiDien;
        }
        public string GetNameByID(int id)
        {
            return db.TAIKHOANs.Find(id).LastName;
        }
        public string GetFullNameByID(int id)
        {
            return db.TAIKHOANs.Find(id).LastName  + " " + db.TAIKHOANs.Find(id).FirstName;
        }
        public bool ClientUserLogin(string username, string password)
        {
            int result = db.TAIKHOANs.Count(n => n.TenTK == username && n.MatKhau == password && n.KichHoat == true && n.ChucVu < 4);
            if (result > 0)
            {
                return true;
            }
            else
                return false;
        }

        public void UpdateAvataByID(int id, string fullpath)
        {
            var result = GetByID(id);
            result.AnhDaiDien = fullpath;
            db.SaveChanges();
        }

        public int SQLChangePassword(int id, string oPassword, string nPassword, string rNPassword, string realpass)
        {
            int result = db.ChangePassword(id, oPassword, nPassword, rNPassword, realpass);
            db = new LemonCatEntities();
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Domain.Entity
{
    /// <summary>
    /// 用户类型(1:学生,2:教师,3:家长)
    /// </summary>
    public enum UserType : int
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,
        /// <summary>
        /// 管理员
        /// </summary>
        Admin  = 1,
        /// <summary>
        /// 用户
        /// </summary>
        Consumerser = 2,

    }
     
    /// <summary>
    /// 用户
    /// </summary>
    public class User 
    {
		 
		/// <summary>
        /// 唯一id
        /// </summary>
		public int Id
        {
            set;
            get;
        }
		
	 
		/// <summary>
        /// 用户名，唯一
        /// </summary>
		public string Username
        {
            set;
            get;
        }
		
		 
		/// <summary>
        /// 密码(加密)
        /// </summary>
		public string Password
        {
            set;
            get;
        }

       
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName
        {
            set;
            get;
        }
 
		 
		/// <summary>
        /// 注册时间
        /// </summary>
		public DateTime RegisterTime
        {
            set;
            get;
        }
		
        
        /// <summary>
        /// 用户类型(1:管理员,2:用户)
        /// </summary>
        public UserType UserType
        {
            set;
            get;
        }

        public string Phone { get; set; }

        public string EMail { get; set; }
    }
}
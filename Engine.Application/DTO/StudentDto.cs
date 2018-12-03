using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Application.DTO
{

    /// <summary>
    /// 学生DTO
    /// </summary>
    public class StudentDto  
    {

        #region Columns
        public int Id { get; set; }
        public int Age { get; set; }

        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        #endregion

    }



}

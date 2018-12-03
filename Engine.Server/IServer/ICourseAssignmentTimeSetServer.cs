 
using Course.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Course.Server
{
	/// <summary>
    /// 走班时段设置服务接口
    /// </summary>

    public interface ICourseAssignmentTimeSetServer : IServer<CourseAssignmentTimeSetting>
    {

    }
}
